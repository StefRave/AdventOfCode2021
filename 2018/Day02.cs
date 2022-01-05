namespace AdventOfCode2018;

public class Day02 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines();

        var count = new int[10];
        foreach (var line in input)
            foreach (int cnt in line.GroupBy(c => c).Select(g => g.Count()).Where(i => i >= 2).Distinct())
                count[cnt]++;
        long answer1 = count.Where(i => i > 0).Aggregate(1L, (a, b) => a * b);

        Advent.AssertAnswer1(answer1, expected: 7192, sampleExpected: 1);

        string answer2 = FindOneDifference(input);
        Advent.AssertAnswer1(answer2, expected: "mbruvapghxlzycbhmfqjonsie", sampleExpected: "fgij");
    }

    private static string FindOneDifference(string[] input)
    {
        for (int i1 = 0; i1 < input.Length - 1; i1++)
            for (int i2 = i1 + 1; i2 < input.Length; i2++)
            {
                string line1 = input[i1];
                string line2 = input[i2];
                int errors = 0;
                int errIndex = 0;
                for (int i = 0; i < line1.Length; i++)
                    if (line1[i] != line2[i])
                    {
                        errors++;
                        errIndex = i;
                    }
                if (errors == 1)
                    return line1[..errIndex] + line1[(errIndex + 1)..];
            }
        throw new Exception("not found");
    }
}