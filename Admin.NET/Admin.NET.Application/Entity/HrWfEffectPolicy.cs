namespace Admin.NET.Application;

/// <summary>审批业务生效策略（通过≠即时写主档）</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_effect_policy", "审批业务生效策略")]
public class HrWfEffectPolicy : EntityBase
{
    [SugarColumn(ColumnName = "biz_type", ColumnDescription = "业务类型", Length = 50)]
    public string BizType { get; set; }

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    /// <summary>Form | Biz</summary>
    [SugarColumn(ColumnName = "base_date_source", ColumnDescription = "基准日来源", Length = 20)]
    public string BaseDateSource { get; set; } = HrWfConsts.EffectBaseFromForm;

    [SugarColumn(ColumnName = "base_date_field", ColumnDescription = "基准日字段", Length = 64)]
    public string BaseDateField { get; set; }

    [SugarColumn(ColumnName = "day_offset", ColumnDescription = "日偏移")]
    public int DayOffset { get; set; }

    /// <summary>ImmediateIfDue | JobOnly</summary>
    [SugarColumn(ColumnName = "apply_mode", ColumnDescription = "生效模式", Length = 20)]
    public string ApplyMode { get; set; } = HrWfConsts.EffectModeImmediateIfDue;

    [SugarColumn(ColumnName = "target_entity", ColumnDescription = "目标实体", Length = 50)]
    public string TargetEntity { get; set; } = "HrEmployee";

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
}
