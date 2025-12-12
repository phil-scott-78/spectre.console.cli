using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli.Internal.Metadata;

/// <summary>
/// Reflection-based implementation of <see cref="ICommandTypeMetadata"/>.
/// </summary>
[RequiresUnreferencedCode("Reflection-based command type metadata.")]
internal sealed class ReflectionCommandTypeMetadata : ICommandTypeMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionCommandTypeMetadata"/> class.
    /// </summary>
    /// <param name="commandType">The command type to analyze.</param>
    public ReflectionCommandTypeMetadata(Type commandType)
    {
        CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
        DescriptionAttribute = commandType.GetCustomAttribute<DescriptionAttribute>();
    }

    /// <inheritdoc />
    public Type CommandType { get; }

    /// <inheritdoc />
    public DescriptionAttribute? DescriptionAttribute { get; }
}