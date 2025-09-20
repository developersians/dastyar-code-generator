using System.Reflection;
using System.Xml.Linq;

namespace Arzfy.CodeGenerator;

public class XmlCommentReader
{
    private readonly XDocument _xml;
    private readonly Assembly _assembly;

    public XmlCommentReader(Assembly assembly)
    {
        _assembly = assembly;
        var xmlPath = Path.ChangeExtension(assembly.Location, ".xml");
        if (!File.Exists(xmlPath))
            throw new FileNotFoundException($"XML doc not found: {xmlPath}. You have to set `GenerateDocumentationFile` to true in your project");

        _xml = XDocument.Load(xmlPath);
    }

    public string? GetSummary(Type type)
    {
        var memberName = $"T:{type.FullName}";
        return GetSummaryByMemberName(memberName);
    }

    public string? GetSummary(PropertyInfo prop)
    {
        var memberName = $"P:{prop.DeclaringType!.FullName}.{prop.Name}";
        return GetSummaryByMemberName(memberName);
    }

    private string? GetSummaryByMemberName(string memberName)
    {
        var element = _xml.Descendants("member")
                          .FirstOrDefault(e => e.Attribute("name")?.Value == memberName);

        return element?.Element("summary")?.Value.Trim();
        //return FormatSummary(element?.Element("summary")?.Value.Trim());
    }

    /// <summary>
    /// Fix line-endings
    /// </summary>
    /// <param name="summary"></param>
    /// <returns></returns>
    private static string? FormatSummary(string? summary)
    {
        if (string.IsNullOrWhiteSpace(summary))
            return null;

        var lines = summary
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Split('\n')
            .Select(line => "/// " + line.Trim());

        return string.Join(Environment.NewLine, lines);
    }
}

