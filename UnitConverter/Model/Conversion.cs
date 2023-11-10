namespace UnitConversion.Model;

public record Conversion(Func<decimal, decimal> ConvertToBase, Func<decimal, decimal> ConvertFromBase);
