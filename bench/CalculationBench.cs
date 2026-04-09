using BenchmarkDotNet.Attributes;

namespace Bunpo.Benchmark;

public class CalculationBench
{
    [Benchmark]
    public void Bunpo()
    {
        var Number = Combinator.Once(Combinator.Digits, float.Parse);

        var Add = Combinator.Once(Combinator.Char('+'), _ => 0f);
        var Sub = Combinator.Once(Combinator.Char('-'), _ => 0f);
        var Mul = Combinator.Once(Combinator.Char('*'), _ => 0f);
        var Div = Combinator.Once(Combinator.Char('/'), _ => 0f);
        var LParen = Combinator.Once(Combinator.Char('('), _ => 0f);
        var RParen = Combinator.Once(Combinator.Char(')'), _ => 0f);

        var lazy_term = Combinator.Lazy<float>();
        var lazy_expr = Combinator.Lazy<float>();

        var factor =
            Number |
            Combinator.Sequence([LParen, lazy_expr.Func, RParen], xs => xs[1]);
        var term =
            Combinator.Sequence([factor, Mul, lazy_term.Func], xs => xs[0] * xs[2]) |
            Combinator.Sequence([factor, Div, lazy_term.Func], xs => xs[0] / xs[2]) |
            factor;
        var expr =
            Combinator.Sequence([term, Add, lazy_expr.Func], xs => xs[0] + xs[2]) |
            Combinator.Sequence([term, Sub, lazy_expr.Func], xs => xs[0] - xs[2]) |
            term;

        lazy_term.LazyFunc = term;
        lazy_expr.LazyFunc = expr;

        _ = expr("(1-2*3/4)+5", 0);
    }
}
