namespace AstroForge;

public enum CelestialBody
{
    Sun,
    Moon,
    Mercury,
    Venus,
    Mars,
    Jupiter,
    Saturn,
    Uranus,
    Neptune,
    Pluto,
    MeanNorthNode
}

public enum ZodiacSign
{
    Aries = 0,
    Taurus = 1,
    Gemini = 2,
    Cancer = 3,
    Leo = 4,
    Virgo = 5,
    Libra = 6,
    Scorpio = 7,
    Sagittarius = 8,
    Capricorn = 9,
    Aquarius = 10,
    Pisces = 11
}

public enum Element
{
    Fire,
    Earth,
    Air,
    Water
}

public enum Modality
{
    Cardinal,
    Fixed,
    Mutable
}

public enum AspectKind
{
    Conjunction,
    Sextile,
    Square,
    Trine,
    Quincunx,
    Opposition
}

public enum HouseSystemKind
{
    WholeSign,
    Equal
}
