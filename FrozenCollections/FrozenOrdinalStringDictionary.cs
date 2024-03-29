﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FrozenCollections.StringComparers;

namespace FrozenCollections;

/// <summary>
/// A frozen dictionary with string keys compared with ordinal semantics.
/// </summary>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
/// <remarks>
/// Frozen dictionaries are immutable and are optimized for situations where a dictionary
/// is created infrequently, but used repeatedly at runtime. They have a relatively high
/// cost to create, but provide excellent lookup performance. These are thus ideal for cases
/// where a dictionary is created at startup of an application and used throughout the life
/// of the application.
/// </remarks>
[DebuggerTypeProxy(typeof(IFrozenOrdinalStringDictionaryDebugView<>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenOrdinalStringDictionary<TValue> : IFrozenDictionary<string, TValue>
{
    private readonly FrozenHashTable _hashTable;
    private readonly string[] _keys;
    private readonly TValue[] _values;
    private readonly StringComparerBase _comparer;

    /// <summary>
    /// Gets an empty frozen string dictionary.
    /// </summary>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Usability is good in this case.")]
    public static FrozenOrdinalStringDictionary<TValue> Empty => default;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenOrdinalStringDictionary{TValue}"/> struct.
    /// </summary>
    /// <param name="pairs">The pairs to initialize the dictionary with.</param>
    /// <param name="ignoreCase">Whether to use case-insensitive semantics.</param>
    /// <exception cref="ArgumentException">If more than 64K pairs are added.</exception>
    /// <remarks>
    /// Tf the same key appears multiple times in the input, the latter one in the sequence takes precedence.
    /// </remarks>
    internal FrozenOrdinalStringDictionary(IEnumerable<KeyValuePair<string, TValue>> pairs, bool ignoreCase = false)
    {
        var d = new Dictionary<string, TValue>(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        foreach (var pair in pairs)
        {
            d[pair.Key] = pair.Value;
        }

        var incoming = d.ToList();

        _keys = incoming.Count == 0 ? Array.Empty<string>() : new string[incoming.Count];
        _values = incoming.Count == 0 ? Array.Empty<TValue>() : new TValue[incoming.Count];
        _comparer = ComparerPicker.Pick(incoming.Select(x => x.Key).ToList(), ignoreCase);

        var keys = _keys;
        var values = _values;
        var comparer = _comparer;
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
    public FrozenList<string> Keys => new(_keys);

    /// <inheritdoc />
    public FrozenList<TValue> Values => new(_values);

    /// <inheritdoc />
    public FrozenPairEnumerator<string, TValue> GetEnumerator() => new(_keys, _values);

    /// <summary>
    /// Gets an enumeration of the dictionary's keys.
    /// </summary>
    IEnumerable<string> IReadOnlyDictionary<string, TValue>.Keys => Count > 0 ? _keys : EmptyReadOnlyList<string>.Instance;

    /// <summary>
    /// Gets an enumeration of the dictionary's values.
    /// </summary>
    IEnumerable<TValue> IReadOnlyDictionary<string, TValue>.Values => Count > 0 ? _values : EmptyReadOnlyList<TValue>.Instance;

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator<KeyValuePair<string, TValue>> IEnumerable<KeyValuePair<string, TValue>>.GetEnumerator()
        => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<string, TValue>>.Instance.GetEnumerator();

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value/pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<string, TValue>>.Instance.GetEnumerator();

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
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Skip for speed")]
    public TValue this[string key]
    {
        get
        {
            if (_comparer != null && !_comparer.TrivialReject(key))
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
            }

            throw new KeyNotFoundException();
        }
    }

    /// <summary>
    /// Checks whether a particular key exists in the dictionary.
    /// </summary>
    /// <param name="key">The key to probe for.</param>
    /// <returns><see langword="true"/> if the key is in the dictionary, otherwise <see langword="false"/>.</returns>
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Skip for speed")]
    public bool ContainsKey(string key)
    {
        if (_comparer != null && !_comparer.TrivialReject(key))
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
        }

        return false;
    }

    /// <summary>
    /// Tries to get a value associated with a specific key.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <returns><see langword="true"/> if the key was found, otherwise <see langword="false"/>.</returns>
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Skip for speed")]
#if NETCOREAPP3_1_OR_GREATER
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
#else
    public bool TryGetValue(string key, out TValue value)
#endif
    {
        if (_comparer != null && !_comparer.TrivialReject(key))
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
        }

        value = default!;
        return false;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Skip for speed")]
    public ref readonly TValue GetByRef(string key)
    {
        if (_comparer != null && !_comparer.TrivialReject(key))
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
        }

        throw new KeyNotFoundException();
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Skip for speed")]
    public ref readonly TValue TryGetByRef(string key)
    {
        if (_comparer != null && !_comparer.TrivialReject(key))
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
        }

        return ref ByReference.Null<TValue>();
    }
}
