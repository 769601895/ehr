namespace Admin.NET.Application;

public class HrAdvQuerySchemeListInput
{
    [Required]
    public string ModuleCode { get; set; }
}

public class HrAdvQuerySchemeOutput
{
    public long Id { get; set; }
    public string ModuleCode { get; set; }
    public string Name { get; set; }
    public string FilterJson { get; set; }
    public long OwnerUserId { get; set; }
    public bool IsShared { get; set; }
    public List<long> ShareRoleIds { get; set; } = [];
    public bool IsOwner { get; set; }
    public DateTime? UpdateTime { get; set; }
}

public class HrAdvQuerySchemeSaveInput
{
    public long? Id { get; set; }

    [Required]
    public string ModuleCode { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string FilterJson { get; set; }

    public bool IsShared { get; set; }

    public List<long>? ShareRoleIds { get; set; }
}

public class HrAdvQuerySchemeIdInput
{
    [Required]
    public long Id { get; set; }
}
