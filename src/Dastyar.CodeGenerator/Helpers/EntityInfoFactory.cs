using Dastyar.CodeGenerator;
using System.Reflection;

public static class EntityInfoFactory
{
    private static XmlCommentReader commentReader;

    public static void SetXmlCommentReader(XmlCommentReader reader)
    {
        commentReader = reader;
    }

    public static EntityInfo FromType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var original = type.Name;
        var pascal = NamingUtils.ToPascalCaseFromUpperSnake(original);
        var camel = NamingUtils.ToCamelCaseFromUpperSnake(original);
        var plural = NamingUtils.ToPluralFromPascal(pascal);
        var camelPlural = NamingUtils.ToPluralFromCamel(camel);
        var ns = type.Namespace?.Split('.').Last() ?? "Unknown";

        // Getting public properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p =>
            {
                var propType = GetTypeName(p.PropertyType);

                return new EntityPropertyInfo(
                    DataType: propType,
                    OriginalName: p.Name,
                    PascalCase: NamingUtils.ToPascalCaseFromUpperSnake(p.Name),
                    CamelCase: NamingUtils.ToCamelCaseFromUpperSnake(p.Name),
                    XmlSummary: commentReader.GetSummary(p) ?? "");
            })
            .ToList();

        // Remove navigation properties
        properties
            .RemoveAll(x => x.OriginalName.EndsWith("Entities") ||
                            x.OriginalName.EndsWith("Entity"));

        return new EntityInfo(
            original,
            pascal,
            camel,
            plural,
            camelPlural,
            ns,
            properties);
    }

    private static string GetTypeName(Type type)
    {
        // Checking nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1")
        {
            var innerType = type.GetGenericArguments()[0];
            return $"{GetTypeName(innerType)}?";
        }

        // Checking promary types
        return type.FullName switch
        {
            "System.Int32" => "int",
            "System.Int16" => "short",
            "System.Int64" => "long",
            "System.Single" => "float",
            "System.Double" => "double",
            "System.Decimal" => "decimal",
            "System.Boolean" => "bool",
            "System.String" => "string",
            "System.Char" => "char",
            "System.Object" => "object",
            "System.Byte" => "byte",
            "System.Byte[]" => "byte[]",
            "System.SByte" => "sbyte",
            "System.UInt32" => "uint",
            "System.UInt16" => "ushort",
            "System.UInt64" => "ulong",
            _ when type.IsGenericType => GetGenericTypeName(type),
            _ => type.Name
        };
    }

    private static string GetGenericTypeName(Type type)
    {
        var typeName = type.Name.Split('`')[0];
        var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
        return $"{typeName}<{genericArgs}>";
    }
}