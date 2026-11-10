using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;

namespace Ekip.Domain.Entities.AI;

public class UserIdentityVector : Entity, IAuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public float[] PersonalityVector { get; private set; } = [];
    public string? AiPersonaSummary { get; private set; }
    public Guid? CityId { get; private set; }
    public Gender UserGender { get; private set; }
    public int BirthYear { get; private set; }
    public int CurrentLevel { get; private set; }
    public int TotalHonor { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
}