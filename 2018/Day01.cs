namespace AdventOfCode2018;

public class Day01 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(c => int.Parse(c))
            .ToArray();


        Advent.AssertAnswer1(input.Sum(), expected: 520, sampleExpected: 1);

        var prevFreq = new HashSet<int>();
        int answer2 = 0;
        int val = 0;
        while(answer2 == 0)
            foreach (var freq in input)
            {
                val += freq;
                if (!prevFreq.Add(val))
                {
                    answer2 = val;
                    break;
                }
            }

        Advent.AssertAnswer2(val, expected: 394, sampleExpected: 14);
    }
}