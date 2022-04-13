using System;
using System.Collections.Generic;

namespace FrozenCollections;

/// <summary>
/// Factory methods for frozen collections.
/// </summary>
/// <remarks>
/// Frozen collections are immutable and are optimized for situations where a collection
/// is created infrequently, but used repeatedly at runtime. They have a relatively high
/// cost to create, but provide excellent lookup performance. These are thus ideal for cases
/// where a collection is created at startup of an application and used throughout the life
/// of the application.
/// </remarks>
public static class Freezer
{
    /// <summary>
    /// Initializes a new dictionary with the given set of key/value pairs.
    /// </summary>
    /// <param name="pairs">The pairs to initialize the dictionary with.</param>
    /// <param name="comparer">The comparer used to compare and hash keys. If this is null, then <see cref="EqualityComparer{T}.Default"/> is used.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <exception cref="ArgumentException">If more than 64K pairs are added.</exception>
    /// <remarks>
    /// Tf the same key appears multiple times in the input, the latter one in the sequence takes precedence.
    /// </remarks>
    /// <returns>A frozen dictionary.</returns>
    public static FrozenDictionary<TKey, TValue> ToFrozenDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>>? pairs, IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        return new FrozenDictionary<TKey, TValue>(pairs ?? Array.Empty<KeyValuePair<TKey, TValue>>(), comparer ?? EqualityComparer<TKey>.Default);
    }

    /// <summary>
    /// Initializes a new dictionary with the given set of key/value pairs.
    /// </summary>
    /// <param name="pairs">The pairs to initialize the dictionary with.</param>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <exception cref="ArgumentException">If more than 64K pairs are added.</exception>
    /// <remarks>
    /// Tf the same key appears multiple times in the input, the latter one in the sequence takes precedence.
    /// </remarks>
    /// <returns>A frozen dictionary.</returns>
    public static FrozenIntDictionary<TValue> ToFrozenDictionary<TValue>(this IEnumerable<KeyValuePair<int, TValue>>? pairs)
    {
        return new FrozenIntDictionary<TValue>(pairs ?? Array.Empty<KeyValuePair<int, TValue>>());
    }

    /// <summary>
    /// Initializes a new dictionary with the given set of key/value pairs.
    /// </summary>
    /// <param name="pairs">The pairs to initialize the dictionary with.</param>
    /// <param name="ignoreCase">Whether to use case-insensitive semantics.</param>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <exception cref="ArgumentException">If more than 64K pairs are added.</exception>
    /// <remarks>
    /// Tf the same key appears multiple times in the input, the latter one in the sequence takes precedence.
    /// </remarks>
    /// <returns>A frozen dictionary.</returns>
    public static FrozenOrdinalStringDictionary<TValue> ToFrozenDictionary<TValue>(this IEnumerable<KeyValuePair<string, TValue>>? pairs, bool ignoreCase = false)
    {
        return new FrozenOrdinalStringDictionary<TValue>(pairs ?? Array.Empty<KeyValuePair<string, TValue>>(), ignoreCase);
        ;
    }

    /// <summary>
    /// Initializes a new set with the given items.
    /// </summary>
    /// <param name="items">The items to initialize the set with.</param>
    /// <param name="comparer">The comparer used to compare and hash items. If this is null, then <see cref="EqualityComparer{T}.Default"/> is used.</param>
    /// <typeparam name="T">The type of the items in the set.</typeparam>
    /// <exception cref="ArgumentException">If more than 64K items are added.</exception>
    /// <returns>A frozen set.</returns>
    public static FrozenSet<T> ToFrozenSet<T>(this IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
        where T : notnull
    {
        return new FrozenSet<T>(items ?? Array.Empty<T>(), comparer ?? EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Initializes a new set with the given items.
    /// </summary>
    /// <param name="items">The items to initialize the set with.</param>
    /// <exception cref="ArgumentException">If more than 64K items are added.</exception>
    /// <returns>A frozen set.</returns>
    public static FrozenIntSet ToFrozenSet(this IEnumerable<int>? items)
    {
        return new FrozenIntSet(items ?? Array.Empty<int>());
    }

    /// <summary>
    /// Initializes a new set with the given items.
    /// </summary>
    /// <param name="items">The items to initialize the set with.</param>
    /// <param name="ignoreCase">Whether to use case-insensitive semantics.</param>
    /// <exception cref="ArgumentException">If more than 64K items are added.</exception>
    /// <returns>A frozen set.</returns>
    public static FrozenOrdinalStringSet ToFrozenSet(this IEnumerable<string>? items, bool ignoreCase = false)
    {
        return new FrozenOrdinalStringSet(items ?? Array.Empty<string>(), ignoreCase);
    }

    /// <summary>
    /// Initializes a new list with the given items.
    /// </summary>
    /// <param name="items">The items to initialize the list with.</param>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <returns>A frozen list.</returns>
    public static FrozenList<T> ToFrozenList<T>(this IEnumerable<T>? items)
    {
        return new FrozenList<T>(items ?? Array.Empty<T>());
    }
}
