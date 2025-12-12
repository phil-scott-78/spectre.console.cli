using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal static class CommandBinder
{
    public static CommandSettings Bind(CommandTree? tree, Type settingsType, ITypeResolver resolver, ICommandMetadataContext metadataContext)
    {
        var lookup = CommandValueResolver.GetParameterValues(tree, resolver, metadataContext);
        var settingsMetadata = metadataContext.GetSettingsMetadata(settingsType);

        // Find the greediest satisfiable constructor
        // Store the resolution cache with each candidate to avoid re-resolving services
        var candidates = new List<(IConstructorMetadata Constructor, int Score, int SatisfiedCount, Dictionary<Type, object> ResolutionCache)>();

        foreach (var constructor in settingsMetadata.Constructors)
        {
            var (isSatisfiable, score, satisfiedCount, resolutionCache) = EvaluateConstructor(constructor, lookup, resolver);
            if (isSatisfiable)
            {
                candidates.Add((constructor, score, satisfiedCount, resolutionCache));
            }
        }

        if (candidates.Count > 0)
        {
            // Sort by score descending, then by satisfied count descending (greedy tie-breaker)
            candidates.Sort((a, b) =>
            {
                var scoreCompare = b.Score.CompareTo(a.Score);
                return scoreCompare != 0 ? scoreCompare : b.SatisfiedCount.CompareTo(a.SatisfiedCount);
            });

            var bestScore = candidates[0].Score;
            var bestSatisfiedCount = candidates[0].SatisfiedCount;

            // Check for ambiguous constructors (multiple with same score AND same satisfied count, and score > 0)
            if (bestScore > 0)
            {
                var bestConstructors = candidates.Where(c => c.Score == bestScore && c.SatisfiedCount == bestSatisfiedCount).ToList();
                if (bestConstructors.Count > 1)
                {
                    throw CommandRuntimeException.AmbiguousConstructors(
                        settingsType,
                        bestConstructors.Select(c => c.Constructor));
                }

                // Use the best constructor, passing the resolution cache to avoid re-resolving
                return CommandConstructorBinder.CreateSettings(lookup, candidates[0].Constructor, resolver, candidates[0].ResolutionCache);
            }
        }

        // Fall back to property injection
        return CommandPropertyBinder.CreateSettings(lookup, settingsType, resolver, metadataContext);
    }

    private static (bool IsSatisfiable, int Score, int SatisfiedCount, Dictionary<Type, object> ResolutionCache) EvaluateConstructor(
        IConstructorMetadata constructor,
        CommandValueLookup lookup,
        ITypeResolver resolver)
    {
        var score = 0;
        var satisfiedCount = 0;
        var resolutionCache = new Dictionary<Type, object>();

        foreach (var parameter in constructor.Parameters)
        {
            // Check if parameter can be satisfied by bound value
            if (lookup.HasParameterWithName(parameter.Name))
            {
                score++;
                satisfiedCount++;
                continue;
            }

            // Check if parameter has a default value
            if (parameter.HasDefaultValue)
            {
                // Default value doesn't contribute to score, but is satisfiable
                satisfiedCount++;
                continue;
            }

            // Check if parameter can be resolved via DI
            // Cache the resolved value so we don't have to resolve again during binding
            var resolved = resolver.Resolve(parameter.ParameterType);
            if (resolved != null)
            {
                resolutionCache[parameter.ParameterType] = resolved;
                // DI resolution doesn't contribute to score (to prefer bound values)
                satisfiedCount++;
                continue;
            }

            // Parameter cannot be satisfied
            return (false, 0, 0, resolutionCache);
        }

        return (true, score, satisfiedCount, resolutionCache);
    }
}