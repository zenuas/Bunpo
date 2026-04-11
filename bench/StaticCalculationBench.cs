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
    public Func<string, int, (int, double)?> BunpoParser = null!;
    public Parser<double> SpracheParser = null!;
    public FSharpFunc<CharStream<Unit>, Reply<double>> FParseParser = null!;
    public Func<ReadOnlySpan<char>, (int, double)?> ManualParser = null!;

    public const string Expr5 = "1-(2*3/4+5)";

    public const string Expr300 =
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

    public StaticCalculationBench()
    {
        BunpoParser = BunpoSetup();
        SpracheParser = SpracheSetup();
        FParseParser = FParseSetup();
        ManualParser = ManualSetup();
    }

    [Benchmark]
    public Func<string, int, (int, double)?> BunpoSetup()
    {
        var Number = Combinator.NaturalNumberDouble;

        var Add = Combinator.Char('+');
        var Sub = Combinator.Char('-');
        var Mul = Combinator.Char('*');
        var Div = Combinator.Char('/');
        var LParen = Combinator.Char('(');
        var RParen = Combinator.Char(')');

        Func<string, int, (int, double)?> expr = null!;

        var factor =
            Number |
            Combinator.Sequence(LParen, Combinator.Lazy(() => expr), RParen, (_, x, _) => x);
        var term = Combinator.ChainLeft(factor, Mul | Div, (left, op, right) => op == '*' ? left * right : left / right);
        expr = Combinator.ChainLeft(term, Add | Sub, (left, op, right) => op == '+' ? left + right : left - right);

        return expr;
    }

    [Benchmark]
    public void Bunpo5Parse()
    {
        var result = BunpoParser.Parse(Expr5);
        if (result != -5.5d) throw new("");
    }

    [Benchmark]
    public void Bunpo300Parse()
    {
        var result = BunpoParser.Parse(Expr300);
        if (result != 3853870d) throw new("");
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

    [Benchmark]
    public void FParse5Parse()
    {
        var result = FParseParser.ParseString(Expr5).Result;
        if (result != -5.5d) throw new("");
    }

    [Benchmark]
    public void FParse300Parse()
    {
        var result = FParseParser.ParseString(Expr300).Result;
        if (result != 3853870d) throw new("");
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
    public void Sprache5Parse()
    {
        var result = SpracheParser.Parse(Expr5);
        if (result != -5.5d) throw new("");
    }

    [Benchmark]
    public void Sprache300Parse()
    {
        var result = SpracheParser.Parse(Expr300);
        if (result != 3853870d) throw new("");
    }

    [Benchmark]
    public Func<ReadOnlySpan<char>, (int, double)?> ManualSetup()
    {
        var Number = (int, double)? (ReadOnlySpan<char> input) =>
        {
            if (input.Length == 0 || !char.IsAsciiDigit(input[0])) return null;
            double value = input[0] - '0';
            for (var i = 1; i < input.Length; i++)
            {
                if (!char.IsAsciiDigit(input[i])) return (i, value);
                value = value * 10 + input[i] - '0';
            }
            return (input.Length, value);
        };

        Func<ReadOnlySpan<char>, (int, double)?> expr = null!;

        var factor = (int, double)? (ReadOnlySpan<char> input) =>
        {
            if (Number(input) is { } num) return num;
            if (input[0] == '(')
            {
                if (expr(input[1..]) is { } e && 1 + e.Item1 < input.Length && input[1 + e.Item1] == ')') return (2 + e.Item1, e.Item2);
            }
            return null;
        };
        var term = (int, double)? (ReadOnlySpan<char> input) =>
        {
            if (factor(input) is { } left)
            {
                while (left.Item1 < input.Length)
                {
                    if (input[left.Item1] == '*')
                    {
                        if (factor(input[(left.Item1 + 1)..]) is { } mul) left = (left.Item1 + 1 + mul.Item1, left.Item2 * mul.Item2);
                        else break;
                    }
                    else if (input[left.Item1] == '/')
                    {
                        if (factor(input[(left.Item1 + 1)..]) is { } div) left = (left.Item1 + 1 + div.Item1, left.Item2 / div.Item2);
                        else break;
                    }
                    else break;
                }
                return left;
            }
            return null;
        };
        expr = (int, double)? (ReadOnlySpan<char> input) =>
        {
            if (term(input) is { } left)
            {
                while (left.Item1 < input.Length)
                {
                    if (input[left.Item1] == '+')
                    {
                        if (term(input[(left.Item1 + 1)..]) is { } add) left = (left.Item1 + 1 + add.Item1, left.Item2 + add.Item2);
                        else break;
                    }
                    else if (input[left.Item1] == '-')
                    {
                        if (term(input[(left.Item1 + 1)..]) is { } sub) left = (left.Item1 + 1 + sub.Item1, left.Item2 - sub.Item2);
                        else break;
                    }
                    else break;
                }
                return left;
            }
            return null;
        };

        return expr;
    }

    [Benchmark]
    public void Manual5Parse()
    {
        var result = ManualParser(Expr5.AsSpan())!.Value.Item2;
        if (result != -5.5d) throw new("");
    }

    [Benchmark]
    public void Manual300Parse()
    {
        var result = ManualParser(Expr300.AsSpan())!.Value.Item2;
        if (result != 3853870d) throw new("");
    }
}
