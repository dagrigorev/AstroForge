namespace AstroForge;

public interface IEphemerisProvider
{
    PlanetPosition GetPosition(CelestialBody body, DateTimeOffset instant, GeoCoordinate? location = null);
}

public interface IHouseCalculator
{
    HouseSet Calculate(GeoCoordinate? location, DateTimeOffset instant);
}

public interface IAspectAnalyzer
{
    IReadOnlyCollection<Aspect> Analyze(IReadOnlyList<PlanetPosition> positions);
}
