using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

/// <summary>
/// A frozen dictionary with integer keys.
/// </summary>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
/// <remarks>
/// Frozen dictionaries are immutable and are optimized for situations where a dictionary
/// is created infrequently, but used repeatedly at runtime. They have a relatively high
/// cost to create, but provide excellent lookup performance. These are thus ideal for cases
/// where a dictionary is created at startup of an application and used throughout the life
/// of the application.
/// </remarks>
[DebuggerTypeProxy(typeof(IFrozenIntDictionaryDebugView<>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenIntDictionary<TValue> : IFrozenDictionary<int, TValue>
{
    private readonly FrozenHashTable _hashTable;
    private readonly TValue[] _values;

    /// <summary>
    /// Gets an empty frozen integer dictionary.
    /// </summary>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Usability is good in this case.")]
    public static FrozenIntDictionary<TValue> Empty => default;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenIntDictionary{TValue}"/> struct.
    /// </summary>
    /// <param name="pairs">The pairs to initialize the dictionary with.</param>
    /// <exception cref="ArgumentException">If more than 64K pairs are added.</exception>
    /// <remarks>
    /// Tf the same key appears multiple times in the input, the latter one in the sequence takes precedence.
    /// </remarks>
    internal FrozenIntDictionary(IEnumerable<KeyValuePair<int, TValue>> pairs)
    {
        var d = new Dictionary<int, TValue>();
        foreach (var pair in pairs)
        {
            d[pair.Key] = pair.Value;
        }

        var incoming = d.ToList();

        _values = incoming.Count == 0 ? Array.Empty<TValue>() : new TValue[incoming.Count];

        var values = _values;
        _hashTable = FrozenHashTable.Create(
            incoming,
            pair => pair.Key,
            (index, pair) => values[index] = pair.Value);
    }

    /// <inheritdoc />
    public FrozenList<int> Keys => new(_hashTable.HashCodes);

    /// <inheritdoc />
    public FrozenList<TValue> Values => new(_values);

    /// <inheritdoc />
    public FrozenPairEnumerator<int, TValue> GetEnumerator() => new(_hashTable.HashCodes, _values);

    /// <summary>
    /// Gets an enumeration of the dictionary's keys.
    /// </summary>
    IEnumerable<int> IReadOnlyDictionary<int, TValue>.Keys => Count > 0 ? _hashTable.HashCodes : EmptyReadOnlyList<int>.Instance;

    /// <summary>
    /// Gets an enumeration of the dictionary's values.
    /// </summary>
    IEnumerable<TValue> IReadOnlyDictionary<int, TValue>.Values => Count > 0 ? _values : EmptyReadOnlyList<TValue>.Instance;

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator<KeyValuePair<int, TValue>> IEnumerable<KeyValuePair<int, TValue>>.GetEnumerator()
        => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<int, TValue>>.Instance.GetEnumerator();

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<int, TValue>>.Instance.GetEnumerator();

    /// <summary>
    /// Gets the number of key/value pairs in the dictionary.
    /// </summary>
    public int Count => _hashTable.Count;

    /// <summary>
    /// Gets the value associated to the given key.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    /// <returns>The associated value.</returns>
    /// <exception cref="KeyNotFoundException">If the key doesn't exist in the dictionary.</exception>
    public TValue this[int key]
    {
        get
        {
            _hashTable.FindMatchingEntries(key, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (key == _hashTable.EntryHashCode(index))
                {
                    return _values[index];
                }

                index++;
            }

            throw new KeyNotFoundException();
        }
    }

    /// <summary>
    /// Checks whether a particular key exists in the dictionary.
    /// </summary>
    /// <param name="key">The key to probe for.</param>
    /// <returns><see langword="true"/> if the key is in the dictionary, otherwise <see langword="false"/>.</returns>
    public bool ContainsKey(int key)
    {
        _hashTable.FindMatchingEntries(key, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (key == _hashTable.EntryHashCode(index))
            {
                return true;
            }

            index++;
        }

        return false;
    }

    /// <summary>
    /// Tries to get a value associated with a specific key.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <returns><see langword="true"/> if the key was found, otherwise <see langword="false"/>.</returns>
#if NETCOREAPP3_1_OR_GREATER
    public bool TryGetValue(int key, [MaybeNullWhen(false)] out TValue value)
#else
    public bool TryGetValue(int key, out TValue value)
#endif
    {
        _hashTable.FindMatchingEntries(key, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (key == _hashTable.EntryHashCode(index))
            {
                value = _values[index];
                return true;
            }

            index++;
        }

        value = default!;
        return false;
    }

    /// <inheritdoc />
    public ref readonly TValue GetByRef(int key)
    {
        _hashTable.FindMatchingEntries(key, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (key == _hashTable.EntryHashCode(index))
            {
                return ref _values[index];
            }

            index++;
        }

        throw new KeyNotFoundException();
    }

    /// <inheritdoc />
    public ref readonly TValue TryGetByRef(int key)
    {
        _hashTable.FindMatchingEntries(key, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (key == _hashTable.EntryHashCode(index))
            {
                return ref _values[index];
            }

            index++;
        }

        return ref ByReference.Null<TValue>();
    }
}
