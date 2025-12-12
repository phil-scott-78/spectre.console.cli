namespace Spectre.Console.Cli.Metadata;

/// <summary>
/// Provides metadata about a constructor and the ability to invoke it.
/// </summary>
/// <remarks>
/// This interface enables AOT-safe constructor invocation by encapsulating
/// the reflection-based <see cref="System.Activator.CreateInstance(Type, object[])"/> pattern.
/// </remarks>
public interface IConstructorMetadata
{
    /// <summary>
    /// Gets the type that declares this constructor.
    /// </summary>
    Type DeclaringType { get; }

    /// <summary>
    /// Gets the parameters of this constructor.
    /// </summary>
    IReadOnlyList<ConstructorParameter> Parameters { get; }

    /// <summary>
    /// Invokes the constructor with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments to pass to the constructor.</param>
    /// <returns>A new instance of the declaring type.</returns>
    object Invoke(object?[] args);
}

/// <summary>
/// Represents a parameter of a constructor.
/// </summary>
public sealed class ConstructorParameter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorParameter"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="parameterType">The type of the parameter.</param>
    /// <param name="hasDefaultValue">Whether the parameter has a default value.</param>
    /// <param name="defaultValue">The default value, if any.</param>
    public ConstructorParameter(string name, Type parameterType, bool hasDefaultValue, object? defaultValue)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ParameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
        HasDefaultValue = hasDefaultValue;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the parameter.
    /// </summary>
    public Type ParameterType { get; }

    /// <summary>
    /// Gets a value indicating whether the parameter has a default value.
    /// </summary>
    public bool HasDefaultValue { get; }

    /// <summary>
    /// Gets the default value of the parameter, if any.
    /// </summary>
    public object? DefaultValue { get; }
}