using Xunit;
using Parser = System.Func<string, int, (int, float)?>;
using ParserChar = System.Func<string, int, (int, char)?>;

namespace Bunpo.Test;

public class CalculationTest
{
    public static Parser Number = Combinator.Once(Combinator.Digits, float.Parse);
    public static ParserChar Spaces = Combinator.Once(Combinator.Spaces.ToOption(), _ => ' ');

    public static ParserChar Add = Combinator.Char('+');
    public static ParserChar Sub = Combinator.Char('-');
    public static ParserChar Mul = Combinator.Char('*');
    public static ParserChar Div = Combinator.Char('/');
    public static ParserChar LParen = Combinator.Char('(');
    public static ParserChar RParen = Combinator.Char(')');

    [Fact]
    public void CalcTest()
    {
        var lazy_expr = Combinator.Lazy<float>();

        var factor =
            Combinator.None<char, float>(Spaces) ^ Number |
            Combinator.None<char, float>(Spaces) ^ Combinator.Sequence([Combinator.None<char, float>(LParen), lazy_expr.Func, Combinator.None<char, float>(Spaces), Combinator.None<char, float>(RParen)], xs => xs[1]);
        var term = Combinator.ChainLeft(factor, Spaces ^ (Mul | Div), (left, op, right) => op == '*' ? left * right : left / right);
        var expr = Combinator.ChainLeft(term, Spaces ^ (Add | Sub), (left, op, right) => op == '+' ? left + right : left - right);

        lazy_expr.LazyFunc = expr;

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
