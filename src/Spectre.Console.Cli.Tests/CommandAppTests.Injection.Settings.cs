#nullable enable

using Microsoft.Extensions.DependencyInjection;

namespace Spectre.Console.Tests.Unit.Cli;

public sealed partial class CommandAppTests
{
    public sealed partial class Injection
    {
        public sealed class Settings
        {
            public sealed class CustomInheritedCommand : Command<CustomInheritedCommandSettings>
            {
                private readonly SomeFakeDependency _dep;

                public CustomInheritedCommand(SomeFakeDependency dep)
                {
                    _dep = dep;
                }

                protected override int Execute(CommandContext context, CustomInheritedCommandSettings settings, CancellationToken cancellationToken)
                {
                    return _dep.GetExitCode();
                }
            }

            public sealed class SomeFakeDependency
            {
                public int GetExitCode()
                {
                    return 22;
                }
            }

            public abstract class CustomBaseCommandSettings : CommandSettings
            {
            }

            public sealed class CustomInheritedCommandSettings : CustomBaseCommandSettings
            {
            }

            private sealed class CustomTypeRegistrar : ITypeRegistrar
            {
                private readonly IServiceCollection _services;

                public CustomTypeRegistrar(IServiceCollection services)
                {
                    _services = services;
                }

                public ITypeResolver Build()
                {
                    return new CustomTypeResolver(_services.BuildServiceProvider());
                }

                public void Register(Type service, Type implementation)
                {
                    _services.AddSingleton(service, implementation);
                }

                public void RegisterInstance(Type service, object implementation)
                {
                    _services.AddSingleton(service, implementation);
                }

                public void RegisterLazy(Type service, Func<object> func)
                {
                    _services.AddSingleton(service, provider => func());
                }

                public void Register<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
                    where TImplementation : TService
                {
                    _services.AddSingleton(typeof(TService), typeof(TImplementation));
                }

                public void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
                    where TImplementation : class
                {
                    _services.AddSingleton(typeof(TImplementation), typeof(TImplementation));
                }

                public void RegisterInstance<TService>(TService implementation)
                    where TService : class
                {
                    ArgumentNullException.ThrowIfNull(implementation);

                    _services.AddSingleton(typeof(TService), implementation);
                }

                public void RegisterLazy<TService>(Func<TService> factory)
                    where TService : class
                {
                    ArgumentNullException.ThrowIfNull(factory);

                    _services.AddSingleton(typeof(TService), provider => factory());
                }
            }

            public sealed class CustomTypeResolver : ITypeResolver
            {
                private readonly IServiceProvider _provider;

                public CustomTypeResolver(IServiceProvider provider)
                {
                    _provider = provider ?? throw new ArgumentNullException(nameof(provider));
                }

                public object? Resolve(Type? type)
                {
                    ArgumentNullException.ThrowIfNull(type);
                    return _provider.GetRequiredService(type);
                }
            }

            [Fact]
            public void Should_Inject_Settings()
            {
                static CustomTypeRegistrar BootstrapServices()
                {
                    var services = new ServiceCollection();
                    services.AddSingleton<SomeFakeDependency, SomeFakeDependency>();
                    return new CustomTypeRegistrar(services);
                }

                // Given
                var app = new CommandAppTester(BootstrapServices());

                app.Configure(config =>
                {
                    config.PropagateExceptions();
                    config.AddBranch<CustomBaseCommandSettings>("foo", b =>
                    {
                        b.AddCommand<CustomInheritedCommand>("bar");
                    });
                });

                // When
                var result = app.Run("foo", "bar");

                // Then
                result.ExitCode.ShouldBe(22);
            }
        }
    }
}