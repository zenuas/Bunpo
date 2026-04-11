using BenchmarkDotNet.Attributes;
using System.Text;

namespace Bunpo.Benchmark;

public class StringBuilderBench
{
    [Benchmark]
    public void Char1()
    {
        var c = 'a';

        var s = $"{c}";
        if (s.Length != 1) throw new();
    }

    [Benchmark]
    public void Char2()
    {
        var c = 'a';

        var s = $"{c}{c}";
        if (s.Length != 2) throw new();
    }

    [Benchmark]
    public void Char3()
    {
        var c = 'a';

        var s = $"{c}{c}{c}";
        if (s.Length != 3) throw new();
    }

    [Benchmark]
    public void Char4()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}";
        if (s.Length != 4) throw new();
    }

    [Benchmark]
    public void Char5()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}";
        if (s.Length != 5) throw new();
    }

    [Benchmark]
    public void Char6()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}";
        if (s.Length != 6) throw new();
    }

    [Benchmark]
    public void Char7()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}{c}";
        if (s.Length != 7) throw new();
    }

    [Benchmark]
    public void Char8()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}{c}{c}";
        if (s.Length != 8) throw new();
    }

    [Benchmark]
    public void Char9()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}{c}{c}{c}";
        if (s.Length != 9) throw new();
    }

    [Benchmark]
    public void Char10()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}";
        if (s.Length != 10) throw new();
    }

    [Benchmark]
    public void Char15()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}";
        if (s.Length != 15) throw new();
    }

    [Benchmark]
    public void Char20()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}";
        if (s.Length != 20) throw new();
    }

    [Benchmark]
    public void Char30()
    {
        var c = 'a';

        var s = $"{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}{c}";
        if (s.Length != 30) throw new();
    }

    [Benchmark]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    [Arguments(8)]
    [Arguments(9)]
    [Arguments(10)]
    [Arguments(15)]
    [Arguments(20)]
    [Arguments(30)]
    public void CharN(int count)
    {
        var c = 'a';

        var s = "";
        for (var i = 0; i < count; i++)
        {
            s += c;
        }
        if (s.Length != count) throw new();
    }

    [Benchmark]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    [Arguments(8)]
    [Arguments(9)]
    [Arguments(10)]
    [Arguments(15)]
    [Arguments(20)]
    [Arguments(30)]
    public void StringBuilderN(int count)
    {
        var c = 'a';

        var s = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            s.Append(c);
        }
        if (s.ToString().Length != count) throw new();
    }
}
