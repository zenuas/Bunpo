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
| Method          | Mean          | Error        | StdDev       | Gen0     | Gen1   | Allocated |
|---------------- |--------------:|-------------:|-------------:|---------:|-------:|----------:|
| BunpoSetup      |     113.91 ns |     1.914 ns |     1.697 ns |   0.1274 | 0.0007 |    1600 B |
| Bunpo5Parse     |      79.79 ns |     1.348 ns |     1.261 ns |        - |      - |         - |
| Bunpo300Parse   |   5,162.57 ns |    77.373 ns |    72.374 ns |        - |      - |         - |
| FParseSetup     |     448.28 ns |     5.402 ns |     4.511 ns |   0.4959 | 0.0105 |    6232 B |
| FParse5Parse    |     276.17 ns |     2.908 ns |     2.720 ns |   0.0324 |      - |     408 B |
| FParse300Parse  |  16,797.90 ns |   201.356 ns |   178.496 ns |   1.6479 |      - |   20968 B |
| SpracheSetup    |     164.69 ns |     3.281 ns |     2.908 ns |   0.1931 | 0.0017 |    2424 B |
| Sprache5Parse   |   6,145.05 ns |    93.162 ns |    82.586 ns |   3.4332 | 0.0458 |   43144 B |
| Sprache300Parse | 384,084.63 ns | 7,396.391 ns | 7,264.245 ns | 209.9609 | 7.8125 | 2634944 B |

FParsec.CSharp version 12.6.0(FParsec version 2.0.0-beta2) used.  
Sprache version 2.3.1 used.  

## Requirement

* .NET 10  

## Author

[zenuas@GitHub](https://github.com/zenuas)  
