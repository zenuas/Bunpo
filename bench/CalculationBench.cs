using BenchmarkDotNet.Attributes;
using Sprache;
using System;

namespace Bunpo.Benchmark;

public class CalculationBench
{
    [Benchmark]
    public void BunpoSetupAndParse()
    {
        var Number = Combinator.Digits.ToOnce(float.Parse);

        var Add = Combinator.Char('+');
        var Sub = Combinator.Char('-');
        var Mul = Combinator.Char('*');
        var Div = Combinator.Char('/');
        var LParen = Combinator.Char('(');
        var RParen = Combinator.Char(')');

        Func<string, int, (int, float)?> expr = null!;

        var factor =
            Number |
            Combinator.Sequence([LParen.ToNone<float>(), Combinator.Lazy(() => expr), RParen.ToNone<float>()], xs => xs[1]);
        var term = Combinator.ChainLeft(factor, Mul | Div, (left, op, right) => op == '*' ? left * right : left / right);
        expr = Combinator.ChainLeft(term, Add | Sub, (left, op, right) => op == '+' ? left + right : left - right);

        var result = expr.Parse("1-(2*3/4+5)");
        if (result != -5.5f) throw new("");
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

        var result = Expression.Parse("1-(2*3/4+5)");
        if (result != -5.5f) throw new("");
    }
}
