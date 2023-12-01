namespace AdventOfCode2023;

public class Day01 : IAdvent
{
    string [] numbers = [
        "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
        "1", "2", "3", "4", "5", "6", "7", "8", "9"];

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();

        int answer1 = input
            .Select(l => Regex.Replace(l, "[^0-9]", ""))
            .Select(l => int.Parse(l[..1] + l[^1..]))
            .Sum();

        Advent.AssertAnswer1(answer1, expected: 54450, sampleExpected: 220);

        var input2 = input
            .Select(l =>
            {
                int first = Find(l, fromStart: true);
                int last = Find(l, fromStart: false);
                return first * 10 + last;
            })
            .ToArray();

        Advent.AssertAnswer2(input2.Sum(), expected: 54265, sampleExpected: 281);
    }

    private int Find(string input, bool fromStart)
    {
        int startIndex = fromStart ? 0 : input.Length - 1;
        int endIndex = fromStart ? input.Length -1 : 0;
        int direction = fromStart ? 1 : -1;

        for (int index = startIndex; index != endIndex; index += direction)
        {
            for (int i = 0; i < numbers.Length; i++)
                if (HasMatchAtIndex(input, index, numbers[i]))
                    return i % 9 + 1;
        }
        return -1;
    }

    bool HasMatchAtIndex(string input, int index, string str)
        => input.Length >= index + str.Length && input[index..(index + str.Length)] == str;
}
