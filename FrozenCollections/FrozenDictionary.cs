using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

/// <summary>
/// A frozen dictionary.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
/// <remarks>
/// Frozen dictionaries are immutable and are optimized for situations where a dictionary
/// is created infrequently, but used repeatedly at runtime. They have a relatively high
/// cost to create, but provide excellent lookup performance. These are thus ideal for cases
/// where a dictionary is created at startup of an application and used throughout the life
/// of the application.
///
/// This is the general-purpose frozen dictionary which can be used with any key type. If you need
/// a dictionary that has a string or integer as key, you will get better performance by using
/// <see cref="FrozenOrdinalStringDictionary{TValue}"/> or <see cref="FrozenIntDictionary{TValue}"/>
/// respectively.
/// </remarks>
[DebuggerTypeProxy(typeof(IFrozenDictionaryDebugView<,>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenDictionary<TKey, TValue> : IFrozenDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly FrozenHashTable _hashTable;
    private readonly TKey[] _keys;
    private readonly TValue[] _values;
    private readonly IEqualityComparer<TKey> _comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="pairs">The pairs to initialize the dictionary with.</param>
    /// <param name="comparer">The comparer used to compare and hash keys.</param>
    /// <exception cref="ArgumentException">If more than 64K pairs are added.</exception>
    /// <remarks>
    /// Tf the same key appears multiple times in the input, the latter one in the sequence takes precedence.
    /// </remarks>
    internal FrozenDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs, IEqualityComparer<TKey> comparer)
    {
        var incoming = new Dictionary<TKey, TValue>(pairs, comparer).ToList();

        _keys = new TKey[incoming.Count];
        _values = new TValue[incoming.Count];
        _comparer = comparer;

        var keys = _keys;
        var values = _values;
        _hashTable = FrozenHashTable.Create(
            incoming,
            item => comparer.GetHashCode(item.Key),
            (index, item) =>
            {
                keys[index] = item.Key;
                values[index] = item.Value;
            });
    }

    /// <inheritdoc />
    public FrozenList<TKey> Keys => new(_keys);

    /// <inheritdoc />
    public FrozenList<TValue> Values => new(_values);

    /// <inheritdoc />
    public PairEnumerator<TKey, TValue> GetEnumerator() => new(_keys, _values);

    /// <summary>
    /// Gets an enumeration of the dictionary's keys.
    /// </summary>
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Count > 0 ? _keys : EmptyReadOnlyList<TKey>.Instance;

    /// <summary>
    /// Gets an enumeration of the dictionary's values.
    /// </summary>
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Count > 0 ? _values : EmptyReadOnlyList<TValue>.Instance;

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<TKey, TValue>>.Instance.Enumerator;

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<TKey, TValue>>.Instance.Enumerator;

    /// <summary>
    /// Gets the number of key/value pairs in the dictionary.
    /// </summary>
    public int Count => _keys.Length;

    /// <inheritdoc />
    public TValue this[TKey key]
    {
        get
        {
            var hashCode = _comparer.GetHashCode(key);
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (hashCode == _hashTable.EntryHashCode(index))
                {
                    if (_comparer.Equals(key, _keys[index]))
                    {
                        return _values[index];
                    }
                }

                index++;
            }

            throw new KeyNotFoundException();
        }
    }

    /// <inheritdoc />
    public bool ContainsKey(TKey key)
    {
        var hashCode = _comparer.GetHashCode(key);
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (_comparer.Equals(key, _keys[index]))
                {
                    return true;
                }
            }

            index++;
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var hashCode = _comparer.GetHashCode(key);
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (_comparer.Equals(key, _keys[index]))
                {
                    value = _values[index];
                    return true;
                }
            }

            index++;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public ref readonly TValue GetByRef(TKey key)
    {
        var hashCode = _comparer.GetHashCode(key);
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (_comparer.Equals(key, _keys[index]))
                {
                    return ref _values[index];
                }
            }

            index++;
        }

        throw new KeyNotFoundException();
    }
}
