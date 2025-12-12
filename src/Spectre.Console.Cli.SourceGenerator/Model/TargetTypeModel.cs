using Microsoft.CodeAnalysis;

namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// Represents the user's partial class marked with [SpectreMetadata].
/// This class implements IEquatable for proper equality comparison in incremental pipelines.
/// </summary>
internal sealed class TargetTypeModel : IEquatable<TargetTypeModel>
{
    /// <summary>
    /// The simple name of the class.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The namespace of the class, or empty string for global namespace.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// The fully qualified name including global:: prefix.
    /// </summary>
    public string FullyQualifiedName { get; }

    /// <summary>
    /// Whether the class is declared as partial.
    /// </summary>
    public bool IsPartial { get; }

    /// <summary>
    /// The declared accessibility of the class.
    /// </summary>
    public Accessibility Accessibility { get; }

    public TargetTypeModel(
        string name,
        string ns,
        string fullyQualifiedName,
        bool isPartial,
        Accessibility accessibility)
    {
        Name = name;
        Namespace = ns;
        FullyQualifiedName = fullyQualifiedName;
        IsPartial = isPartial;
        Accessibility = accessibility;
    }

    public bool Equals(TargetTypeModel? other)
    {
        if (other is null)
        {
            return false;
        }

        return Name == other.Name &&
               Namespace == other.Namespace &&
               FullyQualifiedName == other.FullyQualifiedName &&
               IsPartial == other.IsPartial &&
               Accessibility == other.Accessibility;
    }

    public override bool Equals(object? obj)
    {
        return obj is TargetTypeModel other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 31 + (Name?.GetHashCode() ?? 0);
            hash = hash * 31 + (Namespace?.GetHashCode() ?? 0);
            hash = hash * 31 + (FullyQualifiedName?.GetHashCode() ?? 0);
            hash = hash * 31 + IsPartial.GetHashCode();
            hash = hash * 31 + Accessibility.GetHashCode();
            return hash;
        }
    }
}
