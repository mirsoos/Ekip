namespace Ekip.Application.Exceptions;

public sealed class UniqueConstraintException(
    string? constraintName,
    Exception innerException)
    : Exception($"Database unique constraint '{constraintName ?? "unknown"}' was violated.", innerException)
{
    public string? ConstraintName { get; } = constraintName;
}