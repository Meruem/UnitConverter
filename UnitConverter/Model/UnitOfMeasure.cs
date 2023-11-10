namespace UnitConversion.Model;

/// <summary>
/// Represents value with attached unit type.
/// Example: Value = 4; UnitType = Length.Feet 
/// </summary>
public record UnitOfMeasure
{
    public decimal Value { get; }
    public string Unit { get; }

    private readonly UnitTypeDefinition definition;

    private readonly UnitOfMeasureManager manager = UnitOfMeasureManager.Instance;

    /// <summary>
    /// Creates new instance of the unit of measure.
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="unit">Defines unit.</param>
    /// <typeparam name="TUnitType">Enumeration defining all the values of the unit type.</typeparam>
    /// <returns>New instance of UnitOfMeasure.</returns>
    public static UnitOfMeasure Create<TUnitType>(decimal value, TUnitType unit) where TUnitType : Enum
        => new(value, unit.ToString(), typeof(TUnitType).Name);


    /// <summary>
    /// Parses input string token and creates unit type with specified value.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="token">String token to match with every unit type parser.</param>
    /// <exception cref="ArgumentException">Input is not defined in any unit type parser.</exception>
    public static UnitOfMeasure Parse(decimal value, string token)
    {
        foreach (var kvp in UnitOfMeasureManager.Instance.UnitTypeDefinitions)
        {
            if (kvp.Value.TryParse(token, out var unitTypeValue))
            {
                return new(value, unitTypeValue, kvp.Key);
            }
        }

        throw new ArgumentException($"Unable to parse token: {token}, no defined units match this input.");
    }

    private UnitOfMeasure(decimal value, string unit, string unitTypeName)
    {
        Value = value;
        Unit = unit.ToLower();
        definition = manager.GetUnitTypeDefinition(unitTypeName.ToLower());
    }

    /// <summary>
    /// Returns the value of the unit of measure converted to specified SI prefix.
    /// </summary>
    /// <param name="prefix">SI prefix.</param>
    /// <returns>Converted value by SI prefix</returns>
    /// <exception cref="ArgumentException">Prefix is not valid SI prefix.</exception>
    public decimal ValueWithPrefix(string prefix)
    {
        if (prefix.Trim() == "") return Value;

        if (UnitConverter.SIPrefixTable.TryGetValue(prefix.Trim().ToLower(), out var mul))
        {
            return Value / mul;
        }

        throw new ArgumentException($"Unknown SI prefix: {prefix}");
    }

    /// <summary>
    /// Converts the current unit of measure to target unit type.
    /// </summary>
    /// <param name="toUnitToken">String token defining target unit.</param>
    /// <returns>New unit of measure with converted value.</returns>
    /// <exception cref="ArgumentException">Input token is not supported by unit type parser.</exception>
    public UnitOfMeasure Convert(string toUnitToken)
    {
        if (!definition.TryParse(toUnitToken, out var toUnit))
            throw new ArgumentException(
                $"Target unit type [{toUnitToken}] for conversion is not supported for {definition.Name}.");

        var newValue = definition.GetConvertFunction(Unit, toUnit, Value);
        return new UnitOfMeasure(newValue, toUnit, definition.Name);
    }

    /// <summary>
    /// Converts the current unit of measure to target unit type.
    /// </summary>
    /// <param name="toUnit">Target unit.</param>
    /// <typeparam name="TUnitType">Enumeration defining all the values of the unit type.</typeparam>
    /// <returns>New unit of measure with converted value.</returns>
    public UnitOfMeasure Convert<TUnitType>(TUnitType toUnit) where TUnitType : Enum
        => Convert(toUnit.ToString());

    public override string ToString() => $"{Value} {Unit}";
    public string ToString(string prefix) => $"{ValueWithPrefix(prefix):0.##} {prefix}{Unit}";
}