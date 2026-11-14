using System.Text.RegularExpressions;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.User;

public sealed partial record PhoneNumber
{
    [GeneratedRegex(@"^\+?\d{7,19}$", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    public string Value { get; private init; } = string.Empty;

    private PhoneNumber() { }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserErrors.InvalidPhoneNumber;

        var normalized = value.Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("(", string.Empty, StringComparison.Ordinal)
            .Replace(")", string.Empty, StringComparison.Ordinal);

        if (!Pattern().IsMatch(normalized))
            return UserErrors.InvalidPhoneNumber;

        return new PhoneNumber(normalized);
    }

    internal static PhoneNumber Rehydrate(string value) => new(value);

    public override string ToString() => Value;
}