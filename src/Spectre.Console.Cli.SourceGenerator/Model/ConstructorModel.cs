namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// Represents extracted metadata for a constructor on a settings type.
/// </summary>
internal sealed class ConstructorModel : IEquatable<ConstructorModel>
{
    /// <summary>
    /// Gets the index of this constructor (for naming).
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the parameters of this constructor.
    /// </summary>
    public EquatableArray<ConstructorParameterModel> Parameters { get; }

    public ConstructorModel(int index, EquatableArray<ConstructorParameterModel> parameters)
    {
        Index = index;
        Parameters = parameters;
    }

    public bool Equals(ConstructorModel? other)
    {
        if (other is null) return false;
        return Index == other.Index && Parameters.Equals(other.Parameters);
    }

    public override bool Equals(object? obj) => obj is ConstructorModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            return (Index * 397) ^ Parameters.GetHashCode();
        }
    }
}

/// <summary>
/// Represents a constructor parameter.
/// </summary>
internal sealed class ConstructorParameterModel : IEquatable<ConstructorParameterModel>
{
    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the fully qualified parameter type name.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets whether the parameter has a default value.
    /// </summary>
    public bool HasDefaultValue { get; }

    /// <summary>
    /// Gets the default value as a code expression (if present).
    /// </summary>
    public string? DefaultValueExpression { get; }

    public ConstructorParameterModel(
        string name,
        string typeName,
        bool hasDefaultValue,
        string? defaultValueExpression)
    {
        Name = name;
        TypeName = typeName;
        HasDefaultValue = hasDefaultValue;
        DefaultValueExpression = defaultValueExpression;
    }

    public bool Equals(ConstructorParameterModel? other)
    {
        if (other is null) return false;
        return Name == other.Name &&
               TypeName == other.TypeName &&
               HasDefaultValue == other.HasDefaultValue &&
               DefaultValueExpression == other.DefaultValueExpression;
    }

    public override bool Equals(object? obj) => obj is ConstructorParameterModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = Name?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ (TypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ HasDefaultValue.GetHashCode();
            hash = (hash * 397) ^ (DefaultValueExpression?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
