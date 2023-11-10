namespace UnitConversion.Model;

/// <summary>
/// Definition of a unit type.
/// Contains specification how to convert between the units and how to parse from string tokens.
/// </summary>
internal class UnitTypeDefinition
{
    record Conversion(Func<decimal, decimal> ConvertToBase, Func<decimal, decimal> ConvertFromBase);

    private readonly string baseUnit;
    private readonly Dictionary<string, Conversion> conversionsMap = new();

    // string token -> unitType
    private readonly Dictionary<string, string> parserRuleMap = new();

    public string Name { get; }

    private UnitTypeDefinition(string baseUnit, string name)
    {
        this.baseUnit = baseUnit.ToLower();
        Name = name.ToLower();
    }

    public static UnitTypeDefinition Create<TUnitType>(TUnitType baseUnitType) where TUnitType : Enum => new(
        baseUnitType.ToString(),
        typeof(TUnitType).Name);

    public UnitTypeDefinition WithConversion<TUnitType>(TUnitType fromUnit, Func<decimal, decimal> convertToBaseUnit,
        Func<decimal, decimal> convertFromBaseUnit) where TUnitType : Enum
    {
        conversionsMap.Add(fromUnit.ToString().ToLower(), new Conversion(convertToBaseUnit, convertFromBaseUnit));
        return this;
    }

    public UnitTypeDefinition WithParserRule<TUnitType>(TUnitType unit, params string[] tokens) where TUnitType : Enum
    {
        foreach (var token in tokens)
        {
            parserRuleMap.Add(token, unit.ToString().ToLower());
        }

        return this;
    }

    public bool TryParse(string token, out string value)
    {
        if (parserRuleMap.TryGetValue(token.Trim().ToLower(), out var type))
        {
            value = type;
            return true;
        }

        value = "";
        return false;
    }

    // Converts first from original unit to base unit and then from base unit to target unit.
    public decimal GetConvertFunction(string fromUnit, string toUnit, decimal originalValue)
    {
        var from = fromUnit.ToLower();
        var to = toUnit.ToLower();
        if (from == to) return originalValue;

        decimal newValue = originalValue;
        if (from != baseUnit)
        {
            if (!conversionsMap.TryGetValue(from, out var fromConversion))
                throw new Exception(
                    $"Unsupported conversion for unit type {Name}, unable to convert from [{from}] to base unit [{baseUnit}]");

            newValue = fromConversion.ConvertToBase(newValue);
        }

        if (to != baseUnit)
        {
            if (!conversionsMap.TryGetValue(to, out var toConversion))
                throw new Exception(
                    $"Unsupported conversion for unit type {Name}, unable to convert from base unit [{baseUnit}] to [{to}]");

            newValue = toConversion.ConvertFromBase(newValue);
        }

        return newValue;
    }
}