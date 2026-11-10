using Ekip.Domain.Enums.Ekip;

namespace Ekip.Domain.ValueObjects.Ekip;

public sealed record EkipJoinCriteria
{
    public EkipTargetGender TargetGender { get; init; } = EkipTargetGender.Any;
    public IReadOnlyCollection<HostComposition> AllowedJoinTypes { get; init; } = [];
    public int? MinAge { get; init; }
    public int? MaxAge { get; init; }
    public int? MinLevel { get; init; }
    public decimal? MinPublicScore { get; init; }
    public int? MinHonor { get; init; }
    public bool PremiumOnly { get; init; } = false;
    public bool IsAutoAccept { get; init; } = false;
}