using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal static class CommandPropertyBinder
{
    public static CommandSettings CreateSettings(CommandValueLookup lookup, Type settingsType, ITypeResolver resolver, ICommandMetadataContext metadataContext)
    {
        // First try to resolve from DI container
        if (resolver.Resolve(settingsType) is CommandSettings resolvedSettings)
        {
            SetProperties(lookup, resolvedSettings);
            return resolvedSettings;
        }

        // Fall back to metadata context creation
        var settings = metadataContext.CreateSettings(settingsType);
        SetProperties(lookup, settings);
        return settings;
    }

    private static void SetProperties(CommandValueLookup lookup, CommandSettings settings)
    {
        foreach (var (parameter, value) in lookup)
        {
            if (value != null)
            {
                parameter.Accessor.SetValue(settings, value);
            }
        }

        // Validate the settings.
        var validationResult = settings.Validate();
        if (!validationResult.Successful)
        {
            throw CommandRuntimeException.ValidationFailed(validationResult);
        }
    }
}