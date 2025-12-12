namespace Spectre.Console.Cli;

/// <summary>
/// Representation of a multi map.
/// </summary>
public interface IMultiMap
{
    /// <summary>
    /// Adds a key and a value to the multimap.
    /// </summary>
    /// <param name="pair">The pair to add.</param>
    void Add((object? Key, object? Value) pair);
}

/// <summary>
/// A multi-map implementation that supports multiple values per key.
/// Implements <see cref="ILookup{TKey, TValue}"/>, <see cref="IDictionary{TKey, TValue}"/>,
/// and <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the keys.</typeparam>
/// <typeparam name="TValue">The type of the values.</typeparam>
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes")]
public sealed class MultiMap<TKey, TValue> : IMultiMap, ILookup<TKey, TValue>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly IDictionary<TKey, MultiMapGrouping> _lookup = new Dictionary<TKey, MultiMapGrouping>();
    private readonly IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

    /// <summary>
    /// Gets the number of key/value pairs contained in the multi-map.
    /// </summary>
    public int Count => _lookup.Count;

    /// <summary>
    /// Gets a value indicating whether the multi-map is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets a collection containing all the keys in the multi-map.
    /// </summary>
    public ICollection<TKey> Keys => _lookup.Keys;

    /// <summary>
    /// Gets the collection of values in the multi-map.
    /// </summary>
    public ICollection<TValue> Values => _dictionary.Values;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _lookup.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

    TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => _dictionary[key];

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get
        {
            return _dictionary[key];
        }
        set
        {
            Add(key, value);
        }
    }

    /// <summary>
    /// Retrieves the collection of values associated with the specified key.
    /// Returns an empty collection if the key does not exist in the multi-map.
    /// </summary>
    /// <param name="key">The key whose associated values are to be retrieved.</param>
    /// <returns>An enumerable collection of values associated with the specified key.</returns>
    public IEnumerable<TValue> this[TKey key]
    {
        get
        {
            if (_lookup.TryGetValue(key, out var group))
            {
                return group;
            }

            return Array.Empty<TValue>();
        }
    }

    private sealed class MultiMapGrouping : IGrouping<TKey, TValue>
    {
        private readonly List<TValue> _items;

        public TKey Key { get; }

        public MultiMapGrouping(TKey key, List<TValue> items)
        {
            Key = key;
            _items = items;
        }

        public void Add(TValue value)
        {
            _items.Add(value);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Determines whether the multi-map contains a specific key.
    /// </summary>
    /// <param name="key">The key to locate in the multi-map.</param>
    /// <returns>True if the multi-map contains an element with the specified key; otherwise, false.</returns>
    public bool Contains(TKey key)
    {
        return _lookup.ContainsKey(key);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the groups in the multi-map.
    /// </summary>
    /// <returns>An enumerator for the groups in the multi-map.</returns>
    public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
    {
        foreach (var group in _lookup.Values)
        {
            yield return group;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Adds a key and a value to the multi-map. If the key already exists, the value is added to the existing collection of values.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to associate with the key.</param>
    public void Add(TKey key, TValue value)
    {
        if (!_lookup.ContainsKey(key))
        {
            _lookup[key] = new MultiMapGrouping(key, new List<TValue>());
        }

        _lookup[key].Add(value);
        _dictionary[key] = value;
    }

    /// <summary>
    /// Determines whether the multi-map contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the multi-map.</param>
    /// <returns>True if the multi-map contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(TKey key)
    {
        return Contains(key);
    }

    /// <summary>
    /// Removes the elements with the specified key from the multi-map.
    /// </summary>
    /// <param name="key">The key of the elements to remove.</param>
    /// <returns>
    /// True if the elements are successfully removed; otherwise, false.
    /// This method also returns false if the key was not found in the multi-map.
    /// </returns>
    public bool Remove(TKey key)
    {
        return _lookup.Remove(key);
    }

#if NETSTANDARD2_0
    /// <summary>
    /// Attempts to retrieve the value associated with the specified key in the multi-map.
    /// </summary>
    /// <param name="key">The key whose value is to be retrieved.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>True if the multi-map contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }
#else
    /// <summary>
    /// Attempts to retrieve the value associated with the specified key in the multi-map.
    /// </summary>
    /// <param name="key">The key whose value is to be retrieved.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>True if the multi-map contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }
#endif

    /// <summary>
    /// Adds a key-value pair to the multi-map. If the key already exists, the value is added to the existing collection of values.
    /// </summary>
    /// <param name="item">The key-value pair to add to the multi-map.</param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all keys and values from the multi-map.
    /// </summary>
    public void Clear()
    {
        _lookup.Clear();
    }

    /// <summary>
    /// Determines whether the multi-map contains a specific key-value pair.
    /// </summary>
    /// <param name="item">The key-value pair to locate in the multi-map.</param>
    /// <returns>True if the multi-map contains an element with the specified key-value pair; otherwise, false.</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return Contains(item.Key);
    }

    /// <summary>
    /// Copies the elements of the multi-map to a specified array, starting at a particular index in the array.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from the multi-map. The array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _dictionary.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes a key-value pair from the multi-map.
    /// </summary>
    /// <param name="item">The key-value pair to remove.</param>
    /// <returns>
    /// True if the key-value pair is successfully removed; otherwise, false.
    /// This method also returns false if the key does not exist in the multi-map.
    /// </returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    /// <summary>
    /// Adds a key-value pair to the multi-map.
    /// </summary>
    /// <param name="pair">The key-value pair to be added, where the key and value can be nullable objects.</param>
    public void Add((object? Key, object? Value) pair)
    {
        if (pair.Key != null)
        {
#pragma warning disable CS8604 // Possible null reference argument of value.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Add((TKey)pair.Key, (TValue)pair.Value);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument of value.
        }
    }
}