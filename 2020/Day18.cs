using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day18
    {
        private readonly ITestOutputHelper output;

        public Day18(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 71)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 26)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
        public void DoPart1Examples(string expression, long expectedResult) =>
            Assert.Equal(expectedResult, Calculate(expression));

        [Theory]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 231)]
        [InlineData("2 * 3 + (4 * 5)", 46)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340)]
        public void DoPart2Examples(string expression, long expectedResult) =>
            Assert.Equal(expectedResult, Calculate(expression, additionFirst: true));

        [Fact]
        public void DoDay18()
        {
            var input = File.ReadAllLines("input/input18.txt");

            long result = input.Select(e => Calculate(e, additionFirst: false)).Sum();
            output.WriteLine($"Part1: {result}");

            result = input.Select(e => Calculate(e, additionFirst: true)).Sum();
            output.WriteLine($"Part1: {result}");
        }

        public long Calculate(string expression, bool additionFirst = false)
        {
            return Calculate(expression.Replace(" ", "").GetEnumerator());

            
            long Calculate(IEnumerator<char> input)
            {
                long result = GetNumber(input);
                while (input.MoveNext())
                {
                    if (char.IsDigit(input.Current))
                        throw new ArgumentOutOfRangeException("expr", $"Unexpected digit");
                    char c = input.Current;
                    if (c == ')')
                        break;

                    bool evaluateFirst = c == '*' && additionFirst;
                    long nextValue = evaluateFirst ? 
                        Calculate(input) :
                        GetNumber(input);

                    result = c switch
                    {
                        '+' => result + nextValue,
                        '*' => result * nextValue,
                        _ => throw new ArgumentOutOfRangeException("expr", $"Unexpected expression '{c}'")
                    };
                    if (evaluateFirst)
                        break;
                }
                return result;
            }

            long GetNumber(IEnumerator<char> input)
            {
                if (!input.MoveNext())
                    throw new Exception("Unexpected end of expression");

                return input.Current switch
                {
                    '(' => Calculate(input),
                    char c when char.IsDigit(c) => c - '0',
                    _ => throw new ArgumentOutOfRangeException("expr", $"Unexpected expression '{input.Current}'")
                };
            }
        }
    }
}