namespace AdventOfCode2023;

public class Day10 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100010, sampleExpected: 1010);


        Advent.AssertAnswer2(5432, expected: 200010, sampleExpected: 2010);
    }
}
