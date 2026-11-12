using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Location;

public class Country : BaseEntity, IAggregateRoot, IAuditableEntity
{
    public string NativeName { get; private set; }
    public string EnglishName { get; private set; }
    public string IsoCode { get; private set; }
    public string PhonePrefix { get; private set; }
    public string CurrencyCode { get; private set; }
    public string DefaultTimeZoneId { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private readonly List<Province> _provinces = [];
    public IReadOnlyCollection<Province> Provinces => _provinces.AsReadOnly();
}