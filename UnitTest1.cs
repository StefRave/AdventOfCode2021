using System;
using System.IO;
using System.Linq;
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

                if(input1[i1] + input1[i2] == 2020)
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
                    Character = r.Groups[3].Value,
                    Password = r.Groups[4].Value,
                };
            var result =
                (
                from c in input
                let cnt = Regex.Matches(c.Password, c.Character).Count
                where cnt >= c.Min && cnt <= c.Max
                select 1
                ).Count();
            
            output.WriteLine($"Part1: {result}");

            result =
                (
                from c in input
                where c.Password[c.Min - 1] == c.Character[0] ^ c.Password[c.Max - 1] == c.Character[0]
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

                    if(ValidateNumberInRange(byr, 1920, 2002) &&
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
    }
}
