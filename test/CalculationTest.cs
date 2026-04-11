using Xunit;
using Parser = System.Func<string, int, (int, float)?>;

namespace Bunpo.Test;

public class CalculationTest
{
    [Fact]
    public void CalcTest()
    {
        var Number = Combinator.Digits.ToOnce(float.Parse);
        var Spaces = Combinator.Spaces.ToOption().ToOnce(_ => ' ');

        var Add = Combinator.Char('+');
        var Sub = Combinator.Char('-');
        var Mul = Combinator.Char('*');
        var Div = Combinator.Char('/');
        var LParen = Combinator.Char('(');
        var RParen = Combinator.Char(')');

        Parser expr = null!;

        var factor =
            Spaces.ToNone<float>() ^ Number |
            Spaces.ToNone<float>() ^ Combinator.Sequence(LParen.ToNone<float>(), Combinator.Lazy(() => expr), Spaces.ToNone<float>() ^ RParen.ToNone<float>(), static (_, x, _) => x);
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
        Assert.Equal(expr.Parse(" 1 - ( 2 * 3 / 4 + 5 )"), -5.5f);
    }
}
