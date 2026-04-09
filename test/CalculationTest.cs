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
        Func<string, int, (int, float)?> lazy_term_in = null!;
        Func<string, int, (int, float)?> lazy_term = (input, start) => lazy_term_in(input, start);

        Func<string, int, (int, float)?> lazy_expr_in = null!;
        Func<string, int, (int, float)?> lazy_expr = (input, start) => lazy_expr_in(input, start);

        var factor = Number;
        var term = Combinator.Sequence([factor, Mul, lazy_term], xs => xs[0] * xs[2]) | factor;
        var expr = Combinator.Sequence([term, Add, lazy_expr], xs => xs[0] + xs[2]) | term;

        lazy_term_in = term;
        lazy_expr_in = expr;

        Assert.Equal(expr("1+2", 0)?.Item2, 3f);
        Assert.Equal(expr("3*4", 0)?.Item2, 12f);
        Assert.Equal(expr("1+2*3+4", 0)?.Item2, 11f);
        Assert.Equal(expr("1*2+3*4", 0)?.Item2, 14f);
    }
}
