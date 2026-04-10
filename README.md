# Bunpo [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](LICENSE)

Bunpo is tiny parser combinator  

## Description

Bunpo is tiny parser combinator.  
It creates an LL parser in C# code.  

Bunpoは小さなパーサコンビネータである。  
C#のコードでLL文法パーサーを作ることができる。  

## Usage

Single-character parser:  
```cs
using Bunpo;

var parser = Combinator.Char('a');
_ = parser.Match("abc123xyz"); //=> (0, 1, 'a')
```

Alphabet and number parser:  
```cs
using Bunpo;

var parser = Combinator.String("ab")
    & Combinator.AnyChar
    & Combinator.Chars(Combinator.Digit);

_ = parser.Match("abc123xyz"); //=> (0, 6, "abc123")
```

## Examples

[Four arithmetic operations](https://github.com/zenuas/Bunpo/blob/master/test/CalculationTest.cs)  

## Benchmark

This is a benchmark for Four arithmetic operations parser setup and parser execution.  
The parser compared formulas with 5 numbers and formulas with 300 numbers.  

Four arithmetic operations benchmark:  
| Method          | Mean         | Error       | StdDev      | Gen0     | Gen1   | Allocated |
|---------------- |-------------:|------------:|------------:|---------:|-------:|----------:|
| BunpoSetup      |     112.6 ns |     1.92 ns |     2.35 ns |   0.1301 | 0.0007 |    1632 B |
| Bunpo5Parse     |     264.1 ns |     5.10 ns |     5.01 ns |   0.0567 |      - |     712 B |
| Bunpo300Parse   |  18,683.1 ns |   283.66 ns |   265.33 ns |   3.5095 |      - |   44400 B |
| SpracheSetup    |     163.9 ns |     3.22 ns |     3.16 ns |   0.1931 | 0.0017 |    2424 B |
| Sprache5Parse   |   6,009.8 ns |   120.12 ns |   128.52 ns |   3.4180 | 0.0458 |   42944 B |
| Sprache300Parse | 375,802.4 ns | 5,347.59 ns | 4,465.48 ns | 208.9844 | 7.8125 | 2623888 B |

Sprache version 2.3.1 used.  

## Requirement

* .NET 10  

## Author

[zenuas@GitHub](https://github.com/zenuas)  
