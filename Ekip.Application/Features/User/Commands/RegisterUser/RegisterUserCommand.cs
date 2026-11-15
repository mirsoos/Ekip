using Ekip.Application.Contracts;
using Ekip.Domain.Enums.User;

namespace Ekip.Application.Features.User.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    Guid IdempotencyKey,
    string Username,
    string Password,
    string FirstName,
    string LastName,
    Gender UserGender,
    DateOnly BirthDate,
    string? Email = null,
    string? PhoneNumber = null,
    Guid? CityId = null,
    string? ReferralCode = null) : ICommand<Guid>;