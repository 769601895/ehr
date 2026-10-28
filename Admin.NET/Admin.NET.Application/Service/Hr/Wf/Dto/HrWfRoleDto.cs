namespace Admin.NET.Application;

public class HrWfRolePageInput : BasePageInput
{
    public long? LevelId { get; set; }
    /// <summary>右表：当前树节点的直接子节点；null 表示不过滤父级。</summary>
    public long? ParentId { get; set; }
    public new string? Keyword { get; set; }
    public bool? Enabled { get; set; }
}

public class HrWfRoleTreeOutput
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    /// <summary>空=组织虚拟节点，不参与审核</summary>
    public long? LevelId { get; set; }
    public string? LevelName { get; set; }
    public bool IsVirtual => LevelId is not > 0;
    public int OrderNo { get; set; }
    public bool Enabled { get; set; }
    public List<HrWfRoleTreeOutput> Children { get; set; } = [];
}

public class HrWfRoleScopeItem
{
    [Required]
    [MaxLength(64)]
    public string CcCode { get; set; }

    public bool IncludeChild { get; set; } = true;
}

public class HrWfRoleOutput
{
    public long Id { get; set; }
    /// <summary>空=组织虚拟节点，不参与审核</summary>
    public long? LevelId { get; set; }
    public string? LevelName { get; set; }
    public bool IsVirtual => LevelId is not > 0;
    public string Code { get; set; }
    public string Name { get; set; }
    public long? ParentId { get; set; }
    public int OrderNo { get; set; }
    public bool Enabled { get; set; }
    public string? Remark { get; set; }
    public List<HrWfRoleScopeItem> Scopes { get; set; } = [];
    public List<string> PostIds { get; set; } = [];
}

public class HrWfRoleSaveInput
{
    public long? Id { get; set; }

    /// <summary>空表示组织虚拟节点（无审核等级）</summary>
    public long? LevelId { get; set; }

    [Required(ErrorMessage = "编码不能为空")]
    [MaxLength(50)]
    public string Code { get; set; }

    [Required(ErrorMessage = "名称不能为空")]
    [MaxLength(100)]
    public string Name { get; set; }

    public long? ParentId { get; set; }
    public int OrderNo { get; set; } = 100;
    public bool Enabled { get; set; } = true;
    [MaxLength(256)]
    public string? Remark { get; set; }

    public List<HrWfRoleScopeItem> Scopes { get; set; } = [];
    public List<string> PostIds { get; set; } = [];
}

public class HrWfRoleDeleteInput
{
    [Required]
    public long Id { get; set; }
}
