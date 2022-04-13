using FrozenCollections.StringComparers;
using Xunit;

namespace FrozenCollections.Test;

public static class ComparerTests
{
    private static void Equal(StringComparerBase c, string a, string b, bool fullEqual)
    {
        Assert.True(c.Equals(a, b));
        Assert.Equal(c.GetHashCode(a), c.GetHashCode(b));
        Assert.Equal(fullEqual, c.EqualsFullLength(a, b));
    }

    private static void NotEqual(StringComparerBase c, string a, string b)
    {
        Assert.False(c.Equals(a, b));
        Assert.False(c.EqualsFullLength(a, b));
        Assert.NotEqual(c.GetHashCode(a), c.GetHashCode(b));
    }

    [Fact]
    public static void LeftHand()
    {
        var c = new LeftHandStringComparer
        {
            Index = 0,
            Count = 1
        };

        Assert.False(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "ab", false);
        NotEqual(c, "a", "A");
        NotEqual(c, "a", "b");

        c.Index = 1;
        c.Count = 1;
        Equal(c, "Aa", "Ba", false);
        Equal(c, "Aa", "Baa", false);
        Equal(c, "aa", "Bab", false);
        Equal(c, "Aa", "Aa", true);
        Equal(c, "Aab", "Aab", true);
        NotEqual(c, "Aa", "BA");
        NotEqual(c, "Aa", "Bb");

        c.Index = 1;
        c.Count = 2;
        Equal(c, "Aaa", "Baa", false);
        Equal(c, "Aaa", "Baaa", false);
        Equal(c, "aaa", "Baab", false);
        Equal(c, "Aaa", "Aaa", true);
        Equal(c, "Aaab", "Aaab", true);
        NotEqual(c, "Aaa", "BaA");
        NotEqual(c, "Aaa", "Bab");
    }

    [Fact]
    public static void LeftHandSingleChar()
    {
        var c = new LeftHandSingleCharStringComparer
        {
            Index = 0,
            Count = 1
        };

        Assert.False(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "ab", false);
        NotEqual(c, "a", "A");
        NotEqual(c, "a", "b");

        c.Index = 1;
        c.Count = 1;
        Equal(c, "Aa", "Ba", false);
        Equal(c, "Aa", "Baa", false);
        Equal(c, "aa", "Bab", false);
        Equal(c, "Aa", "Aa", true);
        Equal(c, "Aab", "Aab", true);
        NotEqual(c, "Aa", "BA");
        NotEqual(c, "Aa", "Bb");
    }

    [Fact]
    public static void LeftHandCaseInsensitive()
    {
        var c = new LeftHandCaseInsensitiveStringComparer
        {
            Index = 0,
            Count = 1
        };

        Assert.True(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "A", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "AA", false);
        Equal(c, "a", "ab", false);
        Equal(c, "a", "AB", false);
        NotEqual(c, "a", "b");

        c.Index = 1;
        c.Count = 1;
        Equal(c, "Xa", "Ya", false);
        Equal(c, "Xa", "YA", false);
        Equal(c, "Xa", "Xa", true);
        Equal(c, "Xa", "XA", true);
        Equal(c, "Xa", "Yaa", false);
        Equal(c, "Xa", "YAA", false);
        Equal(c, "Xa", "Yab", false);
        Equal(c, "Xa", "YAB", false);
        NotEqual(c, "Xa", "Yb");
    }

    [Fact]
    public static void LeftHandCaseInsensitiveAscii()
    {
        var c = new LeftHandCaseInsensitiveAsciiStringComparer
        {
            Index = 0,
            Count = 1
        };

        Assert.True(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "A", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "AA", false);
        Equal(c, "a", "ab", false);
        Equal(c, "a", "AB", false);
        NotEqual(c, "a", "b");

        c.Index = 1;
        c.Count = 1;
        Equal(c, "Xa", "Ya", false);
        Equal(c, "Xa", "YA", false);
        Equal(c, "Xa", "Xa", true);
        Equal(c, "Xa", "XA", true);
        Equal(c, "Xa", "Yaa", false);
        Equal(c, "Xa", "YAA", false);
        Equal(c, "Xa", "Yab", false);
        Equal(c, "Xa", "YAB", false);
        NotEqual(c, "Xa", "Yb");
    }

    [Fact]
    public static void RightHand()
    {
        var c = new RightHandStringComparer
        {
            Index = -1,
            Count = 1
        };

        Assert.False(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "ba", false);
        NotEqual(c, "a", "A");
        NotEqual(c, "a", "b");

        c.Index = -2;
        c.Count = 1;
        Equal(c, "aX", "aY", false);
        Equal(c, "XaX", "YaY", false);
        Equal(c, "XaX", "YYaY", false);
        Equal(c, "XXaX", "YaY", false);
        NotEqual(c, "XXaX", "YYa");

        c.Index = -2;
        c.Count = 2;
        Equal(c, "aa", "aa", true);
        Equal(c, "aa", "aaa", false);
        Equal(c, "aa", "baa", false);
        NotEqual(c, "aa", "AA");
        NotEqual(c, "aa", "bb");
    }

    [Fact]
    public static void RightHandSingleChar()
    {
        var c = new RightHandSingleCharStringComparer
        {
            Index = -1,
            Count = 1
        };

        Assert.False(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "ba", false);
        NotEqual(c, "a", "A");
        NotEqual(c, "a", "b");

        c.Index = -2;
        c.Count = 1;
        Equal(c, "aX", "aY", false);
        Equal(c, "XaX", "YaY", false);
        Equal(c, "XaX", "YYaY", false);
        Equal(c, "XXaX", "YaY", false);
        NotEqual(c, "XXaX", "YYa");
    }

    [Fact]
    public static void RightHandCaseInsensitive()
    {
        var c = new RightHandCaseInsensitiveStringComparer
        {
            Index = -1,
            Count = 1
        };

        Assert.True(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "ba", false);
        Equal(c, "a", "A", true);
        Equal(c, "a", "AA", false);
        Equal(c, "a", "BA", false);
        NotEqual(c, "a", "b");

        c.Index = -2;
        c.Count = 1;
        Equal(c, "aX", "aY", false);
        Equal(c, "XaX", "YaY", false);
        Equal(c, "XaX", "YYaY", false);
        Equal(c, "XXaX", "YaY", false);
        Equal(c, "aX", "AY", false);
        Equal(c, "XaX", "YAY", false);
        Equal(c, "XaX", "YYAY", false);
        Equal(c, "XXaX", "YAY", false);
        NotEqual(c, "XXaX", "YYa");

        c.Index = -2;
        c.Count = 2;
        Equal(c, "aa", "aa", true);
        Equal(c, "aa", "aaa", false);
        Equal(c, "aa", "baa", false);
        Equal(c, "aa", "AA", true);
        Equal(c, "aa", "AAA", false);
        Equal(c, "aa", "bAA", false);
        NotEqual(c, "aa", "bb");
    }

    [Fact]
    public static void RightHandCaseInsensitiveAscii()
    {
        var c = new RightHandCaseInsensitiveAsciiStringComparer
        {
            Index = -1,
            Count = 1
        };

        Assert.True(c.CaseInsensitive);

        Equal(c, "a", "a", true);
        Equal(c, "a", "aa", false);
        Equal(c, "a", "ba", false);
        Equal(c, "a", "A", true);
        Equal(c, "a", "AA", false);
        Equal(c, "a", "BA", false);
        NotEqual(c, "a", "b");

        c.Index = -2;
        c.Count = 1;
        Equal(c, "aX", "aY", false);
        Equal(c, "XaX", "YaY", false);
        Equal(c, "XaX", "YYaY", false);
        Equal(c, "XXaX", "YaY", false);
        Equal(c, "aX", "AY", false);
        Equal(c, "XaX", "YAY", false);
        Equal(c, "XaX", "YYAY", false);
        Equal(c, "XXaX", "YAY", false);
        NotEqual(c, "XXaX", "YYa");

        c.Index = -2;
        c.Count = 2;
        Equal(c, "aa", "aa", true);
        Equal(c, "aa", "aaa", false);
        Equal(c, "aa", "baa", false);
        Equal(c, "aa", "AA", true);
        Equal(c, "aa", "AAA", false);
        Equal(c, "aa", "bAA", false);
        NotEqual(c, "aa", "bb");
    }

    [Fact]
    public static void Full()
    {
        var c = new FullStringComparer();

        Assert.False(c.CaseInsensitive);

        Equal(c, "", "", true);
        Equal(c, "A", "A", true);
        Equal(c, "AA", "AA", true);

        NotEqual(c, "A", "AA");
        NotEqual(c, "AA", "A");

        c.MinLength = 1;
        c.MaxLength = 3;
        Assert.True(c.TrivialReject(string.Empty));
        Assert.False(c.TrivialReject("1"));
        Assert.False(c.TrivialReject("2"));
        Assert.False(c.TrivialReject("3"));
        Assert.True(c.TrivialReject("1234"));
    }

    [Fact]
    public static void FullCaseInsensitive()
    {
        var c = new FullCaseInsensitiveStringComparer();

        Assert.True(c.CaseInsensitive);

        Equal(c, "", "", true);
        Equal(c, "A", "A", true);
        Equal(c, "A", "a", true);
        Equal(c, "a", "A", true);
        Equal(c, "AA", "aa", true);
        Equal(c, "aa", "AA", true);

        NotEqual(c, "A", "AA");
        NotEqual(c, "AA", "A");
    }

    [Fact]
    public static void FullCaseInsensitiveAscii()
    {
        var c = new FullCaseInsensitiveAsciiStringComparer();

        Assert.True(c.CaseInsensitive);

        Equal(c, "", "", true);
        Equal(c, "A", "A", true);
        Equal(c, "A", "a", true);
        Equal(c, "a", "A", true);
        Equal(c, "AA", "aa", true);
        Equal(c, "aa", "AA", true);

        NotEqual(c, "A", "AA");
        NotEqual(c, "AA", "A");
    }
}
