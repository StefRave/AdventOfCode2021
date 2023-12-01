namespace AdventOfCode2023;

public class Day07 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100007, sampleExpected: 1007);


        Advent.AssertAnswer2(5432, expected: 200007, sampleExpected: 2007);
    }
}
