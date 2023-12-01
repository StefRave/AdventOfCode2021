namespace AdventOfCode2023;

public class Day16 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();

        Advent.AssertAnswer1(1234, expected: 100016, sampleExpected: 1016);


        Advent.AssertAnswer2(5432, expected: 200016, sampleExpected: 2016);
    }
}
