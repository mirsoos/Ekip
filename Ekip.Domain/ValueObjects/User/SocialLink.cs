using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.User;

public sealed record SocialLink
{
    public string Platform { get; private init; } = string.Empty;
    public string Url { get; private init; } = string.Empty;
    public DateTime LinkedAtUtc { get; private init; }

    private SocialLink() { }

    private SocialLink(string platform, string url, DateTime linkedAtUtc)
    {
        Platform = platform;
        Url = url;
        LinkedAtUtc = linkedAtUtc;
    }

    public static Result<SocialLink> Create(string platform, string url, DateTime linkedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(platform) || string.IsNullOrWhiteSpace(url))
            return UserErrors.InvalidSocialLink;

        var normalizedPlatform = platform.Trim().ToLowerInvariant();
        var trimmedUrl = url.Trim();

        if (normalizedPlatform.Length > 30
            || trimmedUrl.Length > 300
            || !Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return UserErrors.InvalidSocialLink;
        }

        return new SocialLink(normalizedPlatform, trimmedUrl, linkedAtUtc);
    }
}