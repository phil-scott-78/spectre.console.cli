namespace Spectre.Console.Cli.Metadata;

/// <summary>
/// Provides metadata about a command settings type.
/// </summary>
/// <remarks>
/// This interface encapsulates all metadata required to instantiate and populate
/// a settings type without runtime reflection in AOT scenarios.
/// </remarks>
public interface ISettingsMetadata
{
    /// <summary>
    /// Gets the settings type this metadata describes.
    /// </summary>
    Type SettingsType { get; }

    /// <summary>
    /// Gets the properties of the settings type that can be bound to command parameters.
    /// </summary>
    IReadOnlyList<IPropertyAccessor> Properties { get; }

    /// <summary>
    /// Gets the constructors available for creating instances of the settings type.
    /// </summary>
    /// <remarks>
    /// Multiple constructors support the "greediest constructor" selection pattern
    /// used for constructor injection.
    /// </remarks>
    IReadOnlyList<IConstructorMetadata> Constructors { get; }
}