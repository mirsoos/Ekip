using Ekip.Domain.Results;
using FluentValidation;
using MediatR;

namespace Ekip.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : struct, IResult<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        List<Error>? errors = null;

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            if (result.IsValid)
                continue;

            errors ??= [];

            foreach (var failure in result.Errors)
            {
                var hasCustomCode = !string.IsNullOrWhiteSpace(failure.ErrorCode)
                    && failure.ErrorCode.Contains('.');

                var code = hasCustomCode
                    ? failure.ErrorCode
                    : "GENERAL.VALIDATION_ERROR";

                var error = Error.Validation(code, failure.ErrorMessage);

                if (!string.IsNullOrWhiteSpace(failure.PropertyName))
                    error = error.WithMetadata("field", failure.PropertyName);

                if (!hasCustomCode && !string.IsNullOrWhiteSpace(failure.ErrorCode))
                    error = error.WithMetadata("validator", failure.ErrorCode);

                errors.Add(error);
            }
        }

        if (errors is null)
            return await next();

        return TResponse.Failure(new ValidationError(errors));
    }
}