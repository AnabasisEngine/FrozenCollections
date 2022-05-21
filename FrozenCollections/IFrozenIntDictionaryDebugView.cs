using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

internal sealed class IFrozenIntDictionaryDebugView<TValue>
{
    private readonly IFrozenDictionary<int, TValue> _dict;

    public IFrozenIntDictionaryDebugView(IFrozenDictionary<int, TValue> dictionary)
    {
        _dict = dictionary;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [SuppressMessage("Critical Code Smell", "S2365:Properties should not make collection or array copies", Justification = "In debugger only")]
    public KeyValuePair<int, TValue>[] Items => _dict.ToArray();
}
