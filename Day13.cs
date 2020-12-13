using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static System.Math;

namespace AdventOfCode2020
{
    public class Day13
    {
        private readonly ITestOutputHelper output;

        public Day13(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay13()
        {
            var input = File.ReadAllLines("input/input13.txt");
            int depart = int.Parse(input[0]);
            int?[] lines = input[1].Split(',').Select(l => l == "x" ? (int?)null : int.Parse(l)).ToArray();

            var result = lines
                .Where(l => l != null)
                .Select(l => (line: l, depart: (depart + l - 1) / l * l))
                .OrderBy(x => x.depart)
                .First();
            output.WriteLine($"Part1: {result.line * (result.depart - depart)}");

            var lineOffset = lines
                .Select((l, offset) => (l, offset))
                .Where(a => a.l != null)
                .OrderByDescending(a => a.l)
                .Select(a => (rep: (long)a.l, offset: (long)a.offset))
                .Aggregate((a1, a2) => Find(a2.rep, a2.offset, a1.rep, a1.offset));

            output.WriteLine($"Part2: {lineOffset.rep - lineOffset.offset}");

            (long rep, long offset) Find(long v1, long offset1, long v2, long offset2)
            {
                for(long i = offset2; ; i += v2)
                    if ((i + v1 - offset1) % v1 == 0)
                        return (v1 * v2, i);
            }
        }
    }
}