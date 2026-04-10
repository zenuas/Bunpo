using BenchmarkDotNet.Attributes;
using FParsec;
using FParsec.CSharp;
using Microsoft.FSharp.Core;
using Sprache;
using System;
using static FParsec.CSharp.CharParsersCS;
using static FParsec.CSharp.PrimitivesCS;

namespace Bunpo.Benchmark;

public class StaticCalculationBench
{
    public static Func<string, int, (int, double)?> BunpoParser = null!;
    public static Parser<double> SpracheParser = null!;
    public static FSharpFunc<CharStream<Unit>, Reply<double>> FParseParser = null!;

    public StaticCalculationBench()
    {
        BunpoParser = BunpoSetup();
        SpracheParser = SpracheSetup();
        FParseParser = FParseSetup();
    }

    [Benchmark]
    public Func<string, int, (int, double)?> BunpoSetup()
    {
        var Number = Combinator.Digits.ToOnce(double.Parse);

        var Add = Combinator.Char('+');
        var Sub = Combinator.Char('-');
        var Mul = Combinator.Char('*');
        var Div = Combinator.Char('/');
        var LParen = Combinator.Char('(');
        var RParen = Combinator.Char(')');

        Func<string, int, (int, double)?> expr = null!;

        var factor =
            Number |
            Combinator.Sequence(LParen.ToNone<double>(), Combinator.Lazy(() => expr), RParen.ToNone<double>(), (_, x, _) => x);
        var term = Combinator.ChainLeft(factor, Mul | Div, (left, op, right) => op == '*' ? left * right : left / right);
        expr = Combinator.ChainLeft(term, Add | Sub, (left, op, right) => op == '+' ? left + right : left - right);

        return expr;
    }

    [Benchmark]
    public Parser<double> SpracheSetup()
    {
        var Number =
            from dec in Parse.Decimal
            select double.Parse(dec);

        Parser<double> Expression = null!;

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
    public FSharpFunc<CharStream<Unit>, Reply<double>> FParseSetup()
    {
        var expr =
            new OPPBuilder<Unit, double, Unit>()
                .WithOperators(ops => ops
                    .AddInfix("+", 10, (x, y) => x + y)
                    .AddInfix("-", 10, (x, y) => x - y)
                    .AddInfix("*", 20, (x, y) => x * y)
                    .AddInfix("/", 20, (x, y) => x / y))
                .WithTerms(term => Choice(
                    Float,
                    Between(CharP('('), term, CharP(')'))))
                .Build()
                .ExpressionParser;

        return expr;
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
        if (result != -5.5d) throw new("");
    }

    [Benchmark]
    public void Sprache5Parse()
    {
        var result = SpracheParser.Parse(Expr5);
        if (result != -5.5d) throw new("");
    }

    [Benchmark]
    public void FParse5Parse()
    {
        var result = FParseParser.ParseString(Expr5).Result;
        if (result != -5.5d) throw new("");
    }

    [Benchmark]
    public void Bunpo300Parse()
    {
        var result = BunpoParser.Parse(Expr300);
        if (result != 3853870d) throw new("");
    }

    [Benchmark]
    public void Sprache300Parse()
    {
        var result = SpracheParser.Parse(Expr300);
        if (result != 3853870d) throw new("");
    }

    [Benchmark]
    public void FParse300Parse()
    {
        var result = FParseParser.ParseString(Expr300).Result;
        if (result != 3853870d) throw new("");
    }
}
