using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrozenCollections.StringComparers;

// We define this rather than using IEqualityComparer<string>, since virtual dispatch is faster than interface dispatch
internal abstract class StringComparerBase : IEqualityComparer<string>
{
    public int MinLength;
    public int MaxLength;

    public abstract bool Equals(string? x, string? y);
    public abstract int GetHashCode(string s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrivialReject(string s) => s.Length < MinLength || s.Length > MaxLength;

    public virtual bool CaseInsensitive => false;
}
