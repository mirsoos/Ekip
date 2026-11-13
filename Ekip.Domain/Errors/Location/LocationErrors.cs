using Ekip.Domain.Results;

namespace Ekip.Domain.Errors.Location;

public static class LocationErrors
{
    public static readonly Error InvalidLatitude = Error.Validation(
        "LOCATION.INVALID_LATITUDE",
        "Latitude must be a finite number between -90 and +90 degrees.");

    public static readonly Error InvalidLongitude = Error.Validation(
        "LOCATION.INVALID_LONGITUDE",
        "Longitude must be a finite number between -180 and +180 degrees.");
}