using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day19
    {
        private readonly ITestOutputHelper output;

        public Day19(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay19()
        {
            var input = File.ReadAllText("input/input19.txt").SplitByDoubleNewLine();
            Dictionary<int, object> instructions = input[0]
                .SplitByNewLine()
                .Select(l => l.Split(": "))
                .ToDictionary(split => int.Parse(split[0]), split => split[1][0] == '"' ? (object)split[1][1] : ParseNumbers(split[1]));
            var data = input[1].SplitByNewLine();



            int result = CountValid(instructions, data);
            output.WriteLine($"Part1: {result}");

            instructions[8] = ParseNumbers("42 | 42 8");
            instructions[11] = ParseNumbers("42 31 | 42 11 31");
            result = CountValid(instructions, data);
            output.WriteLine($"Part2: {result}");

            int[][] ParseNumbers(string v) => v.Split(" | ").Select(l => l.Split(" ").Select(int.Parse).ToArray()).ToArray();
        }

        private int CountValid(Dictionary<int, object> instructions, string[] data)
        {
            return data.Where(l => MatchRuleNr(l.ToCharArray(), 0).Any(length => length == l.Length)).Count();

            ICollection<int> MatchRuleNr(Span<char> line, int ruleNr)
            {
                if (line.Length == 0)
                    return new int[0];
                return instructions[ruleNr] switch
                {
                    char c => line[0] == c ? new int[] { 1 } : new int[0],
                    int[][] options => MatchOptions(line, options),
                    _ => throw new NotImplementedException()
                };
            }

            ICollection<int> MatchOptions(Span<char> line, int[][] options)
            {
                List<int> results = new();
                foreach (int[] numbers in options)
                    results.AddRange(Match(numbers, line));
                return results;

                ICollection<int> Match(Span<int> numbers, Span<char> leftToMatch)
                {
                    List<int> results = new();

                    int number = numbers[0];
                    numbers = numbers.Slice(1);
                    ICollection<int> lengths = MatchRuleNr(leftToMatch, number);
                    if (numbers.Length == 0)
                        return lengths;

                    foreach (int length in lengths)
                    {
                        foreach (int matchLength in Match(numbers, leftToMatch.Slice(length)))
                            results.Add(matchLength + length);
                    }
                    return results;
                }
            }
        }
    }
}