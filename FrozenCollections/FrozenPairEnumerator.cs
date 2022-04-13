using System;
using System.Collections;
using System.Collections.Generic;

namespace FrozenCollections;

/// <summary>
/// Enumerates the key/value pairs of a dictionary.
/// </summary>
/// <typeparam name="TKey">The types of the dictionary's keys.</typeparam>
/// <typeparam name="TValue">The types of the dictionary's values.</typeparam>
public struct FrozenPairEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
{
    private readonly TKey[] _keys;
    private readonly TValue[] _values;
    private int _index;

    internal FrozenPairEnumerator(TKey[] keys, TValue[] values)
    {
        _keys = keys;
        _values = values;
        _index = -1;
    }

    /// <summary>
    /// Gets the key/value pair at the current position of the enumerator.
    /// </summary>
    public KeyValuePair<TKey, TValue> Current
    {
        get
        {
            if (_index < 0)
            {
                throw new InvalidOperationException();
            }

            return new KeyValuePair<TKey, TValue>(_keys[_index], _values[_index]);
        }
    }

    /// <summary>
    /// Disposes the object.
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
        if (_index < _keys.Length - 1)
        {
            _index++;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the enumerator to its initial state.
    /// </summary>
    void IEnumerator.Reset() => _index = -1;

    /// <summary>
    /// Gets the current value held by the enumerator.
    /// </summary>
    object IEnumerator.Current => Current;
}
