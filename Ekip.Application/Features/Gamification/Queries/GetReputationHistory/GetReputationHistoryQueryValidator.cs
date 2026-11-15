using Ekip.Application.Extensions;
using Ekip.Application.Helpers;
using Ekip.Domain.Errors.Gamification;
using Ekip.Domain.Errors.User;
using FluentValidation;

namespace Ekip.Application.Features.Gamification.Queries.GetReputationHistory;

public sealed class GetReputationHistoryQueryValidator : AbstractValidator<GetReputationHistoryQuery>
{
    public GetReputationHistoryQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithError(UserErrors.InvalidUserId);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithError(GamificationErrors.InvalidPageSize);

        RuleFor(x => x.Cursor)
            .Must(cursor => CursorCodec.TryDecodeId(cursor, out _))
            .WithError(GamificationErrors.InvalidCursor);
    }
}