// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using ReZero.SuperAPI;

namespace Admin.NET.Plugin.ReZero.Service;

/// <summary>
/// 初始化Admin.NET默认模板
/// </summary>
public class RezeroTemplate
{
    public static void InitTemplate()
    {
        // Admin.NET 实体模板
        var entiyTemplate = new ZeroTemplate
        {
            Id = 1300000000100,
            TypeId = TemplateType.Entity,
            Title = "Admin.NET 实体模板",
            TemplateContent = "// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。\r\n//\r\n// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。\r\n//\r\n// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！\r\n\r\nnamespace Admin.NET.Application;\r\n\r\n/// <summary>\r\n/// {{(Model.Description+\"\").Replace(\"\\r\",\"\").Replace(\"\\n\",\"\")}}\r\n///</summary>\r\n[SugarTable(null, \"{{(Model.Description+\"\").Replace(\"\\r\",\"\").Replace(\"\\n\",\"\")}}\")]\r\npublic class {{Model.ClassName}} : EntityBase\r\n{\r\n<% foreach (var item in Model.PropertyGens) {\r\n        var isPrimaryKey = item.IsPrimaryKey ? \",IsPrimaryKey = true\" : \"\";\r\n        var isIdentity = item.IsIdentity ? \",IsIdentity = true\" : \"\"; \r\n        var isIgnore=(item.IsIgnore?\",IsIgnore = true\":\"\");\r\n        var isJson=item.IsJson?\",IsJson= true\":\"\" ; \r\n        var stringValue=item.PropertyType==\"string\"?\"= null!;\":\"\";//C#低版本改模版\r\n%> \r\n    /// <summary>\r\n    /// {{item.Description}}\r\n    ///</summary>\r\n    [SugarColumn(ColumnDescription = \"{{item.Description}}\" {{isPrimaryKey+isIdentity+isIgnore+isJson}}) ]\r\n    public {{item.PropertyType}} {{item.PropertyName}}  { get; set;  } {{stringValue}}\r\n<%} %>\r\n}\r\n    \r\n",
            TemplateContentStyle = "csharp",
            Url = "C:\\Admin.NET.Entity\\{0}.cs",
            SortId = 100,
            CreateTime = DateTime.Parse("2025-03-15 00:00:00")
        };
        //// Admin.NET 接口模板
        //var serviceTemplate = new ZeroTemplate
        //{
        //    Id = 1300000000200,
        //    TypeId = TemplateType.Api,
        //    Title = "Admin.NET 实体模板",
        //    TemplateContent = "// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。\r\n//\r\n// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。\r\n//\r\n// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！\r\n\r\nnamespace Admin.NET.Application;\r\n\r\n/// <summary>\r\n/// {{(Model.Description+\"\").Replace(\"\\r\",\"\").Replace(\"\\n\",\"\")}}\r\n///</summary>\r\n[SugarTable(null, \"{{(Model.Description+\"\").Replace(\"\\r\",\"\").Replace(\"\\n\",\"\")}}\")]\r\npublic class {{Model.ClassName}} : EntityBase\r\n{\r\n<% foreach (var item in Model.PropertyGens) {\r\n        var isPrimaryKey = item.IsPrimaryKey ? \",IsPrimaryKey = true\" : \"\";\r\n        var isIdentity = item.IsIdentity ? \",IsIdentity = true\" : \"\"; \r\n        var isIgnore=(item.IsIgnore?\",IsIgnore = true\":\"\");\r\n        var isJson=item.IsJson?\",IsJson= true\":\"\" ; \r\n        var stringValue=item.PropertyType==\"string\"?\"= null!;\":\"\";//C#低版本改模版\r\n%> \r\n    /// <summary>\r\n    /// {{item.Description}}\r\n    ///</summary>\r\n    [SugarColumn(ColumnDescription = \"{{item.Description}}\" {{isPrimaryKey+isIdentity+isIgnore+isJson}}) ]\r\n    public {{item.PropertyType}} {{item.PropertyName}}  { get; set;  } {{stringValue}}\r\n<%} %>\r\n}\r\n    \r\n",
        //    TemplateContentStyle = "csharp",
        //    Url = "C:\\Admin.NET.Service\\{0}.cs",
        //    SortId = 200,
        //    CreateTime = DateTime.Parse("2025-03-15 00:00:00")
        //};
        ZeroDb.Db.Storageable(entiyTemplate).ExecuteCommand();
    }
}