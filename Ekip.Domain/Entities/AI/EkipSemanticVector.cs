using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.AI;

public class EkipSemanticVector : Entity, IAggregateRoot, IAuditableEntity
{
    public Guid EkipId { get; private set; }
    public float[] ContentVector { get; private set; } = [];
    public Guid CityId { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
}