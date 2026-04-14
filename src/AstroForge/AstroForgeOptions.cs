namespace AstroForge;

public sealed record AstroForgeOptions(
    HouseSystemKind HouseSystem,
    AspectOrbProfile AspectOrbs,
    IReadOnlyList<CelestialBody> DefaultBodies)
{
    public static AstroForgeOptions Default { get; } = new(
        HouseSystem: HouseSystemKind.WholeSign,
        AspectOrbs: AspectOrbProfile.Default,
        DefaultBodies: new[]
        {
            CelestialBody.Sun,
            CelestialBody.Moon,
            CelestialBody.Mercury,
            CelestialBody.Venus,
            CelestialBody.Mars,
            CelestialBody.Jupiter,
            CelestialBody.Saturn,
            CelestialBody.Uranus,
            CelestialBody.Neptune,
            CelestialBody.Pluto,
            CelestialBody.MeanNorthNode
        });
}
