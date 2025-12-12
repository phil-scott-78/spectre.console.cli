namespace Spectre.Console.Cli;

internal abstract class ComponentActivator
{
    public abstract object Activate(DefaultTypeResolver container);
}

internal class CachingActivator : ComponentActivator
{
    private readonly ComponentActivator _activator;
    private object? _result;

    public CachingActivator(ComponentActivator activator)
    {
        _activator = activator ?? throw new ArgumentNullException(nameof(activator));
        _result = null;
    }

    public override object Activate(DefaultTypeResolver container)
    {
        return _result ??= _activator.Activate(container);
    }
}

internal sealed class InstanceActivator : ComponentActivator
{
    private readonly object _instance;

    public InstanceActivator(object instance)
    {
        _instance = instance;
    }

    public override object Activate(DefaultTypeResolver container)
    {
        return _instance;
    }
}

/// <summary>
/// Activator that uses reflection to create instances by finding and invoking constructors.
/// </summary>
/// <remarks>
/// This activator is not compatible with AOT compilation. For AOT scenarios,
/// provide a custom <see cref="ITypeRegistrar"/> that does not use reflection-based activation.
/// </remarks>
[RequiresDynamicCode("ReflectionActivator uses reflection to discover and invoke constructors.")]
[RequiresUnreferencedCode("ReflectionActivator uses reflection to discover and invoke constructors.")]
internal sealed class ReflectionActivator : ComponentActivator
{
    private readonly ConstructorInfo _constructor;
    private readonly List<ParameterInfo> _parameters;

    public ReflectionActivator([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
    {
        _constructor = GetGreediestConstructor(type);
        _parameters = new List<ParameterInfo>(_constructor.GetParameters());
    }

    public override object Activate(DefaultTypeResolver container)
    {
        var parameters = new object?[_parameters.Count];
        for (var i = 0; i < _parameters.Count; i++)
        {
            var parameter = _parameters[i];
            if (parameter.ParameterType == typeof(DefaultTypeResolver))
            {
                parameters[i] = container;
            }
            else
            {
                var resolved = container.Resolve(parameter.ParameterType);
                if (resolved == null)
                {
                    if (!parameter.IsOptional)
                    {
                        throw new InvalidOperationException($"Could not find registration for '{parameter.ParameterType.FullName}'.");
                    }

                    parameters[i] = null;
                }
                else
                {
                    parameters[i] = resolved;
                }
            }
        }

        return _constructor.Invoke(parameters);
    }

    private static ConstructorInfo GetGreediestConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
    {
        ConstructorInfo? current = null;
        var count = -1;
        foreach (var constructor in type.GetTypeInfo().GetConstructors())
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length > count)
            {
                count = parameters.Length;
                current = constructor;
            }
        }

        if (current == null)
        {
            throw new InvalidOperationException($"Could not find a constructor for '{type.FullName}'.");
        }

        return current;
    }
}

/// <summary>
/// Activator that uses generic type parameter with DAM attributes for AOT-safe activation.
/// The DynamicallyAccessedMembers attribute preserves constructor metadata for both
/// trimming and AOT compilation.
/// </summary>
/// <typeparam name="T">The type to activate. Must have its constructors preserved via DAM.</typeparam>
internal sealed class GenericActivator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T> : ComponentActivator
{
    private readonly ConstructorInfo _constructor;
    private readonly List<ParameterInfo> _parameters;

    public GenericActivator()
    {
        _constructor = GetGreediestConstructor();
        _parameters = new List<ParameterInfo>(_constructor.GetParameters());
    }

    public override object Activate(DefaultTypeResolver container)
    {
        var parameters = new object?[_parameters.Count];
        for (var i = 0; i < _parameters.Count; i++)
        {
            var parameter = _parameters[i];
            if (parameter.ParameterType == typeof(DefaultTypeResolver))
            {
                parameters[i] = container;
            }
            else
            {
                var resolved = container.Resolve(parameter.ParameterType);
                if (resolved == null)
                {
                    if (!parameter.IsOptional)
                    {
                        throw new InvalidOperationException($"Could not find registration for '{parameter.ParameterType.FullName}'.");
                    }

                    parameters[i] = null;
                }
                else
                {
                    parameters[i] = resolved;
                }
            }
        }

        return _constructor.Invoke(parameters);
    }

    private static ConstructorInfo GetGreediestConstructor()
    {
        ConstructorInfo? current = null;
        var count = -1;
        foreach (var constructor in typeof(T).GetTypeInfo().GetConstructors())
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length > count)
            {
                count = parameters.Length;
                current = constructor;
            }
        }

        if (current == null)
        {
            throw new InvalidOperationException($"Could not find a constructor for '{typeof(T).FullName}'.");
        }

        return current;
    }
}