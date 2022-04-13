using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace FrozenCollections.Bench;

[MemoryDiagnoser]
public class Program
{
    private const int NotFoundRatio = 2;    // 1 out of NotFoundRatio will query for an unknown key
    private const int NumKeys = 1024;

    private static readonly string[] _stringKeys = new string[NumKeys];
    private static readonly int[] _intKeys = new int[NumKeys];
    private static readonly ComplexKey[] _complexKeys = new ComplexKey[NumKeys];

    private static readonly Dictionary<string, int> _classicOrdinalStringDictionary;
    private static readonly Dictionary<string, int> _classicOrdinalCaseInsensitiveStringDictionary;
    private static readonly Dictionary<int, int> _classicIntDictionary;
    private static readonly Dictionary<ComplexKey, int> _classicComplexDictionary;

    private static readonly FrozenOrdinalStringDictionary<int> _frozenOrdinalStringDictionary;
    private static readonly FrozenOrdinalStringDictionary<int> _frozenOrdinalCaseInsensitiveStringDictionary;
    private static readonly FrozenIntDictionary<int> _frozenIntDictionary;
    private static readonly FrozenDictionary<ComplexKey, int> _frozenComplexDictionary;

    private static readonly HashSet<string> _classicOrdinalStringSet;
    private static readonly HashSet<string> _classicOrdinalCaseInsensitiveStringSet;
    private static readonly HashSet<int> _classicIntSet;
    private static readonly HashSet<ComplexKey> _classicComplexSet;

    private static readonly FrozenOrdinalStringSet _frozenOrdinalStringSet;
    private static readonly FrozenOrdinalStringSet _frozenOrdinalCaseInsensitiveStringSet;
    private static readonly FrozenIntSet _frozenIntSet;
    private static readonly FrozenSet<ComplexKey> _frozenComplexSet;

#pragma warning disable S3963 // "static" fields should be initialized inline
#pragma warning disable S109 // Magic numbers should not be used
#pragma warning disable CA1810 // Initialize reference type static fields inline

    static Program()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _stringKeys[i] = $"Red {i * 10}, Green {i * 12}, Blue {i * 14}";
        }

        for (int i = 0; i < _intKeys.Length; i++)
        {
            _intKeys[i] = i;
        }

        for (int i = 0; i < _complexKeys.Length; i++)
        {
            _complexKeys[i] = new ComplexKey
            {
                K1 = $"Red {i * 10}, Green {i * 12}, Blue {i * 14}",
                K2 = $"Purple {i * 16}",
            };
        }

        _classicOrdinalStringDictionary = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 0; i < _stringKeys.Length; i += NotFoundRatio)
        {
            _classicOrdinalStringDictionary[_stringKeys[i]] = i;
        }

        _classicOrdinalCaseInsensitiveStringDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < _stringKeys.Length; i += NotFoundRatio)
        {
            _classicOrdinalCaseInsensitiveStringDictionary[_stringKeys[i]] = i;
        }

        _classicIntDictionary = new Dictionary<int, int>();
        for (int i = 0; i < _stringKeys.Length; i += NotFoundRatio)
        {
            _classicIntDictionary[i] = i;
        }

        _classicComplexDictionary = new Dictionary<ComplexKey, int>(new ComplexKeyComparer());
        for (int i = 0; i < _complexKeys.Length; i += NotFoundRatio)
        {
            _classicComplexDictionary[_complexKeys[i]] = i;
        }

        _classicOrdinalStringSet = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < _stringKeys.Length; i += NotFoundRatio)
        {
            _classicOrdinalStringSet.Add(_stringKeys[i]);
        }

        _classicOrdinalCaseInsensitiveStringSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < _stringKeys.Length; i += NotFoundRatio)
        {
            _classicOrdinalCaseInsensitiveStringSet.Add(_stringKeys[i]);
        }

        _classicIntSet = new HashSet<int>();
        for (int i = 0; i < _stringKeys.Length; i += NotFoundRatio)
        {
            _classicIntSet.Add(i);
        }

        _classicComplexSet = new HashSet<ComplexKey>(new ComplexKeyComparer());
        for (int i = 0; i < _complexKeys.Length; i += NotFoundRatio)
        {
            _classicComplexSet.Add(_complexKeys[i]);
        }

        _frozenOrdinalStringDictionary = _classicOrdinalStringDictionary.ToFrozenDictionary();
        _frozenOrdinalCaseInsensitiveStringDictionary = _classicOrdinalCaseInsensitiveStringDictionary.ToFrozenDictionary(true);
        _frozenIntDictionary = _classicIntDictionary.ToFrozenDictionary();
        _frozenComplexDictionary = _classicComplexDictionary.ToFrozenDictionary(new ComplexKeyComparer());

        _frozenOrdinalStringSet = _classicOrdinalStringSet.ToFrozenSet();
        _frozenOrdinalCaseInsensitiveStringSet = _classicOrdinalCaseInsensitiveStringSet.ToFrozenSet(true);
        _frozenIntSet = _classicIntSet.ToFrozenSet();
        _frozenComplexSet = _classicComplexSet.ToFrozenSet(new ComplexKeyComparer());
    }

    public static void Main(string[] args)
    {
        var dontRequireSlnToRunBenchmarks = ManualConfig
            .Create(DefaultConfig.Instance)
            .AddJob(Job.MediumRun.WithToolchain(InProcessEmitToolchain.Instance));

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, dontRequireSlnToRunBenchmarks);
    }

#pragma warning disable CA1822 // Mark members as static

    [Benchmark]
    public void ClassicOrdinalStringDictionary()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _classicOrdinalStringDictionary.TryGetValue(_stringKeys[i], out _);
        }
    }

    [Benchmark]
    public void FrozenOrdinalStringDictionary()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _frozenOrdinalStringDictionary.TryGetValue(_stringKeys[i], out _);
        }
    }

    [Benchmark]
    public void ClassicOrdinalCaseInsensitiveStringDictionary()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _classicOrdinalCaseInsensitiveStringDictionary.TryGetValue(_stringKeys[i], out _);
        }
    }

    [Benchmark]
    public void FrozenOrdinalCaseInsensitiveStringDictionary()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _frozenOrdinalCaseInsensitiveStringDictionary.TryGetValue(_stringKeys[i], out _);
        }
    }

    [Benchmark]
    public void ClassicIntDictionary()
    {
        for (int i = 0; i < _intKeys.Length; i++)
        {
            _ = _classicIntDictionary.TryGetValue(_intKeys[i], out _);
        }
    }

    [Benchmark]
    public void FrozenIntDictionary()
    {
        for (int i = 0; i < _intKeys.Length; i++)
        {
            _ = _frozenIntDictionary.TryGetValue(_intKeys[i], out _);
        }
    }

    [Benchmark]
    public void ClassicComplexDictionary()
    {
        for (int i = 0; i < _complexKeys.Length; i++)
        {
            _ = _classicComplexDictionary.TryGetValue(_complexKeys[i], out _);
        }
    }

    [Benchmark]
    public void FrozenComplexDictionary()
    {
        for (int i = 0; i < _complexKeys.Length; i++)
        {
            _ = _frozenComplexDictionary.TryGetValue(_complexKeys[i], out _);
        }
    }

    [Benchmark]
    public void ClassicOrdinalStringSet()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _classicOrdinalStringSet.Contains(_stringKeys[i]);
        }
    }

    [Benchmark]
    public void FrozenOrdinalStringSet()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _frozenOrdinalStringSet.Contains(_stringKeys[i]);
        }
    }

    [Benchmark]
    public void ClassicOrdinalCaseInsensitiveStringSet()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _classicOrdinalCaseInsensitiveStringSet.Contains(_stringKeys[i]);
        }
    }

    [Benchmark]
    public void FrozenOrdinalCaseInsensitiveStringSet()
    {
        for (int i = 0; i < _stringKeys.Length; i++)
        {
            _ = _frozenOrdinalCaseInsensitiveStringSet.Contains(_stringKeys[i]);
        }
    }

    [Benchmark]
    public void ClassicIntSet()
    {
        for (int i = 0; i < _intKeys.Length; i++)
        {
            _ = _classicIntSet.Contains(_intKeys[i]);
        }
    }

    [Benchmark]
    public void FrozenIntSet()
    {
        for (int i = 0; i < _intKeys.Length; i++)
        {
            _ = _frozenIntSet.Contains(_intKeys[i]);
        }
    }

    [Benchmark]
    public void ClassicComplexSet()
    {
        for (int i = 0; i < _complexKeys.Length; i++)
        {
            _ = _classicComplexSet.Contains(_complexKeys[i]);
        }
    }

    [Benchmark]
    public void FrozenComplexSet()
    {
        for (int i = 0; i < _complexKeys.Length; i++)
        {
            _ = _frozenComplexSet.Contains(_complexKeys[i]);
        }
    }

    private sealed class ComplexKey
    {
        public string K1 { get; set; } = string.Empty;
        public string K2 { get; set; } = string.Empty;
    }

    private sealed class ComplexKeyComparer : IEqualityComparer<ComplexKey>
    {
        public bool Equals(ComplexKey? x, ComplexKey? y)
        {
            return string.Equals(x!.K1, y!.K1) && string.Equals(x!.K2, y!.K2);
        }

        public int GetHashCode([DisallowNull] ComplexKey obj)
        {
            return HashCode.Combine(obj.K1, obj.K2);
        }
    }
}
