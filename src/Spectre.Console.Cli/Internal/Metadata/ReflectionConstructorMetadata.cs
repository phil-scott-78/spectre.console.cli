using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli.Internal.Metadata;

/// <summary>
/// Reflection-based implementation of <see cref="IConstructorMetadata"/>.
/// </summary>
[RequiresUnreferencedCode("Reflection-based constructor invocation.")]
internal sealed class ReflectionConstructorMetadata : IConstructorMetadata
{
    private readonly ConstructorInfo _constructor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionConstructorMetadata"/> class.
    /// </summary>
    /// <param name="constructor">The constructor to wrap.</param>
    public ReflectionConstructorMetadata(ConstructorInfo constructor)
    {
        _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));

        DeclaringType = constructor.DeclaringType ?? throw new ArgumentException("Constructor must have a declaring type.", nameof(constructor));
        Parameters = constructor.GetParameters()
            .Select(p => new ConstructorParameter(
                p.Name ?? string.Empty,
                p.ParameterType,
                p.HasDefaultValue,
                p.DefaultValue))
            .ToList();
    }

    /// <inheritdoc />
    public Type DeclaringType { get; }

    /// <inheritdoc />
    public IReadOnlyList<ConstructorParameter> Parameters { get; }

    /// <inheritdoc />
    public object Invoke(object?[] args)
    {
        return _constructor.Invoke(args);
    }
}