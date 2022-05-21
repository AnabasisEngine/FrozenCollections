using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class EmptyFrozenTests
{
    [Fact]
    public static void All()
    {
        Assert.Empty(FrozenDictionary<string, int>.Empty);
        Assert.Empty(FrozenIntDictionary<int>.Empty);
        Assert.Empty(FrozenOrdinalStringDictionary<int>.Empty);
        Assert.Empty(FrozenSet<int>.Empty);
        Assert.Empty(FrozenIntSet.Empty);
        Assert.Empty(FrozenOrdinalStringSet.Empty);
        Assert.Empty(FrozenList<int>.Empty);

        Assert.Empty(default(FrozenDictionary<string, int>));
        Assert.Empty(default(FrozenIntDictionary<int>));
        Assert.Empty(default(FrozenOrdinalStringDictionary<int>));
        Assert.Empty(default(FrozenSet<int>));
        Assert.Empty(default(FrozenIntSet));
        Assert.Empty(default(FrozenOrdinalStringSet));

        Assert.False(default(FrozenDictionary<string, int>).ContainsKey("123"));
        Assert.False(default(FrozenDictionary<string, int>).TryGetValue("123", out var v1));
        Assert.Throws<KeyNotFoundException>(() => default(FrozenDictionary<string, int>)["123"]);
        Assert.Throws<KeyNotFoundException>(() => default(FrozenDictionary<string, int>).GetByRef("123"));
        Assert.True(ByReference.IsNull(default(FrozenDictionary<string, int>).TryGetByRef("123")));
        Assert.False(default(FrozenIntDictionary<int>).ContainsKey(1));
        Assert.False(default(FrozenOrdinalStringDictionary<int>).ContainsKey("123"));
        Assert.False(default(FrozenOrdinalStringDictionary<int>).TryGetValue("123", out v1));
        Assert.Throws<KeyNotFoundException>(() => default(FrozenOrdinalStringDictionary<int>)["123"]);
        Assert.Throws<KeyNotFoundException>(() => default(FrozenOrdinalStringDictionary<int>).GetByRef("123"));
        Assert.True(ByReference.IsNull(default(FrozenOrdinalStringDictionary<int>).TryGetByRef("123")));
        Assert.False(default(FrozenSet<int>).Contains(1));
        Assert.False(default(FrozenIntSet).Contains(1));
        Assert.False(default(FrozenOrdinalStringSet).Contains("123"));
        Assert.False(default(FrozenOrdinalStringSet).IsProperSupersetOf(new[] { "One", "Two" }));

#pragma warning disable IDE0004 // Remove Unnecessary Cast
        Assert.Empty(Freezer.ToFrozenDictionary<string, int>(null));
        Assert.Empty(Freezer.ToFrozenDictionary<int>((IEnumerable<KeyValuePair<int, int>>?)null));
        Assert.Empty(Freezer.ToFrozenDictionary<int>((IEnumerable<KeyValuePair<string, int>>?)null));
        Assert.Empty(Freezer.ToFrozenSet((IEnumerable<byte>?)null));
        Assert.Empty(Freezer.ToFrozenSet((IEnumerable<int>?)null));
        Assert.Empty(Freezer.ToFrozenSet((IEnumerable<string>?)null));

        Assert.False(Freezer.ToFrozenDictionary<string, int>(null).ContainsKey("123"));
        Assert.False(Freezer.ToFrozenDictionary<int>((IEnumerable<KeyValuePair<int, int>>?)null).ContainsKey(1));
        Assert.False(Freezer.ToFrozenDictionary<int>((IEnumerable<KeyValuePair<string, int>>?)null).ContainsKey("123"));
        Assert.False(Freezer.ToFrozenSet((IEnumerable<byte>?)null).Contains(0));
        Assert.False(Freezer.ToFrozenSet((IEnumerable<int>?)null).Contains(0));
        Assert.False(Freezer.ToFrozenSet((IEnumerable<string>?)null).Contains("123"));
#pragma warning restore IDE0004 // Remove Unnecessary Cast
    }
}
