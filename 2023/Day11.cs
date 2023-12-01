namespace AdventOfCode2023;

public class Day11 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100011, sampleExpected: 1011);


        Advent.AssertAnswer2(5432, expected: 200011, sampleExpected: 2011);
    }
}
