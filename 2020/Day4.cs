using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day4
    {
        private readonly ITestOutputHelper output;

        public Day4(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay4()
        {
            var validEyeColors = "amb blu brn gry grn hzl oth".Split(' ').ToHashSet();

            var input = File.ReadAllText("input/input4.txt");

            var candidates = input.SplitByDoubleNewLine();
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
    }
}