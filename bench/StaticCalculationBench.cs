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

    public static string Expr5 = "1-(2*3/4+5)";

    public static string Expr300 =
        "(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)" +
        "+(1-(2*3/4+5)-6+7*8-9/10*11*12+13-(14*15-16+(17+18+19)/20)*21-((22+23-24*25)*26)*27+28-29-30)";

    [Benchmark]
    public void Bunpo5Parse()
    {
        var result = BunpoParser.Parse(Expr5);
        if (result != -5.5f) throw new("");
    }

    [Benchmark]
    public void Sprache5Parse()
    {
        var result = SpracheParser.Parse(Expr5);
        if (result != -5.5f) throw new("");
    }

    [Benchmark]
    public void Bunpo300Parse()
    {
        var result = BunpoParser.Parse(Expr300);
        if (result != 3853870f) throw new("");
    }

    [Benchmark]
    public void Sprache300Parse()
    {
        var result = SpracheParser.Parse(Expr300);
        if (result != 3853870f) throw new("");
    }
}
