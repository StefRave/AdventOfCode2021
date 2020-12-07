using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day5
    {
        private readonly ITestOutputHelper output;

        public Day5(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay5()
        {
            var input = File.ReadAllLines("input/input5.txt")
                .Select(s => s.Aggregate(0, (a, c) => a * 2 + ((c & 4 ^ 4) >> 2)))
                .OrderBy(s => s)
                .ToArray();

            output.WriteLine($"Part1: {input[^1]}");

            int result = input.Where((s, i) => s != input[0] + i).First() - 1;
            output.WriteLine($"Part2: {result}");
        }
    }
}