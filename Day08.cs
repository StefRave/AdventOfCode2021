namespace AdventOfCode2021;

public class Day08
{
    private readonly ITestOutputHelper output;

    public Day08(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(" | "))
            .Select(parts => (signal: Parse(parts[0]), output: Parse(parts[1])))
            .ToArray();

        string[] Parse(string parts) => parts.Split(' ').Select(ToOrderedString).ToArray();
        string ToOrderedString(string s) => new string(s.OrderBy(a => a).ToArray());

        int eight = input.Sum(line => line.output.Count(s => s.Length == 7));
        int one = input.Sum(line => line.output.Count(s => s.Length == 2));
        int four = input.Sum(line => line.output.Count(s => s.Length == 4));
        int seven = input.Sum(line => line.output.Count(s => s.Length == 3));
        Advent.AssertAnswer1(eight+one+four+seven);

        int answer2 = input.Sum(GetOutputValu);
        Advent.AssertAnswer2(answer2);
    }

    private static int GetOutputValu((string[] signal, string[] output) line)
    {
        string[] digits = new string[10];
        digits[8] = line.signal.Single(s => s.Length == 7);
        digits[1] = line.signal.Single(s => s.Length == 2);
        digits[4] = line.signal.Single(s => s.Length == 4);
        digits[7] = line.signal.Single(s => s.Length == 3);

        // 0 6
        // 1 2
        // 2 5
        // 3 5
        // 4 4
        // 5 5
        // 6 6
        // 7 3
        // 8 7
        // 9 6        
        var length6 = line.signal.Where(s => s.Length == 6).ToHashSet();
        var length5 = line.signal.Where(s => s.Length == 5).ToHashSet();
        digits[9] = SelectAndRemove(length6, s => HasAll(7, s) && HasAll(4, s));
        digits[0] = SelectAndRemove(length6, s => HasAll(7, s));
        digits[6] = length6.Single();
        digits[3] = SelectAndRemove(length5, s => HasAll(7, s));
        digits[5] = SelectAndRemove(length5, s => HasOneMissing(9, s));
        digits[2] = length5.Single();

        int outputNumber = 0;
        foreach (var outputDigit in line.output)
            outputNumber = outputNumber * 10 + Array.IndexOf(digits, outputDigit);
        return outputNumber;

        bool HasAll(int digit, string segment) => digits[digit].All(d => segment.Contains(d));
        bool HasOneMissing(int digit, string segment) => digits[digit].Count(d => !segment.Contains(d)) == 1;
        string SelectAndRemove(HashSet<string> options, Func<string, bool> predicate)
        {
            string result = options.Single(predicate);
            options.Remove(result);
            return result;
        }
    }
}
