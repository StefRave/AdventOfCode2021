namespace AdventOfCode2022;

public class Day01 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .Select(x => x.SplitByNewLine().Select(e => int.Parse(e)).ToArray())
            .ToArray();

        var totalPerElf = input
            .Select(ee => ee.Sum())
            .OrderByDescending(e => e)
            .ToArray();
        var maxTotalPerElf = totalPerElf.First();

        Advent.AssertAnswer1(maxTotalPerElf, expected: 67450, sampleExpected: 24000);

        var maxThree = totalPerElf.Take(3).Sum();

        Advent.AssertAnswer2(maxThree, expected: 199357, sampleExpected: 45000);
    }
}