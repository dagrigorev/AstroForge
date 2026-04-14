using Xunit;

namespace AstroForge.Tests;

public sealed class ChartTests
{
    [Fact]
    public void CreateChart_Produces_Bodies_And_Houses()
    {
        var client = AstroForgeClient.CreateDefault();
        var chart = client.CreateChart(new ChartRequest(
            Instant: new DateTimeOffset(1990, 5, 15, 14, 30, 0, TimeSpan.FromHours(3)),
            Location: new GeoCoordinate(55.7558, 37.6176),
            HouseSystem: HouseSystemKind.WholeSign));

        Assert.NotEmpty(chart.Bodies);
        Assert.Equal(12, chart.Houses.Cusps.Count);
        Assert.True(chart.Aspects.Count >= 0);
    }

    [Fact]
    public void WholeSignHouseCalculator_Returns_Expected_First_Cusp_Alignment()
    {
        var calculator = new WholeSignHouseCalculator();
        var houses = calculator.Calculate(new GeoCoordinate(40.7128, -74.0060), new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));
        Assert.Equal(0d, houses.GetCusp(1).NormalizedDegrees % 30d, 6);
    }
}
