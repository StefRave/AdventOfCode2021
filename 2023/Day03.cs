namespace AdventOfCode2023;

public class Day03 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100003, sampleExpected: 1003);


        Advent.AssertAnswer2(5432, expected: 200003, sampleExpected: 2003);
    }
}
