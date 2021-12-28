using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day2
    {
        private readonly ITestOutputHelper output;

        public Day2(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay2()
        {
            var input =
                from l in File.ReadAllLines("input/input2.txt")
                let r = Regex.Match(l, @"(\d+)-(\d+) (\w): (\w+)")
                select new
                {
                    Min = int.Parse(r.Groups[1].Value),
                    Max = int.Parse(r.Groups[2].Value),
                    Character = r.Groups[3].Value[0],
                    Password = r.Groups[4].Value,
                };
            var result =
                (
                from c in input
                let cnt = c.Password.Count(ch => ch == c.Character)
                where cnt >= c.Min && cnt <= c.Max
                select 1
                ).Count();

            output.WriteLine($"Part1: {result}");

            result =
                (
                from c in input
                where c.Password[c.Min - 1] == c.Character ^ c.Password[c.Max - 1] == c.Character
                select 1
                ).Count();

            output.WriteLine($"Part2: {result}");
        }
    }
}