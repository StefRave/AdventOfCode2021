using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day6
    {
        private readonly ITestOutputHelper output;

        public Day6(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay6()
        {
            var input = File.ReadAllText("input/input6.txt");

            string[][] voteLinesPerGroup =
                input.SplitByDoubleNewLine()
                .Select(s => s.SplitByNewLine())
                .ToArray();

            var result = voteLinesPerGroup
                .Select(group => string.Join("", group).Distinct())
                .Sum(c => c.Count());

            output.WriteLine($"Part1: {result}");
#if USE_INTERSECT
            result = voteLinesPerGroup
                .Select(voteLines => voteLines.Aggregate<IEnumerable<char>>((a, b) => a.Intersect(b)).Count())
                .Sum();
#else
            result =
                (
                from voteLines in voteLinesPerGroup
                select GetCorrespondingVoteCount(voteLines)
                ).Sum();
#endif

            output.WriteLine($"Part2: {result}");

            int GetCorrespondingVoteCount(string[] voteLines)
            {
                long letterMask = 0b11111111111111111111111111;
                foreach (var votes in voteLines.Where(l => l != ""))
                {
                    long letterFound = 0;
                    foreach (var letter in votes)
                        letterFound |= 1L << (letter - 'a');
                    letterMask &= letterFound;
                }
                return BitOperations.PopCount((ulong)letterMask);
            }
        }
    }
}