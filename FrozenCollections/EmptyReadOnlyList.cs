using System;
using System.Collections;
using System.Collections.Generic;

namespace FrozenCollections;

// Use an EmptyReadOnlyList to avoid enumerator allocations on empty collections.
internal sealed class EmptyReadOnlyList<T> : IReadOnlyList<T>
{
    public static readonly EmptyReadOnlyList<T> Instance = new();
    public readonly NopEnumerator Enumerator = new();

    internal sealed class NopEnumerator : IEnumerator<T>
    {
        public void Dispose()
        {
            // nop
        }

        public void Reset()
        {
            // nop
        }

        public bool MoveNext() => false;
        public T Current => throw new NotSupportedException();
        object IEnumerator.Current => throw new NotSupportedException();
    }

    public IEnumerator<T> GetEnumerator() => Enumerator;
    IEnumerator IEnumerable.GetEnumerator() => Enumerator;
    public int Count => 0;
    public T this[int index] => throw new ArgumentOutOfRangeException(nameof(index));
}
