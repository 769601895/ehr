using System.Reflection;
using System.Text.Json;
using Furion.DatabaseAccessor;
using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>员工资料导入</summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmpImport", Order = 473, Description = "员工资料导入")]
public class HrEmpImportService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<HrEmpImportTpl> _tplRep;
    private readonly SqlSugarRepository<HrEmpImportTplField> _tplFieldRep;
    private readonly SqlSugarRepository<HrEmpImportBatch> _batchRep;
    private readonly SqlSugarRepository<HrEmpImportBatchRow> _batchRowRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly HrUiFieldDefService _fieldDefSvc;
    private readonly IHrEmpImportFieldFilter _fieldFilter;
    private readonly HrEmpImportValueResolver _valueResolver;
    private readonly HrEmpService _empSvc;
    private readonly HrEmpIdGenerator _empIdGen;

    public HrEmpImportService(
        UserManager userManager,
        SqlSugarRepository<HrEmpImportTpl> tplRep,
        SqlSugarRepository<HrEmpImportTplField> tplFieldRep,
        SqlSugarRepository<HrEmpImportBatch> batchRep,
        SqlSugarRepository<HrEmpImportBatchRow> batchRowRep,
        SqlSugarRepository<HrEmployee> empRep,
        HrUiFieldDefService fieldDefSvc,
        IHrEmpImportFieldFilter fieldFilter,
        HrEmpImportValueResolver valueResolver,
        HrEmpService empSvc,
        HrEmpIdGenerator empIdGen)
    {
        _userManager = userManager;
        _tplRep = tplRep;
        _tplFieldRep = tplFieldRep;
        _batchRep = batchRep;
        _batchRowRep = batchRowRep;
        _empRep = empRep;
        _fieldDefSvc = fieldDefSvc;
        _fieldFilter = fieldFilter;
        _valueResolver = valueResolver;
        _empSvc = empSvc;
        _empIdGen = empIdGen;
    }

    /// <summary>可导入字段池</summary>
    [ApiDescriptionSettings(Name = "FieldPool"), HttpGet]
    [DisplayName("导入字段池")]
    public async Task<List<HrUiFieldPoolItemOutput>> FieldPool([FromQuery] string importMode)
    {
        EnsureMode(importMode);
        var pool = await LoadImportPool(importMode);

        if (string.Equals(importMode, HrEmpImportConsts.ModeUpdate, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var f in pool.Where(u => u.FieldKey.Equals(HrEmpImportConsts.EmpIdFieldKey, StringComparison.OrdinalIgnoreCase)))
            {
                f.Required = true;
                f.Locked = true;
            }
        }

        return pool;
    }

    [ApiDescriptionSettings(Name = "TplPage"), HttpPost]
    [DisplayName("导入模板分页")]
    public async Task<SqlSugarPagedList<HrEmpImportTplOutput>> TplPage(HrEmpImportTplPageInput input)
    {
        input ??= new();
        var userId = _userManager.UserId;
        var q = _tplRep.AsQueryable()
            .Where(u => u.OwnerUserId == userId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ImportMode), u => u.ImportMode == input.ImportMode!.Trim());

        if (string.IsNullOrWhiteSpace(input.Field))
            input.Field = nameof(HrEmpImportTpl.UpdateTime);
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";

        var page = await q.OrderBuilder(input, defaultSortField: nameof(HrEmpImportTpl.UpdateTime), descSort: true)
            .ToPagedListAsync(input.Page, input.PageSize);

        var ids = page.Items.Select(u => u.Id).ToList();
        var fields = ids.Count == 0
            ? []
            : await _tplFieldRep.AsQueryable().Where(u => ids.Contains(u.TplId)).OrderBy(u => u.SortNo).ToListAsync();
        var pool = await LoadImportPool(null);
        var labelMap = pool.ToDictionary(u => u.FieldKey, u => u.Label, StringComparer.OrdinalIgnoreCase);

        var items = page.Items.Select(t => new HrEmpImportTplOutput
        {
            Id = t.Id,
            ImportMode = t.ImportMode,
            Name = t.Name,
            Remark = t.Remark,
            CreateTime = t.CreateTime,
            UpdateTime = t.UpdateTime,
            Fields = fields.Where(f => f.TplId == t.Id).Select(f => new HrEmpImportTplFieldOutput
            {
                FieldKey = f.FieldKey,
                Label = labelMap.TryGetValue(f.FieldKey, out var lb) ? lb : f.FieldKey,
                SortNo = f.SortNo,
                IsLocked = f.IsLocked
            }).ToList()
        }).ToList();

        return new SqlSugarPagedList<HrEmpImportTplOutput>
        {
            Page = page.Page,
            PageSize = page.PageSize,
            Total = page.Total,
            TotalPages = page.TotalPages,
            Items = items,
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage
        };
    }

    [ApiDescriptionSettings(Name = "TplDetail"), HttpGet]
    [DisplayName("导入模板详情")]
    public async Task<HrEmpImportTplOutput> TplDetail([FromQuery] long id)
        => await ToTplOutput(await GetMyTpl(id));

    /// <summary>按作业类型取本账号唯一模板（不存在则创建空模板）</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "TplByMode"), HttpGet]
    [DisplayName("按类型获取导入模板")]
    public async Task<HrEmpImportTplOutput> TplByMode([FromQuery] string importMode)
    {
        EnsureMode(importMode);
        var tpl = await EnsureUniqueTplAsync(importMode.Trim());
        return await ToTplOutput(tpl);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "TplSave"), HttpPost]
    [DisplayName("保存导入模板")]
    public async Task<long> TplSave(HrEmpImportTplSaveInput input)
    {
        EnsureMode(input.ImportMode);
        var mode = input.ImportMode.Trim();
        var fields = (input.Fields ?? []).Where(u => !string.IsNullOrWhiteSpace(u.FieldKey)).ToList();
        if (fields.Count == 0)
            throw Oops.Oh("请至少选择一个字段");

        var pool = await FieldPool(mode);
        var poolKeys = pool.Select(u => u.FieldKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var f in fields)
        {
            if (!poolKeys.Contains(f.FieldKey.Trim()))
                throw Oops.Oh($"字段不可导入：{f.FieldKey}");
            if (HrEmpImportConsts.BannedFieldKeys.Contains(f.FieldKey))
                throw Oops.Oh($"禁止导入字段：{f.FieldKey}");
        }

        var isUpdate = string.Equals(mode, HrEmpImportConsts.ModeUpdate, StringComparison.OrdinalIgnoreCase);
        if (isUpdate && !fields.Any(u => u.FieldKey.Equals(HrEmpImportConsts.EmpIdFieldKey, StringComparison.OrdinalIgnoreCase)))
            throw Oops.Oh("修改模板必须包含工号");

        // 每账号每类型仅一份：以该类型唯一模板为准（忽略多余历史）
        var entity = await EnsureUniqueTplAsync(mode);
        entity.Name = HrEmpImportConsts.FixedName(mode);
        entity.Remark = string.IsNullOrWhiteSpace(input.Remark) ? null : input.Remark.Trim();
        await _tplRep.UpdateAsync(entity);
        await _tplFieldRep.DeleteAsync(u => u.TplId == entity.Id);

        var sort = 10;
        var rows = fields
            .OrderBy(u => u.SortNo == 0 ? int.MaxValue : u.SortNo)
            .Select(u =>
            {
                var key = u.FieldKey.Trim();
                var locked = isUpdate && key.Equals(HrEmpImportConsts.EmpIdFieldKey, StringComparison.OrdinalIgnoreCase);
                return new HrEmpImportTplField
                {
                    Id = YitIdHelper.NextId(),
                    TplId = entity.Id,
                    FieldKey = key,
                    SortNo = u.SortNo > 0 ? u.SortNo : sort += 10,
                    IsLocked = locked
                };
            })
            .GroupBy(u => u.FieldKey, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        if (isUpdate)
        {
            var empId = rows.FirstOrDefault(u => u.FieldKey.Equals(HrEmpImportConsts.EmpIdFieldKey, StringComparison.OrdinalIgnoreCase));
            if (empId != null)
            {
                empId.IsLocked = true;
                empId.SortNo = 1;
            }
        }

        await _tplFieldRep.InsertRangeAsync(rows);
        return entity.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "TplDelete"), HttpPost]
    [DisplayName("删除导入模板")]
    public async Task TplDelete(HrEmpImportTplDeleteInput input)
    {
        var tpl = await GetMyTpl(input.Id);
        await _tplFieldRep.FakeDeleteAsync(u => u.TplId == tpl.Id);
        await _tplRep.FakeDeleteAsync(tpl);
    }

    /// <summary>下载 Excel 模板（标准 xlsx，兼容 WPS）</summary>
    [ApiDescriptionSettings(Name = "DownloadTemplate"), HttpGet]
    [DisplayName("下载导入模板")]
    public async Task<IActionResult> DownloadTemplate([FromQuery] long tplId)
    {
        var detail = await TplDetail(tplId);
        var headers = detail.Fields.OrderBy(u => u.SortNo).Select(u => u.Label ?? u.FieldKey).ToList();
        if (headers.Count == 0)
            throw Oops.Oh("模板无字段");

        var tip = new Dictionary<string, object>();
        foreach (var h in headers)
            tip[h] = "";

        var ms = new MemoryStream();
        await ms.SaveAsAsync(new[] { tip }, sheetName: "Import");
        ms.Seek(0, SeekOrigin.Begin);
        var name = $"{detail.Name}_{detail.ImportMode}.xlsx";
        return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = name
        };
    }

    /// <summary>预览校验（不写库、不占号）</summary>
    [ApiDescriptionSettings(Name = "Preview"), HttpPost]
    [DisplayName("导入预览校验")]
    public async Task<HrEmpImportPreviewOutput> Preview([FromForm] long tplId, IFormFile file)
    {
        var (errors, total, prepared, headerMismatch) = await ValidateFileFullAsync(tplId, file, occupyEmpId: false);
        return new HrEmpImportPreviewOutput
        {
            TotalCount = total,
            Ok = headerMismatch == null && errors.Count == 0,
            Errors = errors,
            HeaderMismatch = headerMismatch,
            Columns = prepared?.Columns ?? [],
            Rows = prepared?.PreviewRows ?? []
        };
    }

    /// <summary>确认导入（整批事务；校验失败记 Failed 历史；写入异常整批回滚）</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Commit"), HttpPost]
    [DisplayName("确认导入")]
    public async Task<HrEmpImportCommitOutput> Commit([FromForm] long tplId, IFormFile file)
    {
        var tpl = await GetMyTpl(tplId);
        var (errors, total, prepared, headerMismatch) = await ValidateFileFullAsync(tplId, file, occupyEmpId: false);

        var batchNo = $"EI{DateTime.Now:yyyyMMddHHmmss}{YitIdHelper.NextId() % 10000:D4}";
        var fieldKeys = prepared?.FieldKeys ?? [];
        var batch = new HrEmpImportBatch
        {
            Id = YitIdHelper.NextId(),
            BatchNo = batchNo,
            ImportMode = tpl.ImportMode,
            TplId = tpl.Id,
            TplNameSnapshot = tpl.Name,
            FieldKeysSnapshot = JsonSerializer.Serialize(fieldKeys),
            FileName = file?.FileName,
            TotalCount = total,
            OperatorUserId = _userManager.UserId,
            Status = HrEmpImportConsts.StatusFailed,
            FailCount = errors.Count,
            SuccessCount = 0
        };

        if (headerMismatch != null || errors.Count > 0)
        {
            if (headerMismatch != null && errors.Count == 0)
            {
                errors =
                [
                    new HrEmpImportRowErrorOutput
                    {
                        RowNo = 1,
                        ErrorMessage = Truncate(FormatHeaderMismatchMessage(headerMismatch), 1000)
                    }
                ];
                batch.FailCount = 1;
            }
            batch.ErrorSummary = Truncate(
                headerMismatch != null
                    ? FormatHeaderMismatchMessage(headerMismatch)
                    : $"校验失败 {errors.Count} 行",
                1000);
            await _batchRep.InsertAsync(batch);
            await InsertFailRows(batch.Id, errors);
            return ToCommitOutput(batch, errors);
        }

        if (string.Equals(tpl.ImportMode, HrEmpImportConsts.ModeAdd, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var row in prepared!.Rows)
                await _empSvc.Add(row.Save);
        }
        else
        {
            foreach (var row in prepared!.Rows)
                await _empSvc.Update(row.Save);
        }

        batch.Status = HrEmpImportConsts.StatusSuccess;
        batch.SuccessCount = total;
        batch.FailCount = 0;
        batch.ErrorSummary = null;
        await _batchRep.InsertAsync(batch);
        return ToCommitOutput(batch, []);
    }

    [ApiDescriptionSettings(Name = "BatchPage"), HttpPost]
    [DisplayName("导入历史分页")]
    public async Task<SqlSugarPagedList<HrEmpImportBatchOutput>> BatchPage(HrEmpImportBatchPageInput input)
    {
        input ??= new();
        var userId = _userManager.UserId;
        var kw = input.Keyword?.Trim();
        var q = _batchRep.AsQueryable()
            .Where(u => u.OperatorUserId == userId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ImportMode), u => u.ImportMode == input.ImportMode!.Trim())
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status), u => u.Status == input.Status!.Trim())
            .WhereIF(!string.IsNullOrWhiteSpace(kw), u => u.BatchNo.Contains(kw!) || (u.FileName != null && u.FileName.Contains(kw!)) || (u.TplNameSnapshot != null && u.TplNameSnapshot.Contains(kw!)));

        if (string.IsNullOrWhiteSpace(input.Field))
            input.Field = nameof(HrEmpImportBatch.CreateTime);
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";

        return await q.OrderBuilder(input, defaultSortField: nameof(HrEmpImportBatch.CreateTime), descSort: true)
            .Select<HrEmpImportBatchOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "FailRows"), HttpPost]
    [DisplayName("导入失败明细")]
    public async Task<SqlSugarPagedList<HrEmpImportRowErrorOutput>> FailRows(HrEmpImportFailRowsInput input)
    {
        var batch = await _batchRep.GetFirstAsync(u => u.Id == input.BatchId && u.OperatorUserId == _userManager.UserId)
            ?? throw Oops.Oh("导入批次不存在");
        _ = batch;
        if (string.IsNullOrWhiteSpace(input.Field))
            input.Field = nameof(HrEmpImportBatchRow.RowNo);
        return await _batchRowRep.AsQueryable()
            .Where(u => u.BatchId == input.BatchId)
            .OrderBuilder(input, defaultSortField: nameof(HrEmpImportBatchRow.RowNo), descSort: false)
            .Select(u => new HrEmpImportRowErrorOutput
            {
                RowNo = u.RowNo,
                EmpId = u.EmpId,
                ErrorMessage = u.ErrorMessage
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    // ---------- helpers ----------

    private async Task<List<HrUiFieldPoolItemOutput>> LoadImportPool(string? importMode)
    {
        var pool = await _fieldDefSvc.QueryPool("hrEmp", "form");
        pool = pool.Where(u => !HrEmpImportConsts.BannedFieldKeys.Contains(u.FieldKey)).ToList();
        if (!string.IsNullOrWhiteSpace(importMode))
            pool = await _fieldFilter.FilterAsync(_userManager.UserId, importMode, pool);
        return pool;
    }

    private static HrEmpImportCommitOutput ToCommitOutput(HrEmpImportBatch batch, List<HrEmpImportRowErrorOutput> errors) => new()
    {
        BatchId = batch.Id,
        BatchNo = batch.BatchNo,
        Status = batch.Status,
        TotalCount = batch.TotalCount,
        SuccessCount = batch.SuccessCount,
        FailCount = batch.FailCount,
        Errors = errors
    };

    private async Task InsertFailRows(long batchId, List<HrEmpImportRowErrorOutput> errors)
    {
        if (errors.Count == 0) return;
        var rows = errors.Select(e => new HrEmpImportBatchRow
        {
            Id = YitIdHelper.NextId(),
            BatchId = batchId,
            RowNo = e.RowNo,
            EmpId = e.EmpId,
            ErrorMessage = Truncate(e.ErrorMessage, 1000)
        }).ToList();
        await _batchRowRep.InsertRangeAsync(rows);
    }

    private async Task<(List<HrEmpImportRowErrorOutput> Errors, int Total, PreparedBatch? Prepared, HrEmpImportHeaderMismatchOutput? HeaderMismatch)> ValidateFileFullAsync(
        long tplId, IFormFile file, bool occupyEmpId)
    {
        _ = occupyEmpId;
        if (file == null || file.Length <= 0)
            throw Oops.Oh("请上传 Excel 文件");
        if (file.Length > HrEmpImportConsts.MaxFileBytes)
            throw Oops.Oh("文件不能超过 5MB");
        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (ext != ".xlsx")
            throw Oops.Oh("仅支持 .xlsx（请用 Excel/WPS 另存为 Excel 工作簿）");

        var tpl = await GetMyTpl(tplId);
        var fieldRows = await _tplFieldRep.AsQueryable().Where(u => u.TplId == tplId).OrderBy(u => u.SortNo).ToListAsync();
        if (fieldRows.Count == 0)
            throw Oops.Oh("模板无字段");

        var pool = await FieldPool(tpl.ImportMode);
        var poolMap = pool.ToDictionary(u => u.FieldKey, u => u, StringComparer.OrdinalIgnoreCase);
        foreach (var f in fieldRows)
        {
            if (!poolMap.ContainsKey(f.FieldKey) || HrEmpImportConsts.BannedFieldKeys.Contains(f.FieldKey))
                throw Oops.Oh($"模板含不可导入字段：{f.FieldKey}");
        }

        var orderedKeys = fieldRows.Select(u => u.FieldKey).ToList();
        var expectedHeaders = orderedKeys.Select(k => poolMap[k].Label ?? k).ToList();

        await using var stream = file.OpenReadStream();
        var sheetRows = (await MiniExcel.QueryAsync(stream, useHeaderRow: true)).Cast<IDictionary<string, object>>().ToList();
        if (sheetRows.Count == 0)
            throw Oops.Oh("文件无数据行");
        if (sheetRows.Count > HrEmpImportConsts.MaxRows)
            throw Oops.Oh($"单批最多 {HrEmpImportConsts.MaxRows} 行");

        var actualHeaders = sheetRows[0].Keys.Select(u => u?.ToString()?.Trim() ?? "").Where(u => u != "").ToList();
        if (actualHeaders.Count != expectedHeaders.Count
            || !actualHeaders.SequenceEqual(expectedHeaders, StringComparer.Ordinal))
        {
            var headerMismatch = BuildHeaderMismatch(expectedHeaders, actualHeaders);
            var headerErrors = new List<HrEmpImportRowErrorOutput>
            {
                new()
                {
                    RowNo = 1,
                    ErrorMessage = Truncate(FormatHeaderMismatchMessage(headerMismatch), 1000)
                }
            };
            return (headerErrors, 0, null, headerMismatch);
        }

        var headerToKey = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 0; i < expectedHeaders.Count; i++)
            headerToKey[expectedHeaders[i]] = orderedKeys[i];

        var errors = new List<HrEmpImportRowErrorOutput>();
        var preparedRows = new List<PreparedRow>();
        var previewRows = new List<HrEmpImportPreviewRowOutput>();
        var columns = orderedKeys.Select(k => new HrEmpImportPreviewColumnOutput
        {
            FieldKey = k,
            Label = poolMap[k].Label ?? k
        }).ToList();
        var empIdsInBatch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var isAdd = string.Equals(tpl.ImportMode, HrEmpImportConsts.ModeAdd, StringComparison.OrdinalIgnoreCase);

        for (var i = 0; i < sheetRows.Count; i++)
        {
            var excelRowNo = i + 2; // 表头第1行
            var dict = sheetRows[i];
            var cellMap = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in dict)
            {
                var h = kv.Key?.Trim() ?? "";
                if (!headerToKey.TryGetValue(h, out var fk)) continue;
                cellMap[fk] = CellToString(kv.Value);
            }

            // 预览单元格按模板字段顺序固定输出（缺列空串）
            var previewCells = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var k in orderedKeys)
                previewCells[k] = cellMap.TryGetValue(k, out var v) ? v : null;

            var rowErrors = new List<string>();
            HrEmpSaveInput? save = null;
            try
            {
                save = await BuildSaveInputAsync(cellMap, poolMap, isAdd);
            }
            catch (Exception ex)
            {
                rowErrors.Add(CleanOopsMessage(ex.Message));
                save = new HrEmpSaveInput
                {
                    EmpName = cellMap.TryGetValue("empName", out var n) ? (n ?? "") : "",
                    EmpId = cellMap.TryGetValue(HrEmpImportConsts.EmpIdFieldKey, out var eid) ? eid : null
                };
            }

            var empIdHint = save.EmpId?.Trim();
            if (isAdd)
            {
                if (string.IsNullOrWhiteSpace(save.EmpName))
                    rowErrors.Add("姓名不能为空");
                if (!string.IsNullOrWhiteSpace(empIdHint))
                {
                    if (!empIdsInBatch.Add(empIdHint))
                        rowErrors.Add($"批次内工号重复：{empIdHint}");
                    else if (await _empIdGen.IsEmpIdUsedAsync(empIdHint))
                        rowErrors.Add($"工号已存在：{empIdHint}");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(empIdHint))
                    rowErrors.Add("工号不能为空");
                else
                {
                    if (!empIdsInBatch.Add(empIdHint))
                        rowErrors.Add($"批次内工号重复：{empIdHint}");
                    else if (!await _empIdGen.IsEmpIdUsedAsync(empIdHint))
                        rowErrors.Add($"工号不存在：{empIdHint}");
                    else
                    {
                        try
                        {
                            save = await MergeUpdateAsync(empIdHint, save, cellMap);
                        }
                        catch (Exception ex)
                        {
                            rowErrors.Add(CleanOopsMessage(ex.Message));
                        }
                    }
                }
            }

            var errText = rowErrors.Count == 0 ? null : Truncate(string.Join("；", rowErrors), 1000);
            previewRows.Add(new HrEmpImportPreviewRowOutput
            {
                RowNo = excelRowNo,
                Cells = previewCells,
                ErrorMessage = errText
            });

            if (rowErrors.Count > 0)
            {
                errors.Add(new HrEmpImportRowErrorOutput
                {
                    RowNo = excelRowNo,
                    EmpId = cellMap.TryGetValue(HrEmpImportConsts.EmpIdFieldKey, out var id) ? id : null,
                    ErrorMessage = errText!
                });
            }
            else if (save != null)
            {
                preparedRows.Add(new PreparedRow { RowNo = excelRowNo, Save = save });
            }
        }

        var prepared = new PreparedBatch
        {
            FieldKeys = orderedKeys,
            Rows = preparedRows,
            Errors = errors,
            Columns = columns,
            PreviewRows = previewRows
        };
        return (errors, sheetRows.Count, prepared, null);
    }

    private static string CleanOopsMessage(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return "校验失败";
        // Furion Oops 有时带类型前缀，取可读正文
        var s = msg.Trim();
        var idx = s.IndexOf(':');
        if (idx > 0 && idx < 40 && s.Length > idx + 1)
            return s[(idx + 1)..].Trim();
        return s;
    }

    private static HrEmpImportHeaderMismatchOutput BuildHeaderMismatch(List<string> expected, List<string> actual)
    {
        var missing = expected.Where(h => !actual.Contains(h, StringComparer.Ordinal)).ToList();
        var extra = actual.Where(h => !expected.Contains(h, StringComparer.Ordinal)).ToList();
        var orderDiffs = new List<string>();
        var n = Math.Min(expected.Count, actual.Count);
        for (var i = 0; i < n; i++)
        {
            if (!string.Equals(expected[i], actual[i], StringComparison.Ordinal))
                orderDiffs.Add($"第{i + 1}列：期望「{expected[i]}」实际「{actual[i]}」");
        }
        return new HrEmpImportHeaderMismatchOutput
        {
            Expected = expected,
            Actual = actual,
            Missing = missing,
            Extra = extra,
            OrderDiffs = orderDiffs
        };
    }

    private static string FormatHeaderMismatchMessage(HrEmpImportHeaderMismatchOutput m)
    {
        var parts = new List<string> { "表头与模板不一致（列名、数量、顺序须完全一致）" };
        if (m.Missing.Count > 0)
            parts.Add($"缺少列：{string.Join("、", m.Missing)}");
        if (m.Extra.Count > 0)
            parts.Add($"多余列：{string.Join("、", m.Extra)}");
        if (m.Missing.Count == 0 && m.Extra.Count == 0 && m.OrderDiffs.Count > 0)
            parts.Add($"顺序不一致：{string.Join("；", m.OrderDiffs)}");
        parts.Add($"期望：{string.Join(" | ", m.Expected)}");
        parts.Add($"实际：{string.Join(" | ", m.Actual)}");
        return string.Join("。", parts);
    }

    private async Task<HrEmpSaveInput> BuildSaveInputAsync(
        Dictionary<string, string?> cellMap,
        Dictionary<string, HrUiFieldPoolItemOutput> poolMap,
        bool isAdd)
    {
        var save = new HrEmpSaveInput { EmpName = "" };
        if (isAdd)
            save.EmpState = 1;

        foreach (var (fieldKey, raw) in cellMap)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (!poolMap.TryGetValue(fieldKey, out var meta)) continue;
            if (HrEmpImportConsts.BannedFieldKeys.Contains(fieldKey)) continue;

            var resolved = await _valueResolver.ResolveAsync(meta, raw);
            SetSaveProp(save, fieldKey, resolved, meta);
        }

        if (isAdd && string.IsNullOrWhiteSpace(save.EmpName))
            save.EmpName = cellMap.TryGetValue("empName", out var n) ? (n ?? "") : "";

        return save;
    }

    private async Task<HrEmpSaveInput> MergeUpdateAsync(string empId, HrEmpSaveInput overlay, Dictionary<string, string?> cellMap)
    {
        var existing = await _empSvc.Detail(empId);
        var save = new HrEmpSaveInput
        {
            EmpId = existing.EmpId,
            CcCode = existing.CcCode,
            DutyId = existing.DutyId,
            PostId = existing.PostId,
            EmpName = existing.EmpName ?? "",
            EmpState = existing.EmpState,
            EmpInDate = existing.EmpInDate,
            EmpBegDate = existing.EmpBegDate,
            EmpTryDate = existing.EmpTryDate,
            EmpFromDate = existing.EmpFromDate,
            EmpSal = existing.EmpSal,
            EmpSex = existing.EmpSex,
            EmpCard = existing.EmpCard,
            EmpBirth = existing.EmpBirth,
            EmpRgt = existing.EmpRgt,
            EmpProvince = existing.EmpProvince,
            EmpCity = existing.EmpCity,
            EmpCountry = existing.EmpCountry,
            EmpResidence = existing.EmpResidence,
            EmpNation = existing.EmpNation,
            EmpMarry = existing.EmpMarry,
            EmpHome = existing.EmpHome,
            EmpAddr = existing.EmpAddr,
            EmpAddrter = existing.EmpAddrter,
            EmpLongmon = existing.EmpLongmon,
            EmpKqLeaDate = existing.EmpKqLeaDate,
            EmpKqIndate = existing.EmpKqIndate,
            EmpCardDate = existing.EmpCardDate,
            EmpCardLimit = existing.EmpCardLimit,
            EmpType = existing.EmpType,
            EmpCardOverdate = existing.EmpCardOverdate,
            EmpWorkDate = existing.EmpWorkDate,
            EmpInDateGroup = existing.EmpInDateGroup,
            EmpPic = existing.EmpPic
        };

        // 仅覆盖 Excel 非空列对应属性
        foreach (var (fieldKey, raw) in cellMap)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (fieldKey.Equals(HrEmpImportConsts.EmpIdFieldKey, StringComparison.OrdinalIgnoreCase)) continue;
            if (HrEmpImportConsts.BannedFieldKeys.Contains(fieldKey)) continue;

            var prop = typeof(HrEmpSaveInput).GetProperty(ToPascal(fieldKey), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null || !prop.CanWrite) continue;
            var overlayProp = typeof(HrEmpSaveInput).GetProperty(prop.Name);
            var val = overlayProp?.GetValue(overlay);
            if (val != null)
                prop.SetValue(save, val);
        }

        save.EmpId = empId;
        // 禁止改状态
        save.EmpState = existing.EmpState;
        return save;
    }

    private static void SetSaveProp(HrEmpSaveInput save, string fieldKey, string? value, HrUiFieldPoolItemOutput meta)
    {
        var prop = typeof(HrEmpSaveInput).GetProperty(ToPascal(fieldKey), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null || !prop.CanWrite) return;

        if (prop.PropertyType == typeof(string) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(string))
        {
            prop.SetValue(save, value);
            return;
        }

        var target = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        if (target == typeof(DateTime))
        {
            if (DateTime.TryParse(value, out var dt))
                prop.SetValue(save, dt);
            else if (double.TryParse(value, out var oa))
                prop.SetValue(save, DateTime.FromOADate(oa));
            else
                throw Oops.Oh($"{meta.Label}日期格式无效：{value}");
            return;
        }

        if (target == typeof(int))
        {
            if (int.TryParse(value, out var n))
                prop.SetValue(save, n);
            else
                throw Oops.Oh($"{meta.Label}须为整数：{value}");
            return;
        }

        if (target == typeof(long))
        {
            if (long.TryParse(value, out var n))
                prop.SetValue(save, n);
            else
                throw Oops.Oh($"{meta.Label}须为数字：{value}");
        }
    }

    private static string ToPascal(string fieldKey)
    {
        if (string.IsNullOrWhiteSpace(fieldKey)) return fieldKey;
        return char.ToUpperInvariant(fieldKey[0]) + fieldKey[1..];
    }

    private static string? CellToString(object? value)
    {
        if (value == null) return null;
        if (value is DateTime dt) return dt.ToString("yyyy-MM-dd");
        if (value is double d)
        {
            // Excel 日期序列
            if (d > 20000 && d < 60000)
            {
                try { return DateTime.FromOADate(d).ToString("yyyy-MM-dd"); }
                catch { /* fallthrough */ }
            }
            return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        var s = value.ToString()?.Trim();
        return string.IsNullOrEmpty(s) ? null : s;
    }

    private async Task<HrEmpImportTplOutput> ToTplOutput(HrEmpImportTpl tpl)
    {
        var fields = await _tplFieldRep.AsQueryable()
            .Where(u => u.TplId == tpl.Id)
            .OrderBy(u => u.SortNo)
            .ToListAsync();
        var pool = await FieldPool(tpl.ImportMode);
        var map = pool.ToDictionary(u => u.FieldKey, u => u, StringComparer.OrdinalIgnoreCase);
        return new HrEmpImportTplOutput
        {
            Id = tpl.Id,
            ImportMode = tpl.ImportMode,
            Name = tpl.Name,
            Remark = tpl.Remark,
            CreateTime = tpl.CreateTime,
            UpdateTime = tpl.UpdateTime,
            Fields = fields.Select(f =>
            {
                map.TryGetValue(f.FieldKey, out var meta);
                return new HrEmpImportTplFieldOutput
                {
                    FieldKey = f.FieldKey,
                    Label = meta?.Label ?? f.FieldKey,
                    SortNo = f.SortNo,
                    IsLocked = f.IsLocked,
                    Required = meta?.Required == true
                };
            }).ToList()
        };
    }

    /// <summary>本账号 + 作业类型仅保留一份模板；多余软删；没有则新建。</summary>
    private async Task<HrEmpImportTpl> EnsureUniqueTplAsync(string importMode)
    {
        var mode = importMode.Trim();
        var userId = _userManager.UserId;
        var list = await _tplRep.AsQueryable()
            .Where(u => u.OwnerUserId == userId && u.ImportMode == mode)
            .OrderByDescending(u => u.UpdateTime)
            .OrderByDescending(u => u.Id)
            .ToListAsync();

        if (list.Count == 0)
        {
            return await _tplRep.InsertReturnEntityAsync(new HrEmpImportTpl
            {
                Id = YitIdHelper.NextId(),
                OwnerUserId = userId,
                ImportMode = mode,
                Name = HrEmpImportConsts.FixedName(mode)
            });
        }

        var keep = list[0];
        keep.Name = HrEmpImportConsts.FixedName(mode);
        if (list.Count > 1)
        {
            foreach (var extra in list.Skip(1))
            {
                await _tplFieldRep.FakeDeleteAsync(u => u.TplId == extra.Id);
                await _tplRep.FakeDeleteAsync(extra);
            }
        }

        return keep;
    }

    private async Task<HrEmpImportTpl> GetMyTpl(long id)
    {
        var tpl = await _tplRep.GetFirstAsync(u => u.Id == id && u.OwnerUserId == _userManager.UserId)
            ?? throw Oops.Oh("模板不存在或无权访问");
        return tpl;
    }

    private static void EnsureMode(string? mode)
    {
        if (string.IsNullOrWhiteSpace(mode)
            || (!mode.Equals(HrEmpImportConsts.ModeAdd, StringComparison.OrdinalIgnoreCase)
                && !mode.Equals(HrEmpImportConsts.ModeUpdate, StringComparison.OrdinalIgnoreCase)))
            throw Oops.Oh("作业类型须为 Add 或 Update");
    }

    private static string Truncate(string? s, int max)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= max ? s : s[..max];
    }

    private sealed class PreparedBatch
    {
        public List<string> FieldKeys { get; set; } = [];
        public List<PreparedRow> Rows { get; set; } = [];
        public List<HrEmpImportRowErrorOutput> Errors { get; set; } = [];
        public List<HrEmpImportPreviewColumnOutput> Columns { get; set; } = [];
        public List<HrEmpImportPreviewRowOutput> PreviewRows { get; set; } = [];
    }

    private sealed class PreparedRow
    {
        public int RowNo { get; set; }
        public HrEmpSaveInput Save { get; set; } = null!;
    }
}
