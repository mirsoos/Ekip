using Ekip.Api.Grpc.Gamification;
using Ekip.Api.Grpc.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Subscription;
using Ekip.Domain.Entities.User;
using Ekip.Domain.Enums.Gamification;
using Ekip.Domain.Enums.User;
using Ekip.Domain.ValueObjects.Subscription;
using Ekip.Domain.ValueObjects.User;
using Ekip.IntegrationTests.Infrastructure;
using Ekip.Persistence.Context;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ekip.IntegrationTests;

public sealed class Phase1VerticalSliceTests(IntegrationTestWebAppFactory factory)
    : IClassFixture<IntegrationTestWebAppFactory>
{
    [Fact]
    public async Task FullPhase1Flow_GrpcRegister_ReferralOutbox_ForUpdate_And_DapperRead_ShouldSucceed()
    {
        using var channel = GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions { HttpClient = factory.CreateDefaultClient() });

        var userClient = new UserGrpc.UserGrpcClient(channel);
        var gamificationClient = new GamificationGrpc.GamificationGrpcClient(channel);

        // 1. Register Inviter (User A) via gRPC + Verify Redis Idempotency Replay
        var inviterIdempotencyKey = Guid.CreateVersion7().ToString("D");
        var registerInviterRequest = new RegisterUserGrpcRequest
        {
            IdempotencyKey = inviterIdempotencyKey,
            Username = "inviter_hero",
            Password = "StrongPassword123!",
            FirstName = "Ali",
            LastName = "Rezaei",
            UserGender = (int)Gender.Male,
            BirthDate = "1998-05-15",
            Email = "inviter@ekip.app"
        };

        var inviterResponse1 = await userClient.RegisterUserAsync(registerInviterRequest);
        var inviterResponse2 = await userClient.RegisterUserAsync(registerInviterRequest);

        Assert.Equal(inviterResponse1.UserId, inviterResponse2.UserId);
        var inviterId = Guid.Parse(inviterResponse1.UserId);

        // Fetch Inviter's generated ReferralCode from DB
        string inviterReferralCode;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EkipDbContext>();
            inviterReferralCode = await db.Set<UserAccount>()
                .Where(u => u.Id == inviterId)
                .Select(u => u.ReferralCode)
                .SingleAsync();
        }

        // 2. Register Referred User (User B) using Inviter's ReferralCode via gRPC
        var registerReferredRequest = new RegisterUserGrpcRequest
        {
            IdempotencyKey = Guid.CreateVersion7().ToString("D"),
            Username = "referred_friend",
            Password = "StrongPassword123!",
            FirstName = "Sara",
            LastName = "Ahmadi",
            UserGender = (int)Gender.Female,
            BirthDate = "2000-10-20",
            Email = "referred@ekip.app",
            ReferralCode = inviterReferralCode
        };

        var referredResponse = await userClient.RegisterUserAsync(registerReferredRequest);
        var referredUserId = Guid.Parse(referredResponse.UserId);

        // 3. Wait for MassTransit Outbox -> RabbitMQ -> UserRegisteredConsumer (FOR UPDATE + Ledger)
        var bonusApplied = await WaitUntilAsync(async () =>
        {
            var history = await gamificationClient.GetReputationHistoryAsync(new GetReputationHistoryGrpcRequest
            {
                UserId = inviterId.ToString("D"),
                PageSize = 10
            });

            return history.Items.Count == 1;
        }, TimeSpan.FromSeconds(15));

        Assert.True(bonusApplied, "Timed out waiting for UserRegisteredConsumer to process ReferralBonus.");

        // 4. Verify JSONB (MoodWidget, SocialLinks, PlanEntitlements) & varchar[] (Interests) Materialization
        var nowUtc = DateTime.UtcNow;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EkipDbContext>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var profile = await db.Set<UserProfile>().SingleAsync(p => p.UserId == inviterId);
            profile.SetMoodWidget(MoodWidget.Create("🔥", "Building Ekip", "#FF5733", nowUtc).Value);
            profile.ReplaceSocialLinks([SocialLink.Create("github", "https://github.com/ekip", nowUtc).Value]);
            profile.ReplaceInterests(["coding", "boardgames"], nowUtc);

            var plan = SubscriptionPlan.Create(
                code: "PRO_TEST",
                title: "Pro Plan",
                price: SubscriptionPrice.Create(99000m, "IRR").Value,
                entitlements: PlanEntitlements.Create(
                    maxActiveEkips: 5,
                    dailyAiMessagesQuota: 50,
                    canCreateRecurringEkips: true,
                    canCreateBusinessEkips: false,
                    customPerks: new Dictionary<string, string> { ["badge"] = "gold" }).Value,
                durationDays: 30).Value;

            db.Set<SubscriptionPlan>().Add(plan);
            await uow.SaveChangesAsync();

            var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
            await cache.RemoveAsync($"user:profile:{inviterId:D}");
        }

        // Verify EF Core HasJsonbConversion round-trip in a fresh scope
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EkipDbContext>();
            var loadedPlan = await db.Set<SubscriptionPlan>()
                .AsNoTracking()
                .SingleAsync(p => p.Code == "PRO_TEST");

            Assert.Equal(5, loadedPlan.Entitlements.MaxActiveEkips);
            Assert.Equal("gold", loadedPlan.Entitlements.CustomPerks!["badge"]);
        }

        // 5. Verify Dapper Read + Redis Cache-Aside via gRPC GetUserProfile
        var profileResponse = await userClient.GetUserProfileAsync(new GetUserProfileGrpcRequest
        {
            UserId = inviterId.ToString("D")
        });

        Assert.Equal("inviter_hero", profileResponse.Username);
        Assert.Equal(30, profileResponse.TotalXp);
        Assert.Equal(1, profileResponse.CurrentLevel);
        Assert.Equal(100, profileResponse.NextLevelXp);
        Assert.NotNull(profileResponse.MoodWidget);
        Assert.Equal("🔥", profileResponse.MoodWidget.MoodEmoji);
        Assert.Single(profileResponse.SocialLinks);
        Assert.Equal("github", profileResponse.SocialLinks[0].Platform);
        Assert.Equal(["coding", "boardgames"], profileResponse.Interests);

        // 6. Verify Dapper Keyset Pagination via gRPC GetReputationHistory
        var historyResponse = await gamificationClient.GetReputationHistoryAsync(new GetReputationHistoryGrpcRequest
        {
            UserId = inviterId.ToString("D"),
            PageSize = 10
        });

        Assert.Single(historyResponse.Items);
        var ledgerItem = historyResponse.Items[0];
        Assert.Equal((int)ReputationEventType.ReferralBonus, ledgerItem.EventType);
        Assert.Equal(30, ledgerItem.XpDelta);
        Assert.Equal(30, ledgerItem.XpBalanceAfter);
        Assert.Equal(5, ledgerItem.HonorDelta);
        Assert.Equal(5, ledgerItem.HonorBalanceAfter);
        Assert.Equal(referredUserId.ToString("D"), ledgerItem.ReferenceEntityId);
    }

    private static async Task<bool> WaitUntilAsync(Func<Task<bool>> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (await condition())
                return true;

            await Task.Delay(250);
        }

        return false;
    }
}