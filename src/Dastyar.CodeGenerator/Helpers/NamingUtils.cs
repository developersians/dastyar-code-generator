using Humanizer;

namespace Dastyar.CodeGenerator;

public static class NamingUtils
{
    public static string ToCamelCaseFromUpperSnake(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return string.Empty;

        var camelCase = parts[0].ToLowerInvariant(); // lowercase for first part

        for (int i = 1; i < parts.Length; i++)
        {
            var part = parts[i].ToLowerInvariant();
            camelCase += char.ToUpperInvariant(part[0]) + part[1..];
        }

        return camelCase;
    }

    public static string ToPascalCaseFromUpperSnake(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);

        return string.Concat(parts.Select(p =>
            char.ToUpperInvariant(p[0]) + p.Substring(1).ToLowerInvariant()));
    }

    public static string ToPluralFromPascal(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return input.Pluralize();
    }

    public static string ToPluralFromCamel(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return input.Pluralize();
    }
}

