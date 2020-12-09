using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day9Bogo
    {
        private readonly ITestOutputHelper output;
        private Random rnd = new();

        public Day9Bogo(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay9Bogo()
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
                bool[] found = new bool[input.Length];
                for(int i = 0; i < 10000000; i++)
                {
                    int start = rnd.Next(sumOf, input.Length);
                    int offset1 = rnd.Next(0, sumOf - 1);
                    int offset2 = rnd.Next(offset1 + 1, sumOf);
                    found[start] |= input[start] == input[start - offset1 - 1] + input[start - offset2 - 1];
                }
                return input[found.Select((b, i) => (b, i)).Single(c => c.i > sumOf && !c.b).i];
            }

            (int offset, int length) FindContiguousSetOfNumbers(long sumToBeFound)
            {
                while(true)
                {
                    int offset = rnd.Next(0, input.Length - 2);
                    int end = rnd.Next(offset + 2, input.Length);
                    if (input.Skip(offset).Take(end - offset).Sum() == sumToBeFound)
                        return (offset, end - offset);
                }
            }
        }
    }
}