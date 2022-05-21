using System;
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

        var fs = comparer == null ? s.ToFrozenSet() : s.ToFrozenSet(comparer);
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
#pragma warning disable IDE0004
        foreach (var v in (IEnumerable<long>)fs)
#pragma warning restore IDE0004
        {
            Assert.Contains(v, s);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
        foreach (var o in (IEnumerable)fs)
        {
            var v = (long)o!;
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
    public static void Big()
    {
        var s = new HashSet<int>();
        for (int i = 0; i < ushort.MaxValue; i++)
        {
            s.Add(i);
        }

        var fs = s.ToFrozenSet();
        Assert.Equal(s.Count, fs.Count);

        s.Add(-1);
        fs = s.ToFrozenSet();
        Assert.Equal(s.Count, fs.Count);
    }

    [Fact]
    public static void Empty()
    {
        var s = new HashSet<long>();
        var fs = s.ToFrozenSet();

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
        var fs = items.ToFrozenSet();

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

            var fs = s.ToFrozenSet();

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

            var other3 = other.ToFrozenSet();
            Assert.Equal(s.IsSubsetOf(other3), fs.IsSubsetOf(other3));
            Assert.Equal(s.IsProperSubsetOf(other3), fs.IsProperSubsetOf(other3));
            Assert.Equal(s.IsSupersetOf(other3), fs.IsSupersetOf(other3));
            Assert.Equal(s.IsProperSupersetOf(other3), fs.IsProperSupersetOf(other3));
            Assert.Equal(s.Overlaps(other3), fs.Overlaps(other3));
            Assert.Equal(s.SetEquals(other3), fs.SetEquals(other3));

            var other4 = new CustomSet(other);
            Assert.Equal(s.IsSubsetOf(other4), fs.IsSubsetOf(other4));
            Assert.Equal(s.IsProperSubsetOf(other4), fs.IsProperSubsetOf(other4));
            Assert.Equal(s.IsSupersetOf(other4), fs.IsSupersetOf(other4));
            Assert.Equal(s.IsProperSupersetOf(other4), fs.IsProperSupersetOf(other4));
            Assert.Equal(s.Overlaps(other4), fs.Overlaps(other4));
            Assert.Equal(s.SetEquals(other4), fs.SetEquals(other4));
        }
    }

    [Fact]
    public static void SpecialComparers()
    {
        var a = new[]
        {
            "abcd",
            "ABCD",
        };

        var fs = a.ToFrozenSet(StringComparer.Ordinal);
        Assert.Equal(typeof(StringComparers.LeftHandSingleCharStringComparer), fs.Comparer.GetType());
        Assert.Equal(2, fs.Count);

        fs = a.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
        Assert.Equal(typeof(StringComparers.LeftHandCaseInsensitiveAsciiStringComparer), fs.Comparer.GetType());
        Assert.Single(fs);
    }

    private sealed class CustomSet : IReadOnlySet<long>
    {
        private readonly HashSet<long> _set;

        public CustomSet(IEnumerable<long> values)
        {
            _set = new HashSet<long>(values);
        }

        public int Count => _set.Count;
        public bool Contains(long item) => _set.Contains(item);
        public IEnumerator<long> GetEnumerator() => _set.GetEnumerator();
        public bool IsProperSubsetOf(IEnumerable<long> other) => _set.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<long> other) => _set.IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<long> other) => _set.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<long> other) => _set.IsSupersetOf(other);
        public bool Overlaps(IEnumerable<long> other) => _set.Overlaps(other);
        public bool SetEquals(IEnumerable<long> other) => _set.SetEquals(other);
        IEnumerator IEnumerable.GetEnumerator() => _set.GetEnumerator();
    }
}
