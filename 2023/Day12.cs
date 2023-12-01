namespace AdventOfCode2023;

public class Day12 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100012, sampleExpected: 1012);


        Advent.AssertAnswer2(5432, expected: 200012, sampleExpected: 2012);
    }
}
