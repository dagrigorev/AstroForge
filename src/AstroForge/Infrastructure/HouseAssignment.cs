namespace AstroForge;

internal static class HouseAssignment
{
    public static int DetermineHouse(Angle longitude, HouseSet houses)
    {
        if (houses.Cusps.Count != 12)
        {
            return 0;
        }

        if (houses.System == HouseSystemKind.WholeSign)
        {
            var ascSign = houses.Ascendant.Sign;
            return ((int)longitude.Sign - (int)ascSign + 12) % 12 + 1;
        }

        for (var index = 0; index < 12; index++)
        {
            var start = houses.Cusps[index].NormalizedDegrees;
            var end = houses.Cusps[(index + 1) % 12].NormalizedDegrees;
            if (Contains(start, end, longitude.NormalizedDegrees))
            {
                return index + 1;
            }
        }

        return 12;
    }

    private static bool Contains(double start, double end, double value)
    {
        if (start < end)
        {
            return value >= start && value < end;
        }

        return value >= start || value < end;
    }
}
