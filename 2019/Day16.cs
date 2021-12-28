using AdventOfCode2019.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2019
{
    public class Day16
    {
        private static string GetInput() => File.ReadAllText(@"Input/input16.txt");
        private Random rnd = new Random();
        private static readonly int[] basePattern = new[] { 0, 1, 0, -1 };

        private static IEnumerable<int> GetPattern(int outputNr)
        {
            for (int j = 0; ; j++)
                yield return basePattern[j / outputNr % basePattern.Length];
        }

        [Theory]
        [InlineData(2, "0,0,1,1,0,0,-1,-1,0,0,1,1,0,0,-1,-1")]
        [InlineData(1, "0,1,0,-1,0,1,0,-1,0,1,0,-1")]
        public void PatternRepeatsLikeIntended(int outputNr, string pattern)
        {
            var intPattern = pattern.Split(',').Select(int.Parse).ToArray();

            GetPattern(outputNr).Take(intPattern.Length).Should().BeEquivalentTo(intPattern);
        }

        [Theory]
        [InlineData("12345678", 4, "01029498")]
        //[InlineData("80871224585914546619083218645595", 100, "24176176")]
        public void TestPart1Example(string inputSignal, int iterations, string expected)
        {
            DoFft(inputSignal, iterations).Substring(0, expected.Length).Should().Be(expected);
        }

        [Theory]
        [InlineData("03036732577212944063491565474664", 100, "84462026")]
        public void TestPart2(string inputSignal, int iterations, string expected)
        {
            DoFft2(inputSignal, iterations).Should().Be(expected);
        }

        private static string DoFft(string inputSignal, int iterations)
        {
            int[] result1 = new int[inputSignal.Length];
            int[] result2 = new int[inputSignal.Length];
            int[] src = inputSignal.Select(c => c - '0').ToArray();
            int[] result = null;
            for (int j = 0; j < iterations; j++)
            {
                result = src == result2 ? result1: result2;
                
                for (int i = 0; i < inputSignal.Length; i++)
                {
                    int calc = 0;
                    for (int k = 0; k < src.Length; k++)
                        calc += src[k] * basePattern[(k + 1) / (i + 1) % basePattern.Length];
                    result[i] = Math.Abs(calc) % 10;
                }
                src = result;
            }
            return new string(result.Select(i => (char)(i + '0')).ToArray());
        }
        private static string DoFft2(string inputSignal, int iterations)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 10000; i++)
                sb.Append(inputSignal);

            // the offset in the assignment is in the last half of the input signal * 10000
            // the pattern is all 1,1,1,1,1,1 at offset and 0,1,1,1,1,1 at offset + 1
            int offset = int.Parse(inputSignal[0..7]);
            inputSignal = sb.ToString()[offset..];

            for (int i = 0; i < iterations; i++)
            {
                int total = inputSignal.Sum(c => c - '0');
                sb = new StringBuilder();
                sb.Append(total % 10);
                for (int j = 1; j < inputSignal.Length; j++)
                {
                    total -= inputSignal[j - 1] - '0';
                    sb.Append(total % 10);
                }
                inputSignal = sb.ToString();
            }
            return inputSignal[0..8];
        }

        [Fact]
        public void DoParts()
        {
            string inputSignal = GetInput();
            inputSignal = inputSignal.Substring(0, inputSignal.Length);

            string result = DoFft(inputSignal, 100).Substring(0, 8);
            Assert.Equal("42205986", result);

            result = DoFft2(inputSignal, 100);
            Assert.Equal("13270205", result);
        }
    }
}
