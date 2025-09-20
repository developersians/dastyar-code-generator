namespace Arzfy.CodeGenerator;

public sealed record EntityPropertyInfo(
    string DataType,
    string OriginalName,
    string PascalCase,
    string CamelCase,
    string XmlSummary);
