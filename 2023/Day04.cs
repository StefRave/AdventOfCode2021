namespace AdventOfCode2023;

public class Day04 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100004, sampleExpected: 1004);


        Advent.AssertAnswer2(5432, expected: 200004, sampleExpected: 2004);
    }
}
