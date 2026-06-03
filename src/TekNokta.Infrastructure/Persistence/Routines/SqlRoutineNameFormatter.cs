using System.Text.RegularExpressions;

namespace TekNokta.Infrastructure.Persistence.Routines;

internal static partial class SqlRoutineNameFormatter
{
    public static string FormatRoutineName(string routineName)
    {
        var parts = routineName
            .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
        {
            throw new ArgumentException("Routine name cannot be empty.", nameof(routineName));
        }

        foreach (var part in parts)
        {
            if (!SqlIdentifierRegex().IsMatch(part))
            {
                throw new ArgumentException($"Invalid routine name segment: {part}", nameof(routineName));
            }
        }

        return string.Join(".", parts.Select(part => $"[{part}]"));
    }

    public static string FormatParameterName(string parameterName)
    {
        var normalizedName = parameterName.Trim().TrimStart('@');

        if (!SqlIdentifierRegex().IsMatch(normalizedName))
        {
            throw new ArgumentException($"Invalid parameter name: {parameterName}", nameof(parameterName));
        }

        return $"@{normalizedName}";
    }

    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$")]
    private static partial Regex SqlIdentifierRegex();
}
