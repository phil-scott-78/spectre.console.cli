using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli.Internal.Metadata;

/// <summary>
/// Reflection-based implementation of <see cref="IPropertyAccessor"/>.
/// </summary>
[RequiresUnreferencedCode("Reflection-based property access.")]
internal sealed class ReflectionPropertyAccessor : IPropertyAccessor
{
    private readonly PropertyInfo _property;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionPropertyAccessor"/> class.
    /// </summary>
    /// <param name="property">The property to wrap.</param>
    public ReflectionPropertyAccessor(PropertyInfo property)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));

        Name = property.Name;
        PropertyType = property.PropertyType;
        DeclaringType = property.DeclaringType ?? throw new ArgumentException("Property must have a declaring type.", nameof(property));

        // Pre-compute all attributes (pattern from CommandModelBuilder)
        OptionAttribute = property.GetCustomAttribute<CommandOptionAttribute>();
        ArgumentAttribute = property.GetCustomAttribute<CommandArgumentAttribute>();
        DescriptionAttribute = property.GetCustomAttribute<DescriptionAttribute>();
        ConverterAttribute = property.GetCustomAttribute<TypeConverterAttribute>();
        DefaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
        PairDeconstructorAttribute = property.GetCustomAttribute<PairDeconstructorAttribute>();
        ValueProviderAttribute = property.GetCustomAttribute<ParameterValueProviderAttribute>();
        ValidationAttributes = property.GetCustomAttributes<ParameterValidationAttribute>(true).ToList();
        CanSet = property.SetMethod != null;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public Type PropertyType { get; }

    /// <inheritdoc />
    public Type DeclaringType { get; }

    /// <inheritdoc />
    public PropertyInfo PropertyInfo => _property;

    /// <inheritdoc />
    public CommandOptionAttribute? OptionAttribute { get; }

    /// <inheritdoc />
    public CommandArgumentAttribute? ArgumentAttribute { get; }

    /// <inheritdoc />
    public DescriptionAttribute? DescriptionAttribute { get; }

    /// <inheritdoc />
    public TypeConverterAttribute? ConverterAttribute { get; }

    /// <inheritdoc />
    public DefaultValueAttribute? DefaultValueAttribute { get; }

    /// <inheritdoc />
    public PairDeconstructorAttribute? PairDeconstructorAttribute { get; }

    /// <inheritdoc />
    public ParameterValueProviderAttribute? ValueProviderAttribute { get; }

    /// <inheritdoc />
    public IReadOnlyList<ParameterValidationAttribute> ValidationAttributes { get; }

    /// <inheritdoc />
    public bool CanSet { get; }

    /// <inheritdoc />
    public object? GetValue(object instance)
    {
        return _property.GetValue(instance);
    }

    /// <inheritdoc />
    public void SetValue(object instance, object? value)
    {
        _property.SetValue(instance, value);
    }
}