namespace AstroForge;

public sealed class DefaultAspectAnalyzer : IAspectAnalyzer
{
    private static readonly (AspectKind Kind, double Angle)[] Candidates =
    {
        (AspectKind.Conjunction, 0d),
        (AspectKind.Sextile, 60d),
        (AspectKind.Square, 90d),
        (AspectKind.Trine, 120d),
        (AspectKind.Quincunx, 150d),
        (AspectKind.Opposition, 180d)
    };

    private readonly AspectOrbProfile _orbProfile;

    public DefaultAspectAnalyzer(AspectOrbProfile orbProfile)
    {
        _orbProfile = orbProfile ?? throw new ArgumentNullException(nameof(orbProfile));
    }

    public IReadOnlyCollection<Aspect> Analyze(IReadOnlyList<PlanetPosition> positions)
    {
        var result = new List<Aspect>();

        for (var i = 0; i < positions.Count; i++)
        {
            for (var j = i + 1; j < positions.Count; j++)
            {
                var first = positions[i];
                var second = positions[j];
                var separation = AngleMath.AbsoluteAngularDistance(first.Longitude.Degrees, second.Longitude.Degrees);

                foreach (var candidate in Candidates)
                {
                    var orb = Math.Abs(separation - candidate.Angle);
                    if (orb > _orbProfile.GetOrb(candidate.Kind))
                    {
                        continue;
                    }

                    var currentDiff = Math.Abs(separation - candidate.Angle);
                    var nextFirst = AngleMath.NormalizeDegrees(first.Longitude.Degrees + first.SpeedLongitudePerDay / 24d);
                    var nextSecond = AngleMath.NormalizeDegrees(second.Longitude.Degrees + second.SpeedLongitudePerDay / 24d);
                    var nextSeparation = AngleMath.AbsoluteAngularDistance(nextFirst, nextSecond);
                    var nextDiff = Math.Abs(nextSeparation - candidate.Angle);

                    result.Add(new Aspect(
                        First: first.Body,
                        Second: second.Body,
                        Kind: candidate.Kind,
                        ExactAngle: candidate.Angle,
                        Orb: orb,
                        Applying: nextDiff < currentDiff));

                    break;
                }
            }
        }

        return result
            .OrderBy(a => a.Orb)
            .ThenBy(a => a.ExactAngle)
            .ToArray();
    }
}
