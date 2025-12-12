namespace Spectre.Console.Cli.SourceGenerator.Tests.Services;

/// <summary>
/// Service interface for testing dependency injection in AOT scenarios.
/// </summary>
public interface ITestService
{
    /// <summary>
    /// Gets a greeting message for the specified name.
    /// </summary>
    string GetMessage(string name);
}
