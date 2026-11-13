using System.Linq.Expressions;
using Ekip.Domain.Base;
using Ekip.Domain.Entities.Admin;
using Ekip.Domain.Entities.AI;
using Ekip.Domain.Entities.Billing;
using Ekip.Domain.Entities.Configuration;
using Ekip.Domain.Entities.Discount;
using Ekip.Domain.Entities.Ekip;
using Ekip.Domain.Entities.EkipApplication;
using Ekip.Domain.Entities.Gamification;
using Ekip.Domain.Entities.Location;
using Ekip.Domain.Entities.Media;
using Ekip.Domain.Entities.Messaging;
using Ekip.Domain.Entities.Moderation;
using Ekip.Domain.Entities.Notification;
using Ekip.Domain.Entities.Review;
using Ekip.Domain.Entities.Subscription;
using Ekip.Domain.Entities.Support;
using Ekip.Domain.Entities.Trend;
using Ekip.Domain.Entities.User;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using EkipEntity = Ekip.Domain.Entities.Ekip.Ekip;

namespace Ekip.Persistence.Context;

public sealed class EkipDbContext(DbContextOptions<EkipDbContext> options)
    : DbContext(options)
{
    // =====================================================================
    // PHASE 1: Identity, Subscriptions, Gamification, Media, Reviews & AI
    // =====================================================================
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserRelation> UserRelations => Set<UserRelation>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<ReputationLedger> ReputationLedgers => Set<ReputationLedger>();
    public DbSet<UserGamificationSummary> UserGamificationSummaries => Set<UserGamificationSummary>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<ProfileReview> ProfileReviews => Set<ProfileReview>();
    public DbSet<UserIdentityVector> UserIdentityVectors => Set<UserIdentityVector>();

    // =====================================================================
    // PHASE 2: Ekip Engine, Recurrence, Companions, Applications & AI
    // =====================================================================
    public DbSet<EkipEntity> Ekips => Set<EkipEntity>();
    public DbSet<RecurringEkip> RecurringEkips => Set<RecurringEkip>();
    public DbSet<EkipHostCompanion> EkipHostCompanions => Set<EkipHostCompanion>();
    public DbSet<EkipMember> EkipMembers => Set<EkipMember>();
    public DbSet<EkipApplication> EkipApplications => Set<EkipApplication>();
    public DbSet<ApplicationCompanion> ApplicationCompanions => Set<ApplicationCompanion>();
    public DbSet<EkipSemanticVector> EkipSemanticVectors => Set<EkipSemanticVector>();

    // =====================================================================
    // PHASE 3: Messaging, AI Companion, Notifications, Moderation & Support
    // =====================================================================
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<AIConversation> AIConversations => Set<AIConversation>();
    public DbSet<AIMessage> AIMessages => Set<AIMessage>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AbuseReport> AbuseReports => Set<AbuseReport>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

    // =====================================================================
    // PHASE 4: Geography, Wallet, Payments & Device Sessions
    // =====================================================================
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<UserWallet> UserWallets => Set<UserWallet>();
    public DbSet<WalletLedger> WalletLedgers => Set<WalletLedger>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<UserDeviceSession> UserDeviceSessions => Set<UserDeviceSession>();

    // =====================================================================
    // PHASE 5: Isolated Admin, Bookmarks, Trends & Settings
    // =====================================================================
    public DbSet<AdminAccount> AdminAccounts => Set<AdminAccount>();
    public DbSet<AdminSession> AdminSessions => Set<AdminSession>();
    public DbSet<AdminAuditLog> AdminAuditLogs => Set<AdminAuditLog>();
    public DbSet<SavedEkip> SavedEkips => Set<SavedEkip>();
    public DbSet<CityTrendSnapshot> CityTrendSnapshots => Set<CityTrendSnapshot>();
    public DbSet<SearchQueryLog> SearchQueryLogs => Set<SearchQueryLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ۱. فعال‌سازی اکستنشن‌های PostgreSQL
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.HasPostgresExtension("vector");

        // ۲. اعمال خودکار تمام ۴۱ فایل IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EkipDbContext).Assembly);

        // ۳. جداول بومی Transactional Outbox / Inbox برای MassTransit
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        // ۴. فیلتر سراسری Soft Delete روی تمامی موجودیت‌های ISoftDeletable
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    /// <summary>
    /// خواندن غیرمخرب DomainEventها از موجودیت‌های ردیابی‌شده برای ثبت در Outbox پیش از Commit نهایی.
    /// </summary>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker
            .Entries<Entity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
    }

    /// <summary>
    /// پاک‌سازی DomainEventها صرفاً پس از Commit موفق تراکنش جهت حفظ سلامت Retry در ExecutionStrategy.
    /// </summary>
    public void ClearDomainEvents()
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.Entity.DomainEvents.Count > 0)
            {
                entry.Entity.ClearDomainEvents();
            }
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsOwned() || !typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isDeletedProp = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(isDeletedProp), parameter);

            // نکته EF Core 10: اگر فیلتر دیگری روی این موجودیت ست شده باشد، می‌توان از Named Query Filter استفاده کرد:
            // entityType.SetQueryFilter("SoftDelete", filter);
            entityType.SetQueryFilter(filter);
        }
    }
}