namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// Represents extracted metadata for a command type.
/// </summary>
internal sealed class CommandTypeModel : IEquatable<CommandTypeModel>
{
    /// <summary>
    /// Gets the fully qualified type name.
    /// </summary>
    public string FullyQualifiedName { get; }

    /// <summary>
    /// Gets the safe name for use in generated identifiers.
    /// </summary>
    public string SafeName { get; }

    /// <summary>
    /// Gets the description from DescriptionAttribute if present.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets whether this type has a public parameterless constructor.
    /// </summary>
    public bool HasParameterlessConstructor { get; }

    /// <summary>
    /// Gets the fully qualified name of the settings type (from ICommand&lt;TSettings&gt;).
    /// </summary>
    public string? SettingsTypeName { get; }

    public CommandTypeModel(
        string fullyQualifiedName,
        string safeName,
        string? description,
        bool hasParameterlessConstructor,
        string? settingsTypeName)
    {
        FullyQualifiedName = fullyQualifiedName;
        SafeName = safeName;
        Description = description;
        HasParameterlessConstructor = hasParameterlessConstructor;
        SettingsTypeName = settingsTypeName;
    }

    public bool Equals(CommandTypeModel? other)
    {
        if (other is null) return false;
        return FullyQualifiedName == other.FullyQualifiedName &&
               SafeName == other.SafeName &&
               Description == other.Description &&
               HasParameterlessConstructor == other.HasParameterlessConstructor &&
               SettingsTypeName == other.SettingsTypeName;
    }

    public override bool Equals(object? obj) => obj is CommandTypeModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = FullyQualifiedName?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ (SafeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (Description?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ HasParameterlessConstructor.GetHashCode();
            hash = (hash * 397) ^ (SettingsTypeName?.GetHashCode() ?? 0);
            return hash;
        }
    }

    /// <summary>
    /// Creates a safe identifier name from a fully qualified type name.
    /// </summary>
    public static string GetSafeName(string fullyQualifiedName)
    {
        return fullyQualifiedName
            .Replace("global::", "")
            .Replace(".", "_")
            .Replace("<", "_")
            .Replace(">", "_")
            .Replace(",", "_")
            .Replace(" ", "");
    }
}
