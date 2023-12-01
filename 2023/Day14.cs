namespace AdventOfCode2023;

public class Day14 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100014, sampleExpected: 1014);


        Advent.AssertAnswer2(5432, expected: 200014, sampleExpected: 2014);
    }
}
