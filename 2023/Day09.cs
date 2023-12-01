namespace AdventOfCode2023;

public class Day09 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100009, sampleExpected: 1009);


        Advent.AssertAnswer2(5432, expected: 200009, sampleExpected: 2009);
    }
}
