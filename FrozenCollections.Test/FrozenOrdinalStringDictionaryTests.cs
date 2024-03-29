﻿using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class FrozenOrdinalStringDictionaryTests
{
    private const int NumEntries = 127;

    [Fact]
    public static void Basic()
    {
        var d = new Dictionary<string, string>();
        for (var i = 0; i < NumEntries; i++)
        {
            d.Add($"K{i}", $"V{i}");
        }

        var fd = d.ToFrozenDictionary();
        Assert.Equal(d.Count, fd.Count);

        foreach (var kvp in d)
        {
            Assert.True(fd.ContainsKey(kvp.Key));
            Assert.True(fd.TryGetValue(kvp.Key, out var value));
            Assert.Equal(kvp.Value, value);
            Assert.Equal(kvp.Value, fd[kvp.Key]);
            Assert.Equal(kvp.Value, fd.GetByRef(kvp.Key));
        }

        var s = new HashSet<string>();
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
        foreach (var kvp in (IEnumerable<KeyValuePair<string, string>>)fd)
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
            var kvp = (KeyValuePair<string, string>)o!;
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
        foreach (var key in ((IReadOnlyDictionary<string, string>)fd).Keys)
        {
            Assert.True(d.ContainsKey(key));
            Assert.True(d.TryGetValue(key, out var value));
            Assert.Equal(fd[key], value);
            Assert.Equal(fd[key], d[key]);
            s.Add(key);
        }

        Assert.Equal(d.Count, s.Count);

        s.Clear();
        foreach (var value in fd.Values)
        {
            Assert.True(d.ContainsValue(value));
            s.Add(value);
        }

        Assert.Equal(d.Count, s.Count);

        s.Clear();
        foreach (var value in ((IReadOnlyDictionary<string, string>)fd).Values)
        {
            Assert.True(d.ContainsValue(value));
            s.Add(value);
        }

        Assert.Equal(d.Count, s.Count);

        Assert.False(fd.ContainsKey("Foo"));
        Assert.False(fd.TryGetValue("Foo", out _));
        Assert.Throws<KeyNotFoundException>(() => fd["Foo"]);
        Assert.Throws<KeyNotFoundException>(() => fd.GetByRef("Foo"));
    }

    [Fact]
    public static void LastWins()
    {
        var a = new[]
        {
            new KeyValuePair<string, string>("0", "Zero"),
            new KeyValuePair<string, string>("1", "One"),
            new KeyValuePair<string, string>("0", "Zero Repeat"),
            new KeyValuePair<string, string>("1", "One Repeat"),
        };

        var fd = a.ToFrozenDictionary(false);
        Assert.Equal("Zero Repeat", fd["0"]);
        Assert.Equal("One Repeat", fd["1"]);
    }

    [Fact]
    public static void Empty()
    {
        var d = new Dictionary<string, string>();
        var fd = d.ToFrozenDictionary();

        Assert.Empty(fd);
        Assert.False(((IEnumerable<KeyValuePair<string, string>>)fd).GetEnumerator().MoveNext());

        Assert.Empty(fd.Keys);
        Assert.Empty(((IReadOnlyDictionary<string, string>)fd).Keys);

        Assert.Empty(fd.Values);
        Assert.Empty(((IReadOnlyDictionary<string, string>)fd).Values);

        Assert.False(fd.ContainsKey("Foo"));
        Assert.False(fd.TryGetValue("Foo", out _));
        Assert.Throws<KeyNotFoundException>(() => fd["Foo"]);
        Assert.Throws<KeyNotFoundException>(() => fd.GetByRef("Foo"));
    }
}
