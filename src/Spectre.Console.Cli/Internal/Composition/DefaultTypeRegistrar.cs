namespace Spectre.Console.Cli;

/// <summary>
/// Default type registrar that uses reflection-based activation for non-generic methods
/// and AOT-safe generic activators for generic methods.
/// </summary>
/// <remarks>
/// The generic registration methods (<see cref="Register{TService, TImplementation}()"/> and
/// <see cref="Register{TImplementation}()"/>) are AOT-safe as they use <see cref="GenericActivator{T}"/>
/// with <see cref="DynamicallyAccessedMembersAttribute"/> to preserve constructor metadata.
/// The non-generic <see cref="Register(Type, Type)"/> method uses reflection-based activation
/// and should only be called when <c>RuntimeFeatures.IsDynamicCodeSupported</c> is true.
/// </remarks>
internal sealed class DefaultTypeRegistrar : ITypeRegistrar
{
    private readonly Queue<Action<ComponentRegistry>> _registry = new();

    [UnconditionalSuppressMessage("AOT", "IL2026:RequiresUnreferencedCode",
        Justification =
            "In AOT scenarios, RegisterKnownTypes uses generic Register<T>() which creates GenericActivator<T> (AOT-safe). " +
            "DefaultTypeResolver only receives AOT-safe activators in this path.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
        Justification =
            "In AOT scenarios, RegisterKnownTypes uses generic Register<T>() which creates GenericActivator<T> (AOT-safe). " +
            "DefaultTypeResolver only receives AOT-safe activators in this path.")]
    public ITypeResolver Build()
    {
        var container = new DefaultTypeResolver();
        while (_registry.Count > 0)
        {
            var action = _registry.Dequeue();
            action(container.Registry);
        }

        return container;
    }

    [UnconditionalSuppressMessage("AOT", "IL2026:RequiresUnreferencedCode",
        Justification =
            "Register(Type, Type) is only called from RegisterDependencies when RuntimeFeatures.IsDynamicCodeSupported is true. " +
            "In AOT scenarios, RegisterKnownTypes uses generic Register<T>() instead.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
        Justification =
            "Register(Type, Type) is only called from RegisterDependencies when RuntimeFeatures.IsDynamicCodeSupported is true. " +
            "In AOT scenarios, RegisterKnownTypes uses generic Register<T>() instead.")]
    [UnconditionalSuppressMessage("AOT", "IL2067:UnrecognizedReflectionPattern",
        Justification =
            "Register(Type, Type) is only called from RegisterDependencies when RuntimeFeatures.IsDynamicCodeSupported is true. " +
            "In AOT scenarios, RegisterKnownTypes uses generic Register<T>() instead.")]
    [UnconditionalSuppressMessage("AOT", "IL2070:UnrecognizedReflectionPattern",
        Justification =
            "Register(Type, Type) is only called from RegisterDependencies when RuntimeFeatures.IsDynamicCodeSupported is true. " +
            "In AOT scenarios, RegisterKnownTypes uses generic Register<T>() instead.")]
    public void Register(Type service, Type implementation)
    {
        var registration =
            new ComponentRegistration(implementation, new ReflectionActivator(implementation), [service]);
        _registry.Enqueue(registry => registry.Register(registration));
    }

    public void RegisterInstance(Type service, object implementation)
    {
        var registration =
            new ComponentRegistration(service, new CachingActivator(new InstanceActivator(implementation)));
        _registry.Enqueue(registry => registry.Register(registration));
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        _registry.Enqueue(registry =>
        {
            var activator = new CachingActivator(new InstanceActivator(factory()));
            var registration = new ComponentRegistration(service, activator);

            registry.Register(registration);
        });
    }

    /// <inheritdoc/>
    public void Register<TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
        where TImplementation : TService
    {
        var registration = new ComponentRegistration(
            typeof(TImplementation),
            new GenericActivator<TImplementation>(),
            [typeof(TService)]);
        _registry.Enqueue(registry => registry.Register(registration));
    }

    /// <inheritdoc/>
    public void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
        where TImplementation : class
    {
        var registration = new ComponentRegistration(
            typeof(TImplementation),
            new GenericActivator<TImplementation>(),
            [typeof(TImplementation)]);
        _registry.Enqueue(registry => registry.Register(registration));
    }

    /// <inheritdoc/>
    public void RegisterInstance<TService>(TService implementation)
        where TService : class
    {
        if (implementation is null)
        {
            throw new ArgumentNullException(nameof(implementation));
        }

        var registration = new ComponentRegistration(
            typeof(TService),
            new CachingActivator(new InstanceActivator(implementation)));
        _registry.Enqueue(registry => registry.Register(registration));
    }

    /// <inheritdoc/>
    public void RegisterLazy<TService>(Func<TService> factory)
        where TService : class
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        _registry.Enqueue(registry =>
        {
            var activator = new CachingActivator(new InstanceActivator(factory()));
            var registration = new ComponentRegistration(typeof(TService), activator);
            registry.Register(registration);
        });
    }
}