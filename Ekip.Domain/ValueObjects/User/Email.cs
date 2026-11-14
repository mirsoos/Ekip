using System.Net.Mail;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.User;

public sealed record Email
{
    public string Value { get; private init; } = string.Empty;

    private Email() { }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserErrors.InvalidEmail;

        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Length > 256 || !MailAddress.TryCreate(normalized, out var parsed) || parsed.Address != normalized)
            return UserErrors.InvalidEmail;

        var atIndex = normalized.IndexOf('@');
        if (atIndex < 0 || normalized.IndexOf('.', atIndex) < 0)
            return UserErrors.InvalidEmail;

        return new Email(normalized);
    }

    internal static Email Rehydrate(string value) => new(value);

    public override string ToString() => Value;
}