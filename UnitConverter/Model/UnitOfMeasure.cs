namespace UnitConversion.Model;

public record UnitOfMeasure
{
    public decimal Value { get; }
    public string UnitType { get; }

    private readonly Quantity quantity;

    private readonly UnitOfMeasureManager manager = UnitOfMeasureManager.Instance;

    public static UnitOfMeasure Create<TUnitType>(decimal value, TUnitType unitType) where TUnitType : Enum
        => new(value, unitType.ToString(), typeof(TUnitType).Name);

    public static UnitOfMeasure Parse(decimal value, string token)
    {
        foreach (var kvp in UnitOfMeasureManager.Instance.Quantities)
        {
            if (kvp.Value.TryParse(token, out var unitTypeValue))
            {
                return new(value, unitTypeValue!, kvp.Key);
            }
        }

        throw new ArgumentException($"Unable to parse token: {token}, no defined units match this input.");
    }

    private UnitOfMeasure(decimal value, string unitType, string quantityName)
    {
        Value = value;
        UnitType = unitType.ToLower();
        quantity = manager.GetQuantity(quantityName.ToLower());
    }

    public decimal ValueWithPrefix(string prefix)
    {
        if (prefix.Trim() == "") return Value;

        if (UnitConverter.SIPrefixTable.TryGetValue(prefix.Trim().ToLower(), out var mul))
        {
            return Value / mul;
        }

        throw new ArgumentException($"Unknown SI prefix: {prefix}");
    }

    public UnitOfMeasure Convert(string toUnitTypeToken)
    {
        if (!quantity.TryParse(toUnitTypeToken, out var toUnitType))
            throw new ArgumentException(
                $"Target unit type [{toUnitTypeToken}] for conversion is not supported for {quantity.Name}.");

        var newValue = quantity.GetConvertFunction(UnitType, toUnitType, Value);
        return new UnitOfMeasure(newValue, toUnitType, quantity.Name);
    }

    public UnitOfMeasure Convert<TUnitType>(TUnitType toUnitType) where TUnitType : Enum
        => Convert(toUnitType.ToString());

    public override string ToString() => $"{Value} {UnitType}";
    public string ToString(string prefix) => $"{ValueWithPrefix(prefix):0.##} {prefix}{UnitType}";
}