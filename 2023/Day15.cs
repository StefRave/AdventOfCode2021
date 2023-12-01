namespace AdventOfCode2023;

public class Day15 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100015, sampleExpected: 1015);


        Advent.AssertAnswer2(5432, expected: 200015, sampleExpected: 2015);
    }
}
