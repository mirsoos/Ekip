using Ekip.Application.Caching;
using Ekip.Application.Interfaces;
using Ekip.Application.Interfaces.Repositories.Gamification;
using Ekip.Domain.Entities.Gamification;
using Ekip.Domain.Entities.User;
using Ekip.Domain.Enums.Gamification;
using Ekip.Domain.Events.User;
using Ekip.Domain.Policies;
using MassTransit;

namespace Ekip.Application.Consumers.Gamification;

public sealed class UserRegisteredConsumer(
    IGamificationRepository gamificationRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService,
    TimeProvider timeProvider) : IConsumer<UserRegisteredDomainEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredDomainEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        if (message.ReferredById is not { } referrerId)
        {
            return;
        }

        var alreadyRewarded = await gamificationRepository.LedgerEntryExistsAsync(message.UserId, ct);
        if (alreadyRewarded)
        {
            return;
        }
        var referrerSummary = await gamificationRepository.GetSummaryForUpdateAsync(referrerId, ct)
            ?? throw new InvalidOperationException($"Referrer gamification summary was not found for user '{referrerId}'.");

        var rewardResult = ReputationPolicy.GetReward(ReputationEventType.ReferralBonus);
        if (rewardResult.IsFailure)
            throw new InvalidOperationException(rewardResult.Error.Message);

        var delta = rewardResult.Value;
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var applyResult = referrerSummary.ApplyReputationEvent(
            ReputationEventType.ReferralBonus,
            delta.XpDelta,
            delta.HonorDelta,
            utcNow);

        if (applyResult.IsFailure)
            throw new InvalidOperationException(applyResult.Error.Message);

        var ledgerResult = ReputationLedger.Create(
            userId: referrerId,
            eventType: ReputationEventType.ReferralBonus,
            xpDelta: delta.XpDelta,
            xpBalanceAfter: referrerSummary.TotalXp,
            honorDelta: delta.HonorDelta,
            honorBalanceAfter: referrerSummary.TotalHonor,
            idempotencyKey: message.UserId,
            actorId: message.UserId,
            description: "Referral bonus for inviting a new user.",
            referenceEntityType: nameof(UserAccount),
            referenceEntityId: message.UserId);

        if (ledgerResult.IsFailure)
            throw new InvalidOperationException(ledgerResult.Error.Message);

        gamificationRepository.AddLedgerEntry(ledgerResult.Value);

        await unitOfWork.SaveChangesAsync(ct);

        await cacheService.RemoveAsync(RedisSchemaKey.UserProfile(referrerId), ct);
    }
}