namespace AdventOfCode2023;

public class Day21 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100021, sampleExpected: 1021);


        Advent.AssertAnswer2(5432, expected: 200021, sampleExpected: 2021);
    }
}
