using Ekip.Application.Caching;
using Ekip.Application.Contracts;
using Ekip.Application.Interfaces;
using Ekip.Application.Interfaces.Repositories.Gamification;
using Ekip.Application.Interfaces.Repositories.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Extensions;
using Ekip.Domain.Factories;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Application.Features.User.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IGamificationRepository gamificationRepository,
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
        var cacheKey = RedisSchemaKey.RegisterIdempotency(request.IdempotencyKey);
        var cachedResult = await idempotencyService.GetCachedResultAsync(cacheKey, cancellationToken);
        if (Guid.TryParseExact(cachedResult, "D", out var existingUserId))
            return existingUserId;

        var lockKey = RedisSchemaKey.RegisterLock(request.IdempotencyKey);
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

            var registrationResult = UserRegistrationFactory.Create(
                username: usernameResult.Value,
                referralCode: newReferralCode,
                passwordHash: passwordHash,
                firstName: request.FirstName,
                lastName: request.LastName,
                gender: request.UserGender,
                birthDate: request.BirthDate,
                utcNow: utcNow,
                email: email,
                phoneNumber: phoneNumber,
                cityId: request.CityId,
                referredById: referredById);

            if (registrationResult.IsFailure)
                return registrationResult.Error;

            var bundle = registrationResult.Value;

            userRepository.AddAccount(bundle.Account);
            userRepository.AddProfile(bundle.Profile);
            gamificationRepository.AddSummary(bundle.Summary);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await idempotencyService.SetCachedResultAsync(
                cacheKey,
                bundle.Account.Id.ToString("D"),
                ResultCacheTtl,
                cancellationToken);

            return bundle.Account.Id;
        }
        finally
        {
            await idempotencyService.ReleaseLockAsync(lockKey, ownerToken, CancellationToken.None);
        }
    }
}