using BenchmarkDotNet.Attributes;
using Sprache;
using System;

namespace Bunpo.Benchmark;

public class AstBench
{
    public class Node()
    {
        public required char Ope { get; init; }
        public float Value { get; init; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }
    }

    public static Func<string, int, (int, Node)?> BunpoParser = null!;
    public static Parser<Node> SpracheParser = null!;

    public AstBench()
    {
        BunpoParser = BunpoSetup();
        SpracheParser = SpracheSetup();
    }

    [Benchmark]
    public Func<string, int, (int, Node)?> BunpoSetup()
    {
        var Number = Combinator.Digits.ToOnce(float.Parse);

        var Add = Combinator.Char('+');
        var Sub = Combinator.Char('-');
        var Mul = Combinator.Char('*');
        var Div = Combinator.Char('/');
        var LParen = Combinator.Char('(');
        var RParen = Combinator.Char(')');

        Func<string, int, (int, Node)?> expr = null!;

        var factor =
            Number.ToOnce(x => new Node { Ope = '9', Value = x }) |
            Combinator.Sequence([LParen.ToNone<Node>(), Combinator.Lazy(() => expr), RParen.ToNone<Node>()], xs => new Node { Ope = '(', Left = xs[1] });
        var term = Combinator.ChainLeft(factor, Mul | Div, (left, op, right) => new Node { Ope = op, Value = 0, Left = left, Right = right });
        expr = Combinator.ChainLeft(term, Add | Sub, (left, op, right) => new Node { Ope = op, Value = 0, Left = left, Right = right });

        return expr;
    }

    [Benchmark]
    public Parser<Node> SpracheSetup()
    {
        var Number =
            from dec in Parse.Decimal
            select new Node { Ope = '9', Value = float.Parse(dec) };

        Parser<Node> Expression = null!;

        var Factor =
            (from lparen in Parse.Char('(')
             from expr in Parse.Ref(() => Expression)
             from rparen in Parse.Char(')')
             select expr).Or(Number);

        var Term =
            Parse.ChainOperator(
                Parse.Char('*').Or(Parse.Char('/')),
                Factor,
                (op, left, right) => new Node { Ope = op, Value = 0, Left = left, Right = right });

        Expression =
            Parse.ChainOperator(
                Parse.Char('+').Or(Parse.Char('-')),
                Term,
                (op, left, right) => new Node { Ope = op, Value = 0, Left = left, Right = right });

        return Expression;
    }

    [Benchmark]
    public void BunpoParse()
    {
        _ = BunpoParser.Parse("1-(2*3/4+5)");
    }

    [Benchmark]
    public void BunpoParseAndEval()
    {
        var result = BunpoParser.Parse("1-(2*3/4+5)");
        if (Eval(result) != -5.5f) throw new("");
    }

    [Benchmark]
    public void SpracheParse()
    {
        _ = SpracheParser.Parse("1-(2*3/4+5)");
    }

    [Benchmark]
    public void SpracheParseAndEval()
    {
        var result = SpracheParser.Parse("1-(2*3/4+5)");
        if (Eval(result) != -5.5f) throw new("");
    }

    public static float Eval(Node node) => node.Ope switch
    {
        '9' => node.Value,
        '+' => Eval(node.Left!) + Eval(node.Right!),
        '-' => Eval(node.Left!) - Eval(node.Right!),
        '*' => Eval(node.Left!) * Eval(node.Right!),
        '/' => Eval(node.Left!) / Eval(node.Right!),
        '(' => Eval(node.Left!),
        _ => throw new()
    };
}
