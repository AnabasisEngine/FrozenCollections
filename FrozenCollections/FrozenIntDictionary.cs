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
[DebuggerTypeProxy(typeof(IFrozenDictionaryDebugView<,>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenIntDictionary<TValue> : IFrozenDictionary<int, TValue>
{
    private readonly FrozenHashTable _hashTable;
    private readonly TValue[] _values;

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
        var incoming = new Dictionary<int, TValue>(pairs).ToList();

        _values = new TValue[incoming.Count];

        var values = _values;
        _hashTable = FrozenHashTable.Create(
            incoming,
            item => item.Key,
            (index, item) => values[index] = item.Value);
    }

    /// <inheritdoc />
    public FrozenList<int> Keys => new(_hashTable.HashCodes);

    /// <inheritdoc />
    public FrozenList<TValue> Values => new(_values);

    /// <inheritdoc />
    public PairEnumerator<int, TValue> GetEnumerator() => new(_hashTable.HashCodes, _values);

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
    IEnumerator<KeyValuePair<int, TValue>> IEnumerable<KeyValuePair<int, TValue>>.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<int, TValue>>.Instance.Enumerator;

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<int, TValue>>.Instance.Enumerator;

    /// <summary>
    /// Gets the number of key/value pairs in the dictionary.
    /// </summary>
    public int Count => _values.Length;

    /// <inheritdoc />
    public TValue this[int key]
    {
        get
        {
            var hashCode = key;
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

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

    /// <inheritdoc />
    public bool ContainsKey(int key)
    {
        var hashCode = key;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

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

    /// <inheritdoc />
    public bool TryGetValue(int key, [MaybeNullWhen(false)] out TValue value)
    {
        var hashCode = key;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (key == _hashTable.EntryHashCode(index))
            {
                value = _values[index];
                return true;
            }

            index++;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public ref readonly TValue GetByRef(int key)
    {
        var hashCode = key;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

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
}
