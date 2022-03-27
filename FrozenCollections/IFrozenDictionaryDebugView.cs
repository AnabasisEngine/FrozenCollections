using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

internal sealed class IFrozenDictionaryDebugView<TKey, TValue>
    where TKey : notnull
{
    private readonly IFrozenDictionary<TKey, TValue> _dict;

    public IFrozenDictionaryDebugView(IFrozenDictionary<TKey, TValue> dictionary)
    {
        _dict = dictionary;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [SuppressMessage("Critical Code Smell", "S2365:Properties should not make collection or array copies", Justification = "In debugger only")]
    public KeyValuePair<TKey, TValue>[] Items => _dict.ToArray();
}
