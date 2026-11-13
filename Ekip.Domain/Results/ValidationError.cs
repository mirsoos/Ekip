using System.Collections.Immutable;

namespace Ekip.Domain.Results;

public sealed record ValidationError : Error
{
    public ImmutableArray<Error> Errors { get; }

    public ValidationError(IEnumerable<Error> errors)
        : base(
            "GENERAL.VALIDATION_FAILED",
            "One or more validation errors occurred.",
            ErrorType.Validation)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var builder = ImmutableArray.CreateBuilder<Error>();

        foreach (var error in errors)
        {
            ArgumentNullException.ThrowIfNull(error);

            if (error == None)
                throw new ArgumentException("Error.None cannot be included in a ValidationError.", nameof(errors));

            if (error is ValidationError nested)
                builder.AddRange(nested.Errors);
            else
                builder.Add(error);
        }

        if (builder.Count == 0)
            throw new ArgumentException("ValidationError must contain at least one error.", nameof(errors));

        Errors = builder.ToImmutable();
    }

    public static ValidationError FromResults(params ReadOnlySpan<Result> results)
    {
        var builder = ImmutableArray.CreateBuilder<Error>();

        foreach (var result in results)
        {
            if (result.IsSuccess)
                continue;

            if (result.Error is ValidationError nested)
                builder.AddRange(nested.Errors);
            else
                builder.Add(result.Error);
        }

        if (builder.Count == 0)
            throw new ArgumentException("Cannot create ValidationError when all results succeeded.", nameof(results));

        return new ValidationError(builder.ToImmutable());
    }

    public bool Equals(ValidationError? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Errors.SequenceEqual(other.Errors);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(base.GetHashCode());
        foreach (var error in Errors)
            hash.Add(error);
        return hash.ToHashCode();
    }
}