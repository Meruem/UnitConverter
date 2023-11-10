namespace UnitConversion.Model;

public class UnitOfMeasureManager
{
    private static readonly Lazy<UnitOfMeasureManager> lazy = new(() => new UnitOfMeasureManager());

    public static UnitOfMeasureManager Instance => lazy.Value;

    internal readonly Dictionary<string, Quantity> Quantities = new();

    public UnitOfMeasureManager()
    {
        var type = typeof(IQuantityProvider);
        var providers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t) && !t.IsInterface);

        foreach (var providerType in providers)
        {
            var provider = (IQuantityProvider)Activator.CreateInstance(providerType)!;
            RegisterQuantity(provider.Get());
        }
    }

    private void RegisterQuantity(Quantity quantity)
    {
        Quantities.Add(quantity.Name, quantity);
    }

    internal Quantity GetQuantity(string quantityName)
    {
        if (Quantities.TryGetValue(quantityName, out var quantity))
        {
            return quantity;
        }

        throw new Exception($"Unspecified quantity type: {quantityName}");
    }
}
