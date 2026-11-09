namespace Ekip.Domain.ValueObjects.User;
public sealed record SocialLink(
    string Platform,
    string Url,
    DateTime LinkedAtUtc
);