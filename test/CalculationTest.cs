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

        Assert.Equal(parser.Parse("1 + 2"), 3f);
        Assert.Equal(parser.Parse("  2+3"), 5f);
        Assert.Equal(parser.Parse("45 + 56"), 101f);
    }

    [Fact]
    public void CalcTest()
    {
        var lazy_term = Combinator.Lazy<float>();
        var lazy_expr = Combinator.Lazy<float>();

        var factor =
            Spaces ^ Number |
            Spaces ^ Combinator.Sequence([LParen, lazy_expr.Func, Spaces, RParen], xs => xs[1]);
        var term =
            Combinator.Sequence([factor, Spaces ^ Mul, lazy_term.Func], xs => xs[0] * xs[2]) |
            Combinator.Sequence([factor, Spaces ^ Div, lazy_term.Func], xs => xs[0] / xs[2]) |
            factor;
        var expr =
            Combinator.Sequence([term, Spaces ^ Add, lazy_expr.Func], xs => xs[0] + xs[2]) |
            Combinator.Sequence([term, Spaces ^ Sub, lazy_expr.Func], xs => xs[0] - xs[2]) |
            term;

        lazy_term.LazyFunc = term;
        lazy_expr.LazyFunc = expr;

        Assert.Equal(expr.Parse(" 1 + 2"), 3f);
        Assert.Equal(expr.Parse(" 3 * 4"), 12f);
        Assert.Equal(expr.Parse(" 1 + 2 * 3 + 4"), 11f);
        Assert.Equal(expr.Parse("1*2+3*4"), 14f);

        Assert.Equal(expr.Parse(" 1 - 2"), -1f);
        Assert.Equal(expr.Parse(" 2 / 4"), 0.5f);
        Assert.Equal(expr.Parse(" 1 - 2 * 3 / 4"), -0.5f);
        Assert.Equal(expr.Parse(" 1 * 2 - 3 * 4"), -10f);
        Assert.Equal(expr.Parse(" ( 1 - 2 * 3 / 4 ) + 5"), 4.5f);
    }
}
