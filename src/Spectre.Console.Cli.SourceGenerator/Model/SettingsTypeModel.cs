namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// Represents extracted metadata for a CommandSettings type.
/// </summary>
internal sealed class SettingsTypeModel : IEquatable<SettingsTypeModel>
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
    /// Gets the namespace of the settings type.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Gets the list of properties on this settings type.
    /// </summary>
    public EquatableArray<PropertyModel> Properties { get; }

    /// <summary>
    /// Gets the list of constructors on this settings type.
    /// </summary>
    public EquatableArray<ConstructorModel> Constructors { get; }

    /// <summary>
    /// Gets the description from DescriptionAttribute if present.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets whether this settings type is abstract.
    /// </summary>
    public bool IsAbstract { get; }

    /// <summary>
    /// Gets whether this type has a public parameterless constructor.
    /// </summary>
    public bool HasParameterlessConstructor =>
        Constructors.AsSpan().ToArray().Any(c => c.Parameters.Count == 0);

    public SettingsTypeModel(
        string fullyQualifiedName,
        string safeName,
        string @namespace,
        EquatableArray<PropertyModel> properties,
        EquatableArray<ConstructorModel> constructors,
        string? description,
        bool isAbstract)
    {
        FullyQualifiedName = fullyQualifiedName;
        SafeName = safeName;
        Namespace = @namespace;
        Properties = properties;
        Constructors = constructors;
        Description = description;
        IsAbstract = isAbstract;
    }

    public bool Equals(SettingsTypeModel? other)
    {
        if (other is null) return false;
        return FullyQualifiedName == other.FullyQualifiedName &&
               SafeName == other.SafeName &&
               Namespace == other.Namespace &&
               Properties.Equals(other.Properties) &&
               Constructors.Equals(other.Constructors) &&
               Description == other.Description &&
               IsAbstract == other.IsAbstract;
    }

    public override bool Equals(object? obj) => obj is SettingsTypeModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = FullyQualifiedName?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ (SafeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (Namespace?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ Properties.GetHashCode();
            hash = (hash * 397) ^ Constructors.GetHashCode();
            hash = (hash * 397) ^ (Description?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ IsAbstract.GetHashCode();
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
