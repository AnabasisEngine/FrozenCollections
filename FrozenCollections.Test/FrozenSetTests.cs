using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class FrozenSetTests
{
    private const int NumEntries = 127;

    [Fact]
    public static void Basic()
    {
        RunBasic(null);
        RunBasic(EqualityComparer<long>.Default);
    }

    private static void RunBasic(IEqualityComparer<long>? comparer)
    {
        var s = new HashSet<long>(comparer);
        for (var i = 0; i < NumEntries; i++)
        {
            s.Add(i);
        }

        var fs = s.Freeze(comparer);
        Assert.Equal(s.Count, fs.Count);

        foreach (var v in s)
        {
            Assert.Contains(v, fs);
        }

        var t = new HashSet<long>();
        foreach (var v in fs)
        {
            Assert.Contains(v, s);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
        foreach (var v in (IEnumerable<long>)fs)
        {
            Assert.Contains(v, s);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
        foreach (var o in (IEnumerable)fs)
        {
            var v = (long)o;
            Assert.Contains(v, s);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
        foreach (var v in fs.Items)
        {
            Assert.Contains(v, s);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        Assert.False(fs.Contains(-1));
    }

    [Fact]
    public static void Empty()
    {
        var s = new HashSet<long>();
        var fs = s.Freeze();

        Assert.Empty(fs);
        Assert.False(((IEnumerable<long>)fs).GetEnumerator().MoveNext());

        Assert.Empty(fs.Items);

        Assert.False(fs.Contains(-1));
    }

    [Fact]
    public static void Duplicates()
    {
        var items = new long[] { 0, 1, 2, 3, 3 };
        var hs = new HashSet<long>(items);
        var fs = items.Freeze();

        Assert.Equal(hs.Count, fs.Count);
    }

    [Theory]
    [InlineData(new long[] { })]
    [InlineData(new long[] { -1 })]
    [InlineData(new long[] { 0 })]
    [InlineData(new long[] { 1 })]
    [InlineData(new long[] { 2 })]
    [InlineData(new long[] { 3 })]
    [InlineData(new long[] { 0, 1 })]
    [InlineData(new long[] { 0, 1, 2 })]
    [InlineData(new long[] { 0, 1, 2, 3 })]
    [InlineData(new long[] { 0, 1, 2, 3, 4 })]
    [InlineData(new long[] { 0, 1, 2, 3, 3, 2, 1, 0 })]
    public static void SetOps(long[] other)
    {
        for (int i = 0; i < 5; i++)
        {
            var s = new HashSet<long>();
            for (var j = 0; j < i; j++)
            {
                s.Add(j);
            }

            var fs = s.Freeze();

            Assert.Equal(s.IsSubsetOf(other), fs.IsSubsetOf(other));
            Assert.Equal(s.IsProperSubsetOf(other), fs.IsProperSubsetOf(other));
            Assert.Equal(s.IsSupersetOf(other), fs.IsSupersetOf(other));
            Assert.Equal(s.IsProperSupersetOf(other), fs.IsProperSupersetOf(other));
            Assert.Equal(s.Overlaps(other), fs.Overlaps(other));
            Assert.Equal(s.SetEquals(other), fs.SetEquals(other));

            var other2 = new HashSet<long>(other);
            Assert.Equal(s.IsSubsetOf(other2), fs.IsSubsetOf(other2));
            Assert.Equal(s.IsProperSubsetOf(other2), fs.IsProperSubsetOf(other2));
            Assert.Equal(s.IsSupersetOf(other2), fs.IsSupersetOf(other2));
            Assert.Equal(s.IsProperSupersetOf(other2), fs.IsProperSupersetOf(other2));
            Assert.Equal(s.Overlaps(other2), fs.Overlaps(other2));
            Assert.Equal(s.SetEquals(other2), fs.SetEquals(other2));

            var other3 = other.Freeze();
            Assert.Equal(s.IsSubsetOf(other3), fs.IsSubsetOf(other3));
            Assert.Equal(s.IsProperSubsetOf(other3), fs.IsProperSubsetOf(other3));
            Assert.Equal(s.IsSupersetOf(other3), fs.IsSupersetOf(other3));
            Assert.Equal(s.IsProperSupersetOf(other3), fs.IsProperSupersetOf(other3));
            Assert.Equal(s.Overlaps(other3), fs.Overlaps(other3));
            Assert.Equal(s.SetEquals(other3), fs.SetEquals(other3));
        }
    }
}
