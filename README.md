# AstroForge

**AstroForge** is a modern .NET library for building astrology-oriented and astronomy-inspired application scenarios: natal charts, chart visualizations, educational products, analytical dashboards, astrographic UI, backend APIs, and internal domain engines.

It is intentionally designed **not** as another thin wrapper around legacy code, but as a **clean new foundation** for .NET 8 and later:

- with a strongly typed domain model,
- with a predictable public API,
- with an extensible architecture,
- without procedural C-style ergonomics,
- without global flags and hidden mutable state,
- without dragging old compromises into a new codebase.

---

## Why AstroForge exists

Older astrology libraries tend to suffer from the same problems over and over again:

- the API grew out of C or C++ and feels unnatural in modern C#;
- the public model is full of flags, arrays, magic numbers, and procedural calls;
- calculations are hard to embed into a proper domain architecture;
- it is hard to swap the ephemeris source;
- it is hard to test;
- it is hard to evolve the library as a real NuGet package for modern .NET applications.

**AstroForge** exists to solve exactly that.

It is meant for cases where you need more than “just give me planetary degrees.” It is built to provide a **usable product foundation** for:

- a website or SaaS with astrological calculations;
- a backend API for a mobile app;
- a natal chart generator;
- a visual astrographic interface;
- a domain engine for recommendations and interpretations;
- an internal library that can be extended over time.

In plain terms: **AstroForge is not just a calculator. It is a baseline SDK for astrology applications on .NET.**

---

## What the library does

In its current version, AstroForge provides:

- natal chart creation from date, time, and coordinates;
- ecliptic longitude calculation for supported celestial bodies;
- zodiac sign detection and degree-within-sign calculation;
- equatorial coordinate calculation;
- horizontal coordinate calculation when a location is available;
- house calculation;
- aspect detection between bodies;
- body and sign catalogs;
- formatting helpers for astrological coordinates;
- a clean object model suitable for UI, API, and business logic integration.

Supported concepts in the current implementation:

- **Celestial bodies**: `Sun`, `Moon`, `Mercury`, `Venus`, `Mars`, `Jupiter`, `Saturn`, `Uranus`, `Neptune`, `Pluto`, `MeanNorthNode`
- **House systems**: `WholeSign`, `Equal`
- **Aspects**: `Conjunction`, `Sextile`, `Square`, `Trine`, `Quincunx`, `Opposition`
- **Typed models**: `Angle`, `GeoCoordinate`, `JulianDate`, `ChartRequest`, `NatalChart`, `PlanetPosition`, `Aspect`, `HouseSet`

---

## What the library deliberately does not do

It is important to be honest about the current scope.

The built-in `AnalyticalEphemerisProvider` is an **embedded analytical provider with moderate precision**, suitable for:

- MVPs;
- product prototypes;
- user interfaces;
- visualizations;
- education;
- internal analytical tools;
- domain logic where API structure matters more than observatory-grade astronomical precision.

It is **not positioned** as a full replacement for Swiss Ephemeris in terms of raw precision.

That means AstroForge, in its current state, is best understood as a **modern architectural core** into which you can later plug:

- a higher-precision ephemeris provider;
- a licensed data source;
- an external astronomical engine;
- additional house systems;
- retrograde detection;
- calculated points;
- fixed stars;
- transits, progressions, synastry, and more.

This is a conscious architectural decision: first build a proper library, then increase precision and coverage.

---

## Core architectural ideas

### 1. Chart-first API

Instead of a procedural API where callers pass arrays, flags, indexes, and output buffers, AstroForge revolves around one main scenario:

1. create a `ChartRequest`;
2. let `AstroForgeClient` build a `NatalChart`;
3. work with the resulting object model.

This makes the library natural to use from:

- ASP.NET Core APIs;
- Blazor applications;
- desktop UIs;
- background jobs;
- unit and integration tests;
- DDD and Clean Architecture systems.

### 2. Strongly typed domain model

Instead of strings, flags, and loosely coordinated primitives, the library uses explicit models such as:

- `Angle`
- `GeoCoordinate`
- `JulianDate`
- `ChartRequest`
- `PlanetPosition`
- `Aspect`
- `HouseSet`
- `NatalChart`

This reduces usage errors and makes application code much easier to read and maintain.

### 3. Extensible pipeline

The main computational responsibilities are abstracted behind clear contracts:

- `IEphemerisProvider`
- `IHouseCalculator`
- `IAspectAnalyzer`

Because of that, you can:

- replace the built-in analytical provider;
- inject your own house calculator;
- change aspect rules;
- build a commercial or high-precision edition without rewriting the public model.

### 4. Minimal dependency surface

The library does not require a heavy runtime ecosystem and is suitable as the core of your own NuGet package.

---

## Target platform

- **Minimum .NET version:** `.NET 8`
- Target framework: `net8.0`

AstroForge is aimed at modern projects and does not try to preserve old target frameworks for the sake of backward compatibility with legacy architecture.

---

## Project structure

```text
AstroForge/
├─ src/
│  └─ AstroForge/
│     ├─ Abstractions.cs
│     ├─ AstroForgeClient.cs
│     ├─ AstroForgeOptions.cs
│     ├─ Catalogs.cs
│     ├─ Enums.cs
│     ├─ Formatting.cs
│     ├─ Models.cs
│     ├─ Infrastructure/
│     │  ├─ AnalyticalEphemerisProvider.cs
│     │  ├─ DefaultAspectAnalyzer.cs
│     │  ├─ HouseAssignment.cs
│     │  ├─ HouseCalculators.cs
│     │  └─ HouseMath.cs
│     └─ Internal/
│        ├─ AngleMath.cs
│        ├─ AstroTime.cs
│        └─ OrbitalElements.cs
└─ tests/
   └─ AstroForge.Tests/
```

---

## Main usage flow

Below is the basic way to use the library.

### 1. Import the namespace

```csharp
using AstroForge;
```

### 2. Create a client

```csharp
var client = AstroForgeClient.CreateDefault();
```

`CreateDefault()` gives you a ready-to-use configuration with:

- `AnalyticalEphemerisProvider`
- a default house calculator
- `DefaultAspectAnalyzer`
- `AstroForgeOptions.Default`

### 3. Build a chart request

```csharp
var request = new ChartRequest(
    Instant: new DateTimeOffset(1990, 5, 15, 14, 30, 0, TimeSpan.FromHours(3)),
    Location: new GeoCoordinate(55.7558, 37.6176),
    HouseSystem: HouseSystemKind.WholeSign);
```

### 4. Create the chart

```csharp
var chart = client.CreateChart(request);
```

### 5. Use the result

```csharp
Console.WriteLine($"Julian Date: {chart.JulianDate}");
Console.WriteLine($"Ascendant: {AstroFormatter.FormatLongitude(chart.Ascendant)}");
Console.WriteLine($"Midheaven: {AstroFormatter.FormatLongitude(chart.Midheaven)}");

foreach (var body in chart.Bodies)
{
    Console.WriteLine(
        $"{body.Body,-15} " +
        $"{AstroFormatter.FormatLongitude(body.Longitude),-12} " +
        $"House {body.House}");
}
```

---

## Full example

```csharp
using AstroForge;

var options = new AstroForgeOptions(
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

var client = AstroForgeClient.CreateDefault(options);

var chart = client.CreateChart(new ChartRequest(
    Instant: new DateTimeOffset(1990, 5, 15, 14, 30, 0, TimeSpan.FromHours(3)),
    Location: new GeoCoordinate(55.7558, 37.6176),
    HouseSystem: HouseSystemKind.WholeSign));

Console.WriteLine("=== Bodies ===");
foreach (var body in chart.Bodies)
{
    var bodyInfo = BodyCatalog.Get(body.Body);
    var signInfo = SignCatalog.Get(body.Sign);

    Console.WriteLine(
        $"{bodyInfo.Symbol} {bodyInfo.Name}: " +
        $"{AstroFormatter.FormatLongitude(body.Longitude)} " +
        $"({signInfo.Name}), house {body.House}, " +
        $"speed {body.SpeedLongitudePerDay:0.####}°/day");
}

Console.WriteLine();
Console.WriteLine("=== Houses ===");
for (var house = 1; house <= 12; house++)
{
    Console.WriteLine($"House {house}: {AstroFormatter.FormatLongitude(chart.Houses.GetCusp(house))}");
}

Console.WriteLine();
Console.WriteLine("=== Aspects ===");
foreach (var aspect in chart.Aspects)
{
    Console.WriteLine(
        $"{aspect.First} {aspect.Kind} {aspect.Second} | " +
        $"orb={aspect.Orb:0.##} | applying={aspect.Applying}");
}
```

---

## Working without coordinates

If no geographic location is provided, the library can still create a chart **without houses**.

Example:

```csharp
var chart = client.CreateChart(new ChartRequest(
    Instant: new DateTimeOffset(1990, 5, 15, 14, 30, 0, TimeSpan.FromHours(3))));
```

In that mode:

- body positions are calculated;
- aspects are calculated;
- houses are empty (`HouseSet.Empty`);
- each body has `House = 0`;
- horizontal coordinates are not calculated.

This is useful when:

- the birth location is unknown;
- you are generating a preliminary chart;
- you only need sign, longitude, or aspects.

---

## Configuring the set of bodies

By default, AstroForge uses `AstroForgeOptions.DefaultBodies`, but you can override that.

```csharp
var request = new ChartRequest(
    Instant: new DateTimeOffset(1990, 5, 15, 14, 30, 0, TimeSpan.FromHours(3)),
    Location: new GeoCoordinate(55.7558, 37.6176),
    HouseSystem: HouseSystemKind.Equal,
    Bodies: new[]
    {
        CelestialBody.Sun,
        CelestialBody.Moon,
        CelestialBody.Mars,
        CelestialBody.Venus
    });

var chart = client.CreateChart(request);
```

This is useful for:

- simplified user interfaces;
- fast preview calculations;
- custom product scenarios;
- feature-tiered product plans.

---

## Configuring aspects

AstroForge uses an `AspectOrbProfile` to define orb allowances.

You can provide your own profile:

```csharp
var customOrbs = new AspectOrbProfile(
    Conjunction: 10,
    Sextile: 4,
    Square: 6,
    Trine: 7,
    Quincunx: 2,
    Opposition: 8);

var options = new AstroForgeOptions(
    HouseSystem: HouseSystemKind.Equal,
    AspectOrbs: customOrbs,
    DefaultBodies: AstroForgeOptions.Default.DefaultBodies);

var client = AstroForgeClient.CreateDefault(options);
```

This allows the library to be adapted to:

- different interpretive traditions;
- different product rules;
- simplified or stricter analysis modes.

---

## Formatting coordinates

For UI and textual reporting, you can use `AstroFormatter`.

```csharp
var longitude = new Angle(123.4567);

Console.WriteLine(AstroFormatter.FormatDegrees(longitude));
Console.WriteLine(AstroFormatter.FormatLongitude(longitude));
```

Example output:

```text
123.46°
3.46 Leo
```

This is useful for:

- planet cards;
- HTML tables;
- PDF reports;
- logs;
- admin dashboards.

---

## Using catalogs

Catalogs provide metadata for bodies and zodiac signs.

```csharp
var venus = BodyCatalog.Get(CelestialBody.Venus);
Console.WriteLine($"{venus.Symbol} {venus.Name} — {venus.Keywords}");

var leo = SignCatalog.Get(ZodiacSign.Leo);
Console.WriteLine($"{leo.Name}: {leo.Element}, {leo.Modality}");
```

This is useful for:

- generating descriptions;
- visual interfaces;
- API responses;
- localization through wrappers or extensions.

---

## Key library types

### `AstroForgeClient`

The main entry point into the library.

It orchestrates the whole pipeline:

- invokes the ephemeris provider;
- calculates houses;
- calculates aspects;
- assembles everything into a single `NatalChart`.

### `ChartRequest`

Represents the input parameters for chart creation:

- `Instant`
- `Location`
- `HouseSystem`
- `Bodies`

### `NatalChart`

The main result of the calculation.

It contains:

- the original request;
- the Julian date;
- the list of bodies;
- the house set;
- the aspects;
- the ascendant;
- the midheaven.

### `PlanetPosition`

Represents the position of a single celestial body:

- longitude;
- latitude;
- sign;
- degree within sign;
- distance;
- longitudinal speed;
- equatorial coordinates;
- horizontal coordinates;
- house.

### `HouseSet`

Represents chart houses:

- house system;
- ascendant;
- midheaven;
- cusps 1 through 12.

### `Aspect`

Represents an aspect between two bodies:

- aspect kind;
- exact angle;
- orb;
- applying vs separating.

---

## Extending the library

One of the main goals of AstroForge is not only to provide a usable API, but to expose clear **extension points**.

### Custom ephemeris provider

You can implement `IEphemerisProvider` and plug in:

- a Swiss Ephemeris bridge;
- a NASA/JPL-based provider;
- an external microservice;
- a commercial provider;
- a cached or distributed provider.

Contract example:

```csharp
public interface IEphemerisProvider
{
    PlanetPosition GetPosition(CelestialBody body, DateTimeOffset instant, GeoCoordinate? location = null);
}
```

### Custom house calculation

You can implement `IHouseCalculator` and add:

- Placidus;
- Koch;
- Campanus;
- Regiomontanus;
- or your own interpretation of house logic.

### Custom aspect analyzer

You can implement `IAspectAnalyzer` and change:

- the supported aspect list;
- applying/separating rules;
- orb allowances;
- filtering of minor and major aspects.

---

## Manual client composition with custom components

```csharp
using AstroForge;

IEphemerisProvider ephemerisProvider = new AnalyticalEphemerisProvider();
IHouseCalculator houseCalculator = new EqualHouseCalculator();
IAspectAnalyzer aspectAnalyzer = new DefaultAspectAnalyzer(AspectOrbProfile.Default);

var client = new AstroForgeClient(
    ephemerisProvider,
    houseCalculator,
    aspectAnalyzer,
    AstroForgeOptions.Default);
```

This is especially useful when the library is embedded into:

- ASP.NET Core DI;
- a dedicated domain service;
- a background pipeline;
- a multi-tenant architecture.

---

## ASP.NET Core integration

Minimal service registration:

```csharp
builder.Services.AddSingleton(_ => AstroForgeClient.CreateDefault());
```

Example endpoint:

```csharp
app.MapPost("/api/chart", (ChartInput input, AstroForgeClient client) =>
{
    var chart = client.CreateChart(new ChartRequest(
        Instant: input.Instant,
        Location: input.Latitude is null || input.Longitude is null
            ? null
            : new GeoCoordinate(input.Latitude.Value, input.Longitude.Value),
        HouseSystem: input.HouseSystem));

    return Results.Ok(chart);
});

public sealed record ChartInput(
    DateTimeOffset Instant,
    double? Latitude,
    double? Longitude,
    HouseSystemKind HouseSystem);
```

This makes it straightforward to expose AstroForge as a backend API for a website, an application, or an internal service.

---

## When AstroForge is a particularly good fit

AstroForge is a strong fit when you want to:

- build a new NuGet package without legacy baggage;
- build an astrology backend on .NET 8;
- integrate calculations into DDD or Clean Architecture;
- work with a real object model instead of a procedural API;
- create a product foundation that can evolve over time;
- gradually increase precision and features without breaking the architecture.

---

## When AstroForge is not yet the best fit

The current implementation is not ideal when you need:

- maximum astronomical precision out of the box;
- full API compatibility with an older Swiss-like package;
- full coverage of house systems and special points in the first version;
- a ready-made interpretation expert system.

In those cases, AstroForge should either be extended or connected to a more precise provider.

---

## Current limitations

At this stage, keep the following in mind:

- the built-in provider is analytical, not a high-precision ephemeris engine;
- there is no full compatibility with the old C-shaped API;
- many specialized modes of professional astrological calculation are not implemented yet;
- sign and body catalogs are not yet a full multilingual localization layer;
- the library is deliberately focused on clean architecture and an extensible foundation first.

---

## Roadmap

Natural next steps for the library include:

- a high-precision provider adapter;
- additional house systems;
- retrograde status;
- calculated points and asteroids;
- transits;
- progressions;
- synastry;
- solar and lunar returns;
- localization layers for catalogs;
- JSON-friendly DTOs for public APIs;
- DI extension methods;
- caching and performance profiles.

---

## Building the project

```bash
dotnet build
```

### Running tests

```bash
dotnet test
```

### Packing as NuGet

```bash
dotnet pack -c Release
```

---

## Positioning in one sentence

If you had to describe AstroForge in one sentence:

> **AstroForge is a modern architectural core for astrology applications on .NET 8+, not just a port of an old library.**

Or even more precisely:

> **AstroForge provides a clear domain model, an extensible calculation pipeline, and a natural C# API for charts, aspects, and astrographic application scenarios.**

---

## Compatibility and migration

AstroForge does not try to preserve the old API shape one to one.

That is important to understand from the start:

- the old procedural style has been replaced by a chart-first model;
- old arrays and flags have been replaced by explicit types and models;
- low-level calls have been replaced by orchestration through `AstroForgeClient`;
- the architecture is designed for future evolution, not for preserving legacy form.

If you need a migration strategy from the old package, see:

- `MIGRATION_NOTES.md`

If you want the analysis of the old library and the reasons behind the rewrite, see:

- `ANALYSIS.md`

---

## Summary

AstroForge exists to let you stop living inside the constraints of old procedural astrology code and move to a modern .NET 8 library model.

It gives you:

- a new architectural foundation;
- a clear public API;
- a strongly typed domain;
- a ready base for visualization and backend integration;
- extension points for precise ephemerides and future growth.

That is its main value.
