using UnitConversion.Model;

namespace UnitConversionTest;

public class ConverterTest
{
    [TestCase("(\"1 meter\", \"feet\")", 3.28)]
    [TestCase("(\"3 kilometer\", \"inches\")", 118110.23)]
    [TestCase("(\"6 mebibyte\", \"kibibit\")", 49152)]
    [TestCase("(\"-10 celsius\", \"fahrenheit\")", 14)]
    [TestCase("(\"4 millifeet\", \"nanoinch\")", 48000000)]
    public void ParseAndConvertToValueTest(string input, decimal expectedValue)
    {
        var result = UnitConverter.ConvertToValue(input);
        Assert.That(result, Is.EqualTo(expectedValue).Within(0.01));
    }

    [TestCase("(\"3 kiloinches\", \"meter\")", "76.2 meter")]
    [TestCase("(\"4 millifeet\", \"nanoinch\")", "48000000 nanoinch")]
    public void ParseAndConvertToStringTest(string input, string expectedOutput)
    {
        var result = UnitConverter.ConvertToString(input);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }
    
    [TestCase("(\"3 kiloinches\", \"meter\")", 76.2, "meter")]
    public void ParseAndConvertToUnitTest(string input, decimal expectedValue, string expectedUnitType)
    {
        var result = UnitConverter.ConvertToUnit(input);
        Assert.That(result.Value, Is.EqualTo(expectedValue));
        Assert.That(result.Unit, Is.EqualTo(expectedUnitType));
    }
}
