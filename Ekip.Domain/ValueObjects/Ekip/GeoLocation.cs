using Ekip.Domain.Errors.Location;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.Ekip;

public sealed record GeoLocation
{
    private const double EarthRadiusMeters = 6_371_000;

    public double Latitude { get; private init; }
    public double Longitude { get; private init; }

    private GeoLocation() { }

    private GeoLocation(double latitude, double longitude)
    {
        Latitude = Math.Round(latitude, 6);
        Longitude = Math.Round(longitude, 6);
    }

    public static Result<GeoLocation> Create(double latitude, double longitude)
    {
        List<Error>? errors = null;

        if (!double.IsFinite(latitude) || latitude is < -90.0 or > 90.0)
        {
            errors ??= [];
            errors.Add(LocationErrors.InvalidLatitude);
        }

        if (!double.IsFinite(longitude) || longitude is < -180.0 or > 180.0)
        {
            errors ??= [];
            errors.Add(LocationErrors.InvalidLongitude);
        }

        if (errors is not null)
        {
            return errors.Count == 1
                ? errors[0]
                : new ValidationError(errors);
        }

        return new GeoLocation(latitude, longitude);
    }

    internal static GeoLocation Rehydrate(double latitude, double longitude) =>
        new(latitude, longitude);

    public GeoLocation CreateJitteredPoint(int minOffsetMeters = 300, int maxOffsetMeters = 500)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minOffsetMeters);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxOffsetMeters, minOffsetMeters);

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