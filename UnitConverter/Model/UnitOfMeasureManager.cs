namespace UnitConversion.Model;

/// <summary>
/// Singleton class to contain all available unit type definitions.
/// </summary>
internal class UnitOfMeasureManager
{
    private static readonly Lazy<UnitOfMeasureManager> lazy = new(() => new UnitOfMeasureManager());

    public static UnitOfMeasureManager Instance => lazy.Value;

    internal readonly Dictionary<string, UnitTypeDefinition> UnitTypeDefinitions = new();

    public UnitOfMeasureManager()
    {
        var type = typeof(IUnitTypeDefinitionProvider);
        var providers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t) && !t.IsInterface);

        foreach (var providerType in providers)
        {
            var provider = (IUnitTypeDefinitionProvider)Activator.CreateInstance(providerType)!;
            RegisterUnitTypeDefinition(provider.Get());
        }
    }

    private void RegisterUnitTypeDefinition(UnitTypeDefinition unitTypeDefinition)
    {
        UnitTypeDefinitions.Add(unitTypeDefinition.Name, unitTypeDefinition);
    }

    internal UnitTypeDefinition GetUnitTypeDefinition(string definitionName)
    {
        if (UnitTypeDefinitions.TryGetValue(definitionName, out var definition))
        {
            return definition;
        }

        throw new Exception($"Unspecified unit type: {definitionName}");
    }
}