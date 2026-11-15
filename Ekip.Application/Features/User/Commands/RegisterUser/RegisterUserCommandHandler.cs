using Ekip.Application.Contracts;
using Ekip.Application.Interfaces;
using Ekip.Application.Interfaces.Repositories.User;
using Ekip.Domain.Entities.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Extensions;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Application.Features.User.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IReferralCodeGenerator referralCodeGenerator,
    IIdempotencyService idempotencyService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<RegisterUserCommand, Guid>
{
    private const int MaxReferralCodeAttempts = 5;
    private static readonly TimeSpan LockTtl = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan ResultCacheTtl = TimeSpan.FromMinutes(10);

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"idempotency:register:{request.IdempotencyKey:D}";
        var cachedResult = await idempotencyService.GetCachedResultAsync(cacheKey, cancellationToken);
        if (Guid.TryParseExact(cachedResult, "D", out var existingUserId))
            return existingUserId;

        var lockKey = $"idempotency:lock:register:{request.IdempotencyKey:D}";
        var ownerToken = Guid.NewGuid().ToString("N");

        if (!await idempotencyService.TryAcquireLockAsync(lockKey, ownerToken, LockTtl, cancellationToken))
            return UserErrors.RegistrationInProgress;

        try
        {
            var usernameResult = Username.Create(request.Username);
            if (usernameResult.IsFailure)
                return usernameResult.Error;

            Email? email = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailResult = Email.Create(request.Email);
                if (emailResult.IsFailure)
                    return emailResult.Error;
                email = emailResult.Value;
            }

            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(request.PhoneNumber);
                if (phoneResult.IsFailure)
                    return phoneResult.Error;
                phoneNumber = phoneResult.Value;
            }

            if (email is null && phoneNumber is null)
                return UserErrors.MissingContactMethod;

            var utcNow = timeProvider.GetUtcNow().UtcDateTime;

            if (await userRepository.IsUsernameTakenAsync(usernameResult.Value, cancellationToken))
                return UserErrors.UsernameAlreadyTaken;

            if (email is not null && await userRepository.IsEmailTakenAsync(email, cancellationToken))
                return UserErrors.EmailAlreadyTaken;

            if (phoneNumber is not null && await userRepository.IsPhoneNumberTakenAsync(phoneNumber, cancellationToken))
                return UserErrors.PhoneNumberAlreadyTaken;

            Guid? referredById = null;
            if (!string.IsNullOrWhiteSpace(request.ReferralCode))
            {
                if (!request.ReferralCode.TryNormalizeCode(4, 20, out var normalizedInputReferral))
                    return UserErrors.ReferralCodeNotFound;

                referredById = await userRepository.GetUserIdByReferralCodeAsync(
                    normalizedInputReferral,
                    cancellationToken);

                if (referredById is null)
                    return UserErrors.ReferralCodeNotFound;
            }

            string? newReferralCode = null;
            for (var attempt = 0; attempt < MaxReferralCodeAttempts; attempt++)
            {
                var candidate = referralCodeGenerator.Generate();
                if (!await userRepository.IsReferralCodeTakenAsync(candidate, cancellationToken))
                {
                    newReferralCode = candidate;
                    break;
                }
            }

            if (newReferralCode is null)
                return UserErrors.ReferralCodeGenerationFailed;

            var passwordHash = await passwordHasher.HashAsync(request.Password, cancellationToken);

            var accountResult = UserAccount.Register(
                username: usernameResult.Value,
                referralCode: newReferralCode,
                utcNow: utcNow,
                email: email,
                phoneNumber: phoneNumber,
                passwordHash: passwordHash,
                referredById: referredById);

            if (accountResult.IsFailure)
                return accountResult.Error;

            var profileResult = UserProfile.Create(
                userId: accountResult.Value.Id,
                firstName: request.FirstName,
                lastName: request.LastName,
                gender: request.UserGender,
                birthDate: request.BirthDate,
                utcNow: utcNow,
                cityId: request.CityId);

            if (profileResult.IsFailure)
                return profileResult.Error;

            userRepository.AddAccount(accountResult.Value);
            userRepository.AddProfile(profileResult.Value);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await idempotencyService.SetCachedResultAsync(
                cacheKey,
                accountResult.Value.Id.ToString("D"),
                ResultCacheTtl,
                cancellationToken);

            return accountResult.Value.Id;
        }
        finally
        {
            await idempotencyService.ReleaseLockAsync(lockKey, ownerToken, CancellationToken.None);
        }
    }
}