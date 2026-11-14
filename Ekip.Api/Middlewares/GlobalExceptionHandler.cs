using Ekip.Api.Extensions;
using Ekip.Domain.Results;
using Microsoft.AspNetCore.Diagnostics;

namespace Ekip.Api.Middlewares;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    private static readonly Error InvalidRequestError = Error.Validation(
        "GENERAL.INVALID_REQUEST",
        "The request format or payload is invalid.");

    private static readonly Error UnhandledError = Error.Failure(
        "GENERAL.UNHANDLED_ERROR",
        "An unexpected error occurred while processing the request.");

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug(
                "HTTP request aborted by client for {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value);

            if (!httpContext.Response.HasStarted)
                httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;

            return true;
        }

        if (exception is BadHttpRequestException)
        {
            var badRequestProblem = InvalidRequestError.ToProblemDetails(httpContext);
            httpContext.Response.StatusCode = badRequestProblem.Status!.Value;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = badRequestProblem
            });
        }

        logger.LogError(
            exception,
            "Unhandled HTTP exception for {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path.Value);

        var problemDetails = UnhandledError.ToProblemDetails(httpContext);
        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}