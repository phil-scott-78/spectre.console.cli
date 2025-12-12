using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal abstract class CommandParameter : ICommandParameterInfo, ICommandParameter
{
    public Guid Id { get; }
    public Type ParameterType { get; }
    public ParameterKind ParameterKind { get; }
    public IPropertyAccessor Accessor { get; }
    public PropertyInfo Property { get; }
    public string? Description { get; }
    public DefaultValueAttribute? DefaultValue { get; }
    public TypeConverterAttribute? Converter { get; }
    public PairDeconstructorAttribute? PairDeconstructor { get; }
    public List<ParameterValidationAttribute> Validators { get; }
    public ParameterValueProviderAttribute? ValueProvider { get; }
    public bool IsRequired { get; set; }
    public bool IsHidden { get; }
    public string PropertyName => Property.Name;

    public virtual bool WantRawValue => ParameterType.IsPairDeconstructable()
        && (PairDeconstructor != null || Converter == null);

    public bool IsFlag => ParameterKind == ParameterKind.Flag;

    /// <summary>
    /// Gets a value indicating whether this parameter is a FlagValue&lt;T&gt;.
    /// This is computed once during model building to avoid runtime reflection.
    /// </summary>
    public bool IsFlagValue { get; }

    protected CommandParameter(
        Type parameterType, ParameterKind parameterKind, IPropertyAccessor accessor,
        string? description, TypeConverterAttribute? converter,
        DefaultValueAttribute? defaultValue,
        PairDeconstructorAttribute? deconstructor,
        ParameterValueProviderAttribute? valueProvider,
        IEnumerable<ParameterValidationAttribute> validators, bool required, bool isHidden)
    {
        Id = Guid.NewGuid();
        ParameterType = parameterType;
        ParameterKind = parameterKind;
        Accessor = accessor;
        Property = accessor.PropertyInfo;
        Description = description;
        Converter = converter;
        DefaultValue = defaultValue;
        PairDeconstructor = deconstructor;
        ValueProvider = valueProvider;
        Validators = new List<ParameterValidationAttribute>(validators);
        IsRequired = required;
        IsHidden = isHidden;

        // Pre-compute IsFlagValue during model building to avoid runtime reflection
        IsFlagValue = ComputeIsFlagValue(parameterType);
    }

    private static bool ComputeIsFlagValue(Type parameterType)
    {
        // Use IsAssignableFrom instead of GetInterfaces() to avoid IL2070 warning
        return typeof(IFlagValue).IsAssignableFrom(parameterType);
    }

    public bool HaveSameBackingPropertyAs(CommandParameter other)
    {
        return CommandParameterComparer.ByBackingProperty.Equals(this, other);
    }

}