using System;

namespace Bunpo;

public class LazyParser<T>()
{
    public Func<string, int, (int, T)?> Func { get; set; } = null!;
    public Func<string, int, (int, T)?> LazyFunc { get; set; } = null!;
}
