using Xunit;
using Parser = System.Func<string, int, (int, float)?>;
using ParserChar = System.Func<string, int, (int, char)?>;

namespace Bunpo.Test;

public class CalculationTest
{
    public static Parser Number = Combinator.Digits.ToOnce(float.Parse);
    public static ParserChar Spaces = Combinator.Spaces.ToOption().ToOnce(_ => ' ');

    public static ParserChar Add = Combinator.Char('+');
    public static ParserChar Sub = Combinator.Char('-');
    public static ParserChar Mul = Combinator.Char('*');
    public static ParserChar Div = Combinator.Char('/');
    public static ParserChar LParen = Combinator.Char('(');
    public static ParserChar RParen = Combinator.Char(')');

    [Fact]
    public void CalcTest()
    {
        Parser expr = null!;
        var lazy_expr = Combinator.Lazy(expr);

        var factor =
            Spaces.ToNone<float>() ^ Number |
            Spaces.ToNone<float>() ^ Combinator.Sequence([LParen.ToNone<float>(), lazy_expr, Spaces.ToNone<float>(), RParen.ToNone<float>()], xs => xs[1]);
        var term = Combinator.ChainLeft(factor, Spaces ^ (Mul | Div), (left, op, right) => op == '*' ? left * right : left / right);
        expr = Combinator.ChainLeft(term, Spaces ^ (Add | Sub), (left, op, right) => op == '+' ? left + right : left - right);

        Assert.Equal(expr.Parse(" 1 + 2"), 3f);
        Assert.Equal(expr.Parse(" 3 * 4"), 12f);
        Assert.Equal(expr.Parse(" 1 + 2 * 3 + 4"), 11f);
        Assert.Equal(expr.Parse("1*2+3*4"), 14f);

        Assert.Equal(expr.Parse(" 1 - 2"), -1f);
        Assert.Equal(expr.Parse(" 2 / 4"), 0.5f);
        Assert.Equal(expr.Parse(" 1 - 2 * 3 / 4"), -0.5f);
        Assert.Equal(expr.Parse(" 1 * 2 - 3 * 4"), -10f);
        Assert.Equal(expr.Parse(" 1 - 2 * 3 / 4 + 5"), 4.5f);
    }
}
