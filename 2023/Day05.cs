namespace AdventOfCode2023;

public class Day05 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100005, sampleExpected: 1005);


        Advent.AssertAnswer2(5432, expected: 200005, sampleExpected: 2005);
    }
}
