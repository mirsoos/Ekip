using Ekip.Application.Extensions;
using Ekip.Domain.Errors.User;
using FluentValidation;

namespace Ekip.Application.Features.User.Queries.GetUserProfile;

public sealed class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithError(UserErrors.InvalidUserId);
    }
}