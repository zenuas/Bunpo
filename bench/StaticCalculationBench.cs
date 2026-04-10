using BenchmarkDotNet.Attributes;
using Sprache;
using System;

namespace Bunpo.Benchmark;

public class StaticCalculationBench
{
    public static Func<string, int, (int, float)?> BunpoParser = null!;
    public static Parser<float> SpracheParser = null!;

    public StaticCalculationBench()
    {
        BunpoParser = BunpoSetup();
        SpracheParser = SpracheSetup();
    }

    [Benchmark]
    public Func<string, int, (int, float)?> BunpoSetup()
    {
        var Number = Combinator.Once(Combinator.Digits, float.Parse);

        var Add = Combinator.Char('+');
        var Sub = Combinator.Char('-');
        var Mul = Combinator.Char('*');
        var Div = Combinator.Char('/');
        var LParen = Combinator.Char('(');
        var RParen = Combinator.Char(')');

        var lazy_expr = Combinator.Lazy<float>();

        var factor =
            Number |
            Combinator.Sequence([Combinator.None<char, float>(LParen), lazy_expr.Func, Combinator.None<char, float>(RParen)], xs => xs[1]);
        var term = Combinator.ChainLeft(factor, Mul | Div, (left, op, right) => op == '*' ? left * right : left / right);
        var expr = Combinator.ChainLeft(term, Add | Sub, (left, op, right) => op == '+' ? left + right : left - right);

        lazy_expr.LazyFunc = expr;
        return expr;
    }

    [Benchmark]
    public Parser<float> SpracheSetup()
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

        return Expression;
    }

    [Benchmark]
    public void BunpoParse()
    {
        var result = BunpoParser.Parse("1-2*3/4+5");
        if (result != 4.5f) throw new("");
    }

    [Benchmark]
    public void SpracheParse()
    {
        var result = SpracheParser.Parse("1-2*3/4+5");
        if (result != 4.5f) throw new("");
    }
}
