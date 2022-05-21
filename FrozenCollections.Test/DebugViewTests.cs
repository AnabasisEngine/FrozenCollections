using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class DebugViewTests
{
    [Fact]
    public static void DictionaryDebugView()
    {
        var d = new Dictionary<string, int>
        {
            { "One", 1 },
            { "Two", 2 },
        };
        var fd = d.ToFrozenDictionary();
        var dv = new IFrozenDictionaryDebugView<string, int>(fd);
        var items = dv.Items;

        Assert.Equal(d.Count, items.Length);
        Assert.NotEqual(items[0].Key, items[1].Key);

        Assert.True(d.ContainsKey(items[0].Key));
        Assert.Equal(d[items[0].Key], items[0].Value);

        Assert.True(d.ContainsKey(items[1].Key));
        Assert.Equal(d[items[1].Key], items[1].Value);
    }

    [Fact]
    public static void StringDictionaryDebugView()
    {
        var d = new Dictionary<string, int>
        {
            { "One", 1 },
            { "Two", 2 },
        };
        var fd = d.ToFrozenDictionary();
        var dv = new IFrozenOrdinalStringDictionaryDebugView<int>(fd);
        var items = dv.Items;

        Assert.Equal(d.Count, items.Length);
        Assert.NotEqual(items[0].Key, items[1].Key);

        Assert.True(d.ContainsKey(items[0].Key));
        Assert.Equal(d[items[0].Key], items[0].Value);

        Assert.True(d.ContainsKey(items[1].Key));
        Assert.Equal(d[items[1].Key], items[1].Value);
    }

    [Fact]
    public static void IntDictionaryDebugView()
    {
        var d = new Dictionary<int, string>
        {
            { 1, "One" },
            { 2, "Two" },
        };
        var fd = d.ToFrozenDictionary();
        var dv = new IFrozenIntDictionaryDebugView<string>(fd);
        var items = dv.Items;

        Assert.Equal(d.Count, items.Length);
        Assert.NotEqual(items[0].Key, items[1].Key);

        Assert.True(d.ContainsKey(items[0].Key));
        Assert.Equal(d[items[0].Key], items[0].Value);

        Assert.True(d.ContainsKey(items[1].Key));
        Assert.Equal(d[items[1].Key], items[1].Value);
    }

    [Fact]
    public static void CollectionDebugView()
    {
        var l = new List<int>
        {
            1,
            2,
        };

        var fl = l.ToFrozenList();
        var dv = new IReadOnlyCollectionDebugView<int>(fl);
        var items = dv.Items;

        Assert.Equal(l.Count, items.Length);
        Assert.Equal(1, l[0]);
        Assert.Equal(2, l[1]);
    }
}
