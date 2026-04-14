namespace AstroForge;

internal static class AngleMath
{
    public static double NormalizeDegrees(double degrees)
    {
        var value = degrees % 360d;
        return value < 0 ? value + 360d : value;
    }

    public static double NormalizeRadians(double radians)
    {
        var value = radians % (2d * Math.PI);
        return value < 0 ? value + 2d * Math.PI : value;
    }

    public static double ToRadians(double degrees) => degrees * Math.PI / 180d;

    public static double ToDegrees(double radians) => radians * 180d / Math.PI;

    public static double SinD(double degrees) => Math.Sin(ToRadians(degrees));

    public static double CosD(double degrees) => Math.Cos(ToRadians(degrees));

    public static double TanD(double degrees) => Math.Tan(ToRadians(degrees));

    public static double Atan2D(double y, double x) => NormalizeDegrees(ToDegrees(Math.Atan2(y, x)));

    public static double AsinD(double value) => ToDegrees(Math.Asin(value));

    public static double AcosD(double value) => ToDegrees(Math.Acos(value));

    public static double SignedAngleDifference(double fromDegrees, double toDegrees)
    {
        var difference = NormalizeDegrees(toDegrees - fromDegrees + 180d) - 180d;
        return difference < -180d ? difference + 360d : difference;
    }

    public static double AbsoluteAngularDistance(double firstDegrees, double secondDegrees) =>
        Math.Abs(SignedAngleDifference(firstDegrees, secondDegrees));
}
