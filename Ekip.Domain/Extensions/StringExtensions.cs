using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Ekip.Domain.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex(@"^[A-Z0-9_]+$", RegexOptions.CultureInvariant)]
    private static partial Regex UpperCodePattern();

    public static bool TryNormalizeCode(
        this string? value,
        int minLength,
        int maxLength,
        [NotNullWhen(true)] out string? normalized)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minLength);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, minLength);

        if (string.IsNullOrWhiteSpace(value))
        {
            normalized = null;
            return false;
        }

        var candidate = value.Trim().ToUpperInvariant();
        if (candidate.Length < minLength
            || candidate.Length > maxLength
            || !UpperCodePattern().IsMatch(candidate))
        {
            normalized = null;
            return false;
        }

        normalized = candidate;
        return true;
    }
}