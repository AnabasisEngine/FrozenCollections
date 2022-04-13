using System;

namespace FrozenCollections.StringComparers;

/// <summary>
/// A comparer for ordinal case-insensitive ascii-only string comparisons.
/// </summary>
/// <remarks>
/// This code doesn't perform any error checks on the input as it assumes
/// the data is always valid. This is ensured by precondition checks before
/// a key is used to perform a dictionary lookup.
/// </remarks>
internal sealed class FullCaseInsensitiveAsciiStringComparer : StringComparerBase
{
    public override bool Equals(string x, string y) => StringComparer.OrdinalIgnoreCase.Equals(x, y);
    public override bool EqualsFullLength(string x, string y) => StringComparer.OrdinalIgnoreCase.Equals(x, y);
    public override int GetHashCode(string s) => Hashing.GetCaseInsensitiveAsciiHashCode(s.AsSpan());
    public override bool CaseInsensitive => true;
}
