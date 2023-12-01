namespace AdventOfCode2023;

public class Day17 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100017, sampleExpected: 1017);


        Advent.AssertAnswer2(5432, expected: 200017, sampleExpected: 2017);
    }
}
