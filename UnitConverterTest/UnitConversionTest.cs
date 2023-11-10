using UnitConversion.Model;
using UnitConversion.Quantities;

namespace UnitConversionTest;

public class UnitConversionTest
{
    [TestCase(Length.Feet, 1, Length.Inch, 12)]
    [TestCase(Length.Meter, 3, Length.Inch, 118.11)]
    [TestCase(Length.Feet, 10, Length.Feet, 10)]
    [TestCase(Length.Inch, 4, Length.Feet, 0.333333)]
    public void LengthConversionTests(Length fromUnit, decimal fromValue, Length toUnit, decimal toValue)
    {
        var l1 = UnitOfMeasure.Create(fromValue, fromUnit);
        var l2 = l1.Convert(toUnit);
        
        Assert.That(l2.Value, Is.EqualTo(toValue).Within(0.001));
    }
    
    [TestCase(Data.Bit, 45, Data.Byte, 5.625)]
    [TestCase(Data.Byte, 2, Data.Bit, 16)]
    public void DataConversionTests(Data fromUnit, decimal fromValue, Data toUnit, decimal toValue)
    {
        var l1 = UnitOfMeasure.Create(fromValue, fromUnit);
        var l2 = l1.Convert(toUnit);
        
        Assert.That(l2.Value, Is.EqualTo(toValue).Within(0.001));
    }
    
    [TestCase(Temperature.Celsius, 45, Temperature.Fahrenheit, 113)]
    [TestCase(Temperature.Celsius, -11, Temperature.Fahrenheit, 12.2)]
    [TestCase(Temperature.Fahrenheit, 32, Temperature.Celsius, 0)]
    public void TemperatureConversionTests(Temperature fromUnit, decimal fromValue, Temperature toUnit, decimal toValue)
    {
        var l1 = UnitOfMeasure.Create(fromValue, fromUnit);
        var l2 = l1.Convert(toUnit);
        
        Assert.That(l2.Value, Is.EqualTo(toValue).Within(0.001));
    }
}
