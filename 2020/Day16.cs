using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day16
    {
        private readonly ITestOutputHelper output;

        public Day16(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay16()
        {
            var input = File.ReadAllText("input/input16.txt")
                .SplitByDoubleNewLine();

            var tokenRanges =
                Regex.Matches(input[0], @"(.*): (?:(\d+)-(\d+)(?: or )?)+")
                .ToDictionary(
                    m => m.Groups[1].Value,
                    m => m.Groups[2].Captures.Select((c, i) =>
                    (
                        from: int.Parse(c.Value), 
                        upto: int.Parse(m.Groups[3].Captures[i].Value))).ToArray()
                    );

            int[] ticketNumbers = input[1].SplitByNewLine()[1].Split(',').Select(int.Parse).ToArray();
            int[][] nearbyTickets = input[2].SplitByNewLine().Skip(1).Select(t => t.Split(',').Select(int.Parse).ToArray()).ToArray();


            long result = nearbyTickets
                .SelectMany(i => i)
                .Where(num => !InAnyRange(num))
                .Sum();
            output.WriteLine($"Part1: {result}");

            var validNearbyTickets = nearbyTickets
                .Where(t => t.All(InAnyRange))
                .ToArray();
            Dictionary<string, HashSet<int>> possibleIndexesPerToken = tokenRanges
                .ToDictionary(kv => kv.Key, kv => GetIndexesInRange(kv.Value, validNearbyTickets).ToHashSet());
            var tokenIndex = GetIndexPerToken(possibleIndexesPerToken);
            result = tokenIndex
                .Where(kv => kv.Key.StartsWith("departure"))
                .Aggregate(1L, (v, kv) => v * ticketNumbers[kv.Value]);
            output.WriteLine($"Part2: {result}");


            bool InAnyRange(int num) => 
                tokenRanges.Any(kv => InRange(num, kv.Value));

            bool InRange(int num, (int from, int upto)[] ranges) =>
                ranges.Any(ft => num >= ft.from && num <= ft.upto);

            IEnumerable<int> GetIndexesInRange((int from, int upto)[] ranges, int[][] tickets) =>
                Enumerable.Range(0, tickets[0].Length)
                    .Where(i => HasAllValuesInRange(tickets.Select(nt => nt[i]).ToArray(), ranges));

            bool HasAllValuesInRange(int[] values, (int from, int upto)[] ranges) =>
                values.All(num => InRange(num, ranges));


            Dictionary<string, int> GetIndexPerToken(Dictionary<string, HashSet<int>> indexesPerToken)
            {
                Dictionary<string, int> tokenNumber = new();

                while (indexesPerToken.Any())
                {
                    foreach (var kv in indexesPerToken.Where(v => v.Value.Count == 1))
                    {
                        int value = kv.Value.First();
                        tokenNumber.Add(kv.Key, value);
                        indexesPerToken.Remove(kv.Key);
                        foreach (var kv2 in indexesPerToken)
                            kv2.Value.Remove(value);
                    }
                }
                return tokenNumber;
            }
        }
    }
}