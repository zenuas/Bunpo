using System;
using Xunit;

namespace Bunpo.Test;

public class BunpoTest
{
    [Fact]
    public void Error()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Combinator.Many(Combinator.AnyChar, 1, 0));
    }

    [Fact]
    public void CharTest()
    {
        Assert.Equal(Combinator.Char('a').Match("abc"), (0, 1));
        Assert.Equal(Combinator.Char('x').Match("abc"), null);

        Assert.Equal(Combinator.AnyChar.Match("abc"), (0, 1));
        Assert.Equal(Combinator.AnyChar.Match("ABC"), (0, 1));
        Assert.Equal(Combinator.AnyChar.Match("123"), (0, 1));
        Assert.Equal(Combinator.AnyChar.Match("   "), (0, 1));
        Assert.Equal(Combinator.AnyChar.Match(""), null);

        Assert.Equal(Combinator.Letter.Match("abc"), (0, 1));
        Assert.Equal(Combinator.Letter.Match("ABC"), (0, 1));
        Assert.Equal(Combinator.Letter.Match("123"), null);
        Assert.Equal(Combinator.Letter.Match("   "), null);
        Assert.Equal(Combinator.Letter.Match(""), null);

        Assert.Equal(Combinator.Digit.Match("abc"), null);
        Assert.Equal(Combinator.Digit.Match("ABC"), null);
        Assert.Equal(Combinator.Digit.Match("123"), (0, 1));
        Assert.Equal(Combinator.Digit.Match("   "), null);
        Assert.Equal(Combinator.Digit.Match(""), null);

        Assert.Equal(Combinator.HexDigit.Match("abc"), (0, 1));
        Assert.Equal(Combinator.HexDigit.Match("ABC"), (0, 1));
        Assert.Equal(Combinator.HexDigit.Match("123"), (0, 1));
        Assert.Equal(Combinator.HexDigit.Match("FGI"), (0, 1));
        Assert.Equal(Combinator.HexDigit.Match("GHI"), null);
        Assert.Equal(Combinator.HexDigit.Match("   "), null);
        Assert.Equal(Combinator.HexDigit.Match(""), null);

        Assert.Equal(Combinator.Word.Match("abc"), (0, 1));
        Assert.Equal(Combinator.Word.Match("ABC"), (0, 1));
        Assert.Equal(Combinator.Word.Match("123"), (0, 1));
        Assert.Equal(Combinator.Word.Match("   "), null);
        Assert.Equal(Combinator.Word.Match(""), null);

        Assert.Equal(Combinator.Space.Match("abc"), null);
        Assert.Equal(Combinator.Space.Match("ABC"), null);
        Assert.Equal(Combinator.Space.Match("123"), null);
        Assert.Equal(Combinator.Space.Match("   "), (0, 1));
        Assert.Equal(Combinator.Space.Match("\r"), (0, 1));
        Assert.Equal(Combinator.Space.Match("\n"), (0, 1));
        Assert.Equal(Combinator.Space.Match("\v"), (0, 1));
        Assert.Equal(Combinator.Space.Match("\f"), (0, 1));
        Assert.Equal(Combinator.Space.Match("\t"), (0, 1));
        Assert.Equal(Combinator.Space.Match(""), null);

        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("abc"), (0, 1));
        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("bc"), (0, 1));
        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("c"), (0, 1));
        Assert.Equal(Combinator.Char(c => c is 'a' or 'b' or 'c').Match("xyz"), null);
    }

    [Fact]
    public void StringTest()
    {
        Assert.Equal(Combinator.String("ab").Match(""), null);
        Assert.Equal(Combinator.String("ab").Match("a"), null);
        Assert.Equal(Combinator.String("ab").Match("ab"), (0, 2));
        Assert.Equal(Combinator.String("ab").Match("abc"), (0, 2));
        Assert.Equal(Combinator.String("xy").Match("abc"), null);

        Assert.Equal(Combinator.String("").Match(""), (0, 0));
        Assert.Equal(Combinator.String("").Match("a"), (0, 0));
        Assert.Equal(Combinator.String("").Match("ab"), (0, 0));

        Assert.Equal(Combinator.String((s, start) => 0).Match("abc"), (0, 0));
        Assert.Equal(Combinator.String((s, start) => null).Match("abc"), null);

        Assert.Equal(Combinator.String((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? 2 : null).Match("abc"), null);
        Assert.Equal(Combinator.String((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? 2 : null).Match("123"), null);
        Assert.Equal(Combinator.String((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? 2 : null).Match("1x"), null);
        Assert.Equal(Combinator.String((s, start) => s.Length >= start + 3 && char.IsAsciiDigit(s[start]) && char.IsAsciiLetter(s[start + 1]) ? 2 : null).Match("1xy"), (0, 2));
    }

    [Fact]
    public void CharClassTest()
    {
        Assert.Equal(Combinator.CharClass().Match("abc"), null);

        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("abc"), null);
        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("xyz"), (0, 1));
        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("yz"), (0, 1));
        Assert.Equal(Combinator.CharClass('x', 'y', 'z').Match("z"), (0, 1));

        Assert.Equal(Combinator.CharClass("xyz").Match("abc"), null);
        Assert.Equal(Combinator.CharClass("xyz").Match("xyz"), (0, 1));
        Assert.Equal(Combinator.CharClass("xyz").Match("yz"), (0, 1));
        Assert.Equal(Combinator.CharClass("xyz").Match("z"), (0, 1));
    }

    [Fact]
    public void ManyTest()
    {
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match(""), (0, 0));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("a"), (0, 1));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("aa"), (0, 2));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("ab"), (0, 1));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("ba"), (0, 0));
        Assert.Equal(Combinator.Many(Combinator.Char('a')).Match("xyz"), (0, 0));
    }

    [Fact]
    public void Many1Test()
    {
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match(""), null);
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("a"), (0, 1));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("aa"), (0, 2));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("ab"), (0, 1));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("ba"), (1, 1));
        Assert.Equal(Combinator.Many1(Combinator.Char('a')).Match("xyz"), null);
    }

    [Fact]
    public void ManyNTest()
    {
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match(""), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("a"), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("aa"), (0, 2));
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("aaa"), (0, 3));
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("aaaa"), (0, 3));
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("ab"), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("ba"), null);
        Assert.Equal(Combinator.Many(Combinator.Char('a'), 2, 3).Match("xyz"), null);
    }

    [Fact]
    public void CaptureTest()
    {
        var result = ("", -1, -1);

        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match(""), null);
        Assert.Equal(result, ("", -1, -1));
        result = ("", -1, -1);
        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match("a"), (0, 1));
        Assert.Equal(result, ("a", 0, 1));
        result = ("", -1, -1);
        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match("aa"), (0, 1));
        Assert.Equal(result, ("aa", 0, 1));
        result = ("", -1, -1);
        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match("ab"), (0, 1));
        Assert.Equal(result, ("ab", 0, 1));
        result = ("", -1, -1);
        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match("abc"), (0, 1));
        Assert.Equal(result, ("abc", 0, 1));
        result = ("", -1, -1);
        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match("bb"), null);
        Assert.Equal(result, ("", -1, -1));
        result = ("", -1, -1);
        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match("ba"), (1, 1));
        Assert.Equal(result, ("ba", 1, 1));
        result = ("", -1, -1);
        Assert.Equal(Combinator.Capture(Combinator.Char('a'), (s, start, length) => result = (s, start, length)).Match("xyz"), null);
        Assert.Equal(result, ("", -1, -1));
        result = ("", -1, -1);
    }

    [Fact]
    public void SequenceTest()
    {
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match(""), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("a"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("aa"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("ab"), (0, 2));
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("abc"), (0, 2));
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("bb"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("ba"), null);
        Assert.Equal(Combinator.Sequence(Combinator.Char('a'), Combinator.Char('b')).Match("xyz"), null);
    }

    [Fact]
    public void ChoiceTest()
    {
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match(""), null);
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("a"), (0, 1));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("aa"), (0, 1));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("ab"), (0, 1));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("abc"), (0, 1));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("bb"), (0, 1));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("ba"), (0, 1));
        Assert.Equal(Combinator.Choice(Combinator.Char('a'), Combinator.Char('b')).Match("xyz"), null);
    }

    [Fact]
    public void ChoiceNTest()
    {
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match(""), null);
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("a"), (0, 1));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("aa"), (0, 1));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("ab"), (0, 1));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("abc"), (0, 1));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("bb"), (0, 1));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("ba"), (0, 1));
        Assert.Equal(Combinator.Choice([Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c')]).Match("xyz"), null);
    }
}
