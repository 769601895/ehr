namespace Admin.NET.Application;

/// <summary>审批表单字段 → 主档写回对照</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_effect_field_map", "审批生效字段对照")]
public class HrWfEffectFieldMap : EntityBase
{
    [SugarColumn(ColumnName = "biz_type", ColumnDescription = "业务类型", Length = 50)]
    public string BizType { get; set; }

    [SugarColumn(ColumnName = "form_field_key", ColumnDescription = "表单字段键", Length = 64)]
    public string FormFieldKey { get; set; }

    /// <summary>Subject | EffectBaseDate | Before | After | Context</summary>
    [SugarColumn(ColumnName = "role", ColumnDescription = "角色", Length = 20)]
    public string Role { get; set; }

    [SugarColumn(ColumnName = "target_entity", ColumnDescription = "目标实体", Length = 50)]
    public string TargetEntity { get; set; } = "HrEmployee";

    [SugarColumn(ColumnName = "db_column", ColumnDescription = "主档列", Length = 64, IsNullable = true)]
    public string? DbColumn { get; set; }

    [SugarColumn(ColumnName = "pair_group", ColumnDescription = "配对组", Length = 64, IsNullable = true)]
    public string? PairGroup { get; set; }

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    [SugarColumn(ColumnName = "sort_no", ColumnDescription = "排序")]
    public int SortNo { get; set; } = 100;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
}
