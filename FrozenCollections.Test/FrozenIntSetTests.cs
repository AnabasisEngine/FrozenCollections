using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class FrozenIntSetTests
{
    private const int NumEntries = 127;

    [Fact]
    public static void Basic()
    {
        var s = new HashSet<int>();
        for (var i = 0; i < NumEntries; i++)
        {
            s.Add(i);
        }

        var fs = s.ToFrozenSet();
        Assert.Equal(s.Count, fs.Count);

        foreach (var v in s)
        {
            Assert.True(fs.Contains(v));
        }

        var t = new HashSet<int>();
        foreach (var v in fs)
        {
            Assert.Contains(v, s);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
#pragma warning disable IDE0004
        foreach (var v in (IEnumerable<int>)fs)
#pragma warning restore IDE0004
        {
            Assert.Contains(v, s);
            t.Add(v);
        }

        Assert.Equal(s.Count, t.Count);

        t.Clear();
        foreach (var o in (IEnumerable)fs)
        {
            var v = (int)o!;
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
        var s = new HashSet<int>();
        var fs = s.ToFrozenSet();

        Assert.Empty(fs);
        Assert.False(((IEnumerable<int>)fs).GetEnumerator().MoveNext());

        Assert.Empty(fs.Items);

        Assert.False(fs.Contains(-1));
    }

    [Fact]
    public static void Duplicates()
    {
        var items = new[] { 0, 1, 2, 3, 3 };
        var hs = new HashSet<int>(items);
        var fs = items.ToFrozenSet();

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
    public static void SetOps(int[] other)
    {
        var l = new List<int>
        {
            0,
            1,
            2,
            3,
            4,
            2049
        };

        var s = new HashSet<int>();
        foreach (var i in l)
        {
            s.Clear();
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

            var other2 = new HashSet<int>(other);
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

    private sealed class CustomSet : IReadOnlySet<int>
    {
        private readonly HashSet<int> _set;

        public CustomSet(IEnumerable<int> values)
        {
            _set = new HashSet<int>(values);
        }

        public int Count => _set.Count;
        public bool Contains(int item) => _set.Contains(item);
        public IEnumerator<int> GetEnumerator() => _set.GetEnumerator();
        public bool IsProperSubsetOf(IEnumerable<int> other) => _set.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<int> other) => _set.IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<int> other) => _set.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<int> other) => _set.IsSupersetOf(other);
        public bool Overlaps(IEnumerable<int> other) => _set.Overlaps(other);
        public bool SetEquals(IEnumerable<int> other) => _set.SetEquals(other);
        IEnumerator IEnumerable.GetEnumerator() => _set.GetEnumerator();
    }
}
