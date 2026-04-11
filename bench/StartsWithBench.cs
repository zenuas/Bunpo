using BenchmarkDotNet.Attributes;
using System;

namespace Bunpo.Benchmark;

public class StartsWithBench
{
    public const string Text10 = "StartsWith";
    public const string Text20 = "aaaa StartsWith aaaa";
    public const string Text100 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa StartsWith aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    public const string Text200 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa StartsWith aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [Benchmark]
    public void StartsWith()
    {
        var b = Text10.StartsWith("StartsWith");
        if (!b) throw new();
    }

    [Benchmark]
    public void SkipToStartsWith20()
    {
        var b = Text20[5..].StartsWith("StartsWith", StringComparison.Ordinal);
        if (!b) throw new();
    }

    [Benchmark]
    public void AsSpanStartsWith20()
    {
        var b = Text20.AsSpan()[5..].StartsWith("StartsWith", StringComparison.Ordinal);
        if (!b) throw new();
    }

    [Benchmark]
    public void Compare20()
    {
        var b = string.CompareOrdinal(Text20, 5, "StartsWith", 0, 10);
        if (b != 0) throw new();
    }

    [Benchmark]
    public void SkipToStartsWith100()
    {
        var b = Text100[45..].StartsWith("StartsWith", StringComparison.Ordinal);
        if (!b) throw new();
    }

    [Benchmark]
    public void AsSpanStartsWith100()
    {
        var b = Text100.AsSpan()[45..].StartsWith("StartsWith", StringComparison.Ordinal);
        if (!b) throw new();
    }

    [Benchmark]
    public void Compare100()
    {
        var b = string.CompareOrdinal(Text100, 45, "StartsWith", 0, 10);
        if (b != 0) throw new();
    }

    [Benchmark]
    public void SkipToStartsWith200()
    {
        var b = Text200[95..].StartsWith("StartsWith", StringComparison.Ordinal);
        if (!b) throw new();
    }

    [Benchmark]
    public void AsSpanStartsWith200()
    {
        var b = Text200.AsSpan()[95..].StartsWith("StartsWith", StringComparison.Ordinal);
        if (!b) throw new();
    }

    [Benchmark]
    public void Compare200()
    {
        var b = string.CompareOrdinal(Text200, 95, "StartsWith", 0, 10);
        if (b != 0) throw new();
    }
}
