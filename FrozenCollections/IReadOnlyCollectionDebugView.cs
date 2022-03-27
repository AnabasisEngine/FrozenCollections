using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

internal sealed class IReadOnlyCollectionDebugView<T>
{
    private readonly IReadOnlyCollection<T> _collection;

    public IReadOnlyCollectionDebugView(IReadOnlyCollection<T> collection)
    {
        _collection = collection;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [SuppressMessage("Critical Code Smell", "S2365:Properties should not make collection or array copies", Justification = "In debugger only")]
    public T[] Items => _collection.ToArray();
}
