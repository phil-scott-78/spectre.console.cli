namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// Represents extracted metadata for a property on a settings type.
/// </summary>
internal sealed class PropertyModel : IEquatable<PropertyModel>
{
    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the fully qualified property type name.
    /// </summary>
    public string PropertyTypeName { get; }

    /// <summary>
    /// Gets the fully qualified declaring type name.
    /// </summary>
    public string DeclaringTypeName { get; }

    /// <summary>
    /// Gets whether the property type is nullable.
    /// </summary>
    public bool IsNullable { get; }

    /// <summary>
    /// Gets whether the property type is a value type.
    /// </summary>
    public bool IsValueType { get; }

    /// <summary>
    /// Gets whether the property has an init-only setter.
    /// </summary>
    public bool IsInitOnly { get; }

    /// <summary>
    /// Gets whether the property has a setter (regular or init-only).
    /// </summary>
    public bool HasSetter { get; }

    /// <summary>
    /// Gets whether the property is an init-only property inherited from a base class.
    /// </summary>
    public bool IsInheritedInit { get; }

    /// <summary>
    /// Gets the CommandOptionAttribute data if present.
    /// </summary>
    public CommandOptionAttributeModel? OptionAttribute { get; }

    /// <summary>
    /// Gets the CommandArgumentAttribute data if present.
    /// </summary>
    public CommandArgumentAttributeModel? ArgumentAttribute { get; }

    /// <summary>
    /// Gets the description string if DescriptionAttribute is present.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the TypeConverterAttribute type name if present.
    /// </summary>
    public string? ConverterTypeName { get; }

    /// <summary>
    /// Gets the default value if DefaultValueAttribute is present.
    /// </summary>
    public DefaultValueAttributeModel? DefaultValue { get; }

    /// <summary>
    /// Gets the PairDeconstructorAttribute type name if present.
    /// </summary>
    public string? PairDeconstructorTypeName { get; }

    /// <summary>
    /// Gets the ParameterValueProviderAttribute (or subclass) if present.
    /// Uses ValidationAttributeModel to capture full attribute data including constructor args.
    /// </summary>
    public ValidationAttributeModel? ValueProviderAttribute { get; }

    /// <summary>
    /// Gets the validation attributes on this property.
    /// </summary>
    public EquatableArray<ValidationAttributeModel> ValidationAttributes { get; }

    /// <summary>
    /// Gets the array element type name if the property type is an array.
    /// </summary>
    public string? ArrayElementTypeName { get; }

    /// <summary>
    /// Gets the FlagValue inner type name if the property type is FlagValue{T}.
    /// </summary>
    public string? FlagValueInnerTypeName { get; }

    /// <summary>
    /// Gets the dictionary key type name if the property type is a dictionary-like type.
    /// </summary>
    public string? DictionaryKeyTypeName { get; }

    /// <summary>
    /// Gets the dictionary value type name if the property type is a dictionary-like type.
    /// </summary>
    public string? DictionaryValueTypeName { get; }

    public PropertyModel(
        string name,
        string propertyTypeName,
        string declaringTypeName,
        bool isNullable,
        bool isValueType,
        bool isInitOnly,
        bool hasSetter,
        bool isInheritedInit,
        CommandOptionAttributeModel? optionAttribute,
        CommandArgumentAttributeModel? argumentAttribute,
        string? description,
        string? converterTypeName,
        DefaultValueAttributeModel? defaultValue,
        string? pairDeconstructorTypeName,
        ValidationAttributeModel? valueProviderAttribute,
        EquatableArray<ValidationAttributeModel> validationAttributes,
        string? arrayElementTypeName,
        string? flagValueInnerTypeName,
        string? dictionaryKeyTypeName,
        string? dictionaryValueTypeName)
    {
        Name = name;
        PropertyTypeName = propertyTypeName;
        DeclaringTypeName = declaringTypeName;
        IsNullable = isNullable;
        IsValueType = isValueType;
        IsInitOnly = isInitOnly;
        HasSetter = hasSetter;
        IsInheritedInit = isInheritedInit;
        OptionAttribute = optionAttribute;
        ArgumentAttribute = argumentAttribute;
        Description = description;
        ConverterTypeName = converterTypeName;
        DefaultValue = defaultValue;
        PairDeconstructorTypeName = pairDeconstructorTypeName;
        ValueProviderAttribute = valueProviderAttribute;
        ValidationAttributes = validationAttributes;
        ArrayElementTypeName = arrayElementTypeName;
        FlagValueInnerTypeName = flagValueInnerTypeName;
        DictionaryKeyTypeName = dictionaryKeyTypeName;
        DictionaryValueTypeName = dictionaryValueTypeName;
    }

    public bool Equals(PropertyModel? other)
    {
        if (other is null) return false;
        return Name == other.Name &&
               PropertyTypeName == other.PropertyTypeName &&
               DeclaringTypeName == other.DeclaringTypeName &&
               IsNullable == other.IsNullable &&
               IsValueType == other.IsValueType &&
               IsInitOnly == other.IsInitOnly &&
               HasSetter == other.HasSetter &&
               IsInheritedInit == other.IsInheritedInit &&
               Equals(OptionAttribute, other.OptionAttribute) &&
               Equals(ArgumentAttribute, other.ArgumentAttribute) &&
               Description == other.Description &&
               ConverterTypeName == other.ConverterTypeName &&
               Equals(DefaultValue, other.DefaultValue) &&
               PairDeconstructorTypeName == other.PairDeconstructorTypeName &&
               Equals(ValueProviderAttribute, other.ValueProviderAttribute) &&
               ValidationAttributes.Equals(other.ValidationAttributes) &&
               ArrayElementTypeName == other.ArrayElementTypeName &&
               FlagValueInnerTypeName == other.FlagValueInnerTypeName &&
               DictionaryKeyTypeName == other.DictionaryKeyTypeName &&
               DictionaryValueTypeName == other.DictionaryValueTypeName;
    }

    public override bool Equals(object? obj) => obj is PropertyModel other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = Name?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ (PropertyTypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (DeclaringTypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ IsNullable.GetHashCode();
            hash = (hash * 397) ^ IsValueType.GetHashCode();
            hash = (hash * 397) ^ IsInitOnly.GetHashCode();
            hash = (hash * 397) ^ HasSetter.GetHashCode();
            hash = (hash * 397) ^ IsInheritedInit.GetHashCode();
            hash = (hash * 397) ^ (OptionAttribute?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (ArgumentAttribute?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (Description?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (ConverterTypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (DefaultValue?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (PairDeconstructorTypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (ValueProviderAttribute?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ ValidationAttributes.GetHashCode();
            hash = (hash * 397) ^ (ArrayElementTypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (FlagValueInnerTypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (DictionaryKeyTypeName?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (DictionaryValueTypeName?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
