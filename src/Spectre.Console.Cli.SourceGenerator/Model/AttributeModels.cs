namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// Represents extracted data from CommandOptionAttribute.
/// </summary>
internal sealed class CommandOptionAttributeModel : IEquatable<CommandOptionAttributeModel>
{
    /// <summary>
    /// Gets the template string passed to the attribute constructor.
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// Gets a value indicating whether the option is required.
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    /// Gets a value indicating whether the option is hidden from help text.
    /// </summary>
    public bool IsHidden { get; }

    public CommandOptionAttributeModel(string template, bool isRequired = false, bool isHidden = false)
    {
        Template = template;
        IsRequired = isRequired;
        IsHidden = isHidden;
    }

    public bool Equals(CommandOptionAttributeModel? other)
    {
        if (other is null) return false;
        return Template == other.Template && IsRequired == other.IsRequired && IsHidden == other.IsHidden;
    }

    public override bool Equals(object? obj) => obj is CommandOptionAttributeModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = Template?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ IsRequired.GetHashCode();
            hash = (hash * 397) ^ IsHidden.GetHashCode();
            return hash;
        }
    }
}

/// <summary>
/// Represents extracted data from CommandArgumentAttribute.
/// </summary>
internal sealed class CommandArgumentAttributeModel : IEquatable<CommandArgumentAttributeModel>
{
    /// <summary>
    /// Gets the position of the argument.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// Gets the name/template of the argument (optional).
    /// </summary>
    public string? Name { get; }

    public CommandArgumentAttributeModel(int position, string? name = null)
    {
        Position = position;
        Name = name;
    }

    public bool Equals(CommandArgumentAttributeModel? other)
    {
        if (other is null) return false;
        return Position == other.Position && Name == other.Name;
    }

    public override bool Equals(object? obj) => obj is CommandArgumentAttributeModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            return (Position * 397) ^ (Name?.GetHashCode() ?? 0);
        }
    }
}

/// <summary>
/// Represents extracted data from DefaultValueAttribute.
/// </summary>
internal sealed class DefaultValueAttributeModel : IEquatable<DefaultValueAttributeModel>
{
    /// <summary>
    /// Gets the default value as a C# code expression string.
    /// </summary>
    public string ValueExpression { get; }

    public DefaultValueAttributeModel(string valueExpression)
    {
        ValueExpression = valueExpression;
    }

    public bool Equals(DefaultValueAttributeModel? other)
    {
        if (other is null) return false;
        return ValueExpression == other.ValueExpression;
    }

    public override bool Equals(object? obj) => obj is DefaultValueAttributeModel other && Equals(other);
    public override int GetHashCode() => ValueExpression?.GetHashCode() ?? 0;
}

/// <summary>
/// Represents a constructor argument for an attribute.
/// </summary>
internal sealed class AttributeArgumentModel : IEquatable<AttributeArgumentModel>
{
    /// <summary>
    /// Gets the value as a code expression string.
    /// </summary>
    public string ValueExpression { get; }

    public AttributeArgumentModel(string valueExpression)
    {
        ValueExpression = valueExpression;
    }

    public bool Equals(AttributeArgumentModel? other)
    {
        if (other is null) return false;
        return ValueExpression == other.ValueExpression;
    }

    public override bool Equals(object? obj) => obj is AttributeArgumentModel other && Equals(other);
    public override int GetHashCode() => ValueExpression?.GetHashCode() ?? 0;
}

/// <summary>
/// Represents a named argument (property assignment) for an attribute.
/// </summary>
internal sealed class AttributeNamedArgumentModel : IEquatable<AttributeNamedArgumentModel>
{
    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value as a code expression string.
    /// </summary>
    public string ValueExpression { get; }

    public AttributeNamedArgumentModel(string name, string valueExpression)
    {
        Name = name;
        ValueExpression = valueExpression;
    }

    public bool Equals(AttributeNamedArgumentModel? other)
    {
        if (other is null) return false;
        return Name == other.Name && ValueExpression == other.ValueExpression;
    }

    public override bool Equals(object? obj) => obj is AttributeNamedArgumentModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Name?.GetHashCode() ?? 0) * 397) ^ (ValueExpression?.GetHashCode() ?? 0);
        }
    }
}

/// <summary>
/// Represents extracted data from a validation attribute.
/// </summary>
internal sealed class ValidationAttributeModel : IEquatable<ValidationAttributeModel>
{
    /// <summary>
    /// Gets the fully qualified type name of the validation attribute.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the constructor arguments for recreating the attribute.
    /// </summary>
    public EquatableArray<AttributeArgumentModel> ConstructorArguments { get; }

    /// <summary>
    /// Gets the named arguments (properties) for the attribute.
    /// </summary>
    public EquatableArray<AttributeNamedArgumentModel> NamedArguments { get; }

    public ValidationAttributeModel(
        string typeName,
        EquatableArray<AttributeArgumentModel> constructorArguments,
        EquatableArray<AttributeNamedArgumentModel> namedArguments)
    {
        TypeName = typeName;
        ConstructorArguments = constructorArguments;
        NamedArguments = namedArguments;
    }

    public bool Equals(ValidationAttributeModel? other)
    {
        if (other is null) return false;
        return TypeName == other.TypeName &&
               ConstructorArguments.Equals(other.ConstructorArguments) &&
               NamedArguments.Equals(other.NamedArguments);
    }

    public override bool Equals(object? obj) => obj is ValidationAttributeModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = TypeName?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ ConstructorArguments.GetHashCode();
            hash = (hash * 397) ^ NamedArguments.GetHashCode();
            return hash;
        }
    }
}
