namespace AstroForge;

public readonly record struct Angle(double Degrees)
{
    public double NormalizedDegrees => AngleMath.NormalizeDegrees(Degrees);
    public double Radians => NormalizedDegrees * Math.PI / 180d;
    public ZodiacSign Sign => (ZodiacSign)(int)(NormalizedDegrees / 30d);
    public double DegreesInSign => NormalizedDegrees % 30d;

    public static Angle FromRadians(double radians) => new(radians * 180d / Math.PI);
    public Angle Normalize() => new(AngleMath.NormalizeDegrees(Degrees));
    public override string ToString() => $"{NormalizedDegrees:0.####}°";
}

public readonly record struct GeoCoordinate(double Latitude, double Longitude, double ElevationMeters = 0)
{
    public void Validate()
    {
        if (Latitude is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(Latitude), "Latitude must be in [-90, 90].");
        }

        if (Longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(Longitude), "Longitude must be in [-180, 180].");
        }
    }
}

public readonly record struct JulianDate(double Value)
{
    public static JulianDate FromDateTimeOffset(DateTimeOffset instant) => new(AstroTime.ToJulianDay(instant));
    public double DaysSinceJ2000 => Value - 2451543.5d;
    public override string ToString() => Value.ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture);
}

public sealed record ChartRequest
{
    public ChartRequest(
        DateTimeOffset Instant,
        GeoCoordinate? Location = null,
        HouseSystemKind HouseSystem = HouseSystemKind.WholeSign,
        IReadOnlyList<CelestialBody>? Bodies = null)
    {
        this.Instant = Instant;
        this.Location = Location;
        this.HouseSystem = HouseSystem;
        this.Bodies = Bodies ?? Array.Empty<CelestialBody>();
    }

    public DateTimeOffset Instant { get; init; }
    public GeoCoordinate? Location { get; init; }
    public HouseSystemKind HouseSystem { get; init; }
    public IReadOnlyList<CelestialBody> Bodies { get; init; }
}

public sealed record PlanetPosition(
    CelestialBody Body,
    Angle Longitude,
    double Latitude,
    double DistanceAu,
    double SpeedLongitudePerDay,
    ZodiacSign Sign,
    double DegreesInSign,
    EquatorialCoordinate Equatorial,
    HorizontalCoordinate? Horizontal,
    int House);

public readonly record struct EquatorialCoordinate(double RightAscension, double Declination);

public readonly record struct HorizontalCoordinate(double Azimuth, double Altitude);

public sealed record Aspect(
    CelestialBody First,
    CelestialBody Second,
    AspectKind Kind,
    double ExactAngle,
    double Orb,
    bool Applying);

public sealed record HouseSet(
    HouseSystemKind System,
    Angle Ascendant,
    Angle Midheaven,
    IReadOnlyList<Angle> Cusps)
{
    public static HouseSet Empty { get; } = new(
        HouseSystemKind.WholeSign,
        new Angle(0d),
        new Angle(0d),
        Enumerable.Range(0, 12).Select(_ => new Angle(0d)).ToArray());

    public Angle GetCusp(int house)
    {
        if (house is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(house), "House index must be between 1 and 12.");
        }

        return Cusps[house - 1];
    }
}

public sealed record NatalChart(
    ChartRequest Request,
    JulianDate JulianDate,
    IReadOnlyList<PlanetPosition> Bodies,
    HouseSet Houses,
    IReadOnlyList<Aspect> Aspects,
    Angle Ascendant,
    Angle Midheaven);

public sealed record AspectOrbProfile(
    double Conjunction,
    double Sextile,
    double Square,
    double Trine,
    double Quincunx,
    double Opposition)
{
    public static AspectOrbProfile Default { get; } = new(
        Conjunction: 8d,
        Sextile: 4d,
        Square: 6d,
        Trine: 7d,
        Quincunx: 3d,
        Opposition: 8d);

    public double GetOrb(AspectKind kind) => kind switch
    {
        AspectKind.Conjunction => Conjunction,
        AspectKind.Sextile => Sextile,
        AspectKind.Square => Square,
        AspectKind.Trine => Trine,
        AspectKind.Quincunx => Quincunx,
        AspectKind.Opposition => Opposition,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };
}

public sealed record BodyInfo(CelestialBody Body, string Name, string Symbol, string Keywords);

public sealed record SignInfo(ZodiacSign Sign, string Name, string Symbol, Element Element, Modality Modality);
