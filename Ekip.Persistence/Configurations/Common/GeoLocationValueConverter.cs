using Ekip.Domain.ValueObjects.Ekip;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace Ekip.Persistence.Configurations.Common;

public sealed class GeoLocationValueConverter : ValueConverter<GeoLocation, Point>
{
    private static readonly GeometryFactory GeometryFactory =
        NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public GeoLocationValueConverter()
        : base(
            geo => GeometryFactory.CreatePoint(new Coordinate(geo.Longitude, geo.Latitude)),
            point => new GeoLocation(point.Y, point.X))
    {
    }
}