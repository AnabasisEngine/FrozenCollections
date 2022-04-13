# Frozen Dictionaries and Sets

This provides fast read-only dictionaries and sets compatible with `IReadOnlyDictionary` and `IReadOnlySet` respectively,
just faster than you're used to.

There are four things that help this code perform better than .NET's standard collections:

* The collections this project creates are truly read-only and immutable. They are created in such a way that
the data is dense and optimally organized for fast read access.

* A bunch of cycles are burned when creating the collections to find an optimal size for the bucket table used
internally. This helps reduce or eliminate hash collisions, which improves average lookup time.

* The project includes specialized collection types which hard code the key type. This specializes the generated
code, avoiding a lot of interface call overhead during runtime.

* Special attention is put on collections with string keys. A variety of `comparers` are selected dynamically to improve
hashing speed during lookups. The code looks through all the keys and tries finds the shortest substring slice across
the strings which produces a unique set of keys. This usually dramatically reduces the number of characters involved in
computing hash codes during lookup.

* These collections can hold at most 64K of state, which covers the 99% case.

Due to the relatively high creation cost of these collections, they are intended to be used primarily
for long-lived collections in your application: collections you initialize on startup and then don't
need to touch again, or infrequently so.

## `IFrozenDictionary`

The classic `IReadOnlyDictionary` interface isn't ideal for enumeration or for large value types:

* If you enumerate an `IReadOnlyDictionary`, whether the dictionary itself, or the Keys or Values property, a little
bit of memory is allocated to hold the enumerator. If your code does this a lot, then you will suffer garbage
collection delays which reduces your application's average throughput.

* If your dictionary contains relatively large value types, then you will suffer copying overhead
whenever you perform lookups.

The `IFrozenDictionary` interface alleviates both of these problems. It enables zero-allocation enumeration for the dictionary and the
Keys and Values properties. The interface also exposes the `GetByRef` method which lets you get a reference to a value in the dictionary,
which lets you use it without copying it around.

For string-based collections, an optimization is provided that tries to minimize the overhead of hashing. The logic looks through
all the keys and tries to select the smallest substring slice across all these strings which are sufficiently unique, and uses
that slice when computing hashes. This typically dramatically reduces the number of characters that need to be hashed for any
access to the collections.

## Performance

Here's what it looks like:

```text
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK=6.0.200
  [Host] : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT

|                                        Method |      Mean |     Error |    StdDev | Allocated |
|---------------------------------------------- |----------:|----------:|----------:|----------:|
|                ClassicOrdinalStringDictionary | 18.511 us | 0.0665 us | 0.0932 us |         - |
|                 FrozenOrdinalStringDictionary |  9.619 us | 0.0623 us | 0.0933 us |         - |
| ClassicOrdinalCaseInsensitiveStringDictionary | 21.608 us | 0.0664 us | 0.0931 us |         - |
|  FrozenOrdinalCaseInsensitiveStringDictionary | 10.138 us | 0.1973 us | 0.2953 us |         - |
|                          ClassicIntDictionary |  4.796 us | 0.0382 us | 0.0560 us |         - |
|                           FrozenIntDictionary |  2.210 us | 0.0084 us | 0.0126 us |         - |
|                      ClassicComplexDictionary | 41.461 us | 0.2098 us | 0.3075 us |         - |
|                       FrozenComplexDictionary | 35.748 us | 0.1236 us | 0.1772 us |         - |
|                       ClassicOrdinalStringSet | 17.612 us | 0.0834 us | 0.1222 us |         - |
|                        FrozenOrdinalStringSet |  9.124 us | 0.0501 us | 0.0718 us |         - |
|        ClassicOrdinalCaseInsensitiveStringSet | 21.164 us | 0.2285 us | 0.3350 us |         - |
|         FrozenOrdinalCaseInsensitiveStringSet |  9.389 us | 0.0402 us | 0.0564 us |         - |
|                                 ClassicIntSet |  4.317 us | 0.0133 us | 0.0186 us |         - |
|                                  FrozenIntSet |  3.241 us | 0.0274 us | 0.0401 us |         - |
|                             ClassicComplexSet | 40.548 us | 0.1182 us | 0.1618 us |         - |
|                              FrozenComplexSet | 35.554 us | 0.3105 us | 0.4552 us |         - |
```

The benchmarks are creating 512 entry dictionaries and sets and performing 1024 lookups in each dictionary. 1/2 the lookups don't find the key they're looking for.

## Improvement Areas

* There are two algorithms which run when initializing the frozen collections which could potentially take a long time, depending on the
specific input given. Perhaps there are smarter designs which could help improve startup performance:

    * `StringComparers/ComparerPicker.Pick` tries to find a slice of unique characters in a set of input strings.

    * `FrozenHasTable.CalcNumBuckets` tries to find the best number of hash buckets in order to minimize collisions.
    Given that it's just sitting in a loop trying ever larger table sizes, it can take a while to run.

* Introduce case-insensitive single-character string `comparers` for added performance.

* Evaluate whether specialized two-character string `comparers` would be useful.
