using System.Text.RegularExpressions;

namespace UnitConversion.Model;

// Examples:
// ("1 meter", "feet") -> "3.28 feet"
// ("3 kiloinches", "meter") -> "76.19 meter"
public static class UnitConverter
{
    private static readonly string pattern = @".*\(""(\S*) (\S*)"", ""(\S*)""\).*";

    /// <summary>
    /// Converts input string into UnitOfMeasure containing value and unit type information.
    /// </summary>
    /// <param name="input">Input string in format ("&lt;value&gt; &lt;prefix+unit&gt;", "&lt;prefix+target_unit&gt;").</param>
    /// <returns>Unit of measure.</returns>
    public static UnitOfMeasure ConvertToUnit(string input)
    {
        var (convertedUnit, _) = Convert(input);
        return convertedUnit;
    }

    /// <summary>
    /// Converts input string into value defined in the input.
    /// </summary>
    /// <param name="input">Input string in format ("&lt;value&gt; &lt;prefix+unit&gt;", "&lt;prefix+target_unit&gt;").</param>
    /// <returns>Converted value.</returns>
    public static decimal ConvertToValue(string input)
    {
        var (convertedUnit, toPrefix) = Convert(input);
        return convertedUnit.ValueWithPrefix(toPrefix);
    }

    /// <summary>
    /// Converts input string into string representation in format &lt;converted_value prefix+target_unit&gt;.
    /// </summary>
    /// <param name="input">Input string in format ("&lt;value&gt; &lt;prefix+unit&gt;", "&lt;prefix+target_unit&gt;").</param>
    /// <returns>String representation of converted value and unit.</returns>
    public static string ConvertToString(string input)
    {
        var (convertedUnit, toPrefix) = Convert(input);
        return convertedUnit.ToString(toPrefix);
    }

    /// <summary>
    /// Converts input string into UnitOfMeasure containing value and unit type information and target SI prefix string.
    /// </summary>
    /// <param name="input">Input string in format ("&lt;value&gt; &lt;prefix+unit&gt;", "&lt;prefix+target_unit&gt;").</param>
    /// <returns>Unit of measure and target SI prefix.</returns>
    public static (UnitOfMeasure unit, string targetSIPrefix) Convert(string input)
    {
        var m = Regex.Match(input, pattern);
        if (!m.Success)
            throw new ArgumentException($"Unable to parse input: {input}, expected format is:" +
                                        $"(\"<value> <prefix+unit>\", \"<target_unit>\")");

        decimal value;
        string fromToken;
        string toToken;

        try
        {
            value = decimal.Parse(m.Groups[1].Value);
            fromToken = m.Groups[2].Value;
            toToken = m.Groups[3].Value;
        }
        catch
        {
            throw new ArgumentException($"Unable to parse input: {input}, expected format is:" +
                                        $"(\"<value> <prefix+unit>\", \"<target_unit>\")");
        }

        var (fromMul, _, fromUnitToken) = ExtractSIPrefix(fromToken);
        var (_, toPrefix, toUnitToken) = ExtractSIPrefix(toToken);

        var unit = UnitOfMeasure.Parse(value * fromMul, fromUnitToken);
        var convertedUnit = unit.Convert(toUnitToken);

        return (convertedUnit, toPrefix);
    }

    private static decimal TenToPower(int exp) => (decimal)Math.Pow(10, exp);
    private static decimal TwoToPower(int exp) => (decimal)Math.Pow(2, exp);

    public static readonly Dictionary<string, decimal> SIPrefixTable = new()
    {
        // disabled some of the prefixes due to decimal range limitations,
        // to support this big number support need to be added
        // { "quetta", TenToPower(30) }, 
        { "ronna", TenToPower(27) },
        { "yotta", TenToPower(24) },
        { "zetta", TenToPower(21) },
        { "exa", TenToPower(18) },
        { "peta", TenToPower(15) },
        { "tera", TenToPower(12) },
        { "giga", TenToPower(9) },
        { "mega", TenToPower(6) },
        { "kilo", TenToPower(3) },
        { "hecto", TenToPower(2) },
        { "deca", TenToPower(1) },
        { "deci", TenToPower(-1) },
        { "centi", TenToPower(-2) },
        { "milli", TenToPower(-3) },
        { "micro", TenToPower(-6) },
        { "nano", TenToPower(-9) },
        { "pico", TenToPower(-12) },
        { "femto", TenToPower(-15) },
        { "atto", TenToPower(-18) },
        { "zepto", TenToPower(-21) },
        { "yocto", TenToPower(-24) },
        { "ronto", TenToPower(-27) },
        // { "quecto", TenToPower(-30) },
        { "kibi", TwoToPower(10) },
        { "mebi", TwoToPower(20) },
        { "gibi", TwoToPower(30) },
        { "tebi", TwoToPower(40) },
        { "pebi", TwoToPower(50) },
        { "exbi", TwoToPower(60) },
        { "zebi", TwoToPower(70) },
        { "yobi", TwoToPower(80) },
    };

    private static (decimal mul, string prefix, string modifiedToken) ExtractSIPrefix(string token)
    {
        foreach (var prefix in SIPrefixTable.Where(prefix => token.StartsWith(prefix.Key)))
        {
            return (prefix.Value, prefix.Key, token.Replace(prefix.Key, ""));
        }

        return (1, "", token);
    }
}