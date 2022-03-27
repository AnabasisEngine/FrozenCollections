using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace FrozenCollections.Test;

public static class FrozenOrdinalCaseInsensitiveStringSetTests
{
    private const int NumEntries = 127;

    [Fact]
    public static void Basic()
    {
        var s = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < NumEntries; i++)
        {
            s.Add($"K{i}");
        }

        var fs = s.Freeze(true);
        Assert.Equal(s.Count, fs.Count);

        foreach (var v in s)
        {
            Assert.Contains(v, fs);
            Assert.True(fs.Contains(v.ToLowerInvariant()));
        }

        var t = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var v in fs)
        {
            Assert.Contains(v, fs);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
        foreach (var v in (IEnumerable<string>)fs)
        {
            Assert.Contains(v, fs);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
        foreach (var o in (IEnumerable)fs)
        {
            var v = (string)o;
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

        Assert.False(fs.Contains("Foo"));
    }

    [Fact]
    public static void Empty()
    {
        var s = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var fs = s.Freeze(true);

        Assert.Empty(fs);
        Assert.False(((IEnumerable<string>)fs).GetEnumerator().MoveNext());

        Assert.Empty(fs.Items);

        Assert.False(fs.Contains("Foo"));
    }

    [Fact]
    public static void Duplicates()
    {
        var items = new[] { "0", "1", "2", "3", "3" };
        var hs = new HashSet<string>(items, StringComparer.OrdinalIgnoreCase);
        var fs = items.Freeze(true);

        Assert.Equal(hs.Count, fs.Count);
    }

    [Theory]
    [InlineData(new int[] { })]
    [InlineData(new[] { -1 })]
    [InlineData(new[] { 0 })]
    [InlineData(new[] { 1 })]
    [InlineData(new[] { 2 })]
    [InlineData(new[] { 3 })]
    [InlineData(new[] { 0, 1 })]
    [InlineData(new[] { 0, 1, 2 })]
    [InlineData(new[] { 0, 1, 2, 3 })]
    [InlineData(new[] { 0, 1, 2, 3, 4 })]
    [InlineData(new[] { 0, 1, 2, 3, 3, 2, 1, 0 })]
    public static void SetOps(int[] otherInts)
    {
        var other = new string[otherInts.Length];
        for (int i = 0; i < other.Length; i++)
        {
            other[i] = otherInts[i].ToString(CultureInfo.InvariantCulture);
        }

        for (int i = 0; i < 5; i++)
        {
            var s = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (var j = 0; j < i; j++)
            {
                s.Add(j.ToString(CultureInfo.InvariantCulture));
            }

            var fs = s.Freeze(true);

            Assert.Equal(s.IsSubsetOf(other), fs.IsSubsetOf(other));
            Assert.Equal(s.IsProperSubsetOf(other), fs.IsProperSubsetOf(other));
            Assert.Equal(s.IsSupersetOf(other), fs.IsSupersetOf(other));
            Assert.Equal(s.IsProperSupersetOf(other), fs.IsProperSupersetOf(other));
            Assert.Equal(s.Overlaps(other), fs.Overlaps(other));
            Assert.Equal(s.SetEquals(other), fs.SetEquals(other));

            var other2 = new HashSet<string>(other);
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
