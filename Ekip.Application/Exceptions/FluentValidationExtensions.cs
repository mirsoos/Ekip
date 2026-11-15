using Ekip.Domain.Results;
using FluentValidation;

namespace Ekip.Application.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Error error)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(error);

        return rule
            .WithErrorCode(error.Code)
            .WithMessage(error.Message);
    }
}