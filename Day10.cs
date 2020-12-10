using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day10
    {
        private readonly ITestOutputHelper output;

        public Day10(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay10()
        {
            var input =
                File.ReadAllLines("input/input10.txt")
                .Select(int.Parse)
                .OrderBy(c => c)
                .ToArray();
            input = input.Concat(new[] { input[^1] + 3 }).ToArray();

            int jolt = 0;
            var diffs = input.Select(i => { int diff = i - jolt; jolt = i; return diff; }).ToArray();

            var groupedDiffs =
                from i in diffs
                group i by i into g
                select g;
            var diffDict = groupedDiffs.ToDictionary(c => c.Key, c => c.Count());

            int result = diffDict[1] * (diffDict[3]);
            output.WriteLine($"Part1: {result}");

            var r = new long[input[^1] + 1];
            r[0] = 1;
            foreach(int i in input)
            {
                for (int j = Math.Max(0, i - 3); j < i; j++)
                    r[i] += r[j];
            }
            output.WriteLine($"Part2: {r[^1]}");
        }
    }
}