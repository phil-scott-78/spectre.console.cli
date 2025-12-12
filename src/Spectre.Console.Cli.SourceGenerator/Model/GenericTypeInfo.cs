namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// Represents a MultiMap key-value type pair.
/// </summary>
internal readonly struct MultiMapTypeKey : IEquatable<MultiMapTypeKey>
{
    public string KeyType { get; }
    public string ValueType { get; }

    public MultiMapTypeKey(string keyType, string valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    public bool Equals(MultiMapTypeKey other)
    {
        return KeyType == other.KeyType && ValueType == other.ValueType;
    }

    public void Deconstruct(out string keyType, out string valueType)
    {
        keyType = KeyType;
        valueType = ValueType;
    }

    public override bool Equals(object? obj) => obj is MultiMapTypeKey other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            return ((KeyType?.GetHashCode() ?? 0) * 397) ^ (ValueType?.GetHashCode() ?? 0);
        }
    }
}

/// <summary>
/// A simple string wrapper that implements IEquatable for use in EquatableArray.
/// </summary>
internal readonly struct EquatableString : IEquatable<EquatableString>
{
    public string Value { get; }

    public EquatableString(string value)
    {
        Value = value;
    }

    public bool Equals(EquatableString other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is EquatableString other && Equals(other);
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    public override string ToString() => Value;

    public static implicit operator string(EquatableString s) => s.Value;
    public static implicit operator EquatableString(string s) => new(s);
}

/// <summary>
/// Holds discovered generic type instantiations that need factories.
/// </summary>
internal sealed class GenericTypeInfo : IEquatable<GenericTypeInfo>
{
    /// <summary>
    /// Gets the set of MultiMap key-value type pairs.
    /// </summary>
    public EquatableArray<MultiMapTypeKey> MultiMapTypes { get; }

    /// <summary>
    /// Gets the set of FlagValue inner types.
    /// </summary>
    public EquatableArray<EquatableString> FlagValueTypes { get; }

    /// <summary>
    /// Gets the set of array element types.
    /// </summary>
    public EquatableArray<EquatableString> ArrayElementTypes { get; }

    /// <summary>
    /// Gets the set of custom TypeConverter types referenced by TypeConverterAttribute.
    /// </summary>
    public EquatableArray<EquatableString> ConverterTypes { get; }

    /// <summary>
    /// Gets the set of custom PairDeconstructor types.
    /// </summary>
    public EquatableArray<EquatableString> PairDeconstructorTypes { get; }

    /// <summary>
    /// Gets the set of custom ValueProvider types.
    /// </summary>
    public EquatableArray<EquatableString> ValueProviderTypes { get; }

    public GenericTypeInfo(
        EquatableArray<MultiMapTypeKey> multiMapTypes,
        EquatableArray<EquatableString> flagValueTypes,
        EquatableArray<EquatableString> arrayElementTypes,
        EquatableArray<EquatableString> converterTypes,
        EquatableArray<EquatableString> pairDeconstructorTypes,
        EquatableArray<EquatableString> valueProviderTypes)
    {
        MultiMapTypes = multiMapTypes;
        FlagValueTypes = flagValueTypes;
        ArrayElementTypes = arrayElementTypes;
        ConverterTypes = converterTypes;
        PairDeconstructorTypes = pairDeconstructorTypes;
        ValueProviderTypes = valueProviderTypes;
    }

    /// <summary>
    /// Creates an empty GenericTypeInfo.
    /// </summary>
    public static GenericTypeInfo Empty { get; } = new(
        EquatableArray<MultiMapTypeKey>.Empty,
        EquatableArray<EquatableString>.Empty,
        EquatableArray<EquatableString>.Empty,
        EquatableArray<EquatableString>.Empty,
        EquatableArray<EquatableString>.Empty,
        EquatableArray<EquatableString>.Empty);

    public bool Equals(GenericTypeInfo? other)
    {
        if (other is null) return false;
        return MultiMapTypes.Equals(other.MultiMapTypes) &&
               FlagValueTypes.Equals(other.FlagValueTypes) &&
               ArrayElementTypes.Equals(other.ArrayElementTypes) &&
               ConverterTypes.Equals(other.ConverterTypes) &&
               PairDeconstructorTypes.Equals(other.PairDeconstructorTypes) &&
               ValueProviderTypes.Equals(other.ValueProviderTypes);
    }

    public override bool Equals(object? obj) => obj is GenericTypeInfo other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = MultiMapTypes.GetHashCode();
            hash = (hash * 397) ^ FlagValueTypes.GetHashCode();
            hash = (hash * 397) ^ ArrayElementTypes.GetHashCode();
            hash = (hash * 397) ^ ConverterTypes.GetHashCode();
            hash = (hash * 397) ^ PairDeconstructorTypes.GetHashCode();
            hash = (hash * 397) ^ ValueProviderTypes.GetHashCode();
            return hash;
        }
    }
}

/// <summary>
/// A mutable builder for collecting generic type information during extraction.
/// </summary>
internal sealed class GenericTypeInfoBuilder
{
    public HashSet<(string KeyType, string ValueType)> MultiMapTypes { get; } = new();
    public HashSet<string> FlagValueTypes { get; } = new();
    public HashSet<string> ArrayElementTypes { get; } = new();
    public HashSet<string> ConverterTypes { get; } = new();
    public HashSet<string> PairDeconstructorTypes { get; } = new();
    public HashSet<string> ValueProviderTypes { get; } = new();

    /// <summary>
    /// Builds an immutable GenericTypeInfo from the collected data.
    /// </summary>
    public GenericTypeInfo Build()
    {
        return new GenericTypeInfo(
            new EquatableArray<MultiMapTypeKey>(
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Select(MultiMapTypes, t => new MultiMapTypeKey(t.KeyType, t.ValueType)))),
            new EquatableArray<EquatableString>(
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Select(FlagValueTypes, s => new EquatableString(s)))),
            new EquatableArray<EquatableString>(
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Select(ArrayElementTypes, s => new EquatableString(s)))),
            new EquatableArray<EquatableString>(
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Select(ConverterTypes, s => new EquatableString(s)))),
            new EquatableArray<EquatableString>(
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Select(PairDeconstructorTypes, s => new EquatableString(s)))),
            new EquatableArray<EquatableString>(
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Select(ValueProviderTypes, s => new EquatableString(s)))));
    }
}
