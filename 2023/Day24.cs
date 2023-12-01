namespace AdventOfCode2023;

public class Day24 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100024, sampleExpected: 1024);


        Advent.AssertAnswer2(5432, expected: 200024, sampleExpected: 2024);
    }
}
