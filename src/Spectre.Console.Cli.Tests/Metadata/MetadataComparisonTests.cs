using Spectre.Console.Cli.Internal.Metadata;
using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Tests.Unit.Cli.Metadata;

/// <summary>
/// Tests that verify source-generated metadata produces identical results
/// to ReflectionMetadataContext for all settings types.
/// </summary>
[ExpectationPath("Metadata")]
public class MetadataComparisonTests(ITestOutputHelper testOutputHelper)
{
    // Two metadata contexts to compare
    private readonly ICommandMetadataContext _reflectionContext = new ReflectionMetadataContext();
    private readonly ICommandMetadataContext _sourceGenContext = new TestMetadata();

    [Fact]
    [Expectation("DogSettings")]
    public Task DogSettings_ArgumentsAndOptions()
    {
        // DogSettings: <AGE> at pos 0, [LEGS] at pos 1 (optional), Name option, GoodBoy option
        return VerifyBothContexts(
            ctx => RunCommand<DogCommand>(ctx, "12", "4", "--name", "Rufus", "--good-boy"));
    }

    [Fact]
    [Expectation("CatSettings")]
    public Task CatSettings_InheritanceAndTypeConverter()
    {
        // CatSettings: [LEGS] at pos 1 (optional), Name option, Agility option
        return VerifyBothContexts(
            ctx => RunCommand<CatCommand>(ctx, "--name", "Whiskers", "--agility", "9"));
    }

    [Fact]
    [Expectation("GiraffeSettings")]
    public Task GiraffeSettings_Inheritance()
    {
        // GiraffeSettings: <LENGTH> at pos 0, [LEGS] at pos 1 (optional), Name option
        return VerifyBothContexts(
            ctx => RunCommand<GiraffeCommand>(ctx, "4", "6", "--name", "Gerry"));
    }

    [Fact]
    [Expectation("HorseSettings")]
    public Task HorseSettings_EnumOption()
    {
        // HorseSettings: [LEGS] at pos 1 (optional), Name option, Day enum option
        return VerifyBothContexts(
            ctx => RunCommand<HorseCommand>(ctx, "--name", "Spirit", "--day", "Friday"));
    }

    [Fact]
    [Expectation("LionSettings")]
    public Task LionSettings_InheritanceChain()
    {
        // LionSettings: <TEETH> at pos 0, inherits from CatSettings
        return VerifyBothContexts(
            ctx => RunCommand<LionCommand>(ctx, "28", "4", "--name", "Simba"));
    }

    [Fact]
    [Expectation("ArgumentOrderSettings")]
    public Task ArgumentOrderSettings_MultiplePositionalArgs()
    {
        // ArgumentOrderSettings: FOO or Qux at pos 0 (bug?), BAR at pos 1, BAZ at pos 2, CORGI at pos 3
        return VerifyBothContexts(
            ctx => RunCommand<ArgumentOrderCommand>(ctx, "1", "2", "3", "4", "5"));
    }

    [Fact]
    [Expectation("ArgumentVectorSettings")]
    public Task ArgumentVectorSettings_StringArray()
    {
        // ArgumentVectorSettings: <Foos> at pos 0 (string array)
        return VerifyBothContexts(
            ctx => RunCommand<ArgumentVectorCommand>(ctx, "first", "second", "third"));
    }

    [Fact]
    [Expectation("StringOptionSettings")]
    public Task StringOptionSettings_SimpleOption()
    {
        // StringOptionSettings: --foo option
        return VerifyBothContexts(
            ctx => RunCommand<StringOptionCommand>(ctx, "--foo", "bar"));
    }

    [Fact]
    [Expectation("OptionVectorSettings")]
    public Task OptionVectorSettings_ArrayOptions()
    {
        // OptionVectorSettings: --foo and --bar array options
        return VerifyBothContexts(
            ctx => RunCommand<OptionVectorCommand>(ctx, "--foo", "a", "--foo", "b", "--bar", "1", "--bar", "2"));
    }

    [Fact]
    [Expectation("HiddenOptionSettings")]
    public Task HiddenOptionSettings_HiddenOption()
    {
        // HiddenOptionSettings: <FOO> at pos 0, --bar hidden option, --baz option
        return VerifyBothContexts(
            ctx => RunCommand<HiddenOptionsCommand>(ctx, "42", "--bar", "5", "--baz", "10"));
    }

    [Fact]
    [Expectation("RequiredOptionsSettings")]
    public Task RequiredOptionsSettings_RequiredOption()
    {
        // RequiredOptionsSettings: --foo required option
        return VerifyBothContexts(
            ctx => RunCommand<RequiredOptionsCommand>(ctx, "--foo", "required_value"));
    }

    [Fact]
    [Expectation("OptionalArgumentWithDefaultValueSettings_NoArgs")]
    public Task OptionalArgumentWithDefaultValueSettings_UsesDefault()
    {
        // OptionalArgumentWithDefaultValueSettings: [GREETING] at pos 0 with default "Hello World"
        return VerifyBothContexts(
            ctx => RunCommand<GreeterCommand>(ctx));
    }

    [Fact]
    [Expectation("OptionalArgumentWithDefaultValueSettings_WithArgs")]
    public Task OptionalArgumentWithDefaultValueSettings_OverridesDefault()
    {
        return VerifyBothContexts(
            ctx => RunCommand<GreeterCommand>(ctx, "Custom Greeting"));
    }

    [Fact]
    [Expectation("FooCommandSettings")]
    public Task FooCommandSettings_OptionalArgument()
    {
        // FooCommandSettings: [QUX] at pos 0 (optional)
        return VerifyBothContexts(
            ctx => RunCommand<FooSettingsCommand>(ctx, "optional_value"));
    }

    [Fact]
    [Expectation("BarCommandSettings")]
    public Task BarCommandSettings_InheritsFromFoo()
    {
        // BarCommandSettings: <CORGI> at pos 0 (required), inherits from FooCommandSettings
        return VerifyBothContexts(
            ctx => RunCommand<BarSettingsCommand>(ctx, "qux_value", "corgi_value"));
    }

    [Fact]
    [Expectation("TurtleSettings")]
    public Task TurtleSettings_ConstructorInjection()
    {
        // TurtleSettings: Name required to be "Lonely George" for validation
        return VerifyBothContexts(
            ctx => RunCommand<TurtleCommand>(ctx, "--name", "Lonely George"));
    }

    [Fact]
    [Expectation("AsynchronousCommandSettings")]
    public Task AsynchronousCommandSettings_AsyncCommand()
    {
        // AsynchronousCommandSettings: --ThrowException option
        return VerifyBothContexts(
            ctx => RunAsyncCommand<AsynchronousCommand>(ctx));
    }

    private Task VerifyBothContexts(Func<ICommandMetadataContext, CommandAppResult> run, int expectedExitCode = 0)
    {
        var reflectionResult = run(_reflectionContext);
        var sourceGenResult = run(_sourceGenContext);

        testOutputHelper.WriteLine(reflectionResult.Output);

        reflectionResult.ExitCode.ShouldBe(expectedExitCode);

        // Assert that both contexts produce identical results
        reflectionResult.Output.ShouldBe(sourceGenResult.Output, "Output should match");
        reflectionResult.ExitCode.ShouldBe(sourceGenResult.ExitCode, "Exit codes should match");

        // Compare settings if available
        if (reflectionResult.Settings != null && sourceGenResult.Settings != null)
        {
            var reflectionType = reflectionResult.Settings.GetType();
            var sourceGenType = sourceGenResult.Settings.GetType();
            reflectionType.ShouldBe(sourceGenType, "Settings types should match");

            // Compare all property values
            foreach (var prop in reflectionType.GetProperties())
            {
                var reflectionValue = prop.GetValue(reflectionResult.Settings);
                var sourceGenValue = prop.GetValue(sourceGenResult.Settings);
                CompareValues(reflectionValue, sourceGenValue, prop.Name);
            }
        }

        // Verify the result using the source-generated path (both should be identical)
        return Verify(new
        {
            sourceGenResult.ExitCode,
            sourceGenResult.Output,
            sourceGenResult.Settings,
        });
    }

    private static void CompareValues(object? reflectionValue, object? sourceGenValue, string propertyName)
    {
        // Handle null cases
        if (reflectionValue == null && sourceGenValue == null)
        {
            return;
        }

        if (reflectionValue == null || sourceGenValue == null)
        {
            reflectionValue.ShouldBe(sourceGenValue, $"Property {propertyName} should match");
            return;
        }

        // Handle special types that don't use value equality
        if (reflectionValue is FileInfo reflectionFile && sourceGenValue is FileInfo sourceGenFile)
        {
            reflectionFile.FullName.ShouldBe(sourceGenFile.FullName, $"Property {propertyName} (FileInfo) should match");
            return;
        }

        if (reflectionValue is DirectoryInfo reflectionDir && sourceGenValue is DirectoryInfo sourceGenDir)
        {
            reflectionDir.FullName.ShouldBe(sourceGenDir.FullName, $"Property {propertyName} (DirectoryInfo) should match");
            return;
        }

        // Handle arrays
        if (reflectionValue is Array reflectionArray && sourceGenValue is Array sourceGenArray)
        {
            reflectionArray.Length.ShouldBe(sourceGenArray.Length, $"Property {propertyName} array length should match");
            for (int i = 0; i < reflectionArray.Length; i++)
            {
                CompareValues(reflectionArray.GetValue(i), sourceGenArray.GetValue(i), $"{propertyName}[{i}]");
            }

            return;
        }

        if (reflectionValue is IFlagValue reflectionFlagValue && sourceGenValue is IFlagValue sourceFlagValue)
        {
            reflectionFlagValue.IsSet.ShouldBe(sourceFlagValue.IsSet);
            reflectionFlagValue.Type.ShouldBe(sourceFlagValue.Type);
            reflectionFlagValue.Value.ShouldBe(sourceFlagValue.Value);

            return;
        }

        // Default comparison
        reflectionValue.ShouldBe(sourceGenValue, $"Property {propertyName} should match");
    }

    private static CommandAppResult RunCommand<TCommand>(ICommandMetadataContext context, params string[] args)
        where TCommand : class, ICommand
    {
        var app = new CommandAppTester { Context = context };
        app.SetDefaultCommand<TCommand>();
        return app.Run(args);
    }

    private static CommandAppResult RunAsyncCommand<TCommand>(ICommandMetadataContext context, params string[] args)
        where TCommand : class, ICommand
    {
        var app = new CommandAppTester { Context = context };
        app.SetDefaultCommand<TCommand>();
        return app.RunAsync(args).GetAwaiter().GetResult();
    }
}
