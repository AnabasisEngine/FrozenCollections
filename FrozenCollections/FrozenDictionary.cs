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

    /// <summary>
    /// Gets an empty frozen dictionary.
    /// </summary>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Usability is good in this case.")]
    public static FrozenDictionary<TKey, TValue> Empty => default;

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
#if NETCOREAPP3_1_OR_GREATER
        var incoming = new Dictionary<TKey, TValue>(pairs, comparer).ToList();
#else
        var d = new Dictionary<TKey, TValue>(comparer);
        foreach (var pair in pairs)
        {
            d[pair.Key] = pair.Value;
        }

        var incoming = d.ToList();
#endif

        _keys = incoming.Count == 0 ? Array.Empty<TKey>() : new TKey[incoming.Count];
        _values = incoming.Count == 0 ? Array.Empty<TValue>() : new TValue[incoming.Count];
        Comparer = comparer;

        var keys = _keys;
        var values = _values;
        _hashTable = FrozenHashTable.Create(
            incoming,
            pair => comparer.GetHashCode(pair.Key),
            (index, pair) =>
            {
                keys[index] = pair.Key;
                values[index] = pair.Value;
            });
    }

    /// <inheritdoc />
    public FrozenList<TKey> Keys => new(_keys);

    /// <inheritdoc />
    public FrozenList<TValue> Values => new(_values);

    /// <inheritdoc />
    public FrozenPairEnumerator<TKey, TValue> GetEnumerator() => new(_keys, _values);

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
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<TKey, TValue>>.Instance.GetEnumerator();

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<TKey, TValue>>.Instance.GetEnumerator();

    /// <summary>
    /// Gets the number of key/value pairs in the dictionary.
    /// </summary>
    public int Count => _hashTable.Count;

    /// <summary>
    /// Gets the comparer used by this dictionary.
    /// </summary>
    public IEqualityComparer<TKey> Comparer { get; }

    /// <summary>
    /// Gets the value associated to the given key.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    /// <returns>The associated value.</returns>
    /// <exception cref="KeyNotFoundException">If the key doesn't exist in the dictionary.</exception>
    public TValue this[TKey key]
    {
        get
        {
            var hashCode = Comparer.GetHashCode(key);
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (hashCode == _hashTable.EntryHashCode(index))
                {
                    if (Comparer.Equals(key, _keys[index]))
                    {
                        return _values[index];
                    }
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
    public bool ContainsKey(TKey key)
    {
        var hashCode = Comparer?.GetHashCode(key) ?? 0;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (Comparer!.Equals(key, _keys[index]))
                {
                    return true;
                }
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
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#else
    public bool TryGetValue(TKey key, out TValue value)
#endif
    {
        var hashCode = Comparer?.GetHashCode(key) ?? 0;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (Comparer!.Equals(key, _keys[index]))
                {
                    value = _values[index];
                    return true;
                }
            }

            index++;
        }

        value = default!;
        return false;
    }

    /// <inheritdoc />
    public ref readonly TValue GetByRef(TKey key)
    {
        var hashCode = Comparer?.GetHashCode(key) ?? 0;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (Comparer!.Equals(key, _keys[index]))
                {
                    return ref _values[index];
                }
            }

            index++;
        }

        throw new KeyNotFoundException();
    }
}
