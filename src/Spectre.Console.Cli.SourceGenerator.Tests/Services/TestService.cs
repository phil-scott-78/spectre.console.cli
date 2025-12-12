namespace Spectre.Console.Cli.SourceGenerator.Tests.Services;

/// <summary>
/// Implementation of <see cref="ITestService"/> for testing DI in AOT scenarios.
/// </summary>
public sealed class TestService : ITestService
{
    /// <inheritdoc />
    public string GetMessage(string name) => $"Hello, {name}!";
}
