using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrozenCollections.StringComparers;

/// <summary>
/// A comparer that operates over a portion of the input strings.
/// </summary>
/// <remarks>
/// This comparer looks at the end of input strings.
///
/// This code doesn't perform any error checks on the input as it assumes
/// the data is always valid. This is ensured by precondition checks before
/// a key is used to perform a dictionary lookup.
/// </remarks>
internal sealed class RightHandStringComparer : PartialStringComparerBase, IEqualityComparer<string>
{
    public override bool Equals(string? x, string? y) => x.AsSpan(x!.Length + Index, Count).Equals(y.AsSpan(y!.Length + Index, Count), StringComparison.Ordinal);
    [SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity", Justification = "This is a teeny bit faster")]
    [SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This is a teeny bit faster")]
    public override bool EqualsFullLength(string x, string y) => string.Equals(x, y);
    public override int GetHashCode(string s) => Hashing.GetHashCode(s.AsSpan(s.Length + Index, Count));
}
