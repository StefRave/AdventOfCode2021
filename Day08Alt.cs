namespace AdventOfCode2021;

public class Day08Alt
{
    private readonly ITestOutputHelper output;

    public Day08Alt(ITestOutputHelper output)
    {
        this.output = output;
    }

    string DigitsInput = @"
  0:      1:      2:      3:      4:
 aaaa    ....    aaaa    aaaa    ....
b    c  .    c  .    c  .    c  b    c
b    c  .    c  .    c  .    c  b    c
 ....    ....    dddd    dddd    dddd
e    f  .    f  e    .  .    f  .    f
e    f  .    f  e    .  .    f  .    f
 gggg    ....    gggg    gggg    ....

  5:      6:      7:      8:      9:
 aaaa    aaaa    aaaa    aaaa    aaaa
b    .  b    .  .    c  b    c  b    c
b    .  b    .  .    c  b    c  b    c
 dddd    dddd    ....    dddd    dddd
.    f  e    f  .    f  e    f  .    f
.    f  e    f  .    f  e    f  .    f
 gggg    gggg    ....    gggg    gggg
";

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(" | "))
            .Select(parts => (signal: Parse(parts[0]), output: Parse(parts[1])))
            .ToArray();

        string[] Parse(string parts) => parts.Split(' ').Select(ToOrderedString).ToArray();
        string ToOrderedString(string s) => new string(s.OrderBy(a => a).ToArray());

        var part1part2 = DigitsInput.SplitByDoubleNewLine();
        var digits = new List<string>();
        foreach (var part in part1part2)
        {
            var chunkedLine = part.SplitByNewLine().Skip(1).Select(l => l.Chunk(8).ToArray());
            for (int i = 0; i <= 4; i++)
                digits.Add(new string(chunkedLine.Select(a => a[i]).SelectMany(a => a).Where(c => char.IsLetter(c)).Distinct().OrderBy(c => c).ToArray()));
        }

        var numbersToSum = digits
            .GroupBy(d => d.Length)
            .Where(g => g.Count() == 1)
            .Select(g => g.Key)
            .ToArray();
        int answer1 = input
            .SelectMany(line => line.output)
            .Count(s => numbersToSum.Contains(s.Length));
        Advent.AssertAnswer1(answer1);

        int answer2 = input.Sum(line => GetOutputValue(line.signal, line.output, digits));
        Advent.AssertAnswer2(answer2);
    }

    private static int GetOutputValue(string[] inputSignal, string[] output, List<string> exampleDigits)
    {
        string[] digits = new string[10];
        var signalsPerLength = inputSignal.GroupBy(s => s.Length).ToDictionary(g => g.Key, g => g.ToHashSet());
        while(signalsPerLength.Any())
        {
            foreach (var (segments, signals) in signalsPerLength.Where(kv => kv.Value.Count == 1).ToArray())
            {
                for (int i = 0; i < digits.Length; i++)
                {
                    if (digits[i] == null && exampleDigits[i].Length == segments)
                    {
                        digits[i] = signals.First();
                        break;
                    }
                }
                signalsPerLength.Remove(segments);
            }
            for (int i = 0; i < digits.Length; i++)
            {
                if (digits[i] != null)
                    continue;
                var possibleSignals = signalsPerLength[exampleDigits[i].Length];

                for (int j = 0; j < digits.Length; j++)
                {
                    if (j == i || digits[j] == null)
                        continue;
                    var exceptCount = exampleDigits[i].Intersect(exampleDigits[j]).Count();
                    var p = possibleSignals.Where(s => digits[j].Intersect(s).Count() == exceptCount).ToList();
                    if (p.Count != 1)
                    {
                        exceptCount = exampleDigits[j].Intersect(exampleDigits[i]).Count();
                        p = possibleSignals.Where(s => s.Intersect(digits[j]).Count() == exceptCount).ToList();
                    }
                    if (p.Count == 1)
                    {
                        digits[i] = p.First();
                        possibleSignals.Remove(digits[i]);
                        if (possibleSignals.Count == 0)
                            signalsPerLength.Remove(digits[i].Length);
                        break;
                    }
                }
            }
        }
        return output.Aggregate(0, (acc, val) => acc * 10 + Array.IndexOf(digits, val));
    }
}
