namespace AdventOfCode2021;

public class Day07 : IAdvent
{
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput().Split(',').Select(int.Parse).OrderBy(x => x).ToArray();
        int minTotalFuelNeeded = CalculateFuel(input, x => x);
        Advent.AssertAnswer1(minTotalFuelNeeded);
        
        minTotalFuelNeeded = CalculateFuel(input, x => x * (x + 1) / 2);
        Advent.AssertAnswer2(minTotalFuelNeeded);
    }

    private static int CalculateFuel(int[] input, Func<int, int> calc)
    {
        int minTotalFuelNeeded = int.MaxValue;
        for (int i = input[0]; i < input[^1]; i++)
        {
            int totalFuelNeeded = input.Sum(x => calc(Math.Abs(x - i)));
            if (totalFuelNeeded < minTotalFuelNeeded)
                minTotalFuelNeeded = totalFuelNeeded;
            else
                break;
        }
        return minTotalFuelNeeded;
    }
}
