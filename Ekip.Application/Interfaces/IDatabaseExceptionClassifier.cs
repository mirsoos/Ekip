namespace Ekip.Application.Interfaces;

public interface IDatabaseExceptionClassifier
{
    string? TryGetUniqueConstraintName(Exception exception);
}