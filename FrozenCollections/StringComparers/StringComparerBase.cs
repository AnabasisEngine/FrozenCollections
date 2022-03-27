using System.Runtime.CompilerServices;

namespace FrozenCollections.StringComparers;

// We define this rather than using IEqualityComparer<string>, since virtual dispatch is faster than interface dispatch
internal abstract class StringComparerBase
{
    public int MinLength;
    public int MaxLength;

    public abstract bool Equals(string x, string y);

    /// <summary>
    /// Perform a full comparison of the string.
    /// </summary>
    /// <remarks>
    /// Since we have comparers that operate on partial strings, we still need a way to perform a full string comparison to confirm a hash table match.
    /// For comparers that don't implement partial string matching, this call returns the same answer as the <see cref="Equals(string, string)"/> method.
    /// </remarks>
    public abstract bool EqualsFullLength(string x, string y);
    public abstract int GetHashCode(string s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrivialReject(string s) => s.Length < MinLength || s.Length > MaxLength;
}
