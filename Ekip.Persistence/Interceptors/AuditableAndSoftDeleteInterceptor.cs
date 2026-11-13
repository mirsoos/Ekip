using Ekip.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ekip.Persistence.Interceptors;

public sealed class AuditableAndSoftDeleteInterceptor(TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditAndSoftDeleteRules(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDeleteRules(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditAndSoftDeleteRules(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            if (entry.Entity is IAppendOnlyEntity && entry.State is EntityState.Modified or EntityState.Deleted)
            {
                throw new InvalidOperationException(
                    $"Entity '{entry.Metadata.ClrType.Name}' is append-only and cannot be modified or deleted.");
            }

            HandleSoftDelete(entry, utcNow);
            HandleAuditing(entry, utcNow);
        }
    }

    private static void HandleSoftDelete(EntityEntry entry, DateTime utcNow)
    {
        if (entry.State != EntityState.Deleted || entry.Entity is not ISoftDeletable)
            return;

        entry.State = EntityState.Modified;
        entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = true;
        entry.Property(nameof(ISoftDeletable.DeletedAtUtc)).CurrentValue = utcNow;

        foreach (var reference in entry.References)
        {
            if (reference.TargetEntry is { State: EntityState.Deleted } targetEntry &&
                targetEntry.Metadata.IsOwned())
            {
                targetEntry.State = EntityState.Unchanged;
            }
        }
    }

    private static void HandleAuditing(EntityEntry entry, DateTime utcNow)
    {
        if (entry.Entity is ICreatedAuditable)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(ICreatedAuditable.CreatedAtUtc)).CurrentValue = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(ICreatedAuditable.CreatedAtUtc)).IsModified = false;
            }
        }

        if (entry.Entity is IAuditableEntity && entry.State == EntityState.Modified)
        {
            entry.Property(nameof(IAuditableEntity.UpdatedAtUtc)).CurrentValue = utcNow;
        }
    }
}