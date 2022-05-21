using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FrozenCollections;

internal sealed class IFrozenOrdinalStringDictionaryDebugView<TValue>
{
    private readonly IFrozenDictionary<string, TValue> _dict;

    public IFrozenOrdinalStringDictionaryDebugView(IFrozenDictionary<string, TValue> dictionary)
    {
        _dict = dictionary;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [SuppressMessage("Critical Code Smell", "S2365:Properties should not make collection or array copies", Justification = "In debugger only")]
    public KeyValuePair<string, TValue>[] Items => _dict.ToArray();
}
