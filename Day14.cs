namespace AdventOfCode2021;

public class Day14
{
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine();
        string polymerTemplate = input[0];
        var insertionRules = input[1]
            .SplitByNewLine()
            .Select(line => line.Split(" -> "))
            .ToDictionary(ir => ir[0], ir => ir[1]);

        var pairs = CreatePairs(polymerTemplate);

        Advent.AssertAnswer1(Solve(pairs, 10));
        Advent.AssertAnswer2(Solve(pairs, 40));

        long Solve(Dictionary<string, long> pairs, int iterations)
        {
            for (int step = 0; step < iterations; step++)
                pairs = DoIteration(insertionRules, pairs);

            return CountMostMinusLeast(polymerTemplate, pairs);
        }
    }

    private Dictionary<string, long> DoIteration(Dictionary<string, string> insertionRules, Dictionary<string, long> pairs)
    {
        var newPairs = new Dictionary<string, long>();
        foreach (var (key, count) in pairs)
        {
            if (insertionRules.TryGetValue(key, out string insertChar))
            {
                newPairs.Update(key[0..1] + insertChar, oldCount => oldCount + count);
                newPairs.Update(insertChar + key[1..2], oldCount => oldCount + count);
            }
            else
                newPairs.Update(key, oldCount => oldCount + count);
        }
        return newPairs;
    }

    private Dictionary<string, long> CreatePairs(string polymerTemplate)
    {
        var pairs = new Dictionary<string, long>();
        for (int i = 0; i < polymerTemplate.Length - 1; i++)
            pairs.Update(polymerTemplate.Substring(i, 2), old => old + 1);
        return pairs;
    }

    private long CountMostMinusLeast(string polymerTemplate, Dictionary<string, long> pairs)
    {
        var countPerChar = new Dictionary<char, long>();
        countPerChar[polymerTemplate[^1]] = 1;
        foreach (var (key, count) in pairs)
            countPerChar.Update(key[0], oldCount => oldCount + count);

        var elementCount = countPerChar
            .OrderBy(pc => pc.Value).ToArray();

        long result = elementCount[^1].Value - elementCount[0].Value;
        return result;
    }
}