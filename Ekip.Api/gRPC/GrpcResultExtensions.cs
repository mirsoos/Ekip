using System.Text.Json;
using Ekip.Domain.Results;
using Grpc.Core;
using DomainResult = Ekip.Domain.Results.Result;

namespace Ekip.Api.gRPC;

public static class GrpcResultExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void EnsureSuccess(this DomainResult result)
    {
        if (result.IsFailure)
            throw result.Error.ToRpcException();
    }

    public static TValue GetValueOrThrowRpc<TValue>(this Result<TValue> result)
    {
        if (result.IsFailure)
            throw result.Error.ToRpcException();

        return result.Value;
    }

    public static RpcException ToRpcException(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCode.InvalidArgument,
            ErrorType.NotFound => StatusCode.NotFound,
            ErrorType.Conflict => StatusCode.FailedPrecondition,
            ErrorType.Unauthorized => StatusCode.Unauthenticated,
            ErrorType.Forbidden => StatusCode.PermissionDenied,
            _ => StatusCode.Internal
        };

        var trailers = new Metadata
        {
            { "x-error-code", error.Code }
        };

        if (error.Metadata is { Count: > 0 })
        {
            trailers.Add("x-error-metadata", JsonSerializer.Serialize(error.Metadata, JsonOptions));
        }

        if (error is ValidationError validationError)
        {
            var errors = validationError.Errors
                .Select(e => new
                {
                    code = e.Code,
                    message = e.Message,
                    metadata = e.Metadata
                })
                .ToArray();

            trailers.Add("x-errors-json", JsonSerializer.Serialize(errors, JsonOptions));
        }

        return new RpcException(new Status(statusCode, error.Message), trailers);
    }
}