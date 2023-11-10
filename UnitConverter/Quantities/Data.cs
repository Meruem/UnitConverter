using UnitConversion.Model;

namespace UnitConversion.Quantities;

public enum Data
{
    Bit,
    Byte,
}

internal class DataQuantity : IQuantityProvider
{
    public Quantity Get() => Quantity.Create(Data.Bit)
        .WithConversion(Data.Byte, x => x * 8, x => x / 8)
        .WithParserRule(Data.Bit, "bit", "bits")
        .WithParserRule(Data.Byte, "byte", "bytes");
}
