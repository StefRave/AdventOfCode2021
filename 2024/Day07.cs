namespace AdventOfCode2024;

public class Day07 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();

        int answer1 = 1;

        Advent.AssertAnswer1(answer1, expected: 11111, sampleExpected: 11111111);

        
        int answer2 = 2;
        Advent.AssertAnswer2(answer2, expected: 22222, sampleExpected: 22222222);
    }
}






