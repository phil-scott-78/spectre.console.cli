namespace Spectre.Console.Cli.Metadata;

/// <summary>
/// Provides metadata about a command type.
/// </summary>
/// <remarks>
/// This interface encapsulates command-level metadata (such as descriptions)
/// that would otherwise require runtime reflection to retrieve.
/// </remarks>
public interface ICommandTypeMetadata
{
    /// <summary>
    /// Gets the command type this metadata describes.
    /// </summary>
    Type CommandType { get; }

    /// <summary>
    /// Gets the <see cref="DescriptionAttribute"/> if present on the command type.
    /// </summary>
    DescriptionAttribute? DescriptionAttribute { get; }
}