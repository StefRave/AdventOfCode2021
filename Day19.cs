using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        readonly string testInput = @"0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: ""a""
5: ""b""";

        [Theory]
        [InlineData(0, "abbbab")]
        public void PossiblePattern(int ruleNr, string pattern)
            => Assert.True(pattern.Length == MatchLength(pattern.ToCharArray(), GetInstructions(testInput), ruleNr));

        [Fact]
        public void DoDay19()
        {
            var input = File.ReadAllText("input/input19.txt").SplitByDoubleNewLine();
            Dictionary<int, object> instructions = GetInstructions(input[0]);
            var data = input[1].SplitByNewLine();

            int result = CountValid(instructions, data);
            output.WriteLine($"Part1: {result}");

            instructions[8] = ParseNumbers("42 | 42 8");
            instructions[11] = ParseNumbers("42 31 | 42 11 31");
            
            result = CountValid(instructions, data);
            output.WriteLine($"Part2: {result}");
        }

        private int CountValid(Dictionary<int, object> instructions, string[] data) =>
            data.Where(l => MatchLength(l.ToCharArray(), instructions, 0) == l.Length).Count();

        private int MatchLength(Span<char> line, Dictionary<int, object> instructions, int ruleNr)
        {
            object instruction = instructions[ruleNr];
            if (instruction is char)
                return line[0] == (char)instruction ? 1 : 0;

            var options = (int[][])instruction;
            foreach (int[] numbers in options)
            {
                var leftToMatch = line;
                bool fail = false;
                foreach (int number in numbers)
                {
                    if (leftToMatch.Length == 0)
                    {
                        fail = true;
                        break;
                    }
                    int length = MatchLength(leftToMatch, instructions, number);
                    if (length == 0)
                    {
                        fail = true;
                        break;
                    }
                    leftToMatch = leftToMatch.Slice(length);
                }
                if (!fail)
                    return line.Length - leftToMatch.Length;
            }
            return 0;
        }

        private Dictionary<int, object> GetInstructions(string input)
        {
            return input.SplitByNewLine()
                .Select(l => l.Split(": "))
                .ToDictionary(split => int.Parse(split[0]), split => split[1][0] == '"' ? (object)split[1][1] : ParseNumbers(split[1]));
        }

        private int[][] ParseNumbers(string v) => v.Split(" | ").Select(l => l.Split(" ").Select(int.Parse).ToArray()).ToArray();
    }
}