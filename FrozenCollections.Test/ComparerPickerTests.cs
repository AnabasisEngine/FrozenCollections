using System;
using FrozenCollections.StringComparers;
using Xunit;

namespace FrozenCollections.Test;

public static class ComparerPickerTests
{
    [Fact]
    public static void Empty()
    {
        Assert.IsType<FullStringComparer>(ComparerPicker.Pick(Array.Empty<string>(), false));
        Assert.IsType<FullCaseInsensitiveAsciiStringComparer>(ComparerPicker.Pick(Array.Empty<string>(), true));
    }

    [Fact]
    public static void LeftHand()
    {
        var c = ComparerPicker.Pick(new[] { "S1" }, false);
        Assert.IsType<LeftHandSingleCharStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "S1", "T1" }, false);
        Assert.IsType<LeftHandSingleCharStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "SA1", "TA1", "SB1" }, false);
        Assert.IsType<LeftHandStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(2, ((PartialStringComparerBase)c).Count);
    }

    [Fact]
    public static void LeftHandCaseInsensitive()
    {
        var c = ComparerPicker.Pick(new[] { "É1" }, true);
        Assert.IsType<LeftHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "É1", "T1" }, true);
        Assert.IsType<LeftHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "ÉA1", "TA1", "ÉB1" }, true);
        Assert.IsType<LeftHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(2, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "ABCDEÉ1ABCDEF", "ABCDETA1ABCDEF", "ABCDESB1ABCDEF" }, true);
        Assert.IsType<LeftHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(5, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "ABCDEFÉ1ABCDEF", "ABCDEFTA1ABCDEF", "ABCDEFSB1ABCDEF" }, true);
        Assert.IsType<LeftHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(6, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "ABCÉDEFÉ1ABCDEF", "ABCÉDEFTA1ABCDEF", "ABCÉDEFSB1ABCDEF" }, true);
        Assert.IsType<LeftHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(7, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);
    }

    [Fact]
    public static void LeftHandCaseInsensitiveAscii()
    {
        var c = ComparerPicker.Pick(new[] { "S1" }, true);
        Assert.IsType<LeftHandCaseInsensitiveAsciiStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "S1", "T1" }, true);
        Assert.IsType<LeftHandCaseInsensitiveAsciiStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "SA1", "TA1", "SB1" }, true);
        Assert.IsType<LeftHandCaseInsensitiveAsciiStringComparer>(c);
        Assert.Equal(0, ((PartialStringComparerBase)c).Index);
        Assert.Equal(2, ((PartialStringComparerBase)c).Count);
    }

    [Fact]
    public static void RightHand()
    {
        var c = ComparerPicker.Pick(new[] { "1S", "1T" }, false);
        Assert.IsType<RightHandSingleCharStringComparer>(c);
        Assert.Equal(-1, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "1AS", "1AT", "1BS" }, false);
        Assert.IsType<RightHandStringComparer>(c);
        Assert.Equal(-2, ((PartialStringComparerBase)c).Index);
        Assert.Equal(2, ((PartialStringComparerBase)c).Count);
    }

    [Fact]
    public static void RightHandCaseInsensitive()
    {
        var c = ComparerPicker.Pick(new[] { "1É", "1T" }, true);
        Assert.IsType<RightHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(-1, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "1AÉ", "1AT", "1BÉ" }, true);
        Assert.IsType<RightHandCaseInsensitiveStringComparer>(c);
        Assert.Equal(-2, ((PartialStringComparerBase)c).Index);
        Assert.Equal(2, ((PartialStringComparerBase)c).Count);
    }

    [Fact]
    public static void RightHandCaseInsensitiveAscii()
    {
        var c = ComparerPicker.Pick(new[] { "1S", "1T" }, true);
        Assert.IsType<RightHandCaseInsensitiveAsciiStringComparer>(c);
        Assert.Equal(-1, ((PartialStringComparerBase)c).Index);
        Assert.Equal(1, ((PartialStringComparerBase)c).Count);

        c = ComparerPicker.Pick(new[] { "1AS", "1AT", "1BS" }, true);
        Assert.IsType<RightHandCaseInsensitiveAsciiStringComparer>(c);
        Assert.Equal(-2, ((PartialStringComparerBase)c).Index);
        Assert.Equal(2, ((PartialStringComparerBase)c).Count);
    }

    [Fact]
    public static void Full()
    {
        var c = ComparerPicker.Pick(new[] { "ABC", "DBC", "ADC", "ABD" }, false);
        Assert.IsType<FullStringComparer>(c);
    }

    [Fact]
    public static void FullCaseInsensitive()
    {
        var c = ComparerPicker.Pick(new[] { "æbc", "DBC", "æDC", "æbd" }, true);
        Assert.IsType<FullCaseInsensitiveStringComparer>(c);
    }

    [Fact]
    public static void FullCaseInsensitiveAscii()
    {
        var c = ComparerPicker.Pick(new[] { "abc", "DBC", "aDC", "abd" }, true);
        Assert.IsType<FullCaseInsensitiveAsciiStringComparer>(c);
    }

    [Fact]
    public static void IsAllAscii()
    {
        Assert.True(ComparerPicker.IsAllAscii("abc"));
        Assert.True(ComparerPicker.IsAllAscii("abcdefghij"));
        Assert.False(ComparerPicker.IsAllAscii("abcdéfghij"));
    }
}
