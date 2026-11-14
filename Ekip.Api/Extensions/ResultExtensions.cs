using System.Diagnostics;
using Ekip.Domain.Results;
using Microsoft.AspNetCore.Mvc;
using DomainResult = Ekip.Domain.Results.Result;
using System.Text.Json.Serialization;

namespace Ekip.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(
        this DomainResult result,
        HttpContext? httpContext = null)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return result.Error.ToProblemActionResult(httpContext);
    }

    public static ActionResult<TValue> ToActionResult<TValue>(
        this Result<TValue> result,
        HttpContext? httpContext = null)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.Error.ToProblemActionResult(httpContext);
    }

    public static ObjectResult ToProblemActionResult(
        this Error error,
        HttpContext? httpContext = null)
    {
        var problemDetails = error.ToProblemDetails(httpContext);

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
            ContentTypes = ["application/problem+json"]
        };
    }

    public static ProblemDetails ToProblemDetails(
        this Error error,
        HttpContext? httpContext = null)
    {
        ArgumentNullException.ThrowIfNull(error);

        var (statusCode, title, typeUri) = error.Type switch
        {
            ErrorType.Validation => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"),
            ErrorType.Unauthorized => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "https://tools.ietf.org/html/rfc9110#section-15.5.2"),
            ErrorType.Forbidden => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "https://tools.ietf.org/html/rfc9110#section-15.5.4"),
            ErrorType.NotFound => (
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://tools.ietf.org/html/rfc9110#section-15.5.5"),
            ErrorType.Conflict => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://tools.ietf.org/html/rfc9110#section-15.5.10"),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Server Failure",
                "https://tools.ietf.org/html/rfc9110#section-15.6.1")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = typeUri,
            Detail = error.Message,
            Instance = httpContext?.Request.Path.Value
        };

        problemDetails.Extensions["errorCode"] = error.Code;

        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(traceId))
            problemDetails.Extensions["traceId"] = traceId;

        if (error.Metadata is { Count: > 0 })
            problemDetails.Extensions["metadata"] = error.Metadata;

        if (error is ValidationError validationError)
        {
            problemDetails.Extensions["errors"] = validationError.Errors
                .Select(e => new ValidationErrorItem(e.Code, e.Message, e.Metadata))
                .ToArray();
        }

        return problemDetails;
    }
    private sealed record ValidationErrorItem(
        string Code,
        string Message,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        IReadOnlyDictionary<string, object>? Metadata);
}