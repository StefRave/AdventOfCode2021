namespace AdventOfCode2021;

public class Day08Alt : IAdvent
{
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
        var optionsByLength = exampleDigits
            .Select((digits, number) => (digits, number))
            .GroupBy(di => di.digits.Length)
            .ToDictionary(g => g.Key, g => g.ToHashSet());
        
        string[] digits = new string[10];

        var toFind = new Queue<string>(inputSignal);
        while (toFind.Count > 0)
        {
            string signal = toFind.Dequeue();

            var options = optionsByLength[signal.Length];
            if(options.Count() == 1)
            {
                var found = options.First();
                digits[found.number] = signal;
                optionsByLength.Remove(signal.Length);
            }
            else
            {
                bool found = false;
                for (int i = 0; i < digits.Length; i++)
                {
                    if (digits[i] == null)
                        continue;
                    int? foundNumber = null;
                    string foundDigits = null;
                    foreach (var (candidateDigits, candidateNumber) in options)
                    {
                        if(candidateDigits.Intersect(exampleDigits[i]).Count() == signal.Intersect(digits[i]).Count() &&
                            exampleDigits[i].Intersect(candidateDigits).Count() == digits[i].Intersect(signal).Count())
                        {
                            foundDigits = candidateDigits;
                            if (foundNumber == null)
                                foundNumber = candidateNumber;
                            else
                            {
                                foundNumber = null; // found more than once
                                break;
                            }
                        }
                    }
                    if (foundNumber != null)
                    {
                        digits[foundNumber.Value] = signal;
                        options.Remove((foundDigits, foundNumber.Value));
                        found = true;
                        break;
                    }
                }
                if (!found)
                    toFind.Enqueue(signal);
            }
        }
        return output.Aggregate(0, (acc, val) => acc * 10 + Array.IndexOf(digits, val));
    }
}
