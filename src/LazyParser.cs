using System;

namespace Bunpo;

public class LazyParser<T>
{
    public Func<string, int, (int, T)?> Func { get; init; }
    public Func<string, int, (int, T)?> LazyFunc { get; set; } = null!;

    public LazyParser()
    {
        Func = (input, length) => LazyFunc(input, length);
    }
}
