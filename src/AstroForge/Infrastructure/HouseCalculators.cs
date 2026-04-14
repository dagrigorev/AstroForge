namespace AstroForge;

public sealed class WholeSignHouseCalculator : IHouseCalculator
{
    public HouseSet Calculate(GeoCoordinate? location, DateTimeOffset instant)
    {
        var coordinate = location ?? throw new InvalidOperationException("A location is required to calculate houses.");
        coordinate.Validate();

        var ascendant = HouseMath.CalculateAscendant(coordinate, instant);
        var midheaven = HouseMath.CalculateMidheaven(coordinate, instant);
        var firstSignStart = Math.Floor(ascendant.NormalizedDegrees / 30d) * 30d;
        var cusps = Enumerable.Range(0, 12)
            .Select(index => new Angle(firstSignStart + (index * 30d)).Normalize())
            .ToArray();

        return new HouseSet(HouseSystemKind.WholeSign, ascendant, midheaven, cusps);
    }
}

public sealed class EqualHouseCalculator : IHouseCalculator
{
    public HouseSet Calculate(GeoCoordinate? location, DateTimeOffset instant)
    {
        var coordinate = location ?? throw new InvalidOperationException("A location is required to calculate houses.");
        coordinate.Validate();

        var ascendant = HouseMath.CalculateAscendant(coordinate, instant);
        var midheaven = HouseMath.CalculateMidheaven(coordinate, instant);
        var cusps = Enumerable.Range(0, 12)
            .Select(index => new Angle(ascendant.NormalizedDegrees + (index * 30d)).Normalize())
            .ToArray();

        return new HouseSet(HouseSystemKind.Equal, ascendant, midheaven, cusps);
    }
}
