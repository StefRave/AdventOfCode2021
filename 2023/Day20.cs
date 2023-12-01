namespace AdventOfCode2023;

public class Day20 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100020, sampleExpected: 1020);


        Advent.AssertAnswer2(5432, expected: 200020, sampleExpected: 2020);
    }
}
