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
            return data.Where(l => MatchRuleNr(l.ToCharArray().AsMemory(), 0).Any(length => length == l.Length)).Count();

            IEnumerable<int> MatchRuleNr(Memory<char> line, int ruleNr)
            {
                if (line.Length == 0)
                    return new int[0];
                return instructions[ruleNr] switch
                {
                    char c => line.Span[0] == c ? new int[] { 1 } : new int[0],
                    int[][] options => MatchOptions(line, options),
                    _ => throw new NotImplementedException()
                };
            }

            IEnumerable<int> MatchOptions(Memory<char> line, int[][] options)
            {
                return 
                    from int[] numbers in options
                    from number in MatchNumbers(line, numbers)
                    select number;
            }

            IEnumerable<int> MatchNumbers(Memory<char> leftToMatch, Memory<int> numbers)
            {
                int number = numbers.Span[0];
                IEnumerable<int> lengths = MatchRuleNr(leftToMatch, number);
                if (numbers.Length == 1)
                    return lengths;

                return
                    from int length in lengths
                    from int matchLength in MatchNumbers(leftToMatch.Slice(length), numbers.Slice(1))
                    select matchLength + length;
            }
        }
    }
}