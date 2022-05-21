using System;
using System.Diagnostics.CodeAnalysis;

namespace FrozenCollections.StringComparers;

/// <summary>
/// A comparer that operates over a single character of the input strings.
/// </summary>
/// <remarks>
/// This comparer looks at the end of input strings.
///
/// This code doesn't perform any error checks on the input as it assumes
/// the data is always valid. This is ensured by precondition checks before
/// a key is used to perform a dictionary lookup.
/// </remarks>
internal sealed class RightHandSingleCharStringComparer : PartialStringComparerBase
{
    [SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity", Justification = "This is a teeny bit faster")]
    [SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This is a teeny bit faster")]
    public override bool Equals(string? x, string? y) => string.Equals(x, y);
    public override bool EqualsPartial(string? x, string? y) => x![x.Length + Index] == y![y.Length + Index];
    public override int GetHashCode(string s) => s[s.Length + Index];
}
