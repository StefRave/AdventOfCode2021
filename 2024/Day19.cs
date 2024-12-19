using CommandLine;

namespace AdventOfCode2024;

public class Day19 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        string[] towels = input[0].Split(", ");
        string[] patterns = input[1].SplitByNewLine();
        int shortestTowel = towels.Min(t => t.Length);
        int answer1 = 0;

        foreach (var pattern in patterns)
            if (CanBeMatchedWithTowels(pattern))
                answer1++;
        Advent.AssertAnswer1(answer1, expected: 319, sampleExpected: 6);


        long answer2 = 0;
        foreach (var pattern in patterns)
            answer2 += PossibleMatches(pattern);
        Advent.AssertAnswer2(answer2, expected: 692575723305545, sampleExpected: 16);


        long PossibleMatches(string pattern)
        {
            var cache = new Dictionary<int, long>();

            return Possible();


            long Possible(int index = 0)
            {
                if (cache.TryGetValue(index, out long solutions))
                    return solutions;

                foreach (var towel in towels)
                {
                    if (pattern[index..].StartsWith(towel))
                    {
                        if (pattern.Length == index + towel.Length)
                            solutions++;
                        if (pattern.Length >= index + towel.Length + shortestTowel)
                            solutions += Possible(index + towel.Length);
                    }
                }
                cache.Add(index, solutions);
                return solutions;
            }
        }

        bool CanBeMatchedWithTowels(string pattern, int index = 0)
        {
            foreach (var towel in towels)
            {
                if (pattern[index..].StartsWith(towel))
                {
                    if (pattern.Length == index + towel.Length)
                        return true;
                    bool match = CanBeMatchedWithTowels(pattern, index + towel.Length);
                    if (match)
                        return true;
                }
            }
            return false;
        }
    }
}