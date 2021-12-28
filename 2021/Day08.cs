namespace AdventOfCode2021;

public class Day08 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(" | "))
            .Select(parts => (signal: Parse(parts[0]), output: Parse(parts[1])))
            .ToArray();

        string[] Parse(string parts) => parts.Split(' ').Select(ToOrderedString).ToArray();
        string ToOrderedString(string s) => new string(s.OrderBy(a => a).ToArray());

        var numbersToSum = new[] { 2, 4, 3, 7 };
        int answer1 = input
            .SelectMany(line => line.output)
            .Count(s => numbersToSum.Contains(s.Length));
        Advent.AssertAnswer1(answer1);

        int answer2 = input.Sum(line => GetOutputValue(line.signal, line.output));
        Advent.AssertAnswer2(answer2);
    }

    private static int GetOutputValue(string[] signal, string[] output)
    {
        string[] digits = new string[10];
        var signals = signal.ToHashSet();
        digits[8] = SelectAndRemove(7);
        digits[1] = SelectAndRemove(2);
        digits[4] = SelectAndRemove(4);
        digits[7] = SelectAndRemove(3);
        digits[9] = SelectAndRemove(6, s => HasAll(digits[7], s) && HasAll(digits[4], s));
        digits[0] = SelectAndRemove(6, s => HasAll(digits[7], s));
        digits[6] = SelectAndRemove(6);
        digits[3] = SelectAndRemove(5, s => HasAll(digits[7], s));
        digits[5] = SelectAndRemove(5, s => HasAll(s, digits[9]));
        digits[2] = SelectAndRemove(5);

        return output.Aggregate(0, (acc, val) => acc * 10 + Array.IndexOf(digits, val));

        bool HasAll(string digit, string segment)
            => !digit.Except(segment).Any();

        string SelectAndRemove(int numberOfSegments, Func<string, bool> predicate = null)
        {
            string result = signals.Single(o => o.Length == numberOfSegments && (predicate == null || predicate(o)));
            signals.Remove(result);
            return result;
        }
    }
}
