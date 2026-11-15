using Ekip.Application.Interfaces.Repositories.User;
using Ekip.Domain.Entities.User;
using Ekip.Domain.ValueObjects.User;
using Ekip.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Persistence.Repositories.User;

internal sealed class UserRepository(EkipDbContext dbContext) : IUserRepository
{
    public Task<bool> IsUsernameTakenAsync(
        Username username,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Set<UserAccount>()
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public Task<bool> IsEmailTakenAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Set<UserAccount>()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public Task<bool> IsPhoneNumberTakenAsync(
        PhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Set<UserAccount>()
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public Task<bool> IsReferralCodeTakenAsync(
        string referralCode,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Set<UserAccount>()
            .AnyAsync(u => u.ReferralCode == referralCode, cancellationToken);
    }

    public Task<Guid?> GetUserIdByReferralCodeAsync(
        string referralCode,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Set<UserAccount>()
            .Where(u => u.ReferralCode == referralCode)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void AddAccount(UserAccount account)
    {
        dbContext.Set<UserAccount>().Add(account);
    }

    public void AddProfile(UserProfile profile)
    {
        dbContext.Set<UserProfile>().Add(profile);
    }
}