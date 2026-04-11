using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Bunpo;

public static class Combinator
{
    public static Func<ReadOnlySpan<char>, (int, char)?> Char(char c) => input => input.Length == 0 || input[0] != c ? null : (1, c);
    public static Func<ReadOnlySpan<char>, (int, char)?> Char(Func<char, bool> f) => input => input.Length == 0 || !f(input[0]) ? null : (1, input[0]);
    public static Func<ReadOnlySpan<char>, (int, string)?> Chars(Func<ReadOnlySpan<char>, (int, char)?> c) => input =>
    {
        // To avoid StringBuilder instance creation, will try to match up to 4 times.
        var r1 = c(input);
        if (r1 is null) return null;
        var length = r1.Value.Item1;
        var r2 = c(input[length..]);
        if (r2 is null) return (length, "" + r1.Value.Item2);
        length += r2.Value.Item1;
        var r3 = c(input[length..]);
        if (r3 is null) return (length, "" + r1.Value.Item2 + r2.Value.Item2);
        length += r3.Value.Item1;
        var r4 = c(input[length..]);
        if (r4 is null) return (length, "" + r1.Value.Item2 + r2.Value.Item2 + r3.Value.Item2);
        length += r4.Value.Item1;

        var chars = new StringBuilder($"{r1.Value.Item2}{r2.Value.Item2}{r3.Value.Item2}{r4.Value.Item2}");
        while (true)
        {
            var result = c(input[length..]);
            if (result is null) break;
            length += result.Value.Item1;
            chars.Append(result.Value.Item2);
        }
        return (length, chars.ToString());
    };
    public static Func<ReadOnlySpan<char>, (int, char)?> CharClass(params char[] chars) => input => input.Length == 0 || !chars.Contains(input[0]) ? null : (1, input[0]);
    public static Func<ReadOnlySpan<char>, (int, char)?> CharClass(string chars) => CharClass(chars.ToCharArray());
    public static Func<ReadOnlySpan<char>, (int, string)?> String(char c) => Once(Char(c), _ => c.ToString());
    public static Func<ReadOnlySpan<char>, (int, string)?> String(string s) => input => input.Length < s.Length || !input.StartsWith(s, StringComparison.Ordinal) ? null : (s.Length, s);
    public static Func<ReadOnlySpan<char>, (int, string)?> String(Func<ReadOnlySpan<char>, (int, char)?> c) => Once(c, x => x.ToString());
    public static Func<ReadOnlySpan<char>, (int, T)?> String<T>(Func<ReadOnlySpan<char>, (int, T)?> f) => input => f(input);

    public static Func<ReadOnlySpan<char>, (int, R)?> Once<T, R>(Func<ReadOnlySpan<char>, (int, T)?> once, Func<T, R> match) => input => once(input) is { } p ? (p.Item1, match(p.Item2)) : null;
    public static Func<ReadOnlySpan<char>, (int, R)?> None<T, R>(Func<ReadOnlySpan<char>, (int, T)?> f) => Once<T, R>(f, _ => default!);
    public static Func<ReadOnlySpan<char>, (int, T?)?> Option<T>(Func<ReadOnlySpan<char>, (int, T)?> once) => input => once(input) is { } p ? p : (0, default);

    public static Func<ReadOnlySpan<char>, (int, T?)?> Many<T>(Func<ReadOnlySpan<char>, (int, T)?> many) => Many(many, xs => xs.LastOrDefault());
    public static Func<ReadOnlySpan<char>, (int, R)?> Many<T, R>(Func<ReadOnlySpan<char>, (int, T)?> many, Func<IReadOnlyList<T>, R> match) => input =>
    {
        var length = 0;
        var values = new List<T>();
        while (true)
        {
            var result = many(input[length..]);
            if (result is null) break;
            length += result.Value.Item1;
            values.Add(result.Value.Item2);
        }
        return (length, match(values));
    };

    public static Func<ReadOnlySpan<char>, (int, T)?> Many1<T>(Func<ReadOnlySpan<char>, (int, T)?> many) => Many1(many, xs => xs.Last());
    public static Func<ReadOnlySpan<char>, (int, R)?> Many1<T, R>(Func<ReadOnlySpan<char>, (int, T)?> many, Func<IReadOnlyList<T>, R> match) => input =>
    {
        var first = many(input);
        if (first is null) return null;

        var length = first.Value.Item1;
        var values = new List<T>() { first.Value.Item2 };
        while (true)
        {
            var result = many(input[length..]);
            if (result is null) break;
            length += result.Value.Item1;
            values.Add(result.Value.Item2);
        }
        return (length, match(values));
    };

    public static Func<ReadOnlySpan<char>, (int, T?)?> Many<T>(Func<ReadOnlySpan<char>, (int, T)?> many, uint min, uint max) => Many(many, min, max, xs => xs.LastOrDefault());
    public static Func<ReadOnlySpan<char>, (int, R)?> Many<T, R>(Func<ReadOnlySpan<char>, (int, T)?> many, uint min, uint max, Func<IReadOnlyList<T>, R> match)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);
        return input =>
        {
            var length = 0;
            var values = new List<T>();
            var i = 0;
            for (; i < min; i++)
            {
                var result = many(input[length..]);
                if (result is null) return null;
                length += result.Value.Item1;
                values.Add(result.Value.Item2);
            }

            for (; i < max; i++)
            {
                var result = many(input[length..]);
                if (result is null) break;
                length += result.Value.Item1;
                values.Add(result.Value.Item2);
            }
            return (length, match(values));
        };
    }

    public static Func<ReadOnlySpan<char>, (int, T)?> ChainLeft<T, TOpe>(Func<ReadOnlySpan<char>, (int, T)?> chain, Func<ReadOnlySpan<char>, (int, TOpe)?> op, Func<T, TOpe, T, T> match) => input =>
    {
        var first = chain(input);
        if (first is null) return null;

        var length = first.Value.Item1;
        var left = first.Value.Item2;
        while (true)
        {
            var opr = op(input[length..]);
            if (opr is null) break;
            var right = chain(input[(length + opr.Value.Item1)..]);
            if (right is null) break;
            length += opr.Value.Item1 + right.Value.Item1;
            left = match(left, opr.Value.Item2, right.Value.Item2);
        }
        return (length, left);
    };

    public static Func<ReadOnlySpan<char>, (int, T2)?> Sequence<T1, T2>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b) => Sequence(a, b, (_, x) => x);
    public static Func<ReadOnlySpan<char>, (int, T3)?> Sequence<T1, T2, T3>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c) => Sequence(a, b, c, (_, _, x) => x);
    public static Func<ReadOnlySpan<char>, (int, T4)?> Sequence<T1, T2, T3, T4>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d) => Sequence(a, b, c, d, (_, _, _, x) => x);
    public static Func<ReadOnlySpan<char>, (int, T5)?> Sequence<T1, T2, T3, T4, T5>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e) => Sequence(a, b, c, d, e, (_, _, _, _, x) => x);
    public static Func<ReadOnlySpan<char>, (int, T6)?> Sequence<T1, T2, T3, T4, T5, T6>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e, Func<ReadOnlySpan<char>, (int, T6)?> f) => Sequence(a, b, c, d, e, f, (_, _, _, _, _, x) => x);
    public static Func<ReadOnlySpan<char>, (int, T7)?> Sequence<T1, T2, T3, T4, T5, T6, T7>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e, Func<ReadOnlySpan<char>, (int, T6)?> f, Func<ReadOnlySpan<char>, (int, T7)?> g) => Sequence(a, b, c, d, e, f, g, (_, _, _, _, _, _, x) => x);
    public static Func<ReadOnlySpan<char>, (int, T8)?> Sequence<T1, T2, T3, T4, T5, T6, T7, T8>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e, Func<ReadOnlySpan<char>, (int, T6)?> f, Func<ReadOnlySpan<char>, (int, T7)?> g, Func<ReadOnlySpan<char>, (int, T8)?> h) => Sequence(a, b, c, d, e, f, g, h, (_, _, _, _, _, _, _, x) => x);
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T1, T2, R>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<T1, T2, R> match) => input =>
    {
        var r1 = a(input);
        if (r1 is null) return null;
        var r2_start = r1.Value.Item1;
        var r2 = b(input[r2_start..]);

        return r2 is { } ? (r2_start + r2.Value.Item1, match(r1.Value.Item2, r2.Value.Item2)) : null;
    };
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T1, T2, T3, R>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<T1, T2, T3, R> match) => input =>
    {
        var r1 = a(input);
        if (r1 is null) return null;
        var r2_start = r1.Value.Item1;
        var r2 = b(input[r2_start..]);
        if (r2 is null) return null;
        var r3_start = r2_start + r2.Value.Item1;
        var r3 = c(input[r3_start..]);

        return r3 is { } ? (r3_start + r3.Value.Item1, match(r1.Value.Item2, r2.Value.Item2, r3.Value.Item2)) : null;
    };
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T1, T2, T3, T4, R>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<T1, T2, T3, T4, R> match) => input =>
    {
        var r1 = a(input);
        if (r1 is null) return null;
        var r2_start = r1.Value.Item1;
        var r2 = b(input[r2_start..]);
        if (r2 is null) return null;
        var r3_start = r2_start + r2.Value.Item1;
        var r3 = c(input[r3_start..]);
        if (r3 is null) return null;
        var r4_start = r3_start + r3.Value.Item1;
        var r4 = d(input[r4_start..]);

        return r4 is { } ? (r4_start + r4.Value.Item1, match(r1.Value.Item2, r2.Value.Item2, r3.Value.Item2, r4.Value.Item2)) : null;
    };
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T1, T2, T3, T4, T5, R>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e, Func<T1, T2, T3, T4, T5, R> match) => input =>
    {
        var r1 = a(input);
        if (r1 is null) return null;
        var r2_start = r1.Value.Item1;
        var r2 = b(input[r2_start..]);
        if (r2 is null) return null;
        var r3_start = r2_start + r2.Value.Item1;
        var r3 = c(input[r3_start..]);
        if (r3 is null) return null;
        var r4_start = r3_start + r3.Value.Item1;
        var r4 = d(input[r4_start..]);
        if (r4 is null) return null;
        var r5_start = r4_start + r4.Value.Item1;
        var r5 = e(input[r5_start..]);

        return r5 is { } ? (r5_start + r5.Value.Item1, match(r1.Value.Item2, r2.Value.Item2, r3.Value.Item2, r4.Value.Item2, r5.Value.Item2)) : null;
    };
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T1, T2, T3, T4, T5, T6, R>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e, Func<ReadOnlySpan<char>, (int, T6)?> f, Func<T1, T2, T3, T4, T5, T6, R> match) => input =>
    {
        var r1 = a(input);
        if (r1 is null) return null;
        var r2_start = r1.Value.Item1;
        var r2 = b(input[r2_start..]);
        if (r2 is null) return null;
        var r3_start = r2_start + r2.Value.Item1;
        var r3 = c(input[r3_start..]);
        if (r3 is null) return null;
        var r4_start = r3_start + r3.Value.Item1;
        var r4 = d(input[r4_start..]);
        if (r4 is null) return null;
        var r5_start = r4_start + r4.Value.Item1;
        var r5 = e(input[r5_start..]);
        if (r5 is null) return null;
        var r6_start = r5_start + r5.Value.Item1;
        var r6 = f(input[r6_start..]);

        return r6 is { } ? (r6_start + r6.Value.Item1, match(r1.Value.Item2, r2.Value.Item2, r3.Value.Item2, r4.Value.Item2, r5.Value.Item2, r6.Value.Item2)) : null;
    };
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T1, T2, T3, T4, T5, T6, T7, R>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e, Func<ReadOnlySpan<char>, (int, T6)?> f, Func<ReadOnlySpan<char>, (int, T7)?> g, Func<T1, T2, T3, T4, T5, T6, T7, R> match) => input =>
    {
        var r1 = a(input);
        if (r1 is null) return null;
        var r2_start = +r1.Value.Item1;
        var r2 = b(input[r2_start..]);
        if (r2 is null) return null;
        var r3_start = r2_start + r2.Value.Item1;
        var r3 = c(input[r3_start..]);
        if (r3 is null) return null;
        var r4_start = r3_start + r3.Value.Item1;
        var r4 = d(input[r4_start..]);
        if (r4 is null) return null;
        var r5_start = r4_start + r4.Value.Item1;
        var r5 = e(input[r5_start..]);
        if (r5 is null) return null;
        var r6_start = r5_start + r5.Value.Item1;
        var r6 = f(input[r6_start..]);
        if (r6 is null) return null;
        var r7_start = r6_start + r6.Value.Item1;
        var r7 = g(input[r7_start..]);

        return r7 is { } ? (r7_start + r7.Value.Item1, match(r1.Value.Item2, r2.Value.Item2, r3.Value.Item2, r4.Value.Item2, r5.Value.Item2, r6.Value.Item2, r7.Value.Item2)) : null;
    };
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<ReadOnlySpan<char>, (int, T1)?> a, Func<ReadOnlySpan<char>, (int, T2)?> b, Func<ReadOnlySpan<char>, (int, T3)?> c, Func<ReadOnlySpan<char>, (int, T4)?> d, Func<ReadOnlySpan<char>, (int, T5)?> e, Func<ReadOnlySpan<char>, (int, T6)?> f, Func<ReadOnlySpan<char>, (int, T7)?> g, Func<ReadOnlySpan<char>, (int, T8)?> h, Func<T1, T2, T3, T4, T5, T6, T7, T8, R> match) => input =>
    {
        var r1 = a(input);
        if (r1 is null) return null;
        var r2_start = r1.Value.Item1;
        var r2 = b(input[r2_start..]);
        if (r2 is null) return null;
        var r3_start = r2_start + r2.Value.Item1;
        var r3 = c(input[r3_start..]);
        if (r3 is null) return null;
        var r4_start = r3_start + r3.Value.Item1;
        var r4 = d(input[r4_start..]);
        if (r4 is null) return null;
        var r5_start = r4_start + r4.Value.Item1;
        var r5 = e(input[r5_start..]);
        if (r5 is null) return null;
        var r6_start = r5_start + r5.Value.Item1;
        var r6 = f(input[r6_start..]);
        if (r6 is null) return null;
        var r7_start = r6_start + r6.Value.Item1;
        var r7 = g(input[r7_start..]);
        if (r7 is null) return null;
        var r8_start = r7_start + r7.Value.Item1;
        var r8 = h(input[r8_start..]);

        return r8 is { } ? (r8_start + r8.Value.Item1, match(r1.Value.Item2, r2.Value.Item2, r3.Value.Item2, r4.Value.Item2, r5.Value.Item2, r6.Value.Item2, r7.Value.Item2, r8.Value.Item2)) : null;
    };
    public static Func<ReadOnlySpan<char>, (int, T?)?> Sequence<T>(params Func<ReadOnlySpan<char>, (int, T)?>[] sequence) => Sequence(sequence, xs => xs.LastOrDefault());
    public static Func<ReadOnlySpan<char>, (int, R)?> Sequence<T, R>(Func<ReadOnlySpan<char>, (int, T)?>[] sequence, Func<IReadOnlyList<T>, R> match) => input =>
    {
        var length = 0;
        var values = new T[sequence.Length];
        for (var i = 0; i < sequence.Length; i++)
        {
            var result = sequence[i](input[length..]);
            if (result is null) return null;
            length += result.Value.Item1;
            values[i] = result.Value.Item2;
        }
        return (length, match(values));
    };

    public static Func<ReadOnlySpan<char>, (int, T)?> Choice<T>(Func<ReadOnlySpan<char>, (int, T)?> left, Func<ReadOnlySpan<char>, (int, T)?> right) => input => left(input) ?? right(input);
    public static Func<ReadOnlySpan<char>, (int, T)?> Choice<T>(params Func<ReadOnlySpan<char>, (int, T)?>[] choice) => input =>
    {
        foreach (var c in choice)
        {
            var result = c(input);
            if (result is { }) return result;
        }
        return null;
    };

    public static Func<ReadOnlySpan<char>, (int, T)?> ZeroWidth<T>(Func<ReadOnlySpan<char>, (bool, T)> f) => input => f(input) is { } p && p.Item1 ? (0, p.Item2) : null;

    public static readonly Func<ReadOnlySpan<char>, (int, char)?> Digit = Char(char.IsAsciiDigit);
    public static readonly Func<ReadOnlySpan<char>, (int, char)?> HexDigit = Char(char.IsAsciiHexDigit);
    public static readonly Func<ReadOnlySpan<char>, (int, char)?> Letter = Char(char.IsAsciiLetter);
    public static readonly Func<ReadOnlySpan<char>, (int, char)?> Word = Char(c => char.IsAsciiLetterOrDigit(c) || c == '_');
    public static readonly Func<ReadOnlySpan<char>, (int, char)?> Space = Char(c => c is ' ' or '\t' or '\v' or '\n' or '\r' or '\f');

    public static readonly Func<ReadOnlySpan<char>, (int, string)?> Digits = Chars(Digit);
    public static readonly Func<ReadOnlySpan<char>, (int, string)?> HexDigits = Chars(HexDigit);
    public static readonly Func<ReadOnlySpan<char>, (int, string)?> Letters = Chars(Letter);
    public static readonly Func<ReadOnlySpan<char>, (int, string)?> Words = Chars(Word);
    public static readonly Func<ReadOnlySpan<char>, (int, string)?> Spaces = Chars(Space);

    public static readonly Func<ReadOnlySpan<char>, (int, char)?> AnyChar = Char(_ => true);
    public static readonly Func<ReadOnlySpan<char>, (int, char)?> Cr = Char('\r');
    public static readonly Func<ReadOnlySpan<char>, (int, char)?> Lf = Char('\n');
    public static readonly Func<ReadOnlySpan<char>, (int, string)?> CrLf = Cr & Lf;
    public static readonly Func<ReadOnlySpan<char>, (int, string)?> NewLine = String(Lf) | CrLf | String(Cr);
    public static readonly Func<ReadOnlySpan<char>, (int, string)?> LineEnd = input => input.Length == 0 || input[0] is '\n' or '\r' ? (0, "") : null;

    public static readonly Func<ReadOnlySpan<char>, (int, string)?> End = input => input.Length == 0 ? (0, "") : null;
    public static Func<ReadOnlySpan<char>, (int, T)?> Error<T>() => input => null;

    public static readonly Func<ReadOnlySpan<char>, (int, int)?> NaturalNumberInt32 = NaturalNumber<int>(Digit, (v, c, _) => v * 10 + (c - '0'));
    public static readonly Func<ReadOnlySpan<char>, (int, long)?> NaturalNumberInt64 = NaturalNumber<long>(Digit, (v, c, _) => v * 10 + (c - '0'));
    public static readonly Func<ReadOnlySpan<char>, (int, decimal)?> NaturalNumberDecimal = NaturalNumber<decimal>(Digit, (v, c, _) => v * 10 + (c - '0'));
    public static readonly Func<ReadOnlySpan<char>, (int, float)?> NaturalNumberSingle = NaturalNumber<float>(Digit, (v, c, _) => v * 10 + (c - '0'));
    public static readonly Func<ReadOnlySpan<char>, (int, float)?> NaturalNumberFloat = NaturalNumberSingle;
    public static readonly Func<ReadOnlySpan<char>, (int, double)?> NaturalNumberDouble = NaturalNumber<double>(Digit, (v, c, _) => v * 10 + (c - '0'));
    public static readonly Func<ReadOnlySpan<char>, (int, float)?> RealNumberSingle = RealNumber<float>(Digit, (v, c, _) => v * 10 + (c - '0'), (v, c, i) => (float)(v + (c - '0') / Math.Pow(10, i)));
    public static readonly Func<ReadOnlySpan<char>, (int, float)?> RealNumberFloat = RealNumberSingle;
    public static readonly Func<ReadOnlySpan<char>, (int, double)?> RealNumberDouble = RealNumber<double>(Digit, (v, c, _) => v * 10 + (c - '0'), (v, c, i) => (v + (c - '0') / Math.Pow(10, i)));
    public static Func<ReadOnlySpan<char>, (int, T)?> NaturalNumber<T>(Func<ReadOnlySpan<char>, (int, char)?> c, Func<T, char, int, T> match) => input =>
    {
        var first = c(input);
        if (first is null) return null;
        var length = first.Value.Item1;
        var i = 1;
        T value = match(default!, first.Value.Item2, i++);
        while (true)
        {
            var result = c(input[length..]);
            if (result is null) break;
            length += result.Value.Item1;
            value = match(value, result.Value.Item2, i++);
        }
        return (length, value);
    };
    public static Func<ReadOnlySpan<char>, (int, T)?> RealNumber<T>(Func<ReadOnlySpan<char>, (int, char)?> c, Func<T, char, int, T> int_match, Func<T, char, int, T> dec_match) where T : IAdditionOperators<T, T, T> => input =>
    {
        var integer = NaturalNumber(c, int_match)(input);
        if (integer is null) return null;
        var point = Char('.')(input[integer.Value.Item1..]);
        if (point is null) return integer;
        var fractional = NaturalNumber(c, dec_match)(input[(integer.Value.Item1 + point.Value.Item1)..]);
        if (fractional is null) return (integer.Value.Item1 + point.Value.Item1, integer.Value.Item2);
        return (integer.Value.Item1 + point.Value.Item1 + fractional.Value.Item1, integer.Value.Item2 + fractional.Value.Item2);
    };

    public static Func<ReadOnlySpan<char>, (int, T)?> Lazy<T>(Func<Func<ReadOnlySpan<char>, (int, T)?>> f) => input => f()(input);
    public static Func<ReadOnlySpan<char>, (int, string)?> Add(Func<ReadOnlySpan<char>, (int, char)?> a, Func<ReadOnlySpan<char>, (int, char)?> b) => Sequence(a, b, (xa, xb) => $"{xa}{xb}");
    public static Func<ReadOnlySpan<char>, (int, string)?> Add(Func<ReadOnlySpan<char>, (int, char)?> a, Func<ReadOnlySpan<char>, (int, string)?> b) => Add(Once(a, x => x.ToString()), b);
    public static Func<ReadOnlySpan<char>, (int, string)?> Add(Func<ReadOnlySpan<char>, (int, string)?> a, Func<ReadOnlySpan<char>, (int, char)?> b) => Add(a, Once(b, x => x.ToString()));
    public static Func<ReadOnlySpan<char>, (int, string)?> Add(Func<ReadOnlySpan<char>, (int, string)?> a, Func<ReadOnlySpan<char>, (int, string)?> b) => Sequence(a, b, (xa, xb) => xa + xb);

    extension(Func<ReadOnlySpan<char>, (int, char)?> self)
    {
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, char)?> a, Func<ReadOnlySpan<char>, (int, char)?> b) => Add(a, b);
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, char)?> a, Func<ReadOnlySpan<char>, (int, string)?> b) => Add(a, b);
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, char)?> a, char b) => Add(a, Char(b));
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, char)?> a, string b) => Add(a, String(b));
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, char)?> a, char[] b) => Add(a, CharClass(b));
        public Func<ReadOnlySpan<char>, (int, R)?> ToNone<R>() => None<char, R>(self);
        public Func<ReadOnlySpan<char>, (int, string)?> ToOnceString() => Once(self, x => x.ToString());
    }
    extension(Func<ReadOnlySpan<char>, (int, string)?> self)
    {
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, string)?> a, Func<ReadOnlySpan<char>, (int, char)?> b) => Add(a, b);
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, string)?> a, Func<ReadOnlySpan<char>, (int, string)?> b) => Add(a, b);
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, string)?> a, char b) => Add(a, Char(b));
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, string)?> a, string b) => Add(a, String(b));
        public static Func<ReadOnlySpan<char>, (int, string)?> operator &(Func<ReadOnlySpan<char>, (int, string)?> a, char[] b) => Add(a, CharClass(b));
    }
    extension(Func<ReadOnlySpan<char>, (int, char[])?> self)
    {
        public Func<ReadOnlySpan<char>, (int, string)?> ToOnceString() => Once(self, xs => string.Join("", xs));
    }
    extension(Func<ReadOnlySpan<char>, (int, string[])?> self)
    {
        public Func<ReadOnlySpan<char>, (int, string)?> ToOnceString() => Once(self, xs => string.Join("", xs));
    }
    extension(Func<ReadOnlySpan<char>, (int, string?)?> self)
    {
        public Func<ReadOnlySpan<char>, (int, string)?> ToOnceString() => Once(self, x => x ?? "");
    }
    extension<T>(Func<ReadOnlySpan<char>, (int, T)?> self)
    {
        public static Func<ReadOnlySpan<char>, (int, T)?> operator ^(Func<ReadOnlySpan<char>, (int, T)?> a, Func<ReadOnlySpan<char>, (int, T)?> b) => Sequence(a, b);
        public static Func<ReadOnlySpan<char>, (int, T)?> operator |(Func<ReadOnlySpan<char>, (int, T)?> a, Func<ReadOnlySpan<char>, (int, T)?> b) => Choice(a, b);
        public Func<ReadOnlySpan<char>, (int, R)?> ToOnce<R>(Func<T, R> match) => Once(self, match);
        public Func<ReadOnlySpan<char>, (int, R)?> ToNone<R>() => None<T, R>(self);
        public Func<ReadOnlySpan<char>, (int, T?)?> ToOption() => Option(self);
        public Func<ReadOnlySpan<char>, (int, T[])?> ToMany() => Many(self, xs => xs.ToArray());
        public Func<ReadOnlySpan<char>, (int, T[])?> ToMany1() => Many1(self, xs => xs.ToArray());
        public Func<ReadOnlySpan<char>, (int, T[])?> ToMany(uint min, uint max) => Many(self, min, max, xs => xs.ToArray());
        public T Parse(string s, int start = 0) => self.Parse(s.AsSpan()[start..]);
        public T Parse(ReadOnlySpan<char> s) => self(s) is { } p ? p.Item2 : throw new("Parser execution failed.");
        public bool TryParse(string s, [MaybeNullWhen(false)] out T result) => self.TryParse(s, 0, out result!);
        public bool TryParse(string s, int start, [MaybeNullWhen(false)] out T result) => self.TryParse(s.AsSpan()[start..], out result!);
        public bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out T result)
        {
            if (self(s) is { } p)
            {
                result = p.Item2;
                return true;
            }
            result = default!;
            return false;
        }
        public bool IsMatch(string s, int start = 0) => self.Match(s, start) is { };
        public (int Start, int Length, T Value)? Match(string s, int start = 0) => self.Match(s.AsSpan(), start);
        public (int Start, int Length, T Value)? Match(ReadOnlySpan<char> s, int start = 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(start);

            for (var i = start; i <= s.Length; i++)
            {
                if (self(s[i..]) is { } result) return (i, result.Item1, result.Item2);
            }
            return null;
        }
    }
}
