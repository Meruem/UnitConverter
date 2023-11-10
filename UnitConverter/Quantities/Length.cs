using UnitConversion.Model;

namespace UnitConversion.Quantities;

public enum Length
{
    Feet,
    Meter,
    Inch
}

internal class LengthQuantity : IQuantityProvider
{
    public Quantity Get() => Quantity.Create(Length.Meter)
        .WithConversion(Length.Feet, x => x * 0.3048m, x => x / 0.3048m)
        .WithConversion(Length.Inch, x => x * 0.0254m, x => x / 0.0254m)
        .WithParserRule(Length.Feet, "feet", "foot")
        .WithParserRule(Length.Meter, "meter", "meters")
        .WithParserRule(Length.Inch, "inch", "inches");
}
