using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Location;

public class Province : BaseEntity, IAggregateRoot, IAuditableEntity
{
    public Guid CountryId { get; private set; }
    public string NativeName { get; private set; }
    public string EnglishName { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private readonly List<City> _cities = [];
    public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();
}