using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day11 : IAdvent
{
    public void Run()
    {
        int serial = Advent.UseSampleData ? 42 : 3463;

        Assert.Equal(4, GetLevel(3, 5, 8));
        Assert.Equal(-5, GetLevel(122, 79, 57));

        var grid = new int[300, 300];
        for (int y = 1; y <= 300; y++)
            for (int x = 1; x <= 300; x++)
                grid[y - 1, x - 1] = GetLevel(x, y, serial);

        var (rX, rY, totalPower) = GetCoordsAndSize(grid, 3);
        Advent.AssertAnswer1($"{rX},{rY}", expected: "235,60", sampleExpected: "21,61");

        int mrX = 0, mrY = 0, mTotalPower = 0, ms = 0;
        int unchangeCount = 0;
        for (int s = 1; s < 300; s++)
        {
            (rX, rY, totalPower) = GetCoordsAndSize(grid, s);
            if (totalPower > mTotalPower)
            {
                mrX = rX;
                mrY = rY;
                mTotalPower = totalPower;
                ms = s;
                unchangeCount = 0;
            }
            if (unchangeCount++ > 5)
                break;
        }
        Advent.AssertAnswer2($"{mrX},{mrY},{ms}", expected: "233,282,11", sampleExpected: "232,251,12");
    }
    private static (int rX, int rY, int totalPower) GetCoordsAndSize(int[,] grid, int size)
    {
        int max = 0;
        int rX = 0, rY = 0;
        for (int y = 1; y <= 300 - size; y++)
            for (int x = 1; x <= 300 - size; x++)
            {
                int sum = 0;
                for (int dy = 0; dy < size; dy++)
                    for (int dx = 0; dx < size; dx++)
                        sum += grid[y + dy - 1, x + dx - 1];
                if (sum > max)
                {
                    max = sum;
                    (rX, rY) = (x, y);
                }
            }
        return (rX, rY, max);
    }

    private int GetLevel(int x, int y, int serial)
    {
        int rackId = x + 10;
        long powerLevel = (rackId * y + (long)serial) * rackId;
        int d100 = (int)((powerLevel / 100) % 10);
        return d100 - 5;
    }
}