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
/// <typeparam name="T">The type of the items in the set.</typeparam>
/// <remarks>
/// Frozen sets are immutable and are optimized for situations where a set
/// is created infrequently, but used repeatedly at runtime. They have a relatively high
/// cost to create, but provide excellent lookup performance. These are thus ideal for cases
/// where a set is created at startup of an application and used throughout the life
/// of the application.
///
/// This is the general-purpose frozen set which can be used with any item type. If you need
/// a set that has a string or integer as key, you will get better performance by using
/// <see cref="FrozenOrdinalStringSet"/> or <see cref="FrozenIntSet"/>
/// respectively.
/// </remarks>
[DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenSet<T> : IFrozenSet<T>, IFindItem<T>
    where T : notnull
{
    private readonly FrozenHashTable _hashTable;
    private readonly T[] _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenSet{T}"/> struct.
    /// </summary>
    /// <param name="items">The items to initialize the set with.</param>
    /// <param name="comparer">The comparer used to compare and hash items.</param>
    /// <exception cref="ArgumentException">If more than 64K items are added.</exception>
    internal FrozenSet(IEnumerable<T> items, IEqualityComparer<T> comparer)
    {
        var incoming = new HashSet<T>(items, comparer).ToList();

        _items = new T[incoming.Count];
        Comparer = comparer;

        var it = _items;
        _hashTable = FrozenHashTable.Create(
            incoming,
            item => comparer.GetHashCode(item),
            (index, item) => it[index] = item);
    }

    /// <inheritdoc />
    public FrozenList<T> Items => new(_items);

    /// <inheritdoc />
    public Enumerator<T> GetEnumerator() => new(_items);

    /// <summary>
    /// Gets an enumeration of the set's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<T>.Instance.Enumerator;

    /// <summary>
    /// Gets an enumeration of the set's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<T>.Instance.Enumerator;

    /// <summary>
    /// Gets the number of items in the set.
    /// </summary>
    public int Count => _items.Length;

    /// <inheritdoc />
    public bool Contains(T item)
    {
        var hashCode = Comparer.GetHashCode(item);
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (Comparer.Equals(item, _items[index]))
                {
                    return true;
                }
            }

            index++;
        }

        return false;
    }

    /// <summary>
    /// Looks up an item's index.
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>The index of the item, or -1 if the item was not found.</returns>
    int IFindItem<T>.FindItemIndex(T item)
    {
        var hashCode = Comparer.GetHashCode(item);
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (hashCode == _hashTable.EntryHashCode(index))
            {
                if (Comparer.Equals(item, _items[index]))
                {
                    return index;
                }
            }

            index++;
        }

        return -1;
    }

    /// <summary>
    /// Gets the comparer used by this set.
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    /// <summary>
    /// Determines whether this set is a proper subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a proper subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsProperSubsetOf(IEnumerable<T> other) => SetSupport.IsProperSubsetOf(this, other);

    /// <summary>
    /// Determines whether this set is a proper superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a proper superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsProperSupersetOf(IEnumerable<T> other) => SetSupport.IsProperSupersetOf(this, other);

    /// <summary>
    /// Determines whether this set is a subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsSubsetOf(IEnumerable<T> other) => SetSupport.IsSubsetOf(this, other);

    /// <summary>
    /// Determines whether this set is a superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsSupersetOf(IEnumerable<T> other) => SetSupport.IsSupersetOf(this, other);

    /// <summary>
    /// Determines whether this set shares any elements with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set and the collection share at least one element; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool Overlaps(IEnumerable<T> other) => SetSupport.Overlaps(this, other);

    /// <summary>
    /// Determines whether this set and collection contain the same elements.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set and the collection contains the exact same elements; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool SetEquals(IEnumerable<T> other) => SetSupport.SetEquals(this, other);
}
