namespace AstroForge;

public static class AstroFormatter
{
    private static readonly string[] SignAbbreviations =
    {
        "Ari", "Tau", "Gem", "Can", "Leo", "Vir", "Lib", "Sco", "Sag", "Cap", "Aqu", "Pis"
    };

    public static string FormatLongitude(Angle angle, int decimals = 2)
    {
        var normalized = angle.Normalize();
        var sign = normalized.Sign;
        var signDegrees = normalized.DegreesInSign;
        return $"{signDegrees.ToString($"0.{new string('0', decimals)}", System.Globalization.CultureInfo.InvariantCulture)} {SignAbbreviations[(int)sign]}";
    }

    public static string FormatDegrees(Angle angle, int decimals = 2) =>
        angle.NormalizedDegrees.ToString($"0.{new string('0', decimals)}°", System.Globalization.CultureInfo.InvariantCulture);
}
