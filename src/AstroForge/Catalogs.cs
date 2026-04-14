namespace AstroForge;

public static class BodyCatalog
{
    private static readonly IReadOnlyDictionary<CelestialBody, BodyInfo> Items =
        new Dictionary<CelestialBody, BodyInfo>
        {
            [CelestialBody.Sun] = new(CelestialBody.Sun, "Sun", "☉", "identity, vitality, will"),
            [CelestialBody.Moon] = new(CelestialBody.Moon, "Moon", "☽", "emotions, memory, body"),
            [CelestialBody.Mercury] = new(CelestialBody.Mercury, "Mercury", "☿", "mind, speech, trade"),
            [CelestialBody.Venus] = new(CelestialBody.Venus, "Venus", "♀", "attraction, beauty, values"),
            [CelestialBody.Mars] = new(CelestialBody.Mars, "Mars", "♂", "action, heat, conflict"),
            [CelestialBody.Jupiter] = new(CelestialBody.Jupiter, "Jupiter", "♃", "growth, law, confidence"),
            [CelestialBody.Saturn] = new(CelestialBody.Saturn, "Saturn", "♄", "structure, limits, endurance"),
            [CelestialBody.Uranus] = new(CelestialBody.Uranus, "Uranus", "♅", "change, disruption, invention"),
            [CelestialBody.Neptune] = new(CelestialBody.Neptune, "Neptune", "♆", "dreams, diffusion, ideals"),
            [CelestialBody.Pluto] = new(CelestialBody.Pluto, "Pluto", "♇", "depth, power, irreversible change"),
            [CelestialBody.MeanNorthNode] = new(CelestialBody.MeanNorthNode, "Mean North Node", "☊", "trajectory, pull, future bias")
        };

    public static BodyInfo Get(CelestialBody body) => Items[body];
}

public static class SignCatalog
{
    private static readonly IReadOnlyDictionary<ZodiacSign, SignInfo> Items =
        new Dictionary<ZodiacSign, SignInfo>
        {
            [ZodiacSign.Aries] = new(ZodiacSign.Aries, "Aries", "♈", Element.Fire, Modality.Cardinal),
            [ZodiacSign.Taurus] = new(ZodiacSign.Taurus, "Taurus", "♉", Element.Earth, Modality.Fixed),
            [ZodiacSign.Gemini] = new(ZodiacSign.Gemini, "Gemini", "♊", Element.Air, Modality.Mutable),
            [ZodiacSign.Cancer] = new(ZodiacSign.Cancer, "Cancer", "♋", Element.Water, Modality.Cardinal),
            [ZodiacSign.Leo] = new(ZodiacSign.Leo, "Leo", "♌", Element.Fire, Modality.Fixed),
            [ZodiacSign.Virgo] = new(ZodiacSign.Virgo, "Virgo", "♍", Element.Earth, Modality.Mutable),
            [ZodiacSign.Libra] = new(ZodiacSign.Libra, "Libra", "♎", Element.Air, Modality.Cardinal),
            [ZodiacSign.Scorpio] = new(ZodiacSign.Scorpio, "Scorpio", "♏", Element.Water, Modality.Fixed),
            [ZodiacSign.Sagittarius] = new(ZodiacSign.Sagittarius, "Sagittarius", "♐", Element.Fire, Modality.Mutable),
            [ZodiacSign.Capricorn] = new(ZodiacSign.Capricorn, "Capricorn", "♑", Element.Earth, Modality.Cardinal),
            [ZodiacSign.Aquarius] = new(ZodiacSign.Aquarius, "Aquarius", "♒", Element.Air, Modality.Fixed),
            [ZodiacSign.Pisces] = new(ZodiacSign.Pisces, "Pisces", "♓", Element.Water, Modality.Mutable)
        };

    public static SignInfo Get(ZodiacSign sign) => Items[sign];
}
