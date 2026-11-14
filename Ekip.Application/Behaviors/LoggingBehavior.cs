using System.Diagnostics;
using Ekip.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ekip.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : struct, IResult
{
    private const long SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            var response = await next();
            var elapsedMs = (long)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

            if (response.IsFailure)
            {
                if (response.Error.Type == ErrorType.Failure)
                {
                    logger.LogError(
                        "Request {RequestName} failed with internal error {ErrorCode} in {ElapsedMs}ms",
                        requestName,
                        response.Error.Code,
                        elapsedMs);
                }
                else
                {
                    logger.LogInformation(
                        "Request {RequestName} rejected with {ErrorType} ({ErrorCode}) in {ElapsedMs}ms",
                        requestName,
                        response.Error.Type,
                        response.Error.Code,
                        elapsedMs);
                }
            }
            else if (elapsedMs > SlowRequestThresholdMs)
            {
                logger.LogWarning(
                    "Slow request {RequestName} completed in {ElapsedMs}ms",
                    requestName,
                    elapsedMs);
            }
            else
            {
                logger.LogDebug(
                    "Request {RequestName} completed in {ElapsedMs}ms",
                    requestName,
                    elapsedMs);
            }

            return response;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            var elapsedMs = (long)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
            logger.LogDebug(
                "Request {RequestName} was cancelled after {ElapsedMs}ms",
                requestName,
                elapsedMs);
            throw;
        }
        catch (Exception ex)
        {
            var elapsedMs = (long)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
            logger.LogError(
                ex,
                "Unhandled exception while processing {RequestName} after {ElapsedMs}ms",
                requestName,
                elapsedMs);
            throw;
        }
    }
}