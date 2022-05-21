using System;
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

        var fl = l.ToFrozenList();
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
#pragma warning disable IDE0004
        foreach (var v in (IEnumerable<int>)fl)
#pragma warning restore IDE0004
        {
            Assert.Equal(index++, v);
        }

        index = 0;
        foreach (var o in (IEnumerable)fl)
        {
            var v = (int)o!;
            Assert.Equal(index++, v);
        }

        Assert.IsType<FrozenEnumerator<int>>(((IEnumerable)fl).GetEnumerator());
        Assert.IsType<FrozenEnumerator<int>>(((IEnumerable<int>)fl).GetEnumerator());
    }

    [Fact]
    public static void Empty()
    {
        var l = new List<int>();
        var fl = l.ToFrozenList();

        Assert.Empty(fl);

        Assert.False(((IEnumerable)fl).GetEnumerator().MoveNext());
        Assert.False(((IEnumerable<int>)fl).GetEnumerator().MoveNext());

        Assert.IsNotType<FrozenEnumerator<int>>(((IEnumerable)fl).GetEnumerator());
        Assert.IsNotType<FrozenEnumerator<int>>(((IEnumerable<int>)fl).GetEnumerator());
    }

    [Fact]
    public static void Enumerator()
    {
        var l = new List<int>();
        var fl = l.ToFrozenList();
        var e = fl.GetEnumerator();

        Assert.Throws<InvalidOperationException>(() => e.Current);

        l.Add(1);
        fl = l.ToFrozenList();
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

    [Fact]
    public static void AsSpan()
    {
        var l = new List<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
        };

        var fl = l.ToFrozenList();
        var s = fl.AsSpan();

        Assert.Equal(l.Count, s.Length);
        for (int i = 0; i < l.Count; i++)
        {
            Assert.Equal(l[i], s[i]);
        }
    }
}
