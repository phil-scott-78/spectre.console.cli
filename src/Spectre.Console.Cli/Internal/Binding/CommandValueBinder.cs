using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal sealed class CommandValueBinder
{
    private readonly CommandValueLookup _lookup;
    private readonly ICommandMetadataContext _metadataContext;

    public CommandValueBinder(CommandValueLookup lookup, ICommandMetadataContext metadataContext)
    {
        _lookup = lookup;
        _metadataContext = metadataContext;
    }

    public void Bind(CommandParameter parameter, ITypeResolver resolver, object? value)
    {
        if (parameter.ParameterKind == ParameterKind.Pair)
        {
            value = GetLookup(parameter, resolver, value);
        }
        else if (parameter.ParameterKind == ParameterKind.Vector)
        {
            value = GetArray(parameter, value);
        }
        else if (parameter.ParameterKind == ParameterKind.FlagWithValue)
        {
            value = GetFlag(parameter, value);
        }

        _lookup.SetValue(parameter, value);
    }

    private object GetLookup(CommandParameter parameter, ITypeResolver resolver, object? value)
    {
        var genericTypes = parameter.Accessor.PropertyType.GetGenericArguments();

        var multimap = (IMultiMap?)_lookup.GetValue(parameter);
        if (multimap == null)
        {
            multimap = _metadataContext.CreateMultiMap(genericTypes[0], genericTypes[1]);
        }

        // Create deconstructor.
        var deconstructorType = parameter.PairDeconstructor?.Type ?? typeof(DefaultPairDeconstructor);
        if (!(resolver.Resolve(deconstructorType) is IPairDeconstructor deconstructor))
        {
            deconstructor = (IPairDeconstructor)_metadataContext.CreatePairDeconstructor(deconstructorType);
        }

        // Deconstruct and add to multimap.
        var pair = deconstructor.Deconstruct(resolver, _metadataContext, genericTypes[0], genericTypes[1], value as string);
        if (pair.Key != null)
        {
            multimap.Add(pair);
        }

        return multimap;
    }

    private object GetArray(CommandParameter parameter, object? value)
    {
        if (value is Array)
        {
            return value;
        }

        // Add a new item to the array
        var array = (Array?)_lookup.GetValue(parameter);
        Array newArray;

        var elementType = parameter.Accessor.PropertyType.GetElementType();
        if (elementType == null)
        {
            throw new InvalidOperationException("Could not get property type.");
        }

        if (array == null)
        {
            newArray = Array.CreateInstance(elementType, 1);
        }
        else
        {
            newArray = Array.CreateInstance(elementType, array.Length + 1);
            array.CopyTo(newArray, 0);
        }

        newArray.SetValue(value, newArray.Length - 1);
        return newArray;
    }

    private object GetFlag(CommandParameter parameter, object? value)
    {
        var flagValue = (IFlagValue?)_lookup.GetValue(parameter);
        if (flagValue == null)
        {
            // Get the underlying type from FlagValue<T>
            var genericArgs = parameter.ParameterType.GetGenericArguments();
            if (genericArgs.Length == 0)
            {
                throw new InvalidOperationException("Could not determine flag value type.");
            }

            flagValue = _metadataContext.CreateFlagValue(genericArgs[0]);
        }

        if (value != null)
        {
            // Null means set, but not with a valid value.
            flagValue.Value = value;
        }

        // If the parameter was mapped, then it's set.
        flagValue.IsSet = true;

        return flagValue;
    }
}