using System;
using FrozenCollections.StringComparers;
using Xunit;

namespace FrozenCollections.Test;

public static class HashingTests
{
    [Fact]
    public static void CaseInsensitive()
    {
        Assert.Equal(
            Hashing.GetCaseInsensitiveHashCode("abcdefghijklmnopqrstuvwxyz01234567890123456789abcdefghijklmnopqrstuvwxyz01234567890123456789".AsSpan()),
            Hashing.GetCaseInsensitiveHashCode("ABCdefghijklmnopqrstuvwxyz01234567890123456789abcdefghijklmnopqrstuvwxyz01234567890123456789".AsSpan()));
    }
}
