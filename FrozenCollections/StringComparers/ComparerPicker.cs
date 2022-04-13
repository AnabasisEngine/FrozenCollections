using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrozenCollections.StringComparers;

internal static class ComparerPicker
{
    /// <summary>
    /// Pick an optimal comparer for the set of strings and case-sensitivity mode.
    /// </summary>
    /// <remarks>
    /// The idea here is to find the shortest substring slice across all the input strings which yields a set of
    /// strings which are maximally unique. The optimal slice is then applied to incoming strings being hashed to
    /// perform the dictionary lookup. Keeping the slices as small as possible minimize the number of characters
    /// involved in hashing, speeding up the whole process.
    ///
    /// What we do here is pretty simple. We loop over the input strings, looking for the shortest slice with a good
    /// enough uniqueness factor. We look at all the strings both left-justified and right-justified as this maximizes
    /// the opportunities to find unique slices, especially in the case of many strings with the same prefix or suffix.
    ///
    /// In whatever slice we end up with, if all the characters involved in the slice aree ASCII and we're doing case-insensitive
    /// operations, then we can select an ASCII-specific case-insensitive comparer which yields faster overall performance.
    /// </remarks>
    public static StringComparerBase Pick(IReadOnlyCollection<string> uniqueStrings, bool ignoreCase)
    {
        var c = PickInternal(uniqueStrings, ignoreCase);

        // now calculate the trivial rejection boundaries
        if (uniqueStrings.Count > 0)
        {
            c.MinLength = int.MaxValue;
            c.MaxLength = 0;
            foreach (var s in uniqueStrings)
            {
                if (s.Length < c.MinLength)
                {
                    c.MinLength = s.Length;
                }

                if (s.Length > c.MaxLength)
                {
                    c.MaxLength = s.Length;
                }
            }
        }

        return c;
    }

    public static StringComparerBase PickInternal(IReadOnlyCollection<string> uniqueStrings, bool ignoreCase)
    {
        const double SufficientUniquenessFactor = 0.95; // 95% is good enough

        if (uniqueStrings.Count == 0)
        {
            return ignoreCase ? new FullCaseInsensitiveAsciiStringComparer() : new FullStringComparer();
        }

        // What is the shortest string? This represent the maximum substring length we consider
        int maxSubstringLength = int.MaxValue;
        foreach (var s in uniqueStrings)
        {
            if (s.Length < maxSubstringLength)
            {
                maxSubstringLength = s.Length;
            }
        }

        PartialStringComparerBase leftComparer = ignoreCase ? new LeftHandCaseInsensitiveStringComparer() : new LeftHandStringComparer();
        PartialStringComparerBase rightComparer = ignoreCase ? new RightHandCaseInsensitiveStringComparer() : new RightHandStringComparer();

        var leftSet = new HashSet<string>(leftComparer as IEqualityComparer<string>);
        var rightSet = new HashSet<string>(rightComparer as IEqualityComparer<string>);
        for (int count = 1; count <= maxSubstringLength; count++)
        {
            for (int index = 0; index < maxSubstringLength - count; index++)
            {
                leftComparer.Index = index;
                leftComparer.Count = count;

                var factor = GetUniquenessFactor(leftSet, uniqueStrings);
                if (factor >= SufficientUniquenessFactor)
                {
                    if (ignoreCase)
                    {
                        foreach (var ss in uniqueStrings)
                        {
                            if (!IsAllAscii(ss.AsSpan(leftComparer.Index, leftComparer.Count)))
                            {
                                // keep the slower non-ascii comparer since we have some non-ascii text
                                return leftComparer;
                            }
                        }

                        // optimize for all-ascii case
                        return new LeftHandCaseInsensitiveAsciiStringComparer
                        {
                            Index = leftComparer.Index,
                            Count = leftComparer.Count,
                        };
                    }

                    // Optimize the single char case
                    if (leftComparer.Count == 1)
                    {
                        return new LeftHandSingleCharStringComparer
                        {
                            Index = leftComparer.Index,
                            Count = 1,
                        };
                    }

                    return leftComparer;
                }

                rightComparer.Index = -index - count;
                rightComparer.Count = count;

                factor = GetUniquenessFactor(rightSet, uniqueStrings);
                if (factor >= SufficientUniquenessFactor)
                {
                    if (ignoreCase)
                    {
                        foreach (var ss in uniqueStrings)
                        {
                            if (!IsAllAscii(ss.AsSpan(ss.Length + rightComparer.Index, rightComparer.Count)))
                            {
                                // keep the slower non-ascii comparer since we have some non-ascii text
                                return rightComparer;
                            }
                        }

                        // optimize for all-ascii case
                        return new RightHandCaseInsensitiveAsciiStringComparer
                        {
                            Index = rightComparer.Index,
                            Count = rightComparer.Count,
                        };
                    }

                    // Optimize the single char case
                    if (rightComparer.Count == 1)
                    {
                        return new RightHandSingleCharStringComparer
                        {
                            Index = rightComparer.Index,
                            Count = 1,
                        };
                    }

                    return rightComparer;
                }
            }
        }

        if (!ignoreCase)
        {
            return new FullStringComparer();
        }

        foreach (var s in uniqueStrings)
        {
            if (!IsAllAscii(s.AsSpan()))
            {
                return new FullCaseInsensitiveStringComparer();
            }
        }

        return new FullCaseInsensitiveAsciiStringComparer();
    }

#pragma warning disable S109 // Magic numbers should not be used

    internal static unsafe bool IsAllAscii(ReadOnlySpan<char> s)
    {
        fixed (char* src = s)
        {
            uint* ptr = (uint*)src;
            int length = s.Length;

            while (length > 3)
            {
#pragma warning disable S1764 // Identical expressions should not be used on both sides of a binary operator
                if (!AllCharsInUInt32AreAscii(*ptr++ | *ptr++))
                {
                    return false;
                }
#pragma warning restore S1764 // Identical expressions should not be used on both sides of a binary operator

                length -= 4;
            }

            char* tail = (char*)ptr;
            while (length-- > 0)
            {
                var ch = *tail++;
                if (ch >= 0x7f)
                {
                    return false;
                }
            }
        }

        return true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AllCharsInUInt32AreAscii(uint value)
        {
            return (value & ~0x007F_007Fu) == 0;
        }
    }

    private static double GetUniquenessFactor(HashSet<string> set, IReadOnlyCollection<string> uniqueStrings)
    {
        set.Clear();
        foreach (var s in uniqueStrings)
        {
            _ = set.Add(s);
        }

        return set.Count / (double)uniqueStrings.Count;
    }
}
