using Ekip.Application.Extensions;
using Ekip.Domain.Errors.User;
using FluentValidation;

namespace Ekip.Application.Features.User.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithError(UserErrors.InvalidIdempotencyKey);

        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50)
            .WithError(UserErrors.InvalidUsername);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .WithError(UserErrors.InvalidPassword);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithError(UserErrors.InvalidFirstName);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .WithError(UserErrors.InvalidLastName);

        RuleFor(x => x.UserGender)
            .IsInEnum()
            .WithError(UserErrors.InvalidGender);

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithError(UserErrors.InvalidBirthDate);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithError(UserErrors.MissingContactMethod);

        RuleFor(x => x.CityId)
            .Must(id => id is null || id != Guid.Empty)
            .WithError(UserErrors.InvalidCityId);
    }
}