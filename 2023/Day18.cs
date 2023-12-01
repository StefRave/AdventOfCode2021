namespace AdventOfCode2023;

public class Day18 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100018, sampleExpected: 1018);


        Advent.AssertAnswer2(5432, expected: 200018, sampleExpected: 2018);
    }
}
