namespace AdventOfCode2023;

public class Day13 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100013, sampleExpected: 1013);


        Advent.AssertAnswer2(5432, expected: 200013, sampleExpected: 2013);
    }
}












