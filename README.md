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
[Naming conventions](https://github.com/zenuas/Bunpo/blob/master/test/NameTest.cs)  

## Benchmark

This is a benchmark for Four arithmetic operations parser setup and parser execution.  
The parser compared formulas with 5 numbers and formulas with 300 numbers.  

Four arithmetic operations benchmark:  
| Method          | Mean          | Error        | StdDev       | Gen0     | Gen1   | Allocated |
|---------------- |--------------:|-------------:|-------------:|---------:|-------:|----------:|
| BunpoSetup      |      88.15 ns |     0.741 ns |     0.657 ns |   0.1045 | 0.0005 |    1312 B |
| Bunpo5Parse     |      78.28 ns |     0.621 ns |     0.550 ns |        - |      - |         - |
| Bunpo300Parse   |   5,063.21 ns |    70.120 ns |    65.590 ns |        - |      - |         - |
| FParseSetup     |     439.70 ns |     6.466 ns |     5.049 ns |   0.4964 | 0.0110 |    6232 B |
| FParse5Parse    |     273.23 ns |     1.940 ns |     1.720 ns |   0.0324 |      - |     408 B |
| FParse300Parse  |  16,480.95 ns |   158.952 ns |   148.684 ns |   1.6479 |      - |   20968 B |
| SpracheSetup    |     164.50 ns |     2.630 ns |     2.196 ns |   0.1931 | 0.0017 |    2424 B |
| Sprache5Parse   |   5,913.91 ns |    30.324 ns |    23.675 ns |   3.4332 | 0.0458 |   43144 B |
| Sprache300Parse | 394,746.39 ns | 7,008.131 ns | 5,852.107 ns | 209.4727 | 7.8125 | 2631104 B |
| ManualSetup     |      19.04 ns |     0.381 ns |     0.408 ns |   0.0191 |      - |     240 B |
| Manual5Parse    |      49.17 ns |     0.186 ns |     0.165 ns |        - |      - |         - |
| Manual300Parse  |   3,080.70 ns |    10.874 ns |     8.490 ns |        - |      - |         - |

FParsec.CSharp version 12.6.0(FParsec version 2.0.0-beta2) used.  
Sprache version 2.3.1 used.  
Manual is LL parser written in C#.  
[Benchmark source code](https://github.com/zenuas/Bunpo/blob/master/bench/StaticCalculationBench.cs)  

## Installation

* [NuGet](https://www.nuget.org/packages/Bunpo/)  
  dotnet add package Bunpo  

## Requirement

* .NET 10  

## Author

[zenuas@GitHub](https://github.com/zenuas)  
