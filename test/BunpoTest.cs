using System;
using Xunit;

namespace Bunpo.Test;

public class BunpoTest
{
    [Fact]
    public void Error()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.Many(Combinator.AnyChar, 1, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.Match("", -1));
        Assert.Equal(Combinator.AnyChar.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.CharClass("a").Match("", -1));
        Assert.Equal(Combinator.CharClass("a").Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.String("a").Match("", -1));
        Assert.Equal(Combinator.String("a").Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToMany().Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToMany().Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToMany1().Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToMany1().Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToMany(0, 1).Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToMany(0, 1).Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToOption().Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToOption().Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.LineEnd.Match("", -1));
        Assert.Equal(Combinator.LineEnd.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.End.Match("", -1));
        Assert.Equal(Combinator.End.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.ZeroWidth(_ => (false, "")).Match("", -1));
        Assert.Equal(Combinator.ZeroWidth(_ => (false, "")).Match("", 1), null);
    }

    [Fact]
    public void CharTest()
    {
        Assert.Equal(Combinator.Char('a').Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.Char('x').Match("abc"), null);

        Assert.Equal(Combinator.AnyChar.Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.AnyChar.Match("ABC"), (0, 1, 'A'));
        Assert.Equal(Combinator.AnyChar.Match("123"), (0, 1, '1'));
        Assert.Equal(Combinator.AnyChar.Match("   "), (0, 1, ' '));
        Assert.Equal(Combinator.AnyChar.Match(""), null);

        Assert.Equal(Combinator.Letter.Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.Letter.Match("ABC"), (0, 1, 'A'));
        Assert.Equal(Combinator.Letter.Match("123"), null);
        Assert.Equal(Combinator.Letter.Match("   "), null);
        Assert.Equal(Combinator.Letter.Match(""), null);

        Assert.Equal(Combinator.Digit.Match("abc"), null);
        Assert.Equal(Combinator.Digit.Match("ABC"), null);
        Assert.Equal(Combinator.Digit.Match("123"), (0, 1, '1'));
        Assert.Equal(Combinator.Digit.Match("   "), null);
        Assert.Equal(Combinator.Digit.Match(""), null);

        Assert.Equal(Combinator.HexDigit.Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.HexDigit.Match("ABC"), (0, 1, 'A'));
        Assert.Equal(Combinator.HexDigit.Match("123"), (0, 1, '1'));
        Assert.Equal(Combinator.HexDigit.Match("FGI"), (0, 1, 'F'));
        Assert.Equal(Combinator.HexDigit.Match("GHI"), null);
        Assert.Equal(Combinator.HexDigit.Match("   "), null);
        Assert.Equal(Combinator.HexDigit.Match(""), null);

        Assert.Equal(Combinator.Word.Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.Word.Match("ABC"), (0, 1, 'A'));
        Assert.Equal(Combinator.Word.Match("123"), (0, 1, '1'));
        Assert.Equal(Combinator.Word.Match("   "), null);
        Assert.Equal(Combinator.Word.Match(""), null);

        Assert.Equal(Combinator.Space.Match("abc"), null);
        Assert.Equal(Combinator.Space.Match("ABC"), null);
        Assert.Equal(Combinator.Space.Match("123"), null);
        Assert.Equal(Combinator.Space.Match("   "), (0, 1, ' '));
        Assert.Equal(Combinator.Space.Match("\r"), (0, 1, '\r'));
        Assert.Equal(Combinator.Space.Match("\n"), (0, 1, '\n'));
        Assert.Equal(Combinator.Space.Match("\v"), (0, 1, '\v'));
        Assert.Equal(Combinator.Space.Match("\f"), (0, 1, '\f'));
        Assert.Equal(Combinator.Space.Match("\t"), (0, 1, '\t'));
        Assert.Equal(Combinator.Space.Match(""), null);

        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("bc"), (0, 1, 'b'));
        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("c"), (0, 1, 'c'));
        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("xyz"), null);
    }

    public void CharsTest()
    {
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(""), null);
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(" a"), (1, 1, "a"));
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(" ab"), (1, 2, "ab"));
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(" abc"), (1, 3, "abc"));
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(" abcd"), (1, 4, "abcd"));
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(" abcde"), (1, 5, "abcde"));
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(" abcdef"), (1, 6, "abcdef"));
        Assert.Equal(Combinator.Chars(Combinator.Letter).Match(" abcdef "), (1, 6, "abcdef"));
    }

    [Fact]
    public void StringTest()
    {
        Assert.Equal(Combinator.String("ab").Match(""), null);
        Assert.Equal(Combinator.String("ab").Match("a"), null);
        Assert.Equal(Combinator.String("ab").Match("ab"), (0, 2, "ab"));
        Assert.Equal(Combinator.String("ab").Match("abc"), (0, 2, "ab"));
        Assert.Equal(Combinator.String("xy").Match("abc"), null);

        Assert.Equal(Combinator.String("").Match(""), (0, 0, ""));
        Assert.Equal(Combinator.String("").Match("a"), (0, 0, ""));
        Assert.Equal(Combinator.String("").Match("ab"), (0, 0, ""));

        Assert.Equal(Combinator.String<float>(s => (0, 1.5f)).Match("abc"), (0, 0, 1.5f));
        Assert.Equal(Combinator.String("a").Match("ab"), (0, 1, "a"));
        Assert.Equal(Combinator.String('a').Match("ab"), (0, 1, "a"));
        Assert.Equal(Combinator.String(s => null).Match("abc"), null);

        Assert.Equal(Combinator.String<float>(s => s.Length >= 3 && char.IsAsciiDigit(s[0]) && char.IsAsciiLetter(s[1]) ? (2, 1.5f) : null).Match("abc"), null);
        Assert.Equal(Combinator.String<float>(s => s.Length >= 3 && char.IsAsciiDigit(s[0]) && char.IsAsciiLetter(s[1]) ? (2, 1.5f) : null).Match("123"), null);
        Assert.Equal(Combinator.String<float>(s => s.Length >= 3 && char.IsAsciiDigit(s[0]) && char.IsAsciiLetter(s[1]) ? (2, 1.5f) : null).Match("1x"), null);
        Assert.Equal(Combinator.String<float>(s => s.Length >= 3 && char.IsAsciiDigit(s[0]) && char.IsAsciiLetter(s[1]) ? (2, 1.5f) : null).Match("1xy"), (0, 2, 1.5f));
    }

    [Fact]
    public void CharClassTest()
    {
        Assert.Equal(Combinator.CharClass().Match("abc"), null);

        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("abc"), null);
        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("xyz"), (0, 1, 'x'));
        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("yz"), (0, 1, 'y'));
        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("z"), (0, 1, 'z'));

        Assert.Equal(Combinator.CharClass("xyz").Match("abc"), null);
        Assert.Equal(Combinator.CharClass("xyz").Match("xyz"), (0, 1, 'x'));
        Assert.Equal(Combinator.CharClass("xyz").Match("yz"), (0, 1, 'y'));
        Assert.Equal(Combinator.CharClass("xyz").Match("z"), (0, 1, 'z'));
    }

    [Fact]
    public void OnceTest()
    {
        Assert.Equal(Combinator.Once(Combinator.Char('a'), _ => 1.5f).Match(""), null);
        Assert.Equal(Combinator.Once(Combinator.Char('a'), _ => 1.5f).Match("a"), (0, 1, 1.5f));

        Assert.Equal(Combinator.Char('a').ToOnce(_ => 1.5f).Match(""), null);
        Assert.Equal(Combinator.Char('a').ToOnce(_ => 1.5f).Match("a"), (0, 1, 1.5f));

        Assert.Equal(Combinator.String("a").ToOnce(_ => 1.5f).Match(""), null);
        Assert.Equal(Combinator.String("a").ToOnce(_ => 1.5f).Match("a"), (0, 1, 1.5f));
    }

    [Fact]
    public void NoneTest()
    {
        Assert.Equal(Combinator.None<char, float>(Combinator.Char('a')).Match(""), null);
        Assert.Equal(Combinator.None<char, float>(Combinator.Char('a')).Match("a"), (0, 1, 0f));

        Assert.Equal(Combinator.None<string, float>(Combinator.String("a")).Match(""), null);
        Assert.Equal(Combinator.None<string, float>(Combinator.String("a")).Match("a"), (0, 1, 0f));

        Assert.Equal(Combinator.None<int, float>(Combinator.String("a").ToOnce(_ => 123)).Match(""), null);
        Assert.Equal(Combinator.None<int, float>(Combinator.String("a").ToOnce(_ => 123)).Match("a"), (0, 1, 0f));

        Assert.Equal(Combinator.Char('a').ToNone<char, float>().Match(""), null);
        Assert.Equal(Combinator.Char('a').ToNone<char, float>().Match("a"), (0, 1, 0f));

        Assert.Equal(Combinator.String("a").ToNone<string, float>().Match(""), null);
        Assert.Equal(Combinator.String("a").ToNone<string, float>().Match("a"), (0, 1, 0f));

        Assert.Equal(Combinator.String("a").ToOnce(_ => 123).ToNone<int, float>().Match(""), null);
        Assert.Equal(Combinator.String("a").ToOnce(_ => 123).ToNone<int, float>().Match("a"), (0, 1, 0f));
    }

    [Fact]
    public void OptionTest()
    {
        Assert.Equal(Combinator.Option(Combinator.Char('a')).Match(""), (0, 0, default));
        Assert.Equal(Combinator.Option(Combinator.Char('a')).Match("a"), (0, 1, 'a'));
        Assert.Equal(Combinator.Option(Combinator.Char('a')).Match("aa"), (0, 1, 'a'));
        Assert.Equal(Combinator.Option(Combinator.Char('a')).Match("ab"), (0, 1, 'a'));
        Assert.Equal(Combinator.Option(Combinator.Char('a')).Match("ba"), (0, 0, default));
        Assert.Equal(Combinator.Option(Combinator.Char('a')).Match("xyz"), (0, 0, default));
    }

    [Fact]
    public void ManyTest()
    {
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match(""), (0, 0, default));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("a"), (0, 1, 'a'));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("aa"), (0, 2, 'a'));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("ab"), (0, 1, 'a'));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("ba"), (0, 0, default));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("xyz"), (0, 0, default));
    }

    [Fact]
    public void Many1Test()
    {
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match(""), null);
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("a"), (0, 1, 'a'));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("aa"), (0, 2, 'a'));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("ab"), (0, 1, 'a'));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("ba"), (1, 1, 'a'));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("xyz"), null);
    }

    [Fact]
    public void ManyNTest()
    {
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match(""), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("a"), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("aa"), (0, 2, 'a'));
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("aaa"), (0, 3, 'a'));
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("aaaa"), (0, 3, 'a'));
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("ab"), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("ba"), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("xyz"), null);
    }

    [Fact]
    public void ChainLeftTest()
    {
        var parser = Combinator.ChainLeft(Combinator.String("a"), Combinator.Char('+'), (left, op, right) => left + op + right);

        Assert.Equal(parser.Match(""), null);
        Assert.Equal(parser.Match("a"), (0, 1, "a"));
        Assert.Equal(parser.Match("a+a"), (0, 3, "a+a"));
        Assert.Equal(parser.Match("a+a+a"), (0, 5, "a+a+a"));
        Assert.Equal(parser.Match("a+"), (0, 1, "a"));
        Assert.Equal(parser.Match("+a"), (1, 1, "a"));
        Assert.Equal(parser.Match("b"), null);
    }

    [Fact]
    public void SequenceTest()
    {
        var seq2 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'));
        Assert.Equal(seq2.Match(""), null);
        Assert.Equal(seq2.Match("a"), null);
        Assert.Equal(seq2.Match("aa"), null);
        Assert.Equal(seq2.Match("ab"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("abc"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("abcd"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("abcde"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("abcdef"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("abcdefg"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("abcdefgh"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("abcdefghi"), (0, 2, 'b'));
        Assert.Equal(seq2.Match("bb"), null);
        Assert.Equal(seq2.Match("ba"), null);
        Assert.Equal(seq2.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), (a, b) => 1.5f).Match("abcdefghi"), (0, 2, 1.5f));

        var seq3 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'));
        Assert.Equal(seq3.Match(""), null);
        Assert.Equal(seq3.Match("a"), null);
        Assert.Equal(seq3.Match("aa"), null);
        Assert.Equal(seq3.Match("ab"), null);
        Assert.Equal(seq3.Match("abc"), (0, 3, 'c'));
        Assert.Equal(seq3.Match("abcd"), (0, 3, 'c'));
        Assert.Equal(seq3.Match("abcde"), (0, 3, 'c'));
        Assert.Equal(seq3.Match("abcdef"), (0, 3, 'c'));
        Assert.Equal(seq3.Match("abcdefg"), (0, 3, 'c'));
        Assert.Equal(seq3.Match("abcdefgh"), (0, 3, 'c'));
        Assert.Equal(seq3.Match("abcdefghi"), (0, 3, 'c'));
        Assert.Equal(seq3.Match("bb"), null);
        Assert.Equal(seq3.Match("ba"), null);
        Assert.Equal(seq3.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), (a, b, c) => 1.5f).Match("abcdefghi"), (0, 3, 1.5f));

        var seq4 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'));
        Assert.Equal(seq4.Match(""), null);
        Assert.Equal(seq4.Match("a"), null);
        Assert.Equal(seq4.Match("aa"), null);
        Assert.Equal(seq4.Match("ab"), null);
        Assert.Equal(seq4.Match("abc"), null);
        Assert.Equal(seq4.Match("abcd"), (0, 4, 'd'));
        Assert.Equal(seq4.Match("abcde"), (0, 4, 'd'));
        Assert.Equal(seq4.Match("abcdef"), (0, 4, 'd'));
        Assert.Equal(seq4.Match("abcdefg"), (0, 4, 'd'));
        Assert.Equal(seq4.Match("abcdefgh"), (0, 4, 'd'));
        Assert.Equal(seq4.Match("abcdefghi"), (0, 4, 'd'));
        Assert.Equal(seq4.Match("bb"), null);
        Assert.Equal(seq4.Match("ba"), null);
        Assert.Equal(seq4.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), (a, b, c, d) => 1.5f).Match("abcdefghi"), (0, 4, 1.5f));

        var seq5 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'));
        Assert.Equal(seq5.Match(""), null);
        Assert.Equal(seq5.Match("a"), null);
        Assert.Equal(seq5.Match("aa"), null);
        Assert.Equal(seq5.Match("ab"), null);
        Assert.Equal(seq5.Match("abc"), null);
        Assert.Equal(seq5.Match("abcd"), null);
        Assert.Equal(seq5.Match("abcde"), (0, 5, 'e'));
        Assert.Equal(seq5.Match("abcdef"), (0, 5, 'e'));
        Assert.Equal(seq5.Match("abcdefg"), (0, 5, 'e'));
        Assert.Equal(seq5.Match("abcdefgh"), (0, 5, 'e'));
        Assert.Equal(seq5.Match("abcdefghi"), (0, 5, 'e'));
        Assert.Equal(seq5.Match("bb"), null);
        Assert.Equal(seq5.Match("ba"), null);
        Assert.Equal(seq5.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), (a, b, c, d, e) => 1.5f).Match("abcdefghi"), (0, 5, 1.5f));

        var seq6 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'));
        Assert.Equal(seq6.Match(""), null);
        Assert.Equal(seq6.Match("a"), null);
        Assert.Equal(seq6.Match("aa"), null);
        Assert.Equal(seq6.Match("ab"), null);
        Assert.Equal(seq6.Match("abc"), null);
        Assert.Equal(seq6.Match("abcd"), null);
        Assert.Equal(seq6.Match("abcde"), null);
        Assert.Equal(seq6.Match("abcdef"), (0, 6, 'f'));
        Assert.Equal(seq6.Match("abcdefg"), (0, 6, 'f'));
        Assert.Equal(seq6.Match("abcdefgh"), (0, 6, 'f'));
        Assert.Equal(seq6.Match("abcdefghi"), (0, 6, 'f'));
        Assert.Equal(seq6.Match("bb"), null);
        Assert.Equal(seq6.Match("ba"), null);
        Assert.Equal(seq6.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), (a, b, c, d, e, f) => 1.5f).Match("abcdefghi"), (0, 6, 1.5f));

        var seq7 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), Combinator.Char('g'));
        Assert.Equal(seq7.Match(""), null);
        Assert.Equal(seq7.Match("a"), null);
        Assert.Equal(seq7.Match("aa"), null);
        Assert.Equal(seq7.Match("ab"), null);
        Assert.Equal(seq7.Match("abc"), null);
        Assert.Equal(seq7.Match("abcd"), null);
        Assert.Equal(seq7.Match("abcde"), null);
        Assert.Equal(seq7.Match("abcdef"), null);
        Assert.Equal(seq7.Match("abcdefg"), (0, 7, 'g'));
        Assert.Equal(seq7.Match("abcdefgh"), (0, 7, 'g'));
        Assert.Equal(seq7.Match("abcdefghi"), (0, 7, 'g'));
        Assert.Equal(seq7.Match("bb"), null);
        Assert.Equal(seq7.Match("ba"), null);
        Assert.Equal(seq7.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), Combinator.Char('g'), (a, b, c, d, e, f, g) => 1.5f).Match("abcdefghi"), (0, 7, 1.5f));

        var seq8 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), Combinator.Char('g'), Combinator.Char('h'));
        Assert.Equal(seq8.Match(""), null);
        Assert.Equal(seq8.Match("a"), null);
        Assert.Equal(seq8.Match("aa"), null);
        Assert.Equal(seq8.Match("ab"), null);
        Assert.Equal(seq8.Match("abc"), null);
        Assert.Equal(seq8.Match("abcd"), null);
        Assert.Equal(seq8.Match("abcde"), null);
        Assert.Equal(seq8.Match("abcdef"), null);
        Assert.Equal(seq8.Match("abcdefg"), null);
        Assert.Equal(seq8.Match("abcdefgh"), (0, 8, 'h'));
        Assert.Equal(seq8.Match("abcdefghi"), (0, 8, 'h'));
        Assert.Equal(seq8.Match("bb"), null);
        Assert.Equal(seq8.Match("ba"), null);
        Assert.Equal(seq8.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), Combinator.Char('g'), Combinator.Char('h'), (a, b, c, d, e, f, g, h) => 1.5f).Match("abcdefghi"), (0, 8, 1.5f));

        var seq9 = Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), Combinator.Char('g'), Combinator.Char('h'), Combinator.Char('i'));
        Assert.Equal(seq9.Match(""), null);
        Assert.Equal(seq9.Match("a"), null);
        Assert.Equal(seq9.Match("aa"), null);
        Assert.Equal(seq9.Match("ab"), null);
        Assert.Equal(seq9.Match("abc"), null);
        Assert.Equal(seq9.Match("abcd"), null);
        Assert.Equal(seq9.Match("abcde"), null);
        Assert.Equal(seq9.Match("abcdef"), null);
        Assert.Equal(seq9.Match("abcdefg"), null);
        Assert.Equal(seq9.Match("abcdefgh"), null);
        Assert.Equal(seq9.Match("abcdefghi"), (0, 9, 'i'));
        Assert.Equal(seq9.Match("bb"), null);
        Assert.Equal(seq9.Match("ba"), null);
        Assert.Equal(seq9.Match("xyz"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), Combinator.Char('g'), Combinator.Char('h'), Combinator.Char('i')).Match("abcdefghi"), (0, 9, 'i'));

        var seq9s = Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'), Combinator.Char('d'), Combinator.Char('e'), Combinator.Char('f'), Combinator.Char('g'), Combinator.Char('h'), Combinator.Char('i')], xs => 1.5f);
        Assert.Equal(seq9s.Match(""), null);
        Assert.Equal(seq9s.Match("a"), null);
        Assert.Equal(seq9s.Match("aa"), null);
        Assert.Equal(seq9s.Match("ab"), null);
        Assert.Equal(seq9s.Match("abc"), null);
        Assert.Equal(seq9s.Match("abcd"), null);
        Assert.Equal(seq9s.Match("abcde"), null);
        Assert.Equal(seq9s.Match("abcdef"), null);
        Assert.Equal(seq9s.Match("abcdefg"), null);
        Assert.Equal(seq9s.Match("abcdefgh"), null);
        Assert.Equal(seq9s.Match("abcdefghi"), (0, 9, 1.5f));
        Assert.Equal(seq9s.Match("bb"), null);
        Assert.Equal(seq9s.Match("ba"), null);
        Assert.Equal(seq9s.Match("xyz"), null);
    }

    [Fact]
    public void ChoiceTest()
    {
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match(""), null);
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("a"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("aa"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("ab"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("bb"), (0, 1, 'b'));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("ba"), (0, 1, 'b'));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("xyz"), null);
    }

    [Fact]
    public void ChoiceNTest()
    {
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match(""), null);
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("a"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("aa"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("ab"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("abc"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("bb"), (0, 1, 'b'));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("ba"), (0, 1, 'b'));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("xyz"), null);
    }

    [Fact]
    public void ZeroWidthTest()
    {
        Assert.Equal(Combinator.ZeroWidth(_ => (false, "")).Match(""), null);
        Assert.Equal(Combinator.ZeroWidth(_ => (true, "xx")).Match(""), (0, 0, "xx"));

        Assert.Equal(Combinator.ZeroWidth(input => input.Length > 0 && input[0] == 'z' ? (true, "z") : (false, "")).Match("xaby"), null);
        Assert.Equal(Combinator.ZeroWidth(input => input.Length > 0 && input[0] == 'b' ? (true, "z") : (false, "")).Match("xaby"), (2, 0, "z"));
    }

    [Fact]
    public void OperatorTest()
    {
        var a = Combinator.Char('a');
        var b = Combinator.Char('b');
        var c = Combinator.Char('c');
        var abc = a & b & c;
        Assert.Equal(abc.Match(""), null);
        Assert.Equal(abc.Match("ab"), null);
        Assert.Equal(abc.Match("ac"), null);
        Assert.Equal(abc.Match("ax"), null);
        Assert.Equal(abc.Match("abc"), (0, 3, "abc"));
        Assert.Equal(abc.Match("xabc"), (1, 3, "abc"));
        Assert.Equal(abc.Match("xybc"), null);

        var ab_c = a & b | Combinator.Chars(c);
        Assert.Equal(ab_c.Match(""), null);
        Assert.Equal(ab_c.Match("ab"), (0, 2, "ab"));
        Assert.Equal(ab_c.Match("ac"), (1, 1, "c"));
        Assert.Equal(ab_c.Match("ax"), null);
        Assert.Equal(ab_c.Match("abc"), (0, 2, "ab"));
        Assert.Equal(ab_c.Match("xabc"), (1, 2, "ab"));
        Assert.Equal(ab_c.Match("xybc"), (3, 1, "c"));
    }

    [Fact]
    public void WordTest()
    {
        Assert.Equal(Combinator.Digit.Match("[  xyzabc123]"), (9, 1, '1'));
        Assert.Equal(Combinator.Digits.Match("[  xyzabc123]"), (9, 3, "123"));

        Assert.Equal(Combinator.HexDigit.Match("[  xyzabc123]"), (6, 1, 'a'));
        Assert.Equal(Combinator.HexDigits.Match("[  xyzabc123]"), (6, 6, "abc123"));

        Assert.Equal(Combinator.Letter.Match("[  xyzabc123]"), (3, 1, 'x'));
        Assert.Equal(Combinator.Letters.Match("[  xyzabc123]"), (3, 6, "xyzabc"));

        Assert.Equal(Combinator.Word.Match("[  xyzabc123]"), (3, 1, 'x'));
        Assert.Equal(Combinator.Words.Match("[  xyzabc123]"), (3, 9, "xyzabc123"));

        Assert.Equal(Combinator.Space.Match("[  xyzabc123]"), (1, 1, ' '));
        Assert.Equal(Combinator.Spaces.Match("[  xyzabc123]"), (1, 2, "  "));
    }

    [Fact]
    public void NewLineTest()
    {
        Assert.Equal(Combinator.Cr.Match("abc123"), null);
        Assert.Equal(Combinator.Lf.Match("abc123"), null);
        Assert.Equal(Combinator.CrLf.Match("abc123"), null);
        Assert.Equal(Combinator.NewLine.Match("abc123"), null);

        Assert.Equal(Combinator.Cr.Match("abc\r123"), (3, 1, '\r'));
        Assert.Equal(Combinator.Lf.Match("abc\r123"), null);
        Assert.Equal(Combinator.CrLf.Match("abc\r123"), null);
        Assert.Equal(Combinator.NewLine.Match("abc\r123"), (3, 1, "\r"));

        Assert.Equal(Combinator.Cr.Match("abc\n123"), null);
        Assert.Equal(Combinator.Lf.Match("abc\n123"), (3, 1, '\n'));
        Assert.Equal(Combinator.CrLf.Match("abc\n123"), null);
        Assert.Equal(Combinator.NewLine.Match("abc\n123"), (3, 1, "\n"));

        Assert.Equal(Combinator.Cr.Match("abc\r\n123"), (3, 1, '\r'));
        Assert.Equal(Combinator.Lf.Match("abc\r\n123"), (4, 1, '\n'));
        Assert.Equal(Combinator.CrLf.Match("abc\r\n123"), (3, 2, "\r\n"));
        Assert.Equal(Combinator.NewLine.Match("abc\r\n123"), (3, 2, "\r\n"));
    }

    [Fact]
    public void LineEndTest()
    {
        Assert.Equal(Combinator.LineEnd.Match("abc123"), (6, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc123", 5), (6, 0, ""));

        Assert.Equal(Combinator.LineEnd.Match("abc\r123"), (3, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\r123", 5), (7, 0, ""));

        Assert.Equal(Combinator.LineEnd.Match("abc\n123"), (3, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\n123", 5), (7, 0, ""));

        Assert.Equal(Combinator.LineEnd.Match("abc\r\n123"), (3, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\r\n123", 5), (8, 0, ""));
    }

    [Fact]
    public void EndTest()
    {
        Assert.Equal(Combinator.End.Match("abc123"), (6, 0, ""));
        Assert.Equal(Combinator.End.Match("abc123", 5), (6, 0, ""));

        Assert.Equal(Combinator.End.Match("abc\r123"), (7, 0, ""));
        Assert.Equal(Combinator.End.Match("abc\r123", 5), (7, 0, ""));

        Assert.Equal(Combinator.End.Match("abc\n123"), (7, 0, ""));
        Assert.Equal(Combinator.End.Match("abc\n123", 5), (7, 0, ""));

        Assert.Equal(Combinator.End.Match("abc\r\n123"), (8, 0, ""));
        Assert.Equal(Combinator.End.Match("abc\r\n123", 5), (8, 0, ""));
    }

    [Fact]
    public void ErrorTest()
    {
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Error<char>()).Match(""), null);
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Error<char>()).Match("a"), (0, 1, 'a'));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Error<char>()).Match("b"), null);

        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Error<char>()).Match(""), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Error<char>()).Match("a"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Error<char>()).Match("b"), null);
    }

    [Fact]
    public void OperatorCharTest()
    {
        var left = Combinator.Char('a');

        Assert.Equal((left & Combinator.Char('b')).Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & Combinator.String("b")).Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & 'b').Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & "b").Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & ['b', 'c']).Match("xaby"), (1, 2, "ab"));

        Assert.Equal(Combinator.Add(left, Combinator.Char('b')).Match("xaby"), (1, 2, "ab"));
        Assert.Equal(Combinator.Add(left, Combinator.String("b")).Match("xaby"), (1, 2, "ab"));
    }

    [Fact]
    public void OperatorStringTest()
    {
        var left = Combinator.String("a");

        Assert.Equal((left & Combinator.Char('b')).Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & Combinator.String("b")).Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & 'b').Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & "b").Match("xaby"), (1, 2, "ab"));
        Assert.Equal((left & ['b', 'c']).Match("xaby"), (1, 2, "ab"));

        Assert.Equal(Combinator.Add(left, Combinator.Char('b')).Match("xaby"), (1, 2, "ab"));
        Assert.Equal(Combinator.Add(left, Combinator.String("b")).Match("xaby"), (1, 2, "ab"));
    }

    [Fact]
    public void ToOnceStringTest()
    {
        Assert.Equal(Combinator.Char('a').ToOnceString().Match("aaa"), (0, 1, "a"));
        Assert.Equal(Combinator.Char('a').ToOption().ToOnceString().Match("aaa"), (0, 1, "a"));
        Assert.Equal(Combinator.Char('a').ToMany().ToOnceString().Match("aaa"), (0, 3, "aaa"));

        Assert.Equal(Combinator.String("a").ToOption().ToOnceString().Match("aaa"), (0, 1, "a"));
        Assert.Equal(Combinator.String("a").ToMany().ToOnceString().Match("aaa"), (0, 3, "aaa"));
    }

    [Fact]
    public void NumberInt32Test()
    {
        Assert.Equal(Combinator.NaturalNumberInt32.Match(""), null);
        Assert.Equal(Combinator.NaturalNumberInt32.Match("a"), null);
        Assert.Equal(Combinator.NaturalNumberInt32.Match("0"), (0, 1, 0));
        Assert.Equal(Combinator.NaturalNumberInt32.Match("1"), (0, 1, 1));
        Assert.Equal(Combinator.NaturalNumberInt32.Match("2147483646"), (0, 10, 2147483646));
        Assert.Equal(Combinator.NaturalNumberInt32.Match("2147483647"), (0, 10, int.MaxValue));
        Assert.Equal(Combinator.NaturalNumberInt32.Match("2147483648"), (0, 10, -2147483648));
    }

    [Fact]
    public void NumberInt64Test()
    {
        Assert.Equal(Combinator.NaturalNumberInt64.Match(""), null);
        Assert.Equal(Combinator.NaturalNumberInt64.Match("a"), null);
        Assert.Equal(Combinator.NaturalNumberInt64.Match("0"), (0, 1, 0));
        Assert.Equal(Combinator.NaturalNumberInt64.Match("1"), (0, 1, 1));
        Assert.Equal(Combinator.NaturalNumberInt64.Match("9223372036854775806"), (0, 19, 9223372036854775806));
        Assert.Equal(Combinator.NaturalNumberInt64.Match("9223372036854775807"), (0, 19, long.MaxValue));
        Assert.Equal(Combinator.NaturalNumberInt64.Match("9223372036854775808"), (0, 19, -9223372036854775808));
    }

    [Fact]
    public void NumberDecimalTest()
    {
        Assert.Equal(Combinator.NaturalNumberDecimal.Match(""), null);
        Assert.Equal(Combinator.NaturalNumberDecimal.Match("a"), null);
        Assert.Equal(Combinator.NaturalNumberDecimal.Match("0"), (0, 1, 0));
        Assert.Equal(Combinator.NaturalNumberDecimal.Match("1"), (0, 1, 1));
        Assert.Equal(Combinator.NaturalNumberDecimal.Match("79228162514264337593543950334"), (0, 29, 79228162514264337593543950334m));
        Assert.Equal(Combinator.NaturalNumberDecimal.Match("79228162514264337593543950335"), (0, 29, decimal.MaxValue));
        Assert.Throws<OverflowException>(() => Combinator.NaturalNumberDecimal.Match("79228162514264337593543950336"));
    }

    [Fact]
    public void NumberSingleTest()
    {
        Assert.Equal(Combinator.NaturalNumberSingle.Match(""), null);
        Assert.Equal(Combinator.NaturalNumberSingle.Match("a"), null);
        Assert.Equal(Combinator.NaturalNumberSingle.Match("0"), (0, 1, 0));
        Assert.Equal(Combinator.NaturalNumberSingle.Match("1"), (0, 1, 1));
        Assert.Equal(Combinator.NaturalNumberSingle.Match("340282246000000000000000000000000000000"), (0, 39, 3.40282246e38f));
        Assert.Equal(Combinator.NaturalNumberSingle.Match("340282347000000000000000000000000000000"), (0, 39, float.MaxValue));

        Assert.Equal(Combinator.RealNumberSingle.Match("0.5"), (0, 3, 0.5f));
        Assert.Equal(Combinator.RealNumberSingle.Match("1.25"), (0, 4, 1.25f));
        Assert.Equal(Combinator.RealNumberSingle.Match("123.125"), (0, 7, 123.125f));

        Assert.Equal(Combinator.NaturalNumberFloat.Match(""), null);
        Assert.Equal(Combinator.NaturalNumberFloat.Match("a"), null);
        Assert.Equal(Combinator.NaturalNumberFloat.Match("0"), (0, 1, 0f));
        Assert.Equal(Combinator.NaturalNumberFloat.Match("1"), (0, 1, 1f));
        Assert.Equal(Combinator.NaturalNumberFloat.Match("340282246000000000000000000000000000000"), (0, 39, 3.40282246e38f));
        Assert.Equal(Combinator.NaturalNumberFloat.Match("340282347000000000000000000000000000000"), (0, 39, float.MaxValue));

        Assert.Equal(Combinator.RealNumberFloat.Match("0.5"), (0, 3, 0.5f));
        Assert.Equal(Combinator.RealNumberFloat.Match("1.25"), (0, 4, 1.25f));
        Assert.Equal(Combinator.RealNumberFloat.Match("123.125"), (0, 7, 123.125f));
        Assert.Equal(Combinator.RealNumberFloat.Match("123."), (0, 4, 123f));
        Assert.Equal(Combinator.RealNumberFloat.Match("123.x"), (0, 4, 123f));
    }

    [Fact]
    public void NumberDoubleTest()
    {
        Assert.Equal(Combinator.NaturalNumberDouble.Match(""), null);
        Assert.Equal(Combinator.NaturalNumberDouble.Match("a"), null);
        Assert.Equal(Combinator.NaturalNumberDouble.Match("0"), (0, 1, 0d));
        Assert.Equal(Combinator.NaturalNumberDouble.Match("1"), (0, 1, 1d));
        Assert.Equal(Combinator.NaturalNumberDouble.Match("179769313486231620000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"), (0, 309, 1.7976931348623145e308));

        Assert.Equal(Combinator.RealNumberDouble.Match("0.5"), (0, 3, 0.5d));
        Assert.Equal(Combinator.RealNumberDouble.Match("1.25"), (0, 4, 1.25d));
        Assert.Equal(Combinator.RealNumberDouble.Match("123.125"), (0, 7, 123.125d));
        Assert.Equal(Combinator.RealNumberDouble.Match("123."), (0, 4, 123d));
        Assert.Equal(Combinator.RealNumberDouble.Match("123.x"), (0, 4, 123d));
    }

    [Fact]
    public void TryParseTest()
    {
        var parser = Combinator.Char('a');

        Assert.Equal(parser.TryParse("1", out var _), false);
        Assert.Equal(parser.TryParse("a", out var a), true);
        Assert.Equal(a, 'a');
        Assert.Equal(parser.TryParse("xa", out var _), false);

        Assert.Equal(parser.TryParse("xa", 0, out var _), false);
        Assert.Equal(parser.TryParse("xa", 1, out var a1), true);
        Assert.Equal(a1, 'a');
    }

    [Fact]
    public void IsMatchTest()
    {
        Assert.Equal(Combinator.Char('a').IsMatch("1"), false);
        Assert.Equal(Combinator.Char('a').IsMatch("a"), true);
        Assert.Equal(Combinator.Char('a').IsMatch("x1", 1), false);
        Assert.Equal(Combinator.Char('a').IsMatch("xa", 1), true);
    }

    [Fact]
    public void Sample1()
    {
        // README.md Usage Sample
        var parser = Combinator.Char('a');
        _ = parser.Match("abc123xyz"); //=> (0, 1, 'a')

        Assert.Equal(parser.Match("abc123xyz"), (0, 1, 'a'));
    }

    [Fact]
    public void Sample2()
    {
        // README.md Usage Sample
        var parser = Combinator.String("ab")
            & Combinator.AnyChar
            & Combinator.Chars(Combinator.Digit);
        _ = parser.Match("abc123xyz"); //=> (0, 6, "abc123")

        Assert.Equal(parser.Match("abc123xyz"), (0, 6, "abc123"));
    }
}
