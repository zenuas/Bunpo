using System;
using System.Linq;

namespace Bunpo;

public static class Combinator
{
    public static Func<string, int, int?> Char(char c) => (input, start) => input.Length <= start || input[start] != c ? null : 1;
    public static Func<string, int, int?> Char(Func<char, bool> f) => (input, start) => input.Length <= start || !f(input[start]) ? null : 1;
    public static Func<string, int, int?> CharClass(params char[] chars) => (input, start) => input.Length <= start || !chars.Contains(input[start]) ? null : 1;
    public static Func<string, int, int?> CharClass(string chars) => CharClass(chars.ToCharArray());
    public static Func<string, int, int?> String(string s) => (input, start) => input.Length < start + s.Length || !input.StartsWith(s, StringComparison.Ordinal) ? null : s.Length;
    public static Func<string, int, int?> String(Func<string, int, int?> f) => (input, start) => f(input, start);

    public static Func<string, int, int?> Option(Func<string, int, int?> once) => (input, start) => once(input, start) ?? 0;

    public static Func<string, int, int?> Many(Func<string, int, int?> many) => (input, start) =>
    {
        var length = 0;
        while (true)
        {
            var result = many(input, start + length);
            if (result is null) break;
            length += result.Value;
        }
        return length;
    };

    public static Func<string, int, int?> Many1(Func<string, int, int?> many) => (input, start) =>
    {
        var first = many(input, start);
        if (first is null) return null;

        var length = first.Value;
        while (true)
        {
            var result = many(input, start + length);
            if (result is null) break;
            length += result.Value;
        }
        return length;
    };

    public static Func<string, int, int?> Many(Func<string, int, int?> many, uint min, uint max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);
        return (input, start) =>
        {
            var length = 0;
            var i = 0;
            for (; i < min; i++)
            {
                var result = many(input, start + length);
                if (result is null) return null;
                length += result.Value;
            }

            for (; i < max; i++)
            {
                var result = many(input, start + length);
                if (result is null) break;
                length += result.Value;
            }
            return length;
        };
    }

    public static Func<string, int, int?> Capture(Func<string, int, int?> capture, Action<string, int, int> match) => (input, start) =>
    {
        var result = capture(input, start);
        if (result is null) return null;
        match(input, start, result.Value);
        return result;
    };

    public static Func<string, int, int?> Sequence(params Func<string, int, int?>[] sequence) => (input, start) =>
    {
        var length = 0;
        foreach (var s in sequence)
        {
            var result = s(input, start + length);
            if (result is null) return null;
            length += result.Value;
        }
        return length;
    };

    public static Func<string, int, int?> Choice(Func<string, int, int?> left, Func<string, int, int?> right) => (input, start) => left(input, start) ?? right(input, start);
    public static Func<string, int, int?> Choice(Func<string, int, int?>[] choice) => (input, start) =>
    {
        foreach (var c in choice)
        {
            var result = c(input, start);
            if (result is { }) return result;
        }
        return null;
    };

    public static Func<string, int, int?> ZeroWidth(Func<string, int, bool> f) => (input, start) => f(input, start) ? 0 : null;

    public static readonly Func<string, int, int?> Digit = Char(char.IsAsciiDigit);
    public static readonly Func<string, int, int?> HexDigit = Char(char.IsAsciiHexDigit);
    public static readonly Func<string, int, int?> Letter = Char(char.IsAsciiLetter);
    public static readonly Func<string, int, int?> Word = Char(c => char.IsAsciiLetterOrDigit(c) || c == '_');
    public static readonly Func<string, int, int?> AnyChar = Char(_ => true);
    public static readonly Func<string, int, int?> Cr = Char('\r');
    public static readonly Func<string, int, int?> Lf = Char('\n');
    public static readonly Func<string, int, int?> CrLf = Cr + Lf;
    public static readonly Func<string, int, int?> NewLine = Lf / CrLf / Cr;
    public static readonly Func<string, int, int?> LineStart = (input, start) => start <= 0 || input[start - 1] is '\n' or '\r' ? 0 : null;
    public static readonly Func<string, int, int?> LineEnd = (input, start) => input.Length <= start || input[start] is '\n' or '\r' ? 0 : null;
    public static readonly Func<string, int, int?> Space = Char(c => c is ' ' or '\t' or '\v' or '\n' or '\r' or '\f');

    public static readonly Func<string, int, int?> Start = (input, start) => start == 0 ? 0 : null;
    public static readonly Func<string, int, int?> End = (input, start) => input.Length <= start ? 0 : null;
    public static readonly Func<string, int, int?> Error = (input, start) => null;

    public static readonly Func<string, int, int?> WordBoundary = (input, start) =>
        (start == 0 && input.Length > 0 && (char.IsAsciiLetterOrDigit(input[0]) || input[0] == '_')) ||
        (input.Length <= start + 1 && (char.IsAsciiLetterOrDigit(input[^1]) || input[^1] == '_')) ||
        (start > 0 && input.Length > start + 1 && (char.IsAsciiLetterOrDigit(input[start]) || input[start] == '_') != (char.IsAsciiLetterOrDigit(input[start + 1]) || input[start + 1] == '_')) ? 0 : null;

    public static readonly Func<string, int, int?> NonWordBoundary = (input, start) =>
        (start == 0 && input.Length > 0 && !(char.IsAsciiLetterOrDigit(input[0]) || input[0] == '_')) ||
        (input.Length <= start + 1 && !(char.IsAsciiLetterOrDigit(input[^1]) || input[^1] == '_')) ||
        (start > 0 && input.Length > start + 1 && (char.IsAsciiLetterOrDigit(input[start]) || input[start] == '_') == (char.IsAsciiLetterOrDigit(input[start + 1]) || input[start + 1] == '_')) ? 0 : null;

    extension(Func<string, int, int?> self)
    {
        public static Func<string, int, int?> operator +(Func<string, int, int?> a, Func<string, int, int?> b) => Sequence(a, b);
        public static Func<string, int, int?> operator /(Func<string, int, int?> a, Func<string, int, int?> b) => Choice(a, b);
        public Func<string, int, int?> ToOption() => Option(self);
        public Func<string, int, int?> ToMany() => Many(self);
        public Func<string, int, int?> ToMany1() => Many1(self);
        public Func<string, int, int?> ToMany(uint min, uint max) => Many(self, min, max);
        public bool IsMatch(string s, int start) => self(s, start) is { };
        public int? Match(string s, int start) => self(s, start);
        public bool IsMatch(string s) => self.Match(s) is { };
        public (int Start, int Length)? Match(string s)
        {
            for (var i = 0; i <= s.Length; i++)
            {
                if (self(s, i) is { } result) return (i, result);
            }
            return null;
        }
    }
}
