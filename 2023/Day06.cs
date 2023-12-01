namespace AdventOfCode2023;

public class Day06 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100006, sampleExpected: 1006);


        Advent.AssertAnswer2(5432, expected: 200006, sampleExpected: 2006);
    }
}
