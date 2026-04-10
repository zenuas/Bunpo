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
| BunpoSetup      |     113.0 ns |     1.71 ns |     1.51 ns |   0.1301 | 0.0007 |    1632 B |
| Bunpo5Parse     |     262.9 ns |     3.58 ns |     2.99 ns |   0.0577 |      - |     728 B |
| Bunpo300Parse   |  18,373.6 ns |   364.95 ns |   341.38 ns |   3.6011 |      - |   45360 B |
| FParseSetup     |     443.8 ns |     4.48 ns |     3.74 ns |   0.4964 | 0.0110 |    6232 B |
| FParse5Parse    |     272.8 ns |     3.04 ns |     2.84 ns |   0.0324 |      - |     408 B |
| FParse300Parse  |  16,870.5 ns |   265.66 ns |   248.50 ns |   1.6479 |      - |   20968 B |
| SpracheSetup    |     167.1 ns |     2.55 ns |     2.13 ns |   0.1931 | 0.0017 |    2424 B |
| Sprache5Parse   |   6,075.0 ns |    73.93 ns |    61.74 ns |   3.4332 | 0.0458 |   43144 B |
| Sprache300Parse | 396,664.4 ns | 7,665.94 ns | 9,125.76 ns | 209.9609 | 7.8125 | 2638784 B |

FParsec.CSharp version 12.6.0(FParsec version 2.0.0-beta2) used.  
Sprache version 2.3.1 used.  

## Requirement

* .NET 10  

## Author

[zenuas@GitHub](https://github.com/zenuas)  
