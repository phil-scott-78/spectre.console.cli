#if NETSTANDARD2_0
using System.Runtime.CompilerServices;

namespace Spectre.Console.Cli;

internal static class ExceptionPolyfills
{
    extension(ArgumentNullException)
    {
        public static void ThrowIfNull([System.Diagnostics.CodeAnalysis.NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
            {
                ThrowArgumentNullException(paramName);
            }
        }
    }

    [DoesNotReturn]
    private static void ThrowArgumentNullException(string? paramName) =>
        throw new ArgumentNullException(paramName);
}
#endif