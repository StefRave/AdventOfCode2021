using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day9
    {
        private readonly ITestOutputHelper output;

        public Day9(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay9()
        {
            var input =
                File.ReadAllLines("input/input9.txt")
                .Select(long.Parse)
                .ToArray();

            long result = FindNumber(25);
            output.WriteLine($"Part1: {result}");

            (int offset, int length) = FindContiguousSetOfNumbers(result);
            long min = input.Skip(offset).Take(length).Min();
            long max = input.Skip(offset).Take(length).Max();
            output.WriteLine($"Part2: {min+max}");

            long FindNumber(int sumOf)
            {
                for (int i = sumOf; i < input.Length; i++)
                {
                    for (int a = -sumOf; a < -1; a++)
                        for (int b = a + 1; b < 0; b++)
                            if (input[i + a] + input[i + b] == input[i])
                                goto there;
                    return input[i];
                there:;
                }
                throw new Exception("Not found");
            }

            (int offset, int length) FindContiguousSetOfNumbers(long sumToBeFound)
            {
                for (int i = 0; i < input.Length - 1; i++)
                {
                    long sum = input[i];
                    for (int j = i + 1; j < input.Length; j++)
                    {
                        sum += input[j];
                        if (sum == sumToBeFound)
                            return (i, j - i);
                        if (sum > sumToBeFound)
                            break;
                    }
                }
                throw new Exception("Not found");
            }
        }
    }
}