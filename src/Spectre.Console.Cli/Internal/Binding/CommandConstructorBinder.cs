using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal static class CommandConstructorBinder
{
    public static CommandSettings CreateSettings(
        CommandValueLookup lookup,
        IConstructorMetadata constructor,
        ITypeResolver resolver,
        Dictionary<Type, object>? resolutionCache = null)
    {
        var parameters = new List<object?>();
        var mapped = new HashSet<Guid>();
        foreach (var parameter in constructor.Parameters)
        {
            if (lookup.TryGetParameterWithName(parameter.Name, out var result))
            {
                parameters.Add(result.Value);
                mapped.Add(result.Parameter.Id);
            }
            else
            {
                // First check the resolution cache to reuse previously resolved instances
                // This avoids instantiating transient services multiple times
                if (resolutionCache != null && resolutionCache.TryGetValue(parameter.ParameterType, out var cached))
                {
                    parameters.Add(cached);
                }
                // Then check for default value
                else if (parameter.HasDefaultValue)
                {
                    parameters.Add(parameter.DefaultValue);
                }
                // Finally try DI resolution
                else
                {
                    var value = resolver.Resolve(parameter.ParameterType);
                    if (value == null)
                    {
                        throw CommandRuntimeException.CouldNotResolveType(parameter.ParameterType);
                    }

                    parameters.Add(value);
                }
            }
        }

        // Create the settings.
        if (constructor.Invoke(parameters.ToArray()) is not CommandSettings settings)
        {
            throw new InvalidOperationException("Could not create settings");
        }

        // Try to do property injection for parameters that wasn't injected.
        foreach (var (parameter, value) in lookup)
        {
            if (!mapped.Contains(parameter.Id) && parameter.Accessor.CanSet)
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

        return settings;
    }
}