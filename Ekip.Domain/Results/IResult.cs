namespace Ekip.Domain.Results;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    Error Error { get; }
}

public interface IResult<TSelf> : IResult
    where TSelf : struct, IResult<TSelf>
{
    static abstract TSelf Failure(Error error);
}