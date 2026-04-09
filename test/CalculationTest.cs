using Xunit;
using Parser = System.Func<string, int, (int, float)?>;

namespace Bunpo.Test;

public class CalculationTest
{
    public static Parser Number = Combinator.Once(Combinator.Digits, float.Parse);
    public static Parser Spaces = Combinator.Once(Combinator.Spaces.ToOption(), _ => 0f);

    public static Parser Add = Combinator.Once(Combinator.Char('+'), _ => 0f);
    public static Parser Sub = Combinator.Once(Combinator.Char('-'), _ => 0f);
    public static Parser Mul = Combinator.Once(Combinator.Char('*'), _ => 0f);
    public static Parser Div = Combinator.Once(Combinator.Char('/'), _ => 0f);
    public static Parser LParen = Combinator.Once(Combinator.Char('('), _ => 0f);
    public static Parser RParen = Combinator.Once(Combinator.Char(')'), _ => 0f);

    [Fact]
    public void PlusTest()
    {
        var parser = Combinator.Sequence([Spaces, Number, Spaces, Add, Spaces, Number], xs => xs[1] + xs[5]);

        Assert.Equal(parser("1 + 2", 0)?.Item2, 3f);
        Assert.Equal(parser("  2+3", 0)?.Item2, 5f);
        Assert.Equal(parser("45 + 56", 0)?.Item2, 101f);
    }

    [Fact]
    public void CalcTest()
    {
        var lazy_term = Combinator.Lazy<float>();
        var lazy_expr = Combinator.Lazy<float>();

        var factor =
            Number |
            Combinator.Sequence([LParen, lazy_expr.Func, RParen], xs => xs[1]);
        var term =
            Combinator.Sequence([factor, Mul, lazy_term.Func], xs => xs[0] * xs[2]) |
            Combinator.Sequence([factor, Div, lazy_term.Func], xs => xs[0] / xs[2]) |
            factor;
        var expr =
            Combinator.Sequence([term, Add, lazy_expr.Func], xs => xs[0] + xs[2]) |
            Combinator.Sequence([term, Sub, lazy_expr.Func], xs => xs[0] - xs[2]) |
            term;

        lazy_term.LazyFunc = term;
        lazy_expr.LazyFunc = expr;

        Assert.Equal(expr("1+2", 0)?.Item2, 3f);
        Assert.Equal(expr("3*4", 0)?.Item2, 12f);
        Assert.Equal(expr("1+2*3+4", 0)?.Item2, 11f);
        Assert.Equal(expr("1*2+3*4", 0)?.Item2, 14f);

        Assert.Equal(expr("1-2", 0)?.Item2, -1f);
        Assert.Equal(expr("2/4", 0)?.Item2, 0.5f);
        Assert.Equal(expr("1-2*3/4", 0)?.Item2, -0.5f);
        Assert.Equal(expr("1*2-3*4", 0)?.Item2, -10f);
        Assert.Equal(expr("(1-2*3/4)+5", 0)?.Item2, 4.5f);
    }
}
