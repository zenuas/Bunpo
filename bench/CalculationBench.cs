using BenchmarkDotNet.Attributes;
using Sprache;

namespace Bunpo.Benchmark;

public class CalculationBench
{
    [Benchmark]
    public void BunpoSetupAndParse()
    {
        var Number = Combinator.Once(Combinator.Digits, float.Parse);
        var Spaces = Combinator.Once(Combinator.Spaces.ToOption(), _ => ' ');

        var Add = Combinator.Char('+');
        var Sub = Combinator.Char('-');
        var Mul = Combinator.Char('*');
        var Div = Combinator.Char('/');
        var LParen = Combinator.Char('(');
        var RParen = Combinator.Char(')');

        var lazy_expr = Combinator.Lazy<float>();

        var factor =
            Combinator.None<char, float>(Spaces) ^ Number |
            Combinator.None<char, float>(Spaces) ^ Combinator.Sequence([Combinator.None<char, float>(LParen), lazy_expr.Func, Combinator.None<char, float>(Spaces), Combinator.None<char, float>(RParen)], xs => xs[1]);
        var term = Combinator.ChainLeft(factor, Spaces ^ (Mul | Div), (left, op, right) => op == '*' ? left * right : left / right);
        var expr = Combinator.ChainLeft(term, Spaces ^ (Add | Sub), (left, op, right) => op == '+' ? left + right : left - right);

        lazy_expr.LazyFunc = expr;

        var result = expr.Parse("1-2*3/4+5");
        if (result != 4.5f) throw new("");
    }

    [Benchmark]
    public void SpracheSetupAndParse()
    {
        var Number =
            from dec in Parse.Decimal
            select float.Parse(dec);

        Parser<float> Expression = null!;

        var Factor =
            (from lparen in Parse.Char('(')
             from expr in Parse.Ref(() => Expression)
             from rparen in Parse.Char(')')
             select expr).Or(Number);

        var Term =
            Parse.ChainOperator(
                Parse.Char('*').Or(Parse.Char('/')),
                Factor,
                (op, left, right) => op == '*' ? left * right : left / right);

        Expression =
            Parse.ChainOperator(
                Parse.Char('+').Or(Parse.Char('-')),
                Term,
                (op, left, right) => op == '+' ? left + right : left - right);

        var result = Expression.Parse("1-2*3/4+5");
        if (result != 4.5f) throw new("");
    }
}
