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
        var abc = a ^ b ^ c;
        Assert.Equal(abc.Match(""), null);
        Assert.Equal(abc.Match("ab"), null);
        Assert.Equal(abc.Match("ac"), null);
        Assert.Equal(abc.Match("ax"), null);
        Assert.Equal(abc.Match("abc"), (0, 3, "abc"));
        Assert.Equal(abc.Match("xabc"), (1, 3, "abc"));
        Assert.Equal(abc.Match("xybc"), null);

        var ab_c = a ^ b | Combinator.String(c);
        Assert.Equal(ab_c.Match(""), null);
        Assert.Equal(ab_c.Match("ab"), (0, 2, "ab"));
        Assert.Equal(ab_c.Match("ac"), (1, 1, "c"));
        Assert.Equal(ab_c.Match("ax"), null);
        Assert.Equal(ab_c.Match("abc"), (0, 2, "ab"));
        Assert.Equal(ab_c.Match("xabc"), (1, 2, "ab"));
        Assert.Equal(ab_c.Match("xybc"), (3, 1, "c"));
    }
}
