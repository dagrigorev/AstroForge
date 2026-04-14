namespace AstroForge;

public sealed class AstroForgeClient
{
    private readonly IEphemerisProvider _ephemerisProvider;
    private readonly IHouseCalculator _houseCalculator;
    private readonly IAspectAnalyzer _aspectAnalyzer;
    private readonly AstroForgeOptions _options;

    public AstroForgeClient(
        IEphemerisProvider ephemerisProvider,
        IHouseCalculator houseCalculator,
        IAspectAnalyzer aspectAnalyzer,
        AstroForgeOptions? options = null)
    {
        _ephemerisProvider = ephemerisProvider ?? throw new ArgumentNullException(nameof(ephemerisProvider));
        _houseCalculator = houseCalculator ?? throw new ArgumentNullException(nameof(houseCalculator));
        _aspectAnalyzer = aspectAnalyzer ?? throw new ArgumentNullException(nameof(aspectAnalyzer));
        _options = options ?? AstroForgeOptions.Default;
    }

    public static AstroForgeClient CreateDefault(AstroForgeOptions? options = null)
    {
        var resolvedOptions = options ?? AstroForgeOptions.Default;
        return new AstroForgeClient(
            new AnalyticalEphemerisProvider(),
            resolvedOptions.HouseSystem switch
            {
                HouseSystemKind.Equal => new EqualHouseCalculator(),
                _ => new WholeSignHouseCalculator()
            },
            new DefaultAspectAnalyzer(resolvedOptions.AspectOrbs),
            resolvedOptions);
    }

    public NatalChart CreateChart(ChartRequest request)
    {
        var instant = request.Instant;
        var jd = JulianDate.FromDateTimeOffset(instant);
        var bodiesToUse = request.Bodies.Count > 0 ? request.Bodies : _options.DefaultBodies;

        var rawPlacements = bodiesToUse
            .Select(body => _ephemerisProvider.GetPosition(body, instant, request.Location))
            .OrderBy(p => p.Longitude.Degrees)
            .ToArray();

        var houseCalculator = request.HouseSystem == _options.HouseSystem
            ? _houseCalculator
            : request.HouseSystem switch
            {
                HouseSystemKind.Equal => new EqualHouseCalculator(),
                _ => new WholeSignHouseCalculator()
            };

        var houses = request.Location is null
            ? HouseSet.Empty
            : houseCalculator.Calculate(request.Location, instant);

        var placements = rawPlacements
            .Select(position => position with
            {
                House = request.Location is null
                    ? 0
                    : HouseAssignment.DetermineHouse(position.Longitude, houses)
            })
            .ToArray();

        var aspects = _aspectAnalyzer.Analyze(placements).ToArray();

        return new NatalChart(
            Request: request,
            JulianDate: jd,
            Bodies: placements,
            Houses: houses,
            Aspects: aspects,
            Ascendant: houses.Ascendant,
            Midheaven: houses.Midheaven);
    }
}
