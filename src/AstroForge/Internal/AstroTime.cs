namespace AstroForge;

internal static class AstroTime
{
    private static readonly DateTimeOffset UnixEpoch = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static double ToJulianDay(DateTimeOffset instant)
    {
        var utc = instant.ToUniversalTime();
        return (utc - UnixEpoch).TotalDays + 2440587.5d;
    }

    public static double DaysSinceJ2000(DateTimeOffset instant) => ToJulianDay(instant) - 2451543.5d;

    public static double MeanObliquityDegrees(DateTimeOffset instant)
    {
        var d = DaysSinceJ2000(instant);
        return 23.4393d - 3.563E-7d * d;
    }

    public static double LocalSiderealTimeDegrees(DateTimeOffset instant, double longitudeDegrees)
    {
        var jd = ToJulianDay(instant);
        var t = (jd - 2451545d) / 36525d;
        var gmst = 280.46061837d
                   + 360.98564736629d * (jd - 2451545d)
                   + 0.000387933d * t * t
                   - (t * t * t) / 38710000d;

        return AngleMath.NormalizeDegrees(gmst + longitudeDegrees);
    }
}
