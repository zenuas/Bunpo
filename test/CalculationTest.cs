using System;
using Xunit;

namespace Bunpo.Test;

public class CalculationTest
{
    public static Func<string, int, (int, float)?> Number = Combinator.Once(Combinator.Digits, float.Parse);
    public static Func<string, int, (int, float)?> Spaces = Combinator.Once(Combinator.Spaces.ToOption(), _ => 0f);

    public static Func<string, int, (int, float)?> Add = Combinator.Once(Combinator.Char('+'), _ => 0f);
    public static Func<string, int, (int, float)?> Sub = Combinator.Once(Combinator.Char('-'), _ => 0f);
    public static Func<string, int, (int, float)?> Mul = Combinator.Once(Combinator.Char('*'), _ => 0f);
    public static Func<string, int, (int, float)?> Div = Combinator.Once(Combinator.Char('/'), _ => 0f);

    [Fact]
    public void PlusTest()
    {
        var parser = Combinator.Sequence([Spaces, Number, Spaces, Add, Spaces, Number], xs => xs[1] + xs[5]);

        Assert.Equal(parser("1 + 2", 0)?.Item2, 3f);
        Assert.Equal(parser("  2+3", 0)?.Item2, 5f);
        Assert.Equal(parser("45 + 56", 0)?.Item2, 101f);
    }

    [Fact]
    public void MulTest()
    {
        var lazy_term = Combinator.Lazy<float>();
        var lazy_expr = Combinator.Lazy<float>();

        var factor = Number;
        var term = Combinator.Sequence([factor, Mul, lazy_term.Func], xs => xs[0] * xs[2]) | factor;
        var expr = Combinator.Sequence([term, Add, lazy_expr.Func], xs => xs[0] + xs[2]) | term;

        lazy_term.LazyFunc = term;
        lazy_expr.LazyFunc = expr;

        Assert.Equal(expr("1+2", 0)?.Item2, 3f);
        Assert.Equal(expr("3*4", 0)?.Item2, 12f);
        Assert.Equal(expr("1+2*3+4", 0)?.Item2, 11f);
        Assert.Equal(expr("1*2+3*4", 0)?.Item2, 14f);
    }
}
