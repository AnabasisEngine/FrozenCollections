using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

/// <summary>
/// A simple frozen list of items.
/// </summary>
/// <typeparam name="T">The item's type.</typeparam>
/// <remarks>
/// This type is a slight improvement over the classic <see cref="List{T}"/>. It uses less memory
/// and enumerates items a bit faster.
/// </remarks>
[DebuggerTypeProxy(typeof(IReadOnlyCollectionDebugView<>))]
[DebuggerDisplay("Count = {Count}")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not appropriate for this type")]
public readonly struct FrozenList<T> : IReadOnlyList<T>
{
    private readonly T[] _items;

    /// <summary>
    /// Gets an empty frozen list.
    /// </summary>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Usability is good in this case.")]
    public static FrozenList<T> Empty { get; } = Array.Empty<T>().ToFrozenList();

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenList{T}"/> struct.
    /// </summary>
    /// <param name="items">The items to track in the list.</param>
    /// <remarks>
    /// Note that this takes a reference to the incoming array and does not copy it. This means that mutating this
    /// array over time will also affect the items that this frozen collection returns.
    /// </remarks>
    internal FrozenList(T[] items)
    {
        _items = items;
    }

    internal FrozenList(IEnumerable<T> items)
    {
        _items = items.ToArray();
    }

    /// <summary>
    /// Gets the element at the specified index in the list.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    public T this[int index] => _items[index];

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public int Count => _items.Length;

    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the list.
    /// </returns>
    public FrozenEnumerator<T> GetEnumerator() => new(_items);

    /// <summary>
    /// Gets an enumeration of this list's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<T>.Instance.GetEnumerator();

    /// <summary>
    /// Gets an enumeration of this list's items.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Count > 0 ? GetEnumerator() : EmptyReadOnlyList<T>.Instance.GetEnumerator();
}
