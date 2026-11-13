namespace Ekip.Domain.Results;

public readonly struct Result : IResult<Result>, IEquatable<Result>
{
    private readonly bool _isSuccess;
    private readonly Error? _error;

    private Result(bool isSuccess, Error? error)
    {
        _isSuccess = isSuccess;
        _error = error;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    public Error Error => _isSuccess
        ? Error.None
        : _error ?? throw new InvalidOperationException("Cannot access Error on an uninitialized Result.");

    public static Result Success() => new(true, null);

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        if (error == Error.None)
            throw new ArgumentException("A failed result cannot contain Error.None.", nameof(error));

        return new(false, error);
    }

    public static Result<TValue> Success<TValue>(TValue value) =>
        Result<TValue>.Success(value);

    public static Result<TValue> Failure<TValue>(Error error) =>
        Result<TValue>.Failure(error);

    public static Result<TValue> FromNullable<TValue>(TValue? value, Error? error = null) =>
        value is not null ? Success(value) : Failure<TValue>(error ?? Error.NullValue);

    public static Result Combine(params ReadOnlySpan<Result> results)
    {
        List<Error>? errors = null;

        foreach (var result in results)
        {
            if (result.IsSuccess)
                continue;

            errors ??= [];
            if (result.Error is ValidationError validationError)
                errors.AddRange(validationError.Errors);
            else
                errors.Add(result.Error);
        }

        return errors switch
        {
            null => Success(),
            { Count: 1 } => Failure(errors[0]),
            _ => Failure(new ValidationError(errors))
        };
    }

    public TOut Match<TOut>(Func<TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return _isSuccess ? onSuccess() : onFailure(Error);
    }

    public static implicit operator Result(Error error) => Failure(error);

    public bool Equals(Result other) =>
        _isSuccess == other._isSuccess && Equals(_error, other._error);

    public override bool Equals(object? obj) =>
        obj is Result other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(_isSuccess, _error);

    public static bool operator ==(Result left, Result right) => left.Equals(right);
    public static bool operator !=(Result left, Result right) => !left.Equals(right);

    public override string ToString() =>
        _isSuccess ? "Success" : $"Failure({(_error?.Code ?? "Uninitialized")})";
}

public readonly struct Result<TValue> : IResult<Result<TValue>>, IEquatable<Result<TValue>>
{
    private readonly bool _isSuccess;
    private readonly TValue? _value;
    private readonly Error? _error;

    private Result(bool isSuccess, TValue? value, Error? error)
    {
        _isSuccess = isSuccess;
        _value = value;
        _error = error;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    public TValue Value => _isSuccess
        ? _value!
        : throw new InvalidOperationException(
            $"Cannot access Value on a failed or uninitialized result ({(_error?.Code ?? "Uninitialized")}).");

    public Error Error => _isSuccess
        ? Error.None
        : _error ?? throw new InvalidOperationException("Cannot access Error on an uninitialized Result.");

    public static Result<TValue> Success(TValue value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "A successful Result<TValue> cannot contain null.");

        return new(true, value, null);
    }

    public static Result<TValue> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        if (error == Error.None)
            throw new ArgumentException("A failed result cannot contain Error.None.", nameof(error));

        return new(false, default, error);
    }

    public TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return _isSuccess ? onSuccess(_value!) : onFailure(Error);
    }

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);

    public static implicit operator Result(Result<TValue> result) =>
        result._isSuccess ? Result.Success() : Result.Failure(result.Error);

    public bool Equals(Result<TValue> other) =>
        _isSuccess == other._isSuccess
        && EqualityComparer<TValue?>.Default.Equals(_value, other._value)
        && Equals(_error, other._error);

    public override bool Equals(object? obj) =>
        obj is Result<TValue> other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(_isSuccess, _value, _error);

    public static bool operator ==(Result<TValue> left, Result<TValue> right) => left.Equals(right);
    public static bool operator !=(Result<TValue> left, Result<TValue> right) => !left.Equals(right);

    public override string ToString() =>
        _isSuccess ? $"Success({_value})" : $"Failure({(_error?.Code ?? "Uninitialized")})";
}