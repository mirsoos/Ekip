namespace Ekip.Domain.ValueObjects.User;
public sealed record OAuthProviderLogin(
    string Provider,
    string ProviderUserId,
    string? Email,
    DateTime LinkedAtUtc
);