using System;
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
[DebuggerTypeProxy(typeof(IFrozenDictionaryDebugView<,>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenOrdinalStringDictionary<TValue> : IFrozenDictionary<string, TValue>
{
    private readonly FrozenHashTable _hashTable;
    private readonly string[] _keys;
    private readonly TValue[] _values;
    private readonly StringComparerBase _comparer;

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
        var incoming = new Dictionary<string, TValue>(pairs).ToList();

        _keys = new string[incoming.Count];
        _values = new TValue[incoming.Count];
        _comparer = ComparerPicker.Pick(incoming.Select(x => x.Key).ToList(), ignoreCase);

        var keys = _keys;
        var values = _values;
        var comparer = _comparer;
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
    public FrozenList<string> Keys => new(_keys);

    /// <inheritdoc />
    public FrozenList<TValue> Values => new(_values);

    /// <inheritdoc />
    public PairEnumerator<string, TValue> GetEnumerator() => new(_keys, _values);

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
        => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<string, TValue>>.Instance.Enumerator;

    /// <summary>
    /// Gets an enumeration of the dictionary's key/value/pairs.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<KeyValuePair<string, TValue>>.Instance.Enumerator;

    /// <summary>
    /// Gets the number of key/value pairs in the dictionary.
    /// </summary>
    public int Count => _keys.Length;

    /// <inheritdoc />
    public TValue this[string key]
    {
        get
        {
            if (!_comparer.TrivialReject(key))
            {
                var hashCode = _comparer.GetHashCode(key);
                _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

                while (index <= endIndex)
                {
                    if (hashCode == _hashTable.EntryHashCode(index))
                    {
                        if (_comparer.EqualsFullLength(key, _keys[index]))
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

    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
        if (!_comparer.TrivialReject(key))
        {
            var hashCode = _comparer.GetHashCode(key);
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (hashCode == _hashTable.EntryHashCode(index))
                {
                    if (_comparer.EqualsFullLength(key, _keys[index]))
                    {
                        return true;
                    }
                }

                index++;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
    {
        if (!_comparer.TrivialReject(key))
        {
            var hashCode = _comparer.GetHashCode(key);
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (hashCode == _hashTable.EntryHashCode(index))
                {
                    if (_comparer.EqualsFullLength(key, _keys[index]))
                    {
                        value = _values[index];
                        return true;
                    }
                }

                index++;
            }
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public ref readonly TValue GetByRef(string key)
    {
        if (!_comparer.TrivialReject(key))
        {
            var hashCode = _comparer.GetHashCode(key);
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (hashCode == _hashTable.EntryHashCode(index))
                {
                    if (_comparer.EqualsFullLength(key, _keys[index]))
                    {
                        return ref _values[index];
                    }
                }

                index++;
            }
        }

        throw new KeyNotFoundException();
    }
}
