namespace AdventOfCode2023;

public class Day09 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.GetLongs())
            .ToArray();

        long answer1 = 0;
        long answer2 = 0;
        foreach (var line in input)
        {
            var list = new List<long[]>();
            var numbers = line;
            while (!numbers.All(n => n == 0))
            {
                list.Add(numbers);
                var newNumbers = new long[numbers.Length - 1];
                for (int i = 0; i < newNumbers.Length; i++)
                    newNumbers[i] = numbers[i + 1] - numbers[i];
                numbers = newNumbers;
            }
            long last = 0;
            long first = 0;
            foreach (var num in list.ToArray().Reverse())
            {
                last = num[^1] + last;
                first = num[0] - first; 
            }

            answer1 += last;
            answer2 += first;
        }
        Advent.AssertAnswer1(answer1, expected: 1930746032, sampleExpected: 114);
        Advent.AssertAnswer2(answer2, expected: 1154, sampleExpected: 2);
    }
}
