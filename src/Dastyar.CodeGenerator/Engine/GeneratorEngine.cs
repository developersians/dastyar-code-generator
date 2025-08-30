using Scriban;
using System.Reflection;
using System.Text;

namespace Dastyar.CodeGenerator;

public sealed class GeneratorEngine
{
    //private static Dictionary<TemplateKind, string> Templates = [];
    private static Dictionary<TemplateKind, TemplateInfo> Templates = [];
    private string OutputDirectory;
    private readonly string SharedDomainDllPath;
    private readonly string DomainDllPath;
    private readonly string IAggregateRootNamespace;
    private PathAssemblyResolver Resolver;
    private short generatedClassesCount = 0;
    private short generatedAggregatesCount = 0;

    private GeneratorEngine(
        string sharedDomainDllPath,
        string domainDllPath,
        string iAggregateRootNamespace)
    {
        ArgumentNullException.ThrowIfNull(sharedDomainDllPath);
        ArgumentNullException.ThrowIfNull(domainDllPath);
        ArgumentNullException.ThrowIfNull(iAggregateRootNamespace);

        SharedDomainDllPath = sharedDomainDllPath;
        DomainDllPath = domainDllPath;
        IAggregateRootNamespace = iAggregateRootNamespace;

        // Loads templates into local cache
        LoadTemplates();

        // Ensures output directory
        CreateOutputDirectory();
    }

    public static GeneratorEngine CreateEngine(
        string sharedDomainDllPath,
        string domainDllPath,
        string iAggregateRootNamespace)
    {
        var engine = new GeneratorEngine(
            sharedDomainDllPath,
            domainDllPath,
            iAggregateRootNamespace);

        return engine.CreateAssemblyResolver();
    }

    public void GenerateTemplates()
    {
        // Load Assemblies using resolver
        using var metadataContext = new MetadataLoadContext(Resolver);
        var sharedKernelAssembly = metadataContext.LoadFromAssemblyPath(SharedDomainDllPath);
        var domainAssembly = metadataContext.LoadFromAssemblyPath(DomainDllPath);
        var iAggregateRootType = sharedKernelAssembly.GetType(IAggregateRootNamespace);

        var reader = new XmlCommentReader(domainAssembly);
        EntityInfoFactory.SetXmlCommentReader(reader);

        // Scan and generate
        foreach (var type in domainAssembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract) continue;

            if (iAggregateRootType!.IsAssignableFrom(type))
            {
                var entityInfo = EntityInfoFactory.FromType(type);

                RenderTemplates(entityInfo);

                generatedAggregatesCount++;
            }
        }

        Console.WriteLine($"Generated {generatedClassesCount} classes for {generatedAggregatesCount} aggregates");
    }

    #region Utility methods
    private static void LoadTemplates()
    {
        foreach (var kind in Enum.GetValues<TemplateKind>())
        {
            string content = File.ReadAllText(@$"D:\Projects\dastyar-code-generator\src\Dastyar.CodeGenerator\Templates\{Enum.GetName(kind)}.scriban");
            string outputFolder = GetOutputFolderName(kind);

            Templates.Add(kind, new TemplateInfo(content, outputFolder));
        }
    }

    private static string GetOutputFolderName(TemplateKind kind)
    {
        return kind switch
        {
            TemplateKind.GetAggregateByIdQuery or
            TemplateKind.GetAggregateByIdQueryHandler => "Application\\GetById",

            TemplateKind.GetAggregateListQuery or
            TemplateKind.GetAggregateListQueryHandler => "Application\\GetList",

            TemplateKind.CreateAggregateCommand or
            TemplateKind.CreateAggregateCommandHandler => "Application\\Create",

            TemplateKind.UpdateAggregateCommand or
            TemplateKind.UpdateAggregateCommandHandler => "Application\\Update",

            TemplateKind.DeleteAggregateCommand or
            TemplateKind.DeleteAggregateCommandHandler => "Application\\Delete",

            TemplateKind.AggregateMapper or
            TemplateKind.AggregateResponse => "Application",

            TemplateKind.CreateAggregateRequest or
            TemplateKind.UpdateAggregateRequest or
            TemplateKind.AggregateController => "Api",

            TemplateKind.IAggregateRepository or
            TemplateKind.AggregateDomainMethods or
            TemplateKind.AggregateErrors => "Domain",

            TemplateKind.AggregateRepository => "Infrastructure\\Persistence",

            TemplateKind.LegacyAggregate => "LegacyEntities",

            _ => ""
        };
    }

    private void CreateOutputDirectory()
    {
        OutputDirectory = Path.Combine(ProjectRootFinder.GetProjectRootPath(), "Generated"); //Directory.GetCurrentDirectory()
        if (!Directory.Exists(OutputDirectory))
            Directory.CreateDirectory(OutputDirectory);
    }

    private GeneratorEngine CreateAssemblyResolver()
    {
        var coreAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var assemblyPaths = Directory.GetFiles(coreAssemblyPath!, "*.dll")
            .Where(x => !x.Contains("System.Runtime.Loader.dll")) // prevent from conflict
            .ToList();

        assemblyPaths.AddRange([SharedDomainDllPath, DomainDllPath]);

        Resolver = new PathAssemblyResolver(assemblyPaths);
        return this;
    }

    private void RenderTemplates(EntityInfo info)
    {
        foreach (var item in Templates)
        {
            var template = Template.Parse(item.Value.Content);
            if (template.HasErrors)
            {
                Console.WriteLine("Template errors:");
                foreach (var error in template.Messages)
                    Console.WriteLine(error);
                return;
            }

            var result = template.Render(info, member => member.Name);

            var targetDir = Path.Combine(OutputDirectory, info.OriginalName, item.Value.OutputFolderName);
            Directory.CreateDirectory(targetDir);

            var filePath = Path.Combine(targetDir, GetFilename(item, info));
            File.WriteAllText(filePath, result, Encoding.UTF8);

            generatedClassesCount++;
        }
    }

    private static string GetFilename(KeyValuePair<TemplateKind, TemplateInfo> template, EntityInfo info)
    {
        return template.Key switch
        {
            TemplateKind.AggregateController => $"{Enum.GetName(template.Key)!.Replace("Aggregate", info.Plural)}.cs",

            TemplateKind.LegacyAggregate => $"{Enum.GetName(template.Key)!.Replace("Aggregate", info.OriginalName)}.cs",

            _ => $"{Enum.GetName(template.Key)!.Replace("Aggregate", info.PascalCase)}.cs"
        };

        //return template.Key == TemplateKind.AggregateController
        //    ? $"{Enum.GetName(template.Key)!.Replace("Aggregate", info.Plural)}.cs"
        //    : $"{Enum.GetName(template.Key)!.Replace("Aggregate", info.PascalCase)}.cs";
    }

    //private bool IsDerivedFromEntity(Type type)
    //{
    //    var entityBaseType = sharedKernelAssembly.GetType("Dastyar.SharedKernel.Domain.Entity");

    //    while (type.BaseType != null)
    //    {
    //        if (type.BaseType.FullName == entityBaseType?.FullName)
    //            return true;

    //        type = type.BaseType;
    //    }
    //    return false;
    //}

    #endregion /Utility methods
}
