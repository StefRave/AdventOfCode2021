namespace AdventOfCode2021;

public class Day07
{
    private readonly ITestOutputHelper output;

    public Day07(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput().Split(',').Select(int.Parse).OrderBy(x => x).ToArray();
        int minTotalFeulNeeded = CalculateFeul(input, x => x);
        Advent.AssertAnswer1(minTotalFeulNeeded);
        
        minTotalFeulNeeded = CalculateFeul(input, x => x * (x + 1) / 2);
        Advent.AssertAnswer2(minTotalFeulNeeded);
    }

    private static int CalculateFeul(int[] input, Func<int, int> calc)
    {
        int minTotalFeulNeeded = int.MaxValue;
        for (int i = input[0]; i < input[^1]; i++)
        {
            int totalFeulNeeded = input.Sum(x => calc(Math.Abs(x - i)));
            if (totalFeulNeeded < minTotalFeulNeeded)
                minTotalFeulNeeded = totalFeulNeeded;
            else
                break;
        }
        return minTotalFeulNeeded;
    }
}
