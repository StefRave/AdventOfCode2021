using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay1()
        {
            var input1 = File.ReadAllLines("input1.txt")
                .Select(c => int.Parse(c))
                .ToArray();

            Random rnd = new Random();

            while (true)
            {
                int i1 = rnd.Next(0, input1.Length);
                int i2 = rnd.Next(0, input1.Length);

                if (input1[i1] + input1[i2] == 2020)
                {
                    output.WriteLine("{0}", input1[i1] * input1[i2]);
                    break;
                }
            }

            while (true)
            {
                int i1 = rnd.Next(0, input1.Length);
                int i2 = rnd.Next(0, input1.Length);
                int i3 = rnd.Next(0, input1.Length);

                if (input1[i1] + input1[i2] + input1[i3] == 2020)
                {
                    output.WriteLine("{0}", input1[i1] * input1[i2] * input1[i3]);
                    break;
                }
            }
        }

        [Fact]
        public void DoDay2()
        {
            var input =
                from l in File.ReadAllLines("input2.txt")
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

        [Fact]
        public void DoDay3()
        {
            var input = File.ReadAllLines("input3.txt");
            int width = input[0].Length;

            long result = CountTrees(3, 1);
            output.WriteLine($"Part1: {result}");

            result =
                CountTrees(1, 1) *
                CountTrees(3, 1) *
                CountTrees(5, 1) *
                CountTrees(7, 1) *
                CountTrees(1, 2);
            output.WriteLine($"Part2: {result}");


            long CountTrees(int dx, int dy)
            {
                int trees = 0;
                int x = 0;
                for (int y = 0; y < input.Length; y += dy)
                {
                    if (input[y][x % width] == '#')
                        trees++;
                    x += dx;
                }
                return trees;
            }

        }

        [Fact]
        public void DoDay4()
        {
            var validEyeColors = "amb blu brn gry grn hzl oth".Split(' ').ToHashSet();

            var input = File.ReadAllText("input4.txt");

            var candidates = Regex.Split(input, @"\r?\n\r?\n", RegexOptions.Singleline);
            int resultPresent = 0;
            int resultValidationOk = 0;
            foreach (var candidate in candidates)
            {
                var keyValues = Regex.Matches(candidate, @"(\w+):(#?\w+)")
                    .ToDictionary(r => r.Groups[1].Value, r => r.Groups[2].Value);

                if (keyValues.TryGetValue("byr", out var byr) &&
                    keyValues.TryGetValue("iyr", out var iyr) &&
                    keyValues.TryGetValue("eyr", out var eyr) &&
                    keyValues.TryGetValue("hgt", out var hgt) &&
                    keyValues.TryGetValue("hcl", out var hcl) &&
                    keyValues.TryGetValue("ecl", out var ecl) &&
                    keyValues.TryGetValue("pid", out var pid))
                {
                    resultPresent++;

                    if (ValidateNumberInRange(byr, 1920, 2002) &&
                        ValidateNumberInRange(iyr, 2010, 2020) &&
                        ValidateNumberInRange(eyr, 2020, 2030) &&
                        ValidateHeight(hgt) &&
                        Regex.Match(hcl, @"^#[0-9a-f]{6}$").Success &&
                        validEyeColors.Contains(ecl) &&
                        Regex.Match(pid, @"^\d{9}$").Success)
                    {
                        resultValidationOk++;
                    }
                }
            }
            output.WriteLine($"Part1: {resultPresent}");
            output.WriteLine($"Part2: {resultValidationOk}");

            bool ValidateNumberInRange(string val, int minIncl, int maxIncl) =>
                int.TryParse(val, out int number) && number >= minIncl && number <= maxIncl;
            bool ValidateHeight(string val)
            {
                string height = val[0..^2];

                return val[^2..] switch
                {
                    "cm" => ValidateNumberInRange(height, 150, 193),
                    "in" => ValidateNumberInRange(height, 59, 76),
                    _ => false
                };
            }
        }

        [Fact]
        public void DoDay5()
        {
            var input = File.ReadAllLines("input5.txt")
                .Select(s => s.Aggregate(0, (a, c) => a * 2 + ((c & 4 ^ 4) >> 2)))
                .OrderBy(s => s)
                .ToArray();

            output.WriteLine($"Part1: {input[^1]}");

            int result = input.Where((s, i) => s != input[0] + i).First() - 1;
            output.WriteLine($"Part2: {result}");
        }

        [Fact]
        public void DoDay6()
        {
            var input = File.ReadAllText("input6.txt");

            string[][] voteLinesPerGroup =
                SplitByDoubleNewLine(input)
                .Select(SplitByNewLine)
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

        public static string[] SplitByNewLine(string input)
            => input.Split(new string[]{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        public static string[] SplitByDoubleNewLine(string input)
            => input.Split(new string[]{"\r\n\r\n", "\n\n"}, StringSplitOptions.RemoveEmptyEntries);
    }
}
