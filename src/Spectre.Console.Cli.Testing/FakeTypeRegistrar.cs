using System.Diagnostics.CodeAnalysis;

namespace Spectre.Console.Cli.Testing;

/// <summary>
/// A fake type registrar suitable for testing.
/// </summary>
public sealed class FakeTypeRegistrar : ITypeRegistrar
{
    /// <summary>
    /// Gets all registrations.
    /// </summary>
    public Dictionary<Type, List<Type>> Registrations { get; }

    /// <summary>
    /// Gets all singleton registrations.
    /// </summary>
    public Dictionary<Type, List<object>> Instances { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTypeRegistrar"/> class.
    /// </summary>
    public FakeTypeRegistrar()
    {
        Registrations = new Dictionary<Type, List<Type>>();
        Instances = new Dictionary<Type, List<object>>();
    }

    /// <inheritdoc/>
    public void Register(Type service, Type implementation)
    {
        if (Registrations.TryGetValue(service, out var registration))
        {
            registration.Add(implementation);
        }
        else
        {
            Registrations.Add(service, [implementation]);
        }
    }

    /// <inheritdoc/>
    public void RegisterInstance(Type service, object implementation)
    {
        if (Instances.TryGetValue(service, out var instance))
        {
            instance.Add(implementation);
        }
        else
        {
            Instances.Add(service, [implementation]);
        }
    }

    /// <inheritdoc/>
    public void RegisterLazy(Type service, Func<object> factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        if (Instances.TryGetValue(service, out var instance))
        {
            instance.Add(factory());
        }
        else
        {
            Instances.Add(service, [factory()]);
        }
    }

    /// <inheritdoc/>
    public ITypeResolver Build()
    {
        return new FakeTypeResolver(Registrations, Instances);
    }

    /// <inheritdoc/>
    public void Register<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
        where TImplementation : TService
    {
        Register(typeof(TService), typeof(TImplementation));
    }

    /// <inheritdoc/>
    public void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
        where TImplementation : class
    {
        Register(typeof(TImplementation), typeof(TImplementation));
    }

    /// <inheritdoc/>
    public void RegisterInstance<TService>(TService implementation)
        where TService : class
    {
        if (implementation is null)
        {
            throw new ArgumentNullException(nameof(implementation));
        }

        RegisterInstance(typeof(TService), implementation);
    }

    /// <inheritdoc/>
    public void RegisterLazy<TService>(Func<TService> factory)
        where TService : class
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        RegisterLazy(typeof(TService), () => factory());
    }
}