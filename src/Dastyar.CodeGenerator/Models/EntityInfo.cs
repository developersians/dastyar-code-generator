using Dastyar.CodeGenerator;

public record EntityInfo(
    ///<summary>
    /// COMPANY
    /// </summary>
    string OriginalName,

    ///<summary>
    /// Company
    /// </summary>
    string PascalCase,

    ///<summary>
    /// company
    /// </summary>
    string CamelCase,

    ///<summary>
    /// Companies
    /// </summary>
    string Plural,

    ///<summary>
    /// companies
    /// </summary>
    string CamelPlural,

    ///<summary>
    /// Dastyar.Domain
    /// </summary>
    string Namespace,

    IEnumerable<EntityPropertyInfo> Properties);