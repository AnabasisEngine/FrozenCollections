# Frozen Dictionaries and Sets

This provides fast read-only dictionaries and sets compatible with IReadOnlyDictionary and IReadOnlySet respectively,
just faster than you're used to. There are four things that help this code perform better than .NET's standard collections:

* The collections this project creates are truly read-only and immutable. They are created in such a way that
the data is dense and optimally organized for fast read access.

* A bunch of cycles are burned when creating the collections to find an optimal size for the bucket table used
internally. This helps reduce or eliminate hash collisions, which improves average lookup time.

* The project includes specialized collection types which hardcode the key type. This specializes the generated
code, avoiding a lot of interface call overhead during runtime.

* Special attention is put on collections with string keys. A variety of comparers are selected dynamically to improve
hashing speed during lookups. The code looks through all the keys and tries finds the shortest substring slice across
the strings which produces a unique set of keys. This usually dramatically reduces the number of characters involved in
computing hash codes during lookup.

* These collections can hold at most 64K of state, which covers the 99% case.

Due to the relatively high creation cost of these collections, they are intended to be used primarily
for long-lived collections in your application: collectionss you initialize on startup and then don't 
need to touch again, or infrequently so.

## IFrozenDictionary

The classic IReadOnlyDictionary interfaces aren't ideal for enumeration or for large value types:

* If you enumerate a IReadOnlyDictionary, whether the dictionary itself, or the Keys or Values property, a little
bit of memory is allocated to hold the enumerator. If your code does this a lot, then you will suffer garbage
collection delays which reduces your application's average throughput.

* If your dictionary contains relatively large value types, then you will suffer copying overhead
whenever you perform lookups.

The IFrozenDictionary interface alleviates both of these problems. It enables zero-allocation enumeration for the dictionary and the
Keys and Values properties. The interface also exposes the GetByRef method which lets you get a reference to a value in the dictionary,
which lets you use it without copying it around.

For string-based collections, an optimization is provided that tries to minimize the overhead of hashing. The logic looks through
all the keys and tries to select the smallest substring slice across all these strings which are sufficiently unique, and uses 
that slice when computing hashes. This typically dramatically reduces the number of characters that need to be hashed for any
access to the collections.

## Perf

Here's what it looks like:

```
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK=6.0.200
  [Host] : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT

|                                        Method |      Mean |     Error |    StdDev |    Median | Allocated |
|---------------------------------------------- |----------:|----------:|----------:|----------:|----------:|
|                ClassicOrdinalStringDictionary | 18.676 us | 0.0646 us | 0.0884 us | 18.645 us |         - |
|                 FrozenOrdinalStringDictionary |  9.888 us | 0.1803 us | 0.2643 us |  9.760 us |         - |
| ClassicOrdinalCaseInsensitiveStringDictionary | 22.222 us | 0.0949 us | 0.1391 us | 22.183 us |         - |
|  FrozenOrdinalCaseInsensitiveStringDictionary |  9.940 us | 0.0545 us | 0.0765 us |  9.908 us |         - |
|                          ClassicIntDictionary |  4.605 us | 0.0162 us | 0.0237 us |  4.599 us |         - |
|                           FrozenIntDictionary |  2.535 us | 0.0073 us | 0.0104 us |  2.532 us |         - |
|                      ClassicComplexDictionary | 41.915 us | 0.0934 us | 0.1339 us | 41.924 us |         - |
|                       FrozenComplexDictionary | 34.190 us | 0.0675 us | 0.0968 us | 34.183 us |         - |
|                       ClassicOrdinalStringSet | 18.004 us | 0.0462 us | 0.0648 us | 17.987 us |         - |
|                        FrozenOrdinalStringSet |  9.338 us | 0.1356 us | 0.1945 us |  9.361 us |         - |
|        ClassicOrdinalCaseInsensitiveStringSet | 21.519 us | 0.0926 us | 0.1328 us | 21.497 us |         - |
|         FrozenOrdinalCaseInsensitiveStringSet |  9.789 us | 0.2204 us | 0.3230 us | 10.018 us |         - |
|                                 ClassicIntSet |  4.428 us | 0.0218 us | 0.0313 us |  4.418 us |         - |
|                                  FrozenIntSet |  3.189 us | 0.0088 us | 0.0129 us |  3.186 us |         - |
|                             ClassicComplexSet | 41.373 us | 0.0794 us | 0.1113 us | 41.363 us |         - |
|                              FrozenComplexSet | 34.110 us | 0.0678 us | 0.0994 us | 34.103 us |         - |
```

The benchmarks are creating 512-entry dictionaries and sets and performing 1024 lookups in each dictionary. 1/2 the lookups don't find the key they're looking for.

## Improvement Areas

* There are two algorithms which run when initializing the frozen collections which could potentially take a long time, depending on the
specific input given. Perhaps there are smarter designs which could help improve startup perf:

    * `StringComparers/ComparerPicker.Pick` tries to find a slice of unique characters in a set of input strings.

    * `FrozenHasTable.CalcNumBuckets` tries to find the best number of hash buckets in order to minimize collisions.
    Given that it's just sitting in a loop trying ever larger table sizes, it can take a while to run.

* Introduce case-insensitive single-character string comparers for added perf.

* Evaluate whether specialized two-character string comparers would be useful.
