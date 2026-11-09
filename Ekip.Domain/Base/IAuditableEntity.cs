namespace Ekip.Domain.Base;
public interface IAuditableEntity : ICreatedAuditable
{
    DateTime? UpdatedAtUtc { get; }
}