namespace Ekip.Domain.ValueObjects.Subscription;
public sealed record QuotaUsage(
    int Used,
    int Limit,
    DateTime? ResetAtUtc
);