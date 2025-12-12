using System.Collections;

namespace Spectre.Console.Cli.SourceGenerator.Model;

/// <summary>
/// An immutable array wrapper with structural equality semantics.
/// Used for incremental generator caching to work correctly.
/// </summary>
/// <typeparam name="T">The element type, must implement IEquatable{T}.</typeparam>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// An empty array.
    /// </summary>
    public static readonly EquatableArray<T> Empty = new(Array.Empty<T>());

    private readonly T[]? _array;

    /// <summary>
    /// Creates a new EquatableArray from the given array.
    /// </summary>
    public EquatableArray(T[] array)
    {
        _array = array;
    }

    /// <summary>
    /// Creates a new EquatableArray from the given enumerable.
    /// </summary>
    public EquatableArray(IEnumerable<T> items)
    {
        _array = items is T[] arr ? arr : new List<T>(items).ToArray();
    }

    /// <summary>
    /// Gets the underlying array as a span.
    /// </summary>
    public ReadOnlySpan<T> AsSpan() => _array.AsSpan();

    /// <summary>
    /// Gets the number of elements.
    /// </summary>
    public int Count => _array?.Length ?? 0;

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    public T this[int index] => (_array ?? Array.Empty<T>())[index];

    /// <inheritdoc />
    public bool Equals(EquatableArray<T> other)
    {
        var self = _array ?? Array.Empty<T>();
        var otherArray = other._array ?? Array.Empty<T>();

        if (self.Length != otherArray.Length)
        {
            return false;
        }

        for (var i = 0; i < self.Length; i++)
        {
            if (!self[i].Equals(otherArray[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is EquatableArray<T> other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var array = _array ?? Array.Empty<T>();
        var hash = 17;

        foreach (var item in array)
        {
            hash = hash * 31 + (item?.GetHashCode() ?? 0);
        }

        return hash;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        var array = _array ?? Array.Empty<T>();
        return ((IEnumerable<T>)array).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
    {
        return !left.Equals(right);
    }
}
