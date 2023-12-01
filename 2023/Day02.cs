namespace AdventOfCode2023;

public class Day02 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100002, sampleExpected: 1002);


        Advent.AssertAnswer2(5432, expected: 200002, sampleExpected: 2002);
    }
}
