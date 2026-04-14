namespace AstroForge;

public sealed class AnalyticalEphemerisProvider : IEphemerisProvider
{
    public PlanetPosition GetPosition(CelestialBody body, DateTimeOffset instant, GeoCoordinate? location = null)
    {
        if (location is { } coordinate)
        {
            coordinate.Validate();
        }

        var raw = CalculateVector(body, instant);
        var future = CalculateVector(body, instant.AddHours(12));
        var past = CalculateVector(body, instant.AddHours(-12));
        var speed = AngleMath.SignedAngleDifference(past.Longitude, future.Longitude);

        var longitude = new Angle(raw.Longitude).Normalize();
        var equatorial = ToEquatorial(raw.Longitude, raw.Latitude, instant);
        HorizontalCoordinate? horizontal =
            location is null ? null : ToHorizontal(equatorial, location.Value, instant);
        var house = 0;

        return new PlanetPosition(
            Body: body,
            Longitude: longitude,
            Latitude: raw.Latitude,
            DistanceAu: raw.Radius,
            SpeedLongitudePerDay: speed,
            Sign: longitude.Sign,
            DegreesInSign: longitude.DegreesInSign,
            Equatorial: equatorial,
            Horizontal: horizontal,
            House: house);
    }

    private static EclipticVector CalculateVector(CelestialBody body, DateTimeOffset instant)
    {
        return body switch
        {
            CelestialBody.Sun => CalculateSun(instant),
            CelestialBody.Moon => CalculateMoon(instant),
            CelestialBody.Mercury => CalculatePlanet(instant, CelestialBody.Mercury),
            CelestialBody.Venus => CalculatePlanet(instant, CelestialBody.Venus),
            CelestialBody.Mars => CalculatePlanet(instant, CelestialBody.Mars),
            CelestialBody.Jupiter => CalculatePlanet(instant, CelestialBody.Jupiter),
            CelestialBody.Saturn => CalculatePlanet(instant, CelestialBody.Saturn),
            CelestialBody.Uranus => CalculatePlanet(instant, CelestialBody.Uranus),
            CelestialBody.Neptune => CalculatePlanet(instant, CelestialBody.Neptune),
            CelestialBody.Pluto => CalculatePlanet(instant, CelestialBody.Pluto),
            CelestialBody.MeanNorthNode => CalculateMeanNode(instant),
            _ => throw new ArgumentOutOfRangeException(nameof(body), body, null)
        };
    }

    private static EclipticVector CalculateSun(DateTimeOffset instant)
    {
        var earth = CalculateHeliocentricBody(GetElements(CelestialBody.Sun, AstroTime.DaysSinceJ2000(instant)));
        return new EclipticVector(-earth.X, -earth.Y, -earth.Z);
    }

    private static EclipticVector CalculatePlanet(DateTimeOffset instant, CelestialBody body)
    {
        var d = AstroTime.DaysSinceJ2000(instant);
        var earthHelio = CalculateHeliocentricBody(GetElements(CelestialBody.Sun, d));
        var planetHelio = CalculateHeliocentricBody(GetElements(body, d));
        return new EclipticVector(
            planetHelio.X - earthHelio.X,
            planetHelio.Y - earthHelio.Y,
            planetHelio.Z - earthHelio.Z);
    }

    private static EclipticVector CalculateMoon(DateTimeOffset instant)
    {
        var d = AstroTime.DaysSinceJ2000(instant);

        var node = 125.1228d - 0.0529538083d * d;
        var inclination = 5.1454d;
        var peri = 318.0634d + 0.1643573223d * d;
        var semiMajorAxisEarthRadii = 60.2666d;
        var eccentricity = 0.054900d;
        var meanAnomaly = 115.3654d + 13.0649929509d * d;

        var sun = CalculateSun(instant);
        var solarLongitude = sun.Longitude;

        var eccentricAnomaly = SolveKepler(meanAnomaly, eccentricity);
        var xv = semiMajorAxisEarthRadii * (Math.Cos(eccentricAnomaly) - eccentricity);
        var yv = semiMajorAxisEarthRadii * (Math.Sqrt(1 - eccentricity * eccentricity) * Math.Sin(eccentricAnomaly));
        var radius = Math.Sqrt(xv * xv + yv * yv);

        var meanLongitude = AngleMath.NormalizeDegrees(node + peri + meanAnomaly);
        var evection = 1.2739d * AngleMath.SinD(2d * (meanLongitude - solarLongitude) - meanAnomaly);
        var annualEquation = 0.1858d * AngleMath.SinD(GetElements(CelestialBody.Sun, d).M);
        var a3 = 0.37d * AngleMath.SinD(GetElements(CelestialBody.Sun, d).M);
        var correctedAnomaly = meanAnomaly + evection - annualEquation - a3;
        var equationOfCenter = 6.2886d * AngleMath.SinD(correctedAnomaly);
        var a4 = 0.214d * AngleMath.SinD(2d * correctedAnomaly);
        var correctedLongitude = meanLongitude + evection + equationOfCenter - annualEquation + a4;
        var variation = 0.6583d * AngleMath.SinD(2d * (correctedLongitude - solarLongitude));
        var trueLongitude = correctedLongitude + variation;
        var correctedNode = node - 0.16d * AngleMath.SinD(GetElements(CelestialBody.Sun, d).M);

        var xh = radius * (AngleMath.CosD(correctedNode) * AngleMath.CosD(trueLongitude - correctedNode)
                         - AngleMath.SinD(correctedNode) * AngleMath.SinD(trueLongitude - correctedNode) * AngleMath.CosD(inclination));
        var yh = radius * (AngleMath.SinD(correctedNode) * AngleMath.CosD(trueLongitude - correctedNode)
                         + AngleMath.CosD(correctedNode) * AngleMath.SinD(trueLongitude - correctedNode) * AngleMath.CosD(inclination));
        var zh = radius * AngleMath.SinD(trueLongitude - correctedNode) * AngleMath.SinD(inclination);

        const double earthRadiusToAu = 6378.14d / 149597870.7d;
        return new EclipticVector(xh * earthRadiusToAu, yh * earthRadiusToAu, zh * earthRadiusToAu);
    }

    private static EclipticVector CalculateMeanNode(DateTimeOffset instant)
    {
        var d = AstroTime.DaysSinceJ2000(instant);
        var nodeLongitude = AngleMath.NormalizeDegrees(125.1228d - 0.0529538083d * d);
        var radians = AngleMath.ToRadians(nodeLongitude);
        return new EclipticVector(Math.Cos(radians), Math.Sin(radians), 0d);
    }

    private static EclipticVector CalculateHeliocentricBody(OrbitalElements elements)
    {
        var eccentricAnomaly = SolveKepler(elements.M, elements.E);
        var xv = elements.A * (Math.Cos(eccentricAnomaly) - elements.E);
        var yv = elements.A * (Math.Sqrt(1d - elements.E * elements.E) * Math.Sin(eccentricAnomaly));
        var trueAnomaly = AngleMath.Atan2D(yv, xv);
        var radius = Math.Sqrt((xv * xv) + (yv * yv));

        var xh = radius * (AngleMath.CosD(elements.N) * AngleMath.CosD(trueAnomaly + elements.W)
                         - AngleMath.SinD(elements.N) * AngleMath.SinD(trueAnomaly + elements.W) * AngleMath.CosD(elements.I));
        var yh = radius * (AngleMath.SinD(elements.N) * AngleMath.CosD(trueAnomaly + elements.W)
                         + AngleMath.CosD(elements.N) * AngleMath.SinD(trueAnomaly + elements.W) * AngleMath.CosD(elements.I));
        var zh = radius * AngleMath.SinD(trueAnomaly + elements.W) * AngleMath.SinD(elements.I);

        return new EclipticVector(xh, yh, zh);
    }

    private static double SolveKepler(double meanAnomalyDegrees, double eccentricity)
    {
        var meanAnomaly = AngleMath.ToRadians(AngleMath.NormalizeDegrees(meanAnomalyDegrees));
        var eccentricAnomaly = meanAnomaly + eccentricity * Math.Sin(meanAnomaly) * (1d + eccentricity * Math.Cos(meanAnomaly));

        for (var i = 0; i < 8; i++)
        {
            eccentricAnomaly -= (eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) - meanAnomaly)
                              / (1d - eccentricity * Math.Cos(eccentricAnomaly));
        }

        return eccentricAnomaly;
    }

    private static OrbitalElements GetElements(CelestialBody body, double d)
    {
        return body switch
        {
            CelestialBody.Sun => new(
                N: 0d,
                I: 0d,
                W: 282.9404d + 4.70935E-5d * d,
                A: 1.000000d,
                E: 0.016709d - 1.151E-9d * d,
                M: 356.0470d + 0.9856002585d * d),
            CelestialBody.Mercury => new(48.3313d + 3.24587E-5d * d, 7.0047d + 5.00E-8d * d, 29.1241d + 1.01444E-5d * d, 0.387098d, 0.205635d + 5.59E-10d * d, 168.6562d + 4.0923344368d * d),
            CelestialBody.Venus => new(76.6799d + 2.46590E-5d * d, 3.3946d + 2.75E-8d * d, 54.8910d + 1.38374E-5d * d, 0.723330d, 0.006773d - 1.302E-9d * d, 48.0052d + 1.6021302244d * d),
            CelestialBody.Mars => new(49.5574d + 2.11081E-5d * d, 1.8497d - 1.78E-8d * d, 286.5016d + 2.92961E-5d * d, 1.523688d, 0.093405d + 2.516E-9d * d, 18.6021d + 0.5240207766d * d),
            CelestialBody.Jupiter => new(100.4542d + 2.76854E-5d * d, 1.3030d - 1.557E-7d * d, 273.8777d + 1.64505E-5d * d, 5.20256d, 0.048498d + 4.469E-9d * d, 19.8950d + 0.0830853001d * d),
            CelestialBody.Saturn => new(113.6634d + 2.38980E-5d * d, 2.4886d - 1.081E-7d * d, 339.3939d + 2.97661E-5d * d, 9.55475d, 0.055546d - 9.499E-9d * d, 316.9670d + 0.0334442282d * d),
            CelestialBody.Uranus => new(74.0005d + 1.3978E-5d * d, 0.7733d + 1.9E-8d * d, 96.6612d + 3.0565E-5d * d, 19.18171d - 1.55E-8d * d, 0.047318d + 7.45E-9d * d, 142.5905d + 0.011725806d * d),
            CelestialBody.Neptune => new(131.7806d + 3.0173E-5d * d, 1.7700d - 2.55E-7d * d, 272.8461d - 6.027E-6d * d, 30.05826d + 3.313E-8d * d, 0.008606d + 2.15E-9d * d, 260.2471d + 0.005995147d * d),
            CelestialBody.Pluto => new(110.30347d, 17.14175d, 113.76329d, 39.48211675d, 0.24882730d, 14.53d + 0.003975709d * d),
            _ => throw new ArgumentOutOfRangeException(nameof(body), body, null)
        };
    }

    private static EquatorialCoordinate ToEquatorial(double longitudeDegrees, double latitudeDegrees, DateTimeOffset instant)
    {
        var epsilon = AstroTime.MeanObliquityDegrees(instant);
        var x = AngleMath.CosD(longitudeDegrees) * AngleMath.CosD(latitudeDegrees);
        var y = AngleMath.SinD(longitudeDegrees) * AngleMath.CosD(latitudeDegrees) * AngleMath.CosD(epsilon)
              - AngleMath.SinD(latitudeDegrees) * AngleMath.SinD(epsilon);
        var z = AngleMath.SinD(longitudeDegrees) * AngleMath.CosD(latitudeDegrees) * AngleMath.SinD(epsilon)
              + AngleMath.SinD(latitudeDegrees) * AngleMath.CosD(epsilon);

        var ra = AngleMath.Atan2D(y, x);
        var dec = AngleMath.AsinD(z);
        return new EquatorialCoordinate(ra, dec);
    }

    private static HorizontalCoordinate ToHorizontal(EquatorialCoordinate equatorial, GeoCoordinate location, DateTimeOffset instant)
    {
        var lst = AstroTime.LocalSiderealTimeDegrees(instant, location.Longitude);
        var hourAngle = AngleMath.NormalizeDegrees(lst - equatorial.RightAscension);
        var altitude = AngleMath.AsinD(
            AngleMath.SinD(location.Latitude) * AngleMath.SinD(equatorial.Declination)
            + AngleMath.CosD(location.Latitude) * AngleMath.CosD(equatorial.Declination) * AngleMath.CosD(hourAngle));

        var azimuth = AngleMath.Atan2D(
            -AngleMath.SinD(hourAngle),
            AngleMath.TanD(equatorial.Declination) * AngleMath.CosD(location.Latitude)
            - AngleMath.SinD(location.Latitude) * AngleMath.CosD(hourAngle));

        return new HorizontalCoordinate(azimuth, altitude);
    }
}
