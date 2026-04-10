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

## Requirement

* .NET 10  

## Author

[zenuas@GitHub](https://github.com/zenuas)  
