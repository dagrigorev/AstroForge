using Xunit;

namespace AstroForge.Tests;

public sealed class JulianDateTests
{
    [Fact]
    public void Jd_Of_J2000_Is_Correct()
    {
        var instant = new DateTimeOffset(2000, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var jd = JulianDate.FromDateTimeOffset(instant);
        Assert.Equal(2451545.0d, jd.Value, 6);
    }

    [Fact]
    public void Formatter_Uses_Sign_Notation()
    {
        var text = AstroFormatter.FormatLongitude(new Angle(123.456), 2);
        Assert.Contains("Leo", text);
    }
}
