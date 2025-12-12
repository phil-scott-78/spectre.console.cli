using Spectre.Console.Cli.SourceGenerator.Model;

namespace Spectre.Console.Cli.SourceGenerator.Extraction;

/// <summary>
/// Collects generic type usages from settings types to generate appropriate factories.
/// </summary>
internal static class GenericTypeCollector
{
    /// <summary>
    /// Collects all generic type instantiations needed from the settings types.
    /// </summary>
    public static GenericTypeInfo Collect(List<SettingsTypeModel> settingsTypes)
    {
        var builder = new GenericTypeInfoBuilder();

        foreach (var settings in settingsTypes)
        {
            foreach (var property in settings.Properties)
            {
                // Collect array element types
                if (property.ArrayElementTypeName is not null)
                {
                    builder.ArrayElementTypes.Add(property.ArrayElementTypeName);
                }

                // Collect FlagValue inner types
                if (property.FlagValueInnerTypeName is not null)
                {
                    builder.FlagValueTypes.Add(property.FlagValueInnerTypeName);
                }

                // Collect dictionary/MultiMap types
                if (property.DictionaryKeyTypeName is not null && property.DictionaryValueTypeName is not null)
                {
                    builder.MultiMapTypes.Add((property.DictionaryKeyTypeName, property.DictionaryValueTypeName));
                }

                // Collect custom types from attributes
                if (property.ConverterTypeName is not null)
                {
                    builder.ConverterTypes.Add(property.ConverterTypeName);
                }

                if (property.PairDeconstructorTypeName is not null)
                {
                    builder.PairDeconstructorTypes.Add(property.PairDeconstructorTypeName);
                }

                if (property.ValueProviderAttribute is not null)
                {
                    builder.ValueProviderTypes.Add(property.ValueProviderAttribute.TypeName);
                }
            }
        }

        return builder.Build();
    }
}
