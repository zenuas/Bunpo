using Xunit;

namespace Bunpo.Test;

public class NameTest
{
    [Fact]
    public void CStyleTest()
    {
        var underbar = Combinator.Char('_').ToMany().ToOnceString();
        var parser = Combinator.WordBoundary
            & underbar
            & Combinator.Letter
            & Combinator.Words.ToOption().ToOnceString();

        Assert.Equal(parser.Match(""), null);
        Assert.Equal(parser.Match("abc123"), (0, 6, "abc123"));
        Assert.Equal(parser.Match("  abc123. "), (2, 6, "abc123"));
        Assert.Equal(parser.Match("  _a "), (2, 2, "_a"));
        Assert.Equal(parser.Match("  1 "), null);
        Assert.Equal(parser.Match("  _1 "), null);
        Assert.Equal(parser.Match("  _1a "), null);
        Assert.Equal(parser.Match("  _1a ab "), (6, 2, "ab"));
    }

    [Fact]
    public void CSharpStyleTest()
    {
        var underbar = Combinator.Char('_').ToMany().ToOnceString();
        var parser = (Combinator.WordBoundary | Combinator.String("@"))
            & underbar
            & Combinator.Letter
            & Combinator.Words.ToOption().ToOnceString();

        Assert.Equal(parser.Match(""), null);
        Assert.Equal(parser.Match("abc123"), (0, 6, "abc123"));
        Assert.Equal(parser.Match("  abc123. "), (2, 6, "abc123"));
        Assert.Equal(parser.Match("  _a "), (2, 2, "_a"));
        Assert.Equal(parser.Match("  1 "), null);
        Assert.Equal(parser.Match("  _1 "), null);
        Assert.Equal(parser.Match("  _1a "), null);
        Assert.Equal(parser.Match("  _1a ab "), (6, 2, "ab"));
        Assert.Equal(parser.Match("@abc123"), (0, 7, "@abc123"));
        Assert.Equal(parser.Match("  @abc123. "), (2, 7, "@abc123"));
        Assert.Equal(parser.Match("  @_a "), (2, 3, "@_a"));
        Assert.Equal(parser.Match("  @1 "), null);
        Assert.Equal(parser.Match("  @_1 "), null);
        Assert.Equal(parser.Match("  @_1a "), null);
        Assert.Equal(parser.Match("  _1a @ab "), (6, 3, "@ab"));
    }

    [Fact]
    public void KebabCaseTest()
    {
        var word = Combinator.Letter & Combinator.Char(char.IsAsciiLetterOrDigit).ToMany().ToOnceString();
        var parser = Combinator.WordBoundary
            & Combinator.ChainLeft(word, Combinator.Char('-'), (left, op, right) => left + op + right);

        Assert.Equal(parser.Match(""), null);
        Assert.Equal(parser.Match("abc123"), (0, 6, "abc123"));
        Assert.Equal(parser.Match("  abc123. "), (2, 6, "abc123"));
        Assert.Equal(parser.Match("  abc123-xyz456. "), (2, 13, "abc123-xyz456"));
        Assert.Equal(parser.Match("  abc123- "), (2, 6, "abc123"));
        Assert.Equal(parser.Match("  abc123-456xyz "), (2, 6, "abc123"));
        Assert.Equal(parser.Match("  _a "), null);
        Assert.Equal(parser.Match("  1 "), null);
        Assert.Equal(parser.Match("  _1 "), null);
        Assert.Equal(parser.Match("  _1a "), null);
        Assert.Equal(parser.Match("  _1a ab "), (6, 2, "ab"));
    }
}
