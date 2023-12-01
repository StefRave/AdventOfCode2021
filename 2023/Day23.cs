namespace AdventOfCode2023;

public class Day23 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100023, sampleExpected: 1023);


        Advent.AssertAnswer2(5432, expected: 200023, sampleExpected: 2023);
    }
}
