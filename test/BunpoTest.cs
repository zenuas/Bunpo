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
        Assert.Equal(Combinator.AnyChar("", -1), null);
        Assert.Equal(Combinator.AnyChar.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.CharClass("a").Match("", -1));
        Assert.Equal(Combinator.CharClass("a")("", -1), null);
        Assert.Equal(Combinator.CharClass("a").Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.String("a").Match("", -1));
        Assert.Equal(Combinator.String("a").Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToMany().Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToMany()("", -1), null);
        Assert.Equal(Combinator.AnyChar.ToMany().Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToMany1().Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToMany1()("", -1), null);
        Assert.Equal(Combinator.AnyChar.ToMany1().Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToMany(0, 1).Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToMany(0, 1)("", -1), null);
        Assert.Equal(Combinator.AnyChar.ToMany(0, 1).Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.AnyChar.ToOption().Match("", -1));
        Assert.Equal(Combinator.AnyChar.ToOption()("", -1), null);
        Assert.Equal(Combinator.AnyChar.ToOption().Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.WordBoundary.Match("", -1));
        Assert.Equal(Combinator.WordBoundary("", -1), null);
        Assert.Equal(Combinator.WordBoundary.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.NonWordBoundary.Match("", -1));
        Assert.Equal(Combinator.NonWordBoundary("", -1), null);
        Assert.Equal(Combinator.NonWordBoundary.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.LineStart.Match("", -1));
        Assert.Equal(Combinator.LineStart("", -1), null);
        Assert.Equal(Combinator.LineStart.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.LineEnd.Match("", -1));
        Assert.Equal(Combinator.LineEnd("", -1), null);
        Assert.Equal(Combinator.LineEnd.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.Start.Match("", -1));
        Assert.Equal(Combinator.Start("", -1), null);
        Assert.Equal(Combinator.Start.Match("", 1), null);

        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.End.Match("", -1));
        Assert.Equal(Combinator.End("", -1), null);
        Assert.Equal(Combinator.End.Match("", 1), null);
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

        Assert.Equal(Combinator.String<float>((s, start) => (0, 1.5f)).Match("abc"), (0, 0, 1.5f));
        Assert.Equal(Combinator.String((s, start) => null).Match("abc"), null);

        Assert.Equal(Combinator.String<float>((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? (2, 1.5f) : null).Match("abc"), null);
        Assert.Equal(Combinator.String<float>((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? (2, 1.5f) : null).Match("123"), null);
        Assert.Equal(Combinator.String<float>((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? (2, 1.5f) : null).Match("1x"), null);
        Assert.Equal(Combinator.String<float>((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? (2, 1.5f) : null).Match("1xy"), (0, 2, 1.5f));
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
    public void SequenceTest()
    {
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match(""), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("a"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("aa"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("ab"), (0, 2, 'b'));
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("abc"), (0, 2, 'b'));
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("bb"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("ba"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("xyz"), null);

        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match(""), null);
        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match("a"), null);
        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match("aa"), null);
        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match("ab"), (0, 2, 'b'));
        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match("abc"), (0, 2, 'b'));
        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match("bb"), null);
        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match("ba"), null);
        Assert.Equal(Combinator.Sequence([Combinator.Char('a'), Combinator.Char('b')]).Match("xyz"), null);
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
    public void WordBoundaryTest()
    {
        Assert.Equal(Combinator.WordBoundary.Match("[  xyzabc123]"), (3, 0, ""));
        Assert.Equal(Combinator.WordBoundary.Match("xyzabc123]"), (0, 0, ""));
        Assert.Equal(Combinator.WordBoundary.Match("[]"), null);
        Assert.Equal(Combinator.WordBoundary.Match("xyz", 0), (0, 0, ""));
        Assert.Equal(Combinator.WordBoundary.Match("xyz", 1), (3, 0, ""));
        Assert.Equal(Combinator.WordBoundary.Match("xyz", 2), (3, 0, ""));
        Assert.Equal(Combinator.WordBoundary.Match("xyz", 3), (3, 0, ""));
    }

    [Fact]
    public void NonWordBoundaryTest()
    {
        Assert.Equal(Combinator.NonWordBoundary.Match("[  xyzabc123]"), (0, 0, ""));
        Assert.Equal(Combinator.NonWordBoundary.Match("xyzabc123]"), (1, 0, ""));
        Assert.Equal(Combinator.NonWordBoundary.Match("[]"), (0, 0, ""));
        Assert.Equal(Combinator.NonWordBoundary.Match("xyz", 0), (1, 0, ""));
        Assert.Equal(Combinator.NonWordBoundary.Match("xyz", 1), (1, 0, ""));
        Assert.Equal(Combinator.NonWordBoundary.Match("xyz", 2), (2, 0, ""));
        Assert.Equal(Combinator.NonWordBoundary.Match("xyz", 3), null);
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
    public void LineTest()
    {
        Assert.Equal(Combinator.LineStart.Match("abc123"), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("abc123", 1), null);
        Assert.Equal(Combinator.LineEnd.Match("abc123"), (6, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc123", 5), (6, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("abc\r123"), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("abc\r123", 1), (4, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\r123"), (3, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\r123", 5), (7, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("abc\n123"), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("abc\n123", 1), (4, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\n123"), (3, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\n123", 5), (7, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("abc\r\n123"), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("abc\r\n123", 1), (5, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\r\n123"), (3, 0, ""));
        Assert.Equal(Combinator.LineEnd.Match("abc\r\n123", 5), (8, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("\ra", 0), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("\ra", 1), (1, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("\r", 0), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("\r", 1), (1, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("\na", 0), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("\na", 1), (1, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("\n", 0), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("\n", 1), (1, 0, ""));

        Assert.Equal(Combinator.LineStart.Match("\r\n", 0), (0, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("\r\n", 1), (2, 0, ""));
        Assert.Equal(Combinator.LineStart.Match("\r\n", 2), (2, 0, ""));
    }

    [Fact]
    public void StartEndTest()
    {
        Assert.Equal(Combinator.Start.Match("abc123"), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("abc123", 1), null);
        Assert.Equal(Combinator.End.Match("abc123"), (6, 0, ""));
        Assert.Equal(Combinator.End.Match("abc123", 5), (6, 0, ""));

        Assert.Equal(Combinator.Start.Match("abc\r123"), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("abc\r123", 1), null);
        Assert.Equal(Combinator.End.Match("abc\r123"), (7, 0, ""));
        Assert.Equal(Combinator.End.Match("abc\r123", 5), (7, 0, ""));

        Assert.Equal(Combinator.Start.Match("abc\n123"), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("abc\n123", 1), null);
        Assert.Equal(Combinator.End.Match("abc\n123"), (7, 0, ""));
        Assert.Equal(Combinator.End.Match("abc\n123", 5), (7, 0, ""));

        Assert.Equal(Combinator.Start.Match("abc\r\n123"), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("abc\r\n123", 1), null);
        Assert.Equal(Combinator.End.Match("abc\r\n123"), (8, 0, ""));
        Assert.Equal(Combinator.End.Match("abc\r\n123", 5), (8, 0, ""));

        Assert.Equal(Combinator.Start.Match("\ra", 0), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("\ra", 1), null);

        Assert.Equal(Combinator.Start.Match("\r", 0), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("\r", 1), null);

        Assert.Equal(Combinator.Start.Match("\na", 0), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("\na", 1), null);

        Assert.Equal(Combinator.Start.Match("\n", 0), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("\n", 1), null);

        Assert.Equal(Combinator.Start.Match("\r\n", 0), (0, 0, ""));
        Assert.Equal(Combinator.Start.Match("\r\n", 1), null);
        Assert.Equal(Combinator.Start.Match("\r\n", 2), null);
    }
}
