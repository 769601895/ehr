using System.Reflection;

namespace Admin.NET.Application;

/// <summary>
/// emp_char1～100 在实体 / DTO 间拷贝（展示与是否下拉仍由字段元数据决定）。
/// </summary>
internal static class HrEmpCharFields
{
    private const int Count = 100;
    private static readonly PropertyInfo[] EntityProps = BuildProps(typeof(HrEmployee));
    private static readonly PropertyInfo[] OutputProps = BuildProps(typeof(HrEmpOutput));
    private static readonly PropertyInfo[] SaveProps = BuildProps(typeof(HrEmpSaveInput));

    public static void CopyFromEntity(HrEmployee source, HrEmpOutput target)
    {
        for (var i = 0; i < Count; i++)
            OutputProps[i].SetValue(target, EntityProps[i].GetValue(source) as string);
    }

    public static void ApplyFromInput(HrEmployee target, HrEmpSaveInput source)
    {
        for (var i = 0; i < Count; i++)
        {
            var raw = SaveProps[i].GetValue(source) as string;
            EntityProps[i].SetValue(target, string.IsNullOrWhiteSpace(raw) ? null : raw.Trim());
        }
    }

    private static PropertyInfo[] BuildProps(Type type)
    {
        var props = new PropertyInfo[Count];
        for (var i = 1; i <= Count; i++)
        {
            var name = "EmpChar" + i;
            props[i - 1] = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?? throw new InvalidOperationException($"缺少属性 {type.Name}.{name}");
        }
        return props;
    }
}
