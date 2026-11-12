using Ekip.Domain.Base;
using Ekip.Domain.ValueObjects.Ekip;

namespace Ekip.Domain.Entities.Location;

public class City : BaseEntity, IAggregateRoot, IAuditableEntity
{
    public Guid CountryId { get; private set; }
    public Guid ProvinceId { get; private set; }
    public string NativeName { get; private set; }     // مثلاً: "خرم‌آباد"
    public string EnglishName { get; private set; }    // مثلاً: "Khorramabad"
    public GeoLocation CenterPoint { get; private set; }
    public double DefaultZoomLevel { get; private set; } = 12.0;
    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
}