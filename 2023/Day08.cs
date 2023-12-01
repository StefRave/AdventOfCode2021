namespace AdventOfCode2023;

public class Day08 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100008, sampleExpected: 1008);


        Advent.AssertAnswer2(5432, expected: 200008, sampleExpected: 2008);
    }
}
