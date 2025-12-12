using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli.Internal.Metadata;

/// <summary>
/// Reflection-based implementation of <see cref="ISettingsMetadata"/>.
/// </summary>
[RequiresUnreferencedCode("Reflection-based settings metadata.")]
internal sealed class ReflectionSettingsMetadata : ISettingsMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionSettingsMetadata"/> class.
    /// </summary>
    /// <param name="settingsType">The settings type to analyze.</param>
    public ReflectionSettingsMetadata(Type settingsType)
    {
        SettingsType = settingsType ?? throw new ArgumentNullException(nameof(settingsType));

        // Enumerate properties walking up the inheritance hierarchy
        // Pattern from CommandModelBuilder
        var properties = new List<IPropertyAccessor>();
        var current = settingsType;
        while (current != null && current != typeof(object))
        {
            var declaredProps = current.GetProperties(
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in declaredProps)
            {
                properties.Add(new ReflectionPropertyAccessor(prop));
            }

            current = current.BaseType;
        }

        Properties = properties;

        // Get all public constructors
        Constructors = settingsType.GetConstructors()
            .Select(c => new ReflectionConstructorMetadata(c))
            .ToList();
    }

    /// <inheritdoc />
    public Type SettingsType { get; }

    /// <inheritdoc />
    public IReadOnlyList<IPropertyAccessor> Properties { get; }

    /// <inheritdoc />
    public IReadOnlyList<IConstructorMetadata> Constructors { get; }
}