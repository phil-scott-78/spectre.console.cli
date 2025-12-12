using System.Collections.Concurrent;
using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli.Internal.Metadata;

/// <summary>
/// Reflection-based implementation of <see cref="ICommandMetadataContext"/>.
/// </summary>
/// <remarks>
/// <para>
/// This implementation uses runtime reflection to discover and invoke
/// command and settings metadata.
/// </para>
/// <para>
/// When used via the source generator (with <c>[SpectreMetadata]</c>), the generator wraps
/// this class with <c>[DynamicDependency]</c> attributes that preserve the required type
/// metadata, making reflection work correctly in AOT scenarios.
/// </para>
/// </remarks>
[RequiresDynamicCode("Reflection-based metadata requires dynamic code. Use the source generator for AOT scenarios.")]
[RequiresUnreferencedCode("Reflection-based metadata requires unreferenced code. Use the source generator for AOT scenarios.")]
public sealed class ReflectionMetadataContext : ICommandMetadataContext
{
    private readonly ConcurrentDictionary<Type, ISettingsMetadata> _settingsCache = new();
    private readonly ConcurrentDictionary<Type, ICommandTypeMetadata> _commandCache = new();

    /// <inheritdoc />
    public CommandSettings CreateSettings(Type settingsType)
    {
        ArgumentNullException.ThrowIfNull(settingsType);

        // Pattern from CommandPropertyBinder
        return (CommandSettings)(Activator.CreateInstance(settingsType)
            ?? throw new InvalidOperationException($"Could not create settings of type {settingsType}."));
    }

    /// <inheritdoc />
    public ISettingsMetadata GetSettingsMetadata(Type settingsType)
    {
        ArgumentNullException.ThrowIfNull(settingsType);

        return _settingsCache.GetOrAdd(settingsType, t => new ReflectionSettingsMetadata(t));
    }

    /// <inheritdoc />
    public ICommandTypeMetadata GetCommandTypeMetadata(Type commandType)
    {
        ArgumentNullException.ThrowIfNull(commandType);

        return _commandCache.GetOrAdd(commandType, t => new ReflectionCommandTypeMetadata(t));
    }

    /// <inheritdoc />
    public Type? GetSettingsTypeForCommand(Type commandType)
    {
        ArgumentNullException.ThrowIfNull(commandType);

        if (!typeof(ICommand).GetTypeInfo().IsAssignableFrom(commandType))
        {
            return null;
        }

        // Walk the type hierarchy looking for ICommand<TSettings>
        var current = commandType;
        while (current != null)
        {
            foreach (var @interface in current.GetTypeInfo().GetInterfaces())
            {
                if (!@interface.GetTypeInfo().IsGenericType)
                {
                    continue;
                }

                if (@interface.GetGenericTypeDefinition() != typeof(ICommand<>))
                {
                    continue;
                }

                return @interface.GenericTypeArguments[0];
            }

            current = current.GetTypeInfo().BaseType;
        }

        return null;
    }

    /// <inheritdoc />
    public object CreatePairDeconstructor(Type deconstructorType)
    {
        ArgumentNullException.ThrowIfNull(deconstructorType);

        return Activator.CreateInstance(deconstructorType)
            ?? throw new InvalidOperationException($"Could not create pair deconstructor of type {deconstructorType}.");
    }

    /// <inheritdoc />
    public object? CreateDefaultValue(Type valueType)
    {
        ArgumentNullException.ThrowIfNull(valueType);

        if (!valueType.IsValueType)
        {
            return null;
        }

        return Activator.CreateInstance(valueType);
    }

    /// <inheritdoc />
    public object CreateInstance(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return Activator.CreateInstance(type)
            ?? throw new InvalidOperationException($"Could not create instance of type {type}.");
    }

    /// <inheritdoc />
    public TypeConverter GetTypeConverter(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeConverterHelper.GetTypeConverter(type);
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "MakeGenericType for FlagValue<T> is safe because FlagValue<T> is preserved via [DynamicDependency] in the generated metadata context.")]
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026",
        Justification = "MakeGenericType for FlagValue<T> is safe because FlagValue<T> is preserved via [DynamicDependency] in the generated metadata context.")]
    public IFlagValue CreateFlagValue(Type underlyingType)
    {
        ArgumentNullException.ThrowIfNull(underlyingType);

        var flagType = typeof(FlagValue<>).MakeGenericType(underlyingType);
        return (IFlagValue)(Activator.CreateInstance(flagType)
            ?? throw new InvalidOperationException($"Could not create FlagValue<{underlyingType}>."));
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "MakeGenericType for MultiMap<,> is safe because MultiMap<,> is preserved via [DynamicDependency] in the generated metadata context.")]
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026",
        Justification = "MakeGenericType for MultiMap<,> is safe because MultiMap<,> is preserved via [DynamicDependency] in the generated metadata context.")]
    public IMultiMap CreateMultiMap(Type keyType, Type valueType)
    {
        ArgumentNullException.ThrowIfNull(keyType);
        ArgumentNullException.ThrowIfNull(valueType);

        var multiMapType = typeof(MultiMap<,>).MakeGenericType(keyType, valueType);
        return (IMultiMap)(Activator.CreateInstance(multiMapType)
            ?? throw new InvalidOperationException($"Could not create MultiMap<{keyType}, {valueType}>."));
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL2075",
        Justification = "Target types are preserved via [DynamicDependency] in the generated metadata context.")]
    public object? ConvertWithConstructorFallback(Type targetType, object input)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentNullException.ThrowIfNull(input);

        var constructor = targetType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            null,
            [input.GetType()],
            null);

        return constructor?.Invoke([input]);
    }

    /// <inheritdoc />
    public void RegisterKnownTypes(ITypeRegistrar registrar, ICommandModel model)
    {
        ArgumentNullException.ThrowIfNull(registrar);
        ArgumentNullException.ThrowIfNull(model);

        // Cast to internal CommandModel to access internal properties
        var commandModel = (CommandModel)model;

        var stack = new Stack<CommandInfo>();
        commandModel.Commands.ForEach(c => stack.Push(c));

        while (stack.Count > 0)
        {
            var command = stack.Pop();

            if (command.SettingsType == null)
            {
                throw new InvalidOperationException("Command setting type cannot be null.");
            }

            if (command.SettingsType is { IsAbstract: false, IsClass: true })
            {
                // Register the settings type
                registrar.Register(command.SettingsType, command.SettingsType);
            }

            if (command.CommandType != null)
            {
                registrar.Register(command.CommandType, command.CommandType);
            }

            foreach (var parameter in command.Parameters)
            {
                var pairDeconstructor = parameter.PairDeconstructor?.Type;
                if (pairDeconstructor != null)
                {
                    registrar.Register(pairDeconstructor, pairDeconstructor);
                }

                var typeConverterTypeName = parameter.Converter?.ConverterTypeName;
                if (!string.IsNullOrWhiteSpace(typeConverterTypeName))
                {
                    var typeConverterType = Type.GetType(typeConverterTypeName);
                    if (typeConverterType == null)
                    {
                        throw new InvalidOperationException($"Could not create type '{typeConverterTypeName}'");
                    }

                    registrar.Register(typeConverterType, typeConverterType);
                }
            }

            foreach (var child in command.Children)
            {
                stack.Push(child);
            }
        }
    }
}
