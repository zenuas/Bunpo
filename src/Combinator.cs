using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bunpo;

public static class Combinator
{
    public static Func<string, int, (int, char)?> Char(char c) => (input, start) => input.Length <= start || start < 0 || input[start] != c ? null : (1, c);
    public static Func<string, int, (int, char)?> Char(Func<char, bool> f) => (input, start) => input.Length <= start || start < 0 || !f(input[start]) ? null : (1, input[start]);
    public static Func<string, int, (int, string)?> Chars(Func<string, int, (int, char)?> c) => Many1(c, xs => string.Join("", xs));
    public static Func<string, int, (int, char)?> CharClass(params char[] chars) => (input, start) => input.Length <= start || start < 0 || !chars.Contains(input[start]) ? null : (1, input[start]);
    public static Func<string, int, (int, char)?> CharClass(string chars) => CharClass(chars.ToCharArray());
    public static Func<string, int, (int, string)?> String(char c) => Once(Char(c), _ => c.ToString());
    public static Func<string, int, (int, string)?> String(string s) => (input, start) => input.Length < start + s.Length || start < 0 || !input[start..].StartsWith(s, StringComparison.Ordinal) ? null : (s.Length, s);
    public static Func<string, int, (int, string)?> String(Func<string, int, (int, char)?> c) => Once(c, x => x.ToString());
    public static Func<string, int, (int, T)?> String<T>(Func<string, int, (int, T)?> f) => (input, start) => f(input, start);

    public static Func<string, int, (int, R)?> Once<T, R>(Func<string, int, (int, T)?> once, Func<T, R> match) => (input, start) => once(input, start) is { } p ? (p.Item1, match(p.Item2)) : null;
    public static Func<string, int, (int, R)?> None<T, R>(Func<string, int, (int, T)?> f) => Once<T, R>(f, _ => default!);
    public static Func<string, int, (int, T?)?> Option<T>(Func<string, int, (int, T)?> once) => (input, start) => input.Length < start || start < 0 ? null : once(input, start) is { } p ? p : (0, default);

    public static Func<string, int, (int, T?)?> Many<T>(Func<string, int, (int, T)?> many) => Many(many, static xs => xs.LastOrDefault());
    public static Func<string, int, (int, R)?> Many<T, R>(Func<string, int, (int, T)?> many, Func<IReadOnlyList<T>, R> match) => (input, start) =>
    {
        if (start < 0 || start > input.Length) return null;
        var length = 0;
        var values = new List<T>();
        while (true)
        {
            var result = many(input, start + length);
            if (result is null) break;
            length += result.Value.Item1;
            values.Add(result.Value.Item2);
        }
        return (length, match(values));
    };

    public static Func<string, int, (int, T)?> Many1<T>(Func<string, int, (int, T)?> many) => Many1(many, static xs => xs.Last());
    public static Func<string, int, (int, R)?> Many1<T, R>(Func<string, int, (int, T)?> many, Func<IReadOnlyList<T>, R> match) => (input, start) =>
    {
        if (start < 0 || start > input.Length) return null;
        var first = many(input, start);
        if (first is null) return null;

        var length = first.Value.Item1;
        var values = new List<T>() { first.Value.Item2 };
        while (true)
        {
            var result = many(input, start + length);
            if (result is null) break;
            length += result.Value.Item1;
            values.Add(result.Value.Item2);
        }
        return (length, match(values));
    };

    public static Func<string, int, (int, T?)?> Many<T>(Func<string, int, (int, T)?> many, uint min, uint max) => Many(many, min, max, static xs => xs.LastOrDefault());
    public static Func<string, int, (int, R)?> Many<T, R>(Func<string, int, (int, T)?> many, uint min, uint max, Func<IReadOnlyList<T>, R> match)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);
        return (input, start) =>
        {
            if (start < 0 || start > input.Length) return null;
            var length = 0;
            var values = new List<T>();
            var i = 0;
            for (; i < min; i++)
            {
                var result = many(input, start + length);
                if (result is null) return null;
                length += result.Value.Item1;
                values.Add(result.Value.Item2);
            }

            for (; i < max; i++)
            {
                var result = many(input, start + length);
                if (result is null) break;
                length += result.Value.Item1;
                values.Add(result.Value.Item2);
            }
            return (length, match(values));
        };
    }

    public static Func<string, int, (int, T)?> ChainLeft<T, TOpe>(Func<string, int, (int, T)?> chain, Func<string, int, (int, TOpe)?> op, Func<T, TOpe, T, T> match) => (input, start) =>
    {
        if (start < 0 || start > input.Length) return null;
        var first = chain(input, start);
        if (first is null) return null;

        var length = first.Value.Item1;
        var left = first.Value.Item2;
        while (true)
        {
            var opr = op(input, start + length);
            if (opr is null) break;
            length += opr.Value.Item1;
            var right = chain(input, start + length);
            if (right is null) break;
            length += right.Value.Item1;
            left = match(left, opr.Value.Item2, right.Value.Item2);
        }
        return (length, left);
    };

    public static Func<string, int, (int, T2)?> Sequence<T1, T2>(Func<string, int, (int, T1)?> a, Func<string, int, (int, T2)?> b) => Sequence(a, b, static (_, xb) => xb);
    public static Func<string, int, (int, R)?> Sequence<T1, T2, R>(Func<string, int, (int, T1)?> a, Func<string, int, (int, T2)?> b, Func<T1, T2, R> match) => (input, start) =>
    {
        if (start < 0 || start > input.Length) return null;
        var r1 = a(input, start);
        if (r1 is null) return null;
        var next = start + r1.Value.Item1;
        if (next > input.Length) return null;
        var r2 = b(input, next);
        return r2 is { } ? (r1.Value.Item1 + r2.Value.Item1, match(r1.Value.Item2, r2.Value.Item2)) : null;
    };
    public static Func<string, int, (int, T?)?> Sequence<T>(params Func<string, int, (int, T)?>[] sequence) => Sequence(sequence, static xs => xs.LastOrDefault());
    public static Func<string, int, (int, R)?> Sequence<T, R>(Func<string, int, (int, T)?>[] sequence, Func<IReadOnlyList<T>, R> match) => (input, start) =>
    {
        if (start < 0 || start > input.Length) return null;
        var length = 0;
        var values = new List<T>();
        foreach (var s in sequence)
        {
            var result = s(input, start + length);
            if (result is null) return null;
            length += result.Value.Item1;
            values.Add(result.Value.Item2);
        }
        return (length, match(values));
    };

    public static Func<string, int, (int, T)?> Choice<T>(Func<string, int, (int, T)?> left, Func<string, int, (int, T)?> right) => (input, start) => left(input, start) ?? right(input, start);
    public static Func<string, int, (int, T)?> Choice<T>(params Func<string, int, (int, T)?>[] choice) => (input, start) =>
    {
        if (start < 0 || start > input.Length) return null;
        foreach (var c in choice)
        {
            var result = c(input, start);
            if (result is { }) return result;
        }
        return null;
    };

    public static Func<string, int, (int, T)?> ZeroWidth<T>(Func<string, int, (bool, T)> f) => (input, start) => start >= 0 && start <= input.Length && f(input, start) is { } p && p.Item1 ? (0, p.Item2) : null;

    public static readonly Func<string, int, (int, char)?> Digit = Char(char.IsAsciiDigit);
    public static readonly Func<string, int, (int, char)?> HexDigit = Char(char.IsAsciiHexDigit);
    public static readonly Func<string, int, (int, char)?> Letter = Char(char.IsAsciiLetter);
    public static readonly Func<string, int, (int, char)?> Word = Char(c => char.IsAsciiLetterOrDigit(c) || c == '_');
    public static readonly Func<string, int, (int, char)?> Space = Char(c => c is ' ' or '\t' or '\v' or '\n' or '\r' or '\f');

    public static readonly Func<string, int, (int, string)?> Digits = Chars(Digit);
    public static readonly Func<string, int, (int, string)?> HexDigits = Chars(HexDigit);
    public static readonly Func<string, int, (int, string)?> Letters = Chars(Letter);
    public static readonly Func<string, int, (int, string)?> Words = Chars(Word);
    public static readonly Func<string, int, (int, string)?> Spaces = Chars(Space);

    public static readonly Func<string, int, (int, char)?> AnyChar = Char(_ => true);
    public static readonly Func<string, int, (int, char)?> Cr = Char('\r');
    public static readonly Func<string, int, (int, char)?> Lf = Char('\n');
    public static readonly Func<string, int, (int, string)?> CrLf = Cr & Lf;
    public static readonly Func<string, int, (int, string)?> NewLine = String(Lf) | CrLf | String(Cr);
    public static readonly Func<string, int, (int, string)?> LineStart = (input, start) => start == 0 || (start > 0 && (input[start - 1] == '\n' || (input[start - 1] == '\r' && (input.Length == start || (input.Length > start && input[start] != '\n'))))) ? (0, "") : null;
    public static readonly Func<string, int, (int, string)?> LineEnd = (input, start) => start >= 0 && (input.Length <= start || input[start] is '\n' or '\r') ? (0, "") : null;

    public static readonly Func<string, int, (int, string)?> Start = (input, start) => start == 0 ? (0, "") : null;
    public static readonly Func<string, int, (int, string)?> End = (input, start) => input.Length <= start ? (0, "") : null;
    public static readonly Func<string, int, (int, string)?> Error = (input, start) => null;

    public static readonly Func<string, int, (int, string)?> WordBoundary = (input, start) =>
        (start == 0 && input.Length > 0 && (char.IsAsciiLetterOrDigit(input[0]) || input[0] == '_')) ||
        (input.Length == start && input.Length > 0 && (char.IsAsciiLetterOrDigit(input[^1]) || input[^1] == '_')) ||
        (start > 0 && input.Length > start && (char.IsAsciiLetterOrDigit(input[start]) || input[start] == '_') != (char.IsAsciiLetterOrDigit(input[start - 1]) || input[start - 1] == '_')) ? (0, "") : null;

    public static readonly Func<string, int, (int, string)?> NonWordBoundary = (input, start) =>
        (start == 0 && input.Length > 0 && !(char.IsAsciiLetterOrDigit(input[0]) || input[0] == '_')) ||
        (input.Length == start && input.Length > 0 && !(char.IsAsciiLetterOrDigit(input[^1]) || input[^1] == '_')) ||
        (start > 0 && input.Length > start && (char.IsAsciiLetterOrDigit(input[start]) || input[start] == '_') == (char.IsAsciiLetterOrDigit(input[start - 1]) || input[start - 1] == '_')) ? (0, "") : null;

    public static Func<string, int, (int, T)?> Lazy<T>(Func<Func<string, int, (int, T)?>> f) => (input, start) => f()(input, start);
    public static Func<string, int, (int, string)?> Add(Func<string, int, (int, char)?> a, Func<string, int, (int, char)?> b) => Sequence(a, b, static (xa, xb) => $"{xa}{xb}");
    public static Func<string, int, (int, string)?> Add(Func<string, int, (int, char)?> a, Func<string, int, (int, string)?> b) => Add(Once(a, x => x.ToString()), b);
    public static Func<string, int, (int, string)?> Add(Func<string, int, (int, string)?> a, Func<string, int, (int, char)?> b) => Add(a, Once(b, x => x.ToString()));
    public static Func<string, int, (int, string)?> Add(Func<string, int, (int, string)?> a, Func<string, int, (int, string)?> b) => Sequence(a, b, static (xa, xb) => xa + xb);

    extension(Func<string, int, (int, char)?> self)
    {
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, char)?> a, Func<string, int, (int, char)?> b) => Add(a, b);
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, char)?> a, Func<string, int, (int, string)?> b) => Add(a, b);
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, char)?> a, char b) => Add(a, Char(b));
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, char)?> a, string b) => Add(a, String(b));
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, char)?> a, char[] b) => Add(a, CharClass(b));
        public Func<string, int, (int, R)?> ToNone<R>() => None<char, R>(self);
    }
    extension(Func<string, int, (int, string)?> self)
    {
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, string)?> a, Func<string, int, (int, char)?> b) => Add(a, b);
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, string)?> a, Func<string, int, (int, string)?> b) => Add(a, b);
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, string)?> a, char b) => Add(a, Char(b));
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, string)?> a, string b) => Add(a, String(b));
        public static Func<string, int, (int, string)?> operator &(Func<string, int, (int, string)?> a, char[] b) => Add(a, CharClass(b));
    }
    extension<T>(Func<string, int, (int, T)?> self)
    {
        public static Func<string, int, (int, T)?> operator ^(Func<string, int, (int, T)?> a, Func<string, int, (int, T)?> b) => Sequence([a, b], static xs => xs.Last());
        public static Func<string, int, (int, T)?> operator |(Func<string, int, (int, T)?> a, Func<string, int, (int, T)?> b) => Choice(a, b);
        public Func<string, int, (int, R)?> ToOnce<R>(Func<T, R> match) => Once(self, match);
        public Func<string, int, (int, R)?> ToNone<R>() => None<T, R>(self);
        public Func<string, int, (int, T?)?> ToOption() => Option(self);
        public Func<string, int, (int, T[])?> ToMany() => Many(self, xs => xs.ToArray());
        public Func<string, int, (int, T[])?> ToMany1() => Many1(self, xs => xs.ToArray());
        public Func<string, int, (int, T[])?> ToMany(uint min, uint max) => Many(self, min, max, xs => xs.ToArray());
        public T Parse(string s, int start = 0) => self(s, start) is { } p ? p.Item2 : throw new("Parser execution failed.");
        public bool TryParse(string s, out T result) => self.TryParse(s, 0, out result!);
        public bool TryParse(string s, int start, [MaybeNullWhen(false)] out T result)
        {
            if (self(s, start) is { } p)
            {
                result = p.Item2;
                return true;
            }
            result = default!;
            return false;
        }
        public bool IsMatch(string s, int start = 0) => self.Match(s, start) is { };
        public (int Start, int Length, T Value)? Match(string s, int start = 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(start);

            for (var i = start; i <= s.Length; i++)
            {
                if (self(s, i) is { } result) return (i, result.Item1, result.Item2);
            }
            return null;
        }
    }
}
