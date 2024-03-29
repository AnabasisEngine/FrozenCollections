﻿using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FrozenCollections.StringComparers;

// Stryker is not at all effective for hash codes, so we disable it
// Stryker disable all

[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "All sorts of magic numbers here, that's OK")]
internal static class Hashing
{
    public static unsafe int GetHashCode(ReadOnlySpan<char> s)
    {
        uint hash1 = (5381 << 16) + 5381;
        uint hash2 = hash1;

        fixed (char* c = s)
        {
            uint* ptr = (uint*)c;
            int length = s.Length;

            while (length > 3)
            {
                hash1 = BitOperations.RotateLeft(hash1, 5) + hash1 ^ *ptr++;
                hash2 = BitOperations.RotateLeft(hash2, 5) + hash2 ^ *ptr++;
                length -= 4;
            }

            char* tail = (char*)ptr;
            while (length-- > 0)
            {
                hash2 = BitOperations.RotateLeft(hash2, 5) + hash2 ^ *tail++;
            }

            return (int)(hash1 + (hash2 * 1_566_083_941));
        }
    }

    // useful if the string only contains ASCII characterss
    public static unsafe int GetCaseInsensitiveAsciiHashCode(ReadOnlySpan<char> s)
    {
        uint hash1 = (5381 << 16) + 5381;
        uint hash2 = hash1;

        fixed (char* src = s)
        {
            uint* ptr = (uint*)src;
            int length = s.Length;

            // We "normalize to lowercase" every char by ORing with 0x0020. This casts
            // a very wide net because it will change, e.g., '^' to '~'. But that should
            // be ok because we expect this to be very rare in practice.
            const uint NormalizeToLowercase = 0x0020_0020u; // valid both for big-endian and for little-endian

            while (length > 3)
            {
                hash1 = BitOperations.RotateLeft(hash1, 5) + hash1 ^ (*ptr++ | NormalizeToLowercase);
                hash2 = BitOperations.RotateLeft(hash2, 5) + hash2 ^ (*ptr++ | NormalizeToLowercase);
                length -= 4;
            }

            char* tail = (char*)ptr;
            while (length-- > 0)
            {
                hash2 = BitOperations.RotateLeft(hash2, 5) + hash2 ^ (*tail++ | NormalizeToLowercase);
            }
        }

        return (int)(hash1 + (hash2 * 1_566_083_941));
    }

    [SkipLocalsInit]
    public static unsafe int GetCaseInsensitiveHashCode(ReadOnlySpan<char> s)
    {
        int length = s.Length;

        char[]? borrowedArr = null;
        if (length > 64)
        {
            borrowedArr = ArrayPool<char>.Shared.Rent(length);
        }

        Span<char> scratch = length <= 64 ? stackalloc char[64] : borrowedArr.AsSpan(0, length);
        int charsWritten = s.ToUpperInvariant(scratch);   // WARNING: this really should be ToUpperOrdinal, but .NET doesn't offer this as a primitive

        uint hash1 = (5381 << 16) + 5381;
        uint hash2 = hash1;

        fixed (char* src = scratch)
        {
            uint* ptr = (uint*)src;
            while (length > 3)
            {
                hash1 = (BitOperations.RotateLeft(hash1, 5) + hash1) ^ *ptr++;
                hash2 = (BitOperations.RotateLeft(hash2, 5) + hash2) ^ *ptr++;
                length -= 4;
            }

            char* tail = (char*)ptr;
            while (length-- > 0)
            {
                hash2 = BitOperations.RotateLeft(hash2, 5) + hash2 ^ *tail++;
            }
        }

        if (borrowedArr != null)
        {
            ArrayPool<char>.Shared.Return(borrowedArr);
        }

        return (int)(hash1 + (hash2 * 1_566_083_941));
    }
}
