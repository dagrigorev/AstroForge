namespace AstroForge;

internal static class HouseMath
{
    public static Angle CalculateAscendant(GeoCoordinate location, DateTimeOffset instant)
    {
        var epsilon = AstroTime.MeanObliquityDegrees(instant);
        var theta = AstroTime.LocalSiderealTimeDegrees(instant, location.Longitude);
        var longitude = AngleMath.Atan2D(
            -AngleMath.CosD(theta),
            AngleMath.SinD(theta) * AngleMath.CosD(epsilon) + AngleMath.TanD(location.Latitude) * AngleMath.SinD(epsilon));
        return new Angle(longitude).Normalize();
    }

    public static Angle CalculateMidheaven(GeoCoordinate location, DateTimeOffset instant)
    {
        var epsilon = AstroTime.MeanObliquityDegrees(instant);
        var theta = AstroTime.LocalSiderealTimeDegrees(instant, location.Longitude);
        var mc = AngleMath.Atan2D(AngleMath.SinD(theta) * AngleMath.CosD(epsilon), AngleMath.CosD(theta));
        return new Angle(mc).Normalize();
    }
}
