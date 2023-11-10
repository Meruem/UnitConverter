using UnitConversion.Model;

namespace UnitConversion.UnitTypes;

public enum Temperature
{
    Celsius,
    Fahrenheit,
}

internal class TemperatureUnitTypeDefinition  : IUnitTypeDefinitionProvider
{
    public UnitTypeDefinition Get() => UnitTypeDefinition.Create(Temperature.Celsius)
        .WithConversion(Temperature.Fahrenheit, x => (x - 32m) * (5m / 9m),
            x => (9m / 5m) * x + 32)
        .WithParserRule(Temperature.Celsius, "celsius")
        .WithParserRule(Temperature.Fahrenheit, "fahrenheit");
}
