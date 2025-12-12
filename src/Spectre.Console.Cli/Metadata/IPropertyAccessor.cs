namespace Spectre.Console.Cli.Metadata;

/// <summary>
/// Provides access to a property on a settings type, including pre-computed attribute data.
/// </summary>
/// <remarks>
/// This interface enables AOT-safe property access by encapsulating reflection operations
/// that can be performed at compile-time via source generation, or at runtime via reflection.
/// </remarks>
public interface IPropertyAccessor
{
    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    Type PropertyType { get; }

    /// <summary>
    /// Gets the type that declares this property.
    /// </summary>
    Type DeclaringType { get; }

    /// <summary>
    /// Gets the underlying <see cref="System.Reflection.PropertyInfo"/>.
    /// </summary>
    /// <remarks>
    /// Retained for backward compatibility with existing code that uses PropertyInfo
    /// directly (e.g., for error messages, caching keys).
    /// </remarks>
    PropertyInfo PropertyInfo { get; }

    /// <summary>
    /// Gets the value of the property from the specified instance.
    /// </summary>
    /// <param name="instance">The object instance to get the value from.</param>
    /// <returns>The property value.</returns>
    object? GetValue(object instance);

    /// <summary>
    /// Sets the value of the property on the specified instance.
    /// </summary>
    /// <param name="instance">The object instance to set the value on.</param>
    /// <param name="value">The value to set.</param>
    void SetValue(object instance, object? value);

    /// <summary>
    /// Gets a value indicating whether <see cref="SetValue"/> can be called on this property.
    /// </summary>
    /// <remarks>
    /// Returns <c>true</c> for properties with regular setters or init-only setters (when supported).
    /// Returns <c>false</c> for get-only properties.
    /// </remarks>
    bool CanSet { get; }

    /// <summary>
    /// Gets the <see cref="CommandOptionAttribute"/> if present on the property.
    /// </summary>
    CommandOptionAttribute? OptionAttribute { get; }

    /// <summary>
    /// Gets the <see cref="CommandArgumentAttribute"/> if present on the property.
    /// </summary>
    CommandArgumentAttribute? ArgumentAttribute { get; }

    /// <summary>
    /// Gets the <see cref="DescriptionAttribute"/> if present on the property.
    /// </summary>
    DescriptionAttribute? DescriptionAttribute { get; }

    /// <summary>
    /// Gets the <see cref="TypeConverterAttribute"/> if present on the property.
    /// </summary>
    TypeConverterAttribute? ConverterAttribute { get; }

    /// <summary>
    /// Gets the <see cref="DefaultValueAttribute"/> if present on the property.
    /// </summary>
    DefaultValueAttribute? DefaultValueAttribute { get; }

    /// <summary>
    /// Gets the <see cref="PairDeconstructorAttribute"/> if present on the property.
    /// </summary>
    PairDeconstructorAttribute? PairDeconstructorAttribute { get; }

    /// <summary>
    /// Gets the <see cref="ParameterValueProviderAttribute"/> if present on the property.
    /// </summary>
    ParameterValueProviderAttribute? ValueProviderAttribute { get; }

    /// <summary>
    /// Gets the list of <see cref="ParameterValidationAttribute"/> instances on the property.
    /// </summary>
    IReadOnlyList<ParameterValidationAttribute> ValidationAttributes { get; }
}