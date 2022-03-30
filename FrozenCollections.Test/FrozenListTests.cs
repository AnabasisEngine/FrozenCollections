using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class FrozenListTests
{
    private const int NumEntries = 127;

    [Fact]
    public static void Basic()
    {
        var l = new List<int>();
        for (var i = 0; i < NumEntries; i++)
        {
            l.Add(i);
        }

        var fl = l.FreezeAsList();
        Assert.Equal(l.Count, fl.Count);

        for (var i = 0; i < l.Count; i++)
        {
            Assert.Equal(i, fl[i]);
        }

        int index = 0;
        foreach (var v in fl)
        {
            Assert.Equal(index++, v);
        }

        index = 0;
        foreach (var v in (IEnumerable<int>)fl)
        {
            Assert.Equal(index++, v);
        }

        index = 0;
        foreach (var o in (IEnumerable)fl)
        {
            var v = (int)o;
            Assert.Equal(index++, v);
        }
    }

    [Fact]
    public static void Empty()
    {
        var l = new List<int>();
        var fl = l.FreezeAsList();

        Assert.Empty(fl);
        Assert.False(((IEnumerable<int>)fl).GetEnumerator().MoveNext());
    }

    [Fact]
    public static void Enumerator()
    {
        var l = new List<int>();
        var fl = l.FreezeAsList();
        var e = fl.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() => e.Current);
        l.Add(1);
        fl = l.FreezeAsList();
        e = fl.GetEnumerator();
        var e2 = (IEnumerator<int>)e;
        Assert.True(e2.MoveNext());
        Assert.Equal(1, e2.Current);
        Assert.False(e2.MoveNext());
        e2.Reset();
        Assert.True(e2.MoveNext());
        Assert.Equal(1, e2.Current);
        Assert.False(e2.MoveNext());
    }
}
