using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace FrozenCollections.Test;

public static class FrozenIntDictionaryTests
{
    private const int NumEntries = 127;

    [Fact]
    public static void Basic()
    {
        var d = new Dictionary<int, string>();
        for (var i = 0; i < NumEntries; i++)
        {
            d.Add(i, $"V{i}");
        }

        var fd = d.Freeze();
        Assert.Equal(d.Count, fd.Count);

        foreach (var kvp in d)
        {
            Assert.True(fd.ContainsKey(kvp.Key));
            Assert.True(fd.TryGetValue(kvp.Key, out var value));
            Assert.Equal(kvp.Value, value);
            Assert.Equal(kvp.Value, fd[kvp.Key]);
            Assert.Equal(kvp.Value, fd.GetByRef(kvp.Key));
        }

        var s = new HashSet<int>();
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
        foreach (var kvp in (IEnumerable<KeyValuePair<int, string>>)fd)
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
            var kvp = (KeyValuePair<int, string>)o;
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
        foreach (var key in ((IReadOnlyDictionary<int, string>)fd).Keys)
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
        foreach (var value in ((IReadOnlyDictionary<int, string>)fd).Values)
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
        var d = new Dictionary<int, string>();
        var fd = d.Freeze();

        Assert.Empty(fd);
        Assert.False(((IEnumerable<KeyValuePair<int, string>>)fd).GetEnumerator().MoveNext());

        Assert.Empty(fd.Keys);
        Assert.Empty(((IReadOnlyDictionary<int, string>)fd).Keys);

        Assert.Empty(fd.Values);
        Assert.Empty(((IReadOnlyDictionary<int, string>)fd).Values);

        Assert.False(fd.ContainsKey(-1));
        Assert.False(fd.TryGetValue(-1, out _));
        Assert.Throws<KeyNotFoundException>(() => fd[-1]);
        Assert.Throws<KeyNotFoundException>(() => fd.GetByRef(-1));
    }
}
