namespace Spectre.Console.Cli;

internal static class CommandParameterComparer
{
    public static readonly ByBackingPropertyComparer ByBackingProperty = new ByBackingPropertyComparer();

    public sealed class ByBackingPropertyComparer : IEqualityComparer<CommandParameter?>
    {
        public bool Equals(CommandParameter? x, CommandParameter? y)
        {
            if (x is null || y is null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            // AOT-safe identity comparison using accessor properties instead of MetadataToken
            return x.Accessor.DeclaringType == y.Accessor.DeclaringType &&
                   x.Accessor.Name == y.Accessor.Name &&
                   x.Accessor.PropertyType == y.Accessor.PropertyType;
        }

        public int GetHashCode(CommandParameter? obj)
        {
            if (obj is null)
            {
                return 0;
            }

            // AOT-safe hash code using accessor properties
            unchecked
            {
                var hash = obj.Accessor.DeclaringType?.GetHashCode() ?? 0;
                hash = (hash * 397) ^ (obj.Accessor.Name?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (obj.Accessor.PropertyType?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}