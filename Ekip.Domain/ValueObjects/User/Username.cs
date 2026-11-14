using System.Text.RegularExpressions;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.User;

public sealed partial record Username
{
    [GeneratedRegex(@"^[a-z0-9_.]{3,50}$", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    public string Value { get; private init; } = string.Empty;

    private Username() { }

    private Username(string value) => Value = value;

    public static Result<Username> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserErrors.InvalidUsername;

        var normalized = value.Trim().ToLowerInvariant();
        if (!Pattern().IsMatch(normalized))
            return UserErrors.InvalidUsername;

        return new Username(normalized);
    }

    internal static Username Rehydrate(string value) => new(value);

    public override string ToString() => Value;
}