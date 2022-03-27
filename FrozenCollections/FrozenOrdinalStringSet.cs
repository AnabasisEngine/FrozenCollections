using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FrozenCollections.StringComparers;

namespace FrozenCollections;

/// <summary>
/// A frozen set with strings compared with ordinal semantics.
/// </summary>
/// <remarks>
/// Frozen sets are immutable and are optimized for situations where a set
/// is created infrequently, but used repeatedly at runtime. They have a relatively high
/// cost to create, but provide excellent lookup performance. These are thus ideal for cases
/// where a set is created at startup of an application and used throughout the life
/// of the application.
/// </remarks>
[DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenOrdinalStringSet : IFrozenSet<string>, IFindItem<string>
{
    private readonly FrozenHashTable _hashTable;
    private readonly string[] _items;
    private readonly StringComparerBase _comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenOrdinalStringSet"/> struct.
    /// </summary>
    /// <param name="items">The items to initialize the set with.</param>
    /// <param name="ignoreCase">Whether to use case-insensitive semantics.</param>
    /// <exception cref="ArgumentException">If more than 64K items are added.</exception>
    internal FrozenOrdinalStringSet(IEnumerable<string> items, bool ignoreCase = false)
    {
        var incoming = new HashSet<string>(items).ToList();

        _items = new string[incoming.Count];
        _comparer = ComparerPicker.Pick(incoming, ignoreCase);

        var it = _items;
        var comparer = _comparer;
        _hashTable = FrozenHashTable.Create(
            incoming,
            item => comparer.GetHashCode(item),
            (index, item) => it[index] = item);
    }

    /// <inheritdoc />
    public FrozenList<string> Items => new(_items);

    /// <inheritdoc />
    public Enumerator<string> GetEnumerator() => new(_items);

    /// <summary>
    /// Gets an enumeration of the set's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<string>.Instance.Enumerator;

    /// <summary>
    /// Gets an enumeration of the set's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<string>.Instance.Enumerator;

    /// <summary>
    /// Gets the number of items in the set.
    /// </summary>
    public int Count => _items.Length;

    /// <inheritdoc />
    public bool Contains(string item)
    {
        if (!_comparer.TrivialReject(item))
        {
            var hashCode = _comparer.GetHashCode(item);
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (hashCode == _hashTable.EntryHashCode(index))
                {
                    if (_comparer.EqualsFullLength(item, _items[index]))
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
    /// Looks up an item's index.
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>The index of the item, or -1 if the item was not found.</returns>
    int IFindItem<string>.FindItemIndex(string item)
    {
        if (!_comparer.TrivialReject(item))
        {
            var hashCode = _comparer.GetHashCode(item);
            _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

            while (index <= endIndex)
            {
                if (hashCode == _hashTable.EntryHashCode(index))
                {
                    if (_comparer.EqualsFullLength(item, _items[index]))
                    {
                        return index;
                    }
                }

                index++;
            }
        }

        return -1;
    }

    /// <summary>
    /// Determines whether this set is a proper subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a proper subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsProperSubsetOf(IEnumerable<string> other) => SetSupport.IsProperSubsetOf(this, other);

    /// <summary>
    /// Determines whether this set is a proper superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a proper superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsProperSupersetOf(IEnumerable<string> other) => SetSupport.IsProperSupersetOf(this, other);

    /// <summary>
    /// Determines whether this set is a subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsSubsetOf(IEnumerable<string> other) => SetSupport.IsSubsetOf(this, other);

    /// <summary>
    /// Determines whether this set is a superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsSupersetOf(IEnumerable<string> other) => SetSupport.IsSupersetOf(this, other);

    /// <summary>
    /// Determines whether this set shares any elements with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set and the collection share at least one element; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool Overlaps(IEnumerable<string> other) => SetSupport.Overlaps(this, other);

    /// <summary>
    /// Determines whether this set and collection contain the same elements.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set and the collection contains the exact same elements; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool SetEquals(IEnumerable<string> other) => SetSupport.SetEquals(this, other);
}
