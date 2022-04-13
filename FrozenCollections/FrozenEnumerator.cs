using System;
using System.Collections;
using System.Collections.Generic;

namespace FrozenCollections;

/// <summary>
/// Enumerates the entries of a set.
/// </summary>
/// <typeparam name="T">The types of the set's entries.</typeparam>
public struct FrozenEnumerator<T> : IEnumerator<T>
{
    private readonly T[] _entries;
    private int _index;

    internal FrozenEnumerator(T[] entries)
    {
        _entries = entries;
        _index = -1;
    }

    /// <summary>
    /// Gets the entry at the current position of the enumerator.
    /// </summary>
    public T Current
    {
        get
        {
            if (_index < 0)
            {
                throw new InvalidOperationException();
            }

            return _entries[_index];
        }
    }

    /// <summary>
    /// Dispose this object.
    /// </summary>
    void IDisposable.Dispose()
    {
        // nothing to do
    }

    /// <summary>
    /// Advances the enumerator to the next key/value pair of the dictionary.
    /// </summary>
    /// <returns><see langword="true" /> if the enumerator was successfully advanced to the next pair; <see langword="false" /> if the enumerator has passed the end of the dictionary.</returns>
    public bool MoveNext()
    {
        if (_index < _entries.Length - 1)
        {
            _index++;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the enumerator to being enumeration anew.
    /// </summary>
    void IEnumerator.Reset() => _index = -1;

    /// <summary>
    /// Gets the current value held by the enumerator.
    /// </summary>
    object IEnumerator.Current => Current!;
}
