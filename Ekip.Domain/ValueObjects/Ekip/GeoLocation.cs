using Ekip.Domain.Exceptions;

namespace Ekip.Domain.ValueObjects.Ekip;

public sealed record GeoLocation
{
    private const double EarthRadiusMeters = 6_371_000;

    public double Latitude { get; init; }
    public double Longitude { get; init; }

    private GeoLocation() { }

    public GeoLocation(double latitude, double longitude)
    {
        if (double.IsNaN(latitude) || latitude < -90.0 || latitude > 90.0)
            throw new DomainException("عرض جغرافیایی (Latitude) باید بین -90 و +90 درجه باشد.");

        if (double.IsNaN(longitude) || longitude < -180.0 || longitude > 180.0)
            throw new DomainException("طول جغرافیایی (Longitude) باید بین -180 و +180 درجه باشد.");

        Latitude = Math.Round(latitude, 6);   // دقت ~11 سانتی‌متر (استاندارد GPS)
        Longitude = Math.Round(longitude, 6);
    }

    /// <summary>
    /// تولید نقطه عمومی روی نقشه با انحراف تصادفی (Jitter) بین حداقل و حداکثر متر مشخص‌شده
    /// جهت حفظ حریم خصوصی اکیپ‌های دارای مکان مخفی (IsDetailsPrivate = true)
    /// </summary>
    public GeoLocation CreateJitteredPoint(int minOffsetMeters = 300, int maxOffsetMeters = 500)
    {
        if (minOffsetMeters < 0 || maxOffsetMeters <= minOffsetMeters)
            throw new DomainException("بازه انحراف مکانی نامعتبر است.");

        var distanceMeters = Random.Shared.NextDouble() * (maxOffsetMeters - minOffsetMeters) + minOffsetMeters;
        var angleRadians = Random.Shared.NextDouble() * 2.0 * Math.PI;

        var deltaLat = (distanceMeters * Math.Cos(angleRadians)) / 111_320.0;
        var metersPerDegreeLon = 111_320.0 * Math.Cos(Latitude * Math.PI / 180.0);
        var deltaLon = Math.Abs(metersPerDegreeLon) > 0.0001
            ? (distanceMeters * Math.Sin(angleRadians)) / metersPerDegreeLon
            : 0.0;

        var newLat = Math.Clamp(Latitude + deltaLat, -90.0, 90.0);
        var newLon = Math.Clamp(Longitude + deltaLon, -180.0, 180.0);

        return new GeoLocation(newLat, newLon);
    }

    /// <summary>
    /// محاسبه فاصله مستقیم (Haversine) تا یک نقطه دیگر بر حسب متر (جهت بررسی شعاع حضور و غیاب Check-In)
    /// </summary>
    public double DistanceInMetersTo(GeoLocation other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var lat1 = DegreesToRadians(Latitude);
        var lat2 = DegreesToRadians(other.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);
}