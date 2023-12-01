namespace AdventOfCode2023;

public class Day22 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100022, sampleExpected: 1022);


        Advent.AssertAnswer2(5432, expected: 200022, sampleExpected: 2022);
    }
}
