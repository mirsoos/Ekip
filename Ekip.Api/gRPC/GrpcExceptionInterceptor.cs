using Ekip.Domain.Results;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ekip.Api.gRPC;

public sealed class GrpcExceptionInterceptor(
    ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{
    private static readonly Error UnhandledError = Error.Failure(
        "GENERAL.UNHANDLED_ERROR",
        "An unexpected error occurred while processing the request.");

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            throw MapUnhandledException(ex, context);
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(requestStream, context);
        }
        catch (Exception ex)
        {
            throw MapUnhandledException(ex, context);
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context);
        }
        catch (Exception ex)
        {
            throw MapUnhandledException(ex, context);
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (Exception ex)
        {
            throw MapUnhandledException(ex, context);
        }
    }

    private RpcException MapUnhandledException(Exception ex, ServerCallContext context)
    {
        if (ex is RpcException rpcException)
            return rpcException;

        if (ex is OperationCanceledException && context.CancellationToken.IsCancellationRequested)
            return new RpcException(new Status(StatusCode.Cancelled, "The request was cancelled by the client."));

        logger.LogError(ex, "Unhandled gRPC exception for method {Method}", context.Method);
        return UnhandledError.ToRpcException();
    }
}