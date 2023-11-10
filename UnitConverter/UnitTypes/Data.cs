using UnitConversion.Model;

namespace UnitConversion.UnitTypes;

public enum Data
{
    Bit,
    Byte,
}

internal class DataUnitTypeDefinition : IUnitTypeDefinitionProvider
{
    public UnitTypeDefinition Get() => UnitTypeDefinition.Create(Data.Bit)
        .WithConversion(Data.Byte, x => x * 8, x => x / 8)
        .WithParserRule(Data.Bit, "bit", "bits")
        .WithParserRule(Data.Byte, "byte", "bytes");
}
