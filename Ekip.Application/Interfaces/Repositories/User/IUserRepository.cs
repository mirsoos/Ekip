using Ekip.Domain.Entities.User;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Application.Interfaces.Repositories.User;

public interface IUserRepository
{
    Task<bool> IsUsernameTakenAsync(Username username, CancellationToken cancellationToken = default);
    Task<bool> IsEmailTakenAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneNumberTakenAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> IsReferralCodeTakenAsync(string referralCode, CancellationToken cancellationToken = default);
    Task<Guid?> GetUserIdByReferralCodeAsync(string referralCode, CancellationToken cancellationToken = default);

    void AddAccount(UserAccount account);
    void AddProfile(UserProfile profile);
}