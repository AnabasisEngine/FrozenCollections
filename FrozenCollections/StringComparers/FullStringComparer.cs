using System;
using System.Diagnostics.CodeAnalysis;

namespace FrozenCollections.StringComparers;

/// <summary>
/// A comparer for ordinal string comparisons.
/// </summary>
/// <remarks>
/// This code doesn't perform any error checks on the input as it assumes
/// the data is always valid. This is ensured by precondition checks before
/// a key is used to perform a dictionary lookup.
/// </remarks>
[SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity", Justification = "This is a teeny bit faster")]
[SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This is a teeny bit faster")]
internal sealed class FullStringComparer : StringComparerBase
{
    public override bool Equals(string x, string y) => string.Equals(x, y);
    public override bool EqualsFullLength(string x, string y) => string.Equals(x, y);
    public override int GetHashCode(string s) => Hashing.GetHashCode(s.AsSpan());
}
