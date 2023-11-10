# UnitConverter
Library for conversion between different units of the same quantity. For example allows to convert between meters and inches, 
temperature in celsius to fahrenheit.

### Usage

Class [UnitConverter](UnitConverter/Model/UnitConverter.cs) exposes static methods to parse input string and produce result in form of string, value or 
[UnitOfMeasure](UnitConverter/Model/UnitOfMeasure.cs) class representing the resulting data.

Examples:
```csharp
    var stringResult = UnitConverter.ConvertToString("(\"1 meter\", \"feet\")"); // 3.28 feet
    var valueResult = UnitConverter.ConvertToValue("(\"3 kiloinches\", \"meter\")"); // 76.2
```

The [UnitOfMeasure](UnitConverter/Model/UnitOfMeasure.cs) class can be also constructed in code to create new representation
of the unit type:
```csharp
    var unit = UnitOfMeasure.Create(Length.Inch, 4);
    var newUnit = unit.Convert(Length.Feet);
```

### Extensibility

In order to support additional unit types, following steps need to be implemented:

1) Create new enumeration containing all the unit types:
    ```csharp
      public enum Length
      {
         Feet,
         Meter,
         Inch
      }
    ```
2) Create a quantity provider implementing the IQuantityProvider and specify
- Base unit
- Conversion methods to and from the base unit for every unit in the enumeration
- Parsing rules how to convert string token into unit

   ```csharp
   internal class LengthQuantity : IQuantityProvider
   {
       public Quantity Get() => Quantity.Create(Length.Meter) // base unit
           .WithConversion(Length.Feet, x => x * 0.3048m, x => x / 0.3048m) // how to convert feet->meter and meter->feet
           .WithConversion(Length.Inch, x => x * 0.0254m, x => x / 0.0254m)
           .WithParserRule(Length.Feet, "feet", "foot")
           .WithParserRule(Length.Meter, "meter", "meters")
           .WithParserRule(Length.Inch, "inch", "inches");
   }
   ```
  
This class will be automatically picked by the UnitOfMeasureManager class and allow for the automatic parsing.