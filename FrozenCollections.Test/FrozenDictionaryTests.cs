using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class FrozenDictionaryTests
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
        var d = new Dictionary<long, string>(comparer);
        for (var i = 0; i < NumEntries; i++)
        {
            d.Add(i, $"V{i}");
        }

        var fd = d.ToFrozenDictionary(comparer);
        Assert.Equal(d.Count, fd.Count);

        foreach (var kvp in d)
        {
            Assert.True(fd.ContainsKey(kvp.Key));
            Assert.True(fd.TryGetValue(kvp.Key, out var value));
            Assert.Equal(kvp.Value, value);
            Assert.Equal(kvp.Value, fd[kvp.Key]);
            Assert.Equal(kvp.Value, fd.GetByRef(kvp.Key));
        }

        var s = new HashSet<long>();
        foreach (var kvp in fd)
        {
            Assert.True(d.ContainsKey(kvp.Key));
            Assert.True(d.TryGetValue(kvp.Key, out var value));
            Assert.Equal(kvp.Value, value);
            Assert.Equal(kvp.Value, d[kvp.Key]);
            s.Add(kvp.Key);
        }

        Assert.Equal(d.Count, s.Count);

        s.Clear();
        foreach (var kvp in (IEnumerable<KeyValuePair<long, string>>)fd)
        {
            Assert.True(d.ContainsKey(kvp.Key));
            Assert.True(d.TryGetValue(kvp.Key, out var value));
            Assert.Equal(kvp.Value, value);
            Assert.Equal(kvp.Value, d[kvp.Key]);
            s.Add(kvp.Key);
        }

        Assert.Equal(d.Count, s.Count);

        s.Clear();
        foreach (var o in (IEnumerable)fd)
        {
            var kvp = (KeyValuePair<long, string>)o!;
            Assert.True(d.ContainsKey(kvp.Key));
            Assert.True(d.TryGetValue(kvp.Key, out var value));
            Assert.Equal(kvp.Value, value);
            Assert.Equal(kvp.Value, d[kvp.Key]);
            s.Add(kvp.Key);
        }

        Assert.Equal(d.Count, s.Count);

        s.Clear();
        foreach (var key in fd.Keys)
        {
            Assert.True(d.ContainsKey(key));
            Assert.True(d.TryGetValue(key, out var value));
            Assert.Equal(fd[key], value);
            Assert.Equal(fd[key], d[key]);
            s.Add(key);
        }

        Assert.Equal(d.Count, s.Count);

        s.Clear();
        foreach (var key in ((IReadOnlyDictionary<long, string>)fd).Keys)
        {
            Assert.True(d.ContainsKey(key));
            Assert.True(d.TryGetValue(key, out var value));
            Assert.Equal(fd[key], value);
            Assert.Equal(fd[key], d[key]);
            s.Add(key);
        }

        Assert.Equal(d.Count, s.Count);

        var s2 = new HashSet<string>();
        foreach (var value in fd.Values)
        {
            Assert.True(d.ContainsValue(value));
            s2.Add(value);
        }

        Assert.Equal(d.Count, s2.Count);

        s2.Clear();
        foreach (var value in ((IReadOnlyDictionary<long, string>)fd).Values)
        {
            Assert.True(d.ContainsValue(value));
            s2.Add(value);
        }

        Assert.Equal(d.Count, s2.Count);

        Assert.False(fd.ContainsKey(-1));
        Assert.False(fd.TryGetValue(-1, out _));
        Assert.Throws<KeyNotFoundException>(() => fd[-1]);
        Assert.Throws<KeyNotFoundException>(() => fd.GetByRef(-1));
    }

    [Fact]
    public static void Empty()
    {
        var d = new Dictionary<long, string>();
        var fd = d.ToFrozenDictionary();

        Assert.Empty(fd);
        Assert.False(((IEnumerable<KeyValuePair<long, string>>)fd).GetEnumerator().MoveNext());

        Assert.Empty(fd.Keys);
        Assert.Empty(((IReadOnlyDictionary<long, string>)fd).Keys);

        Assert.Empty(fd.Values);
        Assert.Empty(((IReadOnlyDictionary<long, string>)fd).Values);

        Assert.False(fd.ContainsKey(-1));
        Assert.False(fd.TryGetValue(-1, out _));
        Assert.Throws<KeyNotFoundException>(() => fd[-1]);
        Assert.Throws<KeyNotFoundException>(() => fd.GetByRef(-1));
    }

    [Fact]
    public static void PairEnumerator()
    {
        var d = new Dictionary<string, int>();
        var fd = d.ToFrozenDictionary();
        var e = fd.GetEnumerator();

        Assert.Throws<InvalidOperationException>(() => e.Current);

        d.Add("One", 1);
        fd = d.ToFrozenDictionary();
        e = fd.GetEnumerator();

        var e2 = (IEnumerator<KeyValuePair<string, int>>)e;
        Assert.True(e2.MoveNext());
        Assert.Equal("One", e2.Current.Key);
        Assert.False(e2.MoveNext());
        e2.Reset();
        Assert.True(e2.MoveNext());
        Assert.Equal("One", e2.Current.Key);
        Assert.False(e2.MoveNext());
    }
}
