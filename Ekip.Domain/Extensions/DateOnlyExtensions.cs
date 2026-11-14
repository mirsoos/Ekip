namespace Ekip.Domain.Extensions;

public static class DateOnlyExtensions
{
    public static int CalculateAge(this DateOnly birthDate, DateOnly referenceDate)
    {
        var age = referenceDate.Year - birthDate.Year;
        if (birthDate > referenceDate.AddYears(-age))
            age--;

        return age;
    }
}