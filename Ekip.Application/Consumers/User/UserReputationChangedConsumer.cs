using Ekip.Application.Caching;
using Ekip.Application.Interfaces;
using Ekip.Domain.Events.Gamification;
using MassTransit;

namespace Ekip.Application.Consumers.User;

public sealed class UserReputationChangedConsumer(
    ICacheService cacheService) : IConsumer<UserReputationChangedDomainEvent>
{
    public async Task Consume(ConsumeContext<UserReputationChangedDomainEvent> context)
    {
        var cacheKey = RedisSchemaKey.UserProfile(context.Message.UserId);
        await cacheService.RemoveAsync(cacheKey, context.CancellationToken);
    }
}