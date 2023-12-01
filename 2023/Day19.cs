namespace AdventOfCode2023;

public class Day19 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100019, sampleExpected: 1019);


        Advent.AssertAnswer2(5432, expected: 200019, sampleExpected: 2019);
    }
}
