using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.User;

public sealed record OAuthProviderLogin
{
    public string Provider { get; private init; } = string.Empty;
    public string ProviderUserId { get; private init; } = string.Empty;
    public string? Email { get; private init; }
    public DateTime LinkedAtUtc { get; private init; }

    private OAuthProviderLogin() { }

    private OAuthProviderLogin(
        string provider,
        string providerUserId,
        string? email,
        DateTime linkedAtUtc)
    {
        Provider = provider;
        ProviderUserId = providerUserId;
        Email = email;
        LinkedAtUtc = linkedAtUtc;
    }

    public static Result<OAuthProviderLogin> Create(
        string provider,
        string providerUserId,
        string? email,
        DateTime linkedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerUserId))
            return UserErrors.InvalidOAuthProvider;

        var normalizedProvider = provider.Trim().ToLowerInvariant();
        var normalizedProviderUserId = providerUserId.Trim();

        string? normalizedEmail = null;
        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Domain.ValueObjects.User.Email.Create(email);
            if (emailResult.IsFailure)
                return emailResult.Error;

            normalizedEmail = emailResult.Value.Value;
        }

        return new OAuthProviderLogin(normalizedProvider, normalizedProviderUserId, normalizedEmail, linkedAtUtc);
    }
}