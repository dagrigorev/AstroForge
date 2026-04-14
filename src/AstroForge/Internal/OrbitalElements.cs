namespace AstroForge;

internal readonly record struct OrbitalElements(double N, double I, double W, double A, double E, double M);

internal readonly record struct EclipticVector(double X, double Y, double Z)
{
    public double Radius => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
    public double Longitude => AngleMath.Atan2D(Y, X);
    public double Latitude => AngleMath.AsinD(Z / Radius);
}
