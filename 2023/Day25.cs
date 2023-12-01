namespace AdventOfCode2023;

public class Day25 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100025, sampleExpected: 1025);


        Advent.AssertAnswer2(5432, expected: 200025, sampleExpected: 2025);
    }
}
