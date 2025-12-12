namespace Spectre.Console.Cli.Metadata;

/// <summary>
/// Provides access to command and settings metadata, enabling AOT-safe operations.
/// </summary>
/// <remarks>
/// <para>
/// This interface is the primary abstraction for eliminating runtime reflection
/// in AOT scenarios. It supports two construction paths:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// <strong>Explicit Context (AOT-Safe)</strong>: A pre-built context passed to <see cref="CommandApp"/>,
/// performing zero runtime reflection. Intended for Native AOT, trimmed environments,
/// and source-generated metadata.
/// </description>
/// </item>
/// <item>
/// <description>
/// <strong>Reflection-Based Context (Non-AOT)</strong>: A context built dynamically using reflection,
/// guarded by <c>RuntimeFeature.IsDynamicCodeSupported</c>.
/// </description>
/// </item>
/// </list>
/// </remarks>
public interface ICommandMetadataContext
{
    /// <summary>
    /// Creates a new instance of the specified settings type.
    /// </summary>
    /// <param name="settingsType">The type of settings to create.</param>
    /// <returns>A new instance of the settings type.</returns>
    CommandSettings CreateSettings(Type settingsType);

    /// <summary>
    /// Gets the metadata for the specified settings type.
    /// </summary>
    /// <param name="settingsType">The type to get metadata for.</param>
    /// <returns>The settings metadata.</returns>
    ISettingsMetadata GetSettingsMetadata(Type settingsType);

    /// <summary>
    /// Gets the metadata for the specified command type.
    /// </summary>
    /// <param name="commandType">The command type to get metadata for.</param>
    /// <returns>The command type metadata, or <c>null</c> if not found.</returns>
    ICommandTypeMetadata? GetCommandTypeMetadata(Type commandType);

    /// <summary>
    /// Gets the settings type associated with a command type.
    /// </summary>
    /// <param name="commandType">The command type (must implement <see cref="ICommand{TSettings}"/>).</param>
    /// <returns>The settings type, or <c>null</c> if the command type is not found.</returns>
    /// <remarks>
    /// This method extracts the TSettings type argument from ICommand&lt;TSettings&gt;.
    /// In AOT mode, this is pre-computed at compile time. In reflection mode, this
    /// uses runtime reflection to inspect the generic interface.
    /// </remarks>
    Type? GetSettingsTypeForCommand(Type commandType);

    /// <summary>
    /// Creates an instance of the specified pair deconstructor type.
    /// </summary>
    /// <param name="deconstructorType">The type of pair deconstructor to create.</param>
    /// <returns>A new instance of the pair deconstructor (implements internal IPairDeconstructor).</returns>
    object CreatePairDeconstructor(Type deconstructorType);

    /// <summary>
    /// Creates a default value for the specified value type.
    /// </summary>
    /// <param name="valueType">The value type to create a default instance of.</param>
    /// <returns>The default value, or <c>null</c> if the type is not a value type.</returns>
    object? CreateDefaultValue(Type valueType);

    /// <summary>
    /// Creates an instance of the specified type.
    /// </summary>
    /// <param name="type">The type to instantiate.</param>
    /// <returns>A new instance of the type.</returns>
    /// <remarks>
    /// This is used as a fallback when the type resolver returns null.
    /// </remarks>
    object CreateInstance(Type type);

    /// <summary>
    /// Gets a TypeConverter for the specified type.
    /// </summary>
    /// <param name="type">The type to get a converter for.</param>
    /// <returns>A TypeConverter instance capable of converting to/from the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no suitable converter can be found.</exception>
    /// <remarks>
    /// This method centralizes type converter resolution to support AOT scenarios.
    /// In reflection mode, it uses TypeDescriptor as a fallback.
    /// </remarks>
    TypeConverter GetTypeConverter(Type type);

    /// <summary>
    /// Creates a FlagValue&lt;T&gt; instance for the specified underlying type.
    /// </summary>
    /// <param name="underlyingType">The underlying type T for the FlagValue&lt;T&gt;.</param>
    /// <returns>A new IFlagValue instance.</returns>
    /// <remarks>
    /// This method centralizes generic type construction for FlagValue&lt;T&gt; to support AOT scenarios.
    /// </remarks>
    IFlagValue CreateFlagValue(Type underlyingType);

    /// <summary>
    /// Creates a MultiMap&lt;TKey, TValue&gt; instance for the specified key and value types.
    /// </summary>
    /// <param name="keyType">The key type for the MultiMap.</param>
    /// <param name="valueType">The value type for the MultiMap.</param>
    /// <returns>A new IMultiMap instance.</returns>
    /// <remarks>
    /// This method centralizes generic type construction for MultiMap&lt;,&gt; to support AOT scenarios.
    /// </remarks>
    IMultiMap CreateMultiMap(Type keyType, Type valueType);

    /// <summary>
    /// Attempts to convert an input value to the target type using constructor-based fallback.
    /// </summary>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="input">The input value to convert.</param>
    /// <returns>The converted value, or <c>null</c> if conversion is not possible.</returns>
    /// <remarks>
    /// This is used as a fallback when TypeConverter.ConvertFrom fails.
    /// It attempts to find a constructor that accepts the input type.
    /// In AOT mode, this returns <c>null</c> as dynamic code is not supported.
    /// </remarks>
    object? ConvertWithConstructorFallback(Type targetType, object input);

    /// <summary>
    /// Registers all known command and settings types with the type registrar.
    /// </summary>
    /// <param name="registrar">The type registrar to register types with.</param>
    /// <param name="model">The command model containing the configured commands.</param>
    /// <remarks>
    /// <para>
    /// This method registers types needed for command execution with the DI container.
    /// </para>
    /// <para>
    /// In source-generated implementations, this delegates to the underlying
    /// reflection-based context to iterate through the command model and
    /// register discovered types.
    /// </para>
    /// <para>
    /// In reflection-based implementations, this iterates through the command model
    /// and registers discovered types using reflection.
    /// </para>
    /// </remarks>
    void RegisterKnownTypes(ITypeRegistrar registrar, ICommandModel model);
}
