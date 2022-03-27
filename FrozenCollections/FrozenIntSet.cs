using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

/// <summary>
/// A frozen set of integers.
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
public readonly struct FrozenIntSet : IFrozenSet<int>, IFindItem<int>
{
    private readonly FrozenHashTable _hashTable;

    internal FrozenIntSet(IEnumerable<int> items)
    {
        var incoming = new HashSet<int>(items).ToList();

        _hashTable = FrozenHashTable.Create(
            incoming,
            item => item,
            (_, _) => { });
    }

    /// <inheritdoc />
    public FrozenList<int> Items => new(_hashTable.HashCodes);

    /// <inheritdoc />
    public Enumerator<int> GetEnumerator() => new(_hashTable.HashCodes);

    /// <summary>
    /// Gets an enumeration of the set's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<int>.Instance.Enumerator;

    /// <summary>
    /// Gets an enumeration of the set's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<int>.Instance.Enumerator;

    /// <summary>
    /// Gets the number of items in the set.
    /// </summary>
    public int Count => _hashTable.HashCodes.Length;

    /// <inheritdoc />
    public bool Contains(int item)
    {
        var hashCode = item;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (item == _hashTable.EntryHashCode(index))
            {
                return true;
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
    int IFindItem<int>.FindItemIndex(int item)
    {
        var hashCode = item;
        _hashTable.FindMatchingEntries(hashCode, out var index, out var endIndex);

        while (index <= endIndex)
        {
            if (item == _hashTable.EntryHashCode(index))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    /// <summary>
    /// Determines whether this set is a proper subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a proper subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsProperSubsetOf(IEnumerable<int> other) => SetSupport.IsProperSubsetOf(this, other);

    /// <summary>
    /// Determines whether this set is a proper superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a proper superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsProperSupersetOf(IEnumerable<int> other) => SetSupport.IsProperSupersetOf(this, other);

    /// <summary>
    /// Determines whether this set is a subset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a subset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsSubsetOf(IEnumerable<int> other) => SetSupport.IsSubsetOf(this, other);

    /// <summary>
    /// Determines whether this set is a superset of the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set is a superset of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool IsSupersetOf(IEnumerable<int> other) => SetSupport.IsSupersetOf(this, other);

    /// <summary>
    /// Determines whether this set shares any elements with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set and the collection share at least one element; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool Overlaps(IEnumerable<int> other) => SetSupport.Overlaps(this, other);

    /// <summary>
    /// Determines whether this set and collection contain the same elements.
    /// </summary>
    /// <param name="other">The collection to compare.</param>
    /// <returns><see langword="true" /> if the set and the collection contains the exact same elements; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
    public bool SetEquals(IEnumerable<int> other) => SetSupport.SetEquals(this, other);
}
