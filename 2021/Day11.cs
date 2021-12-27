namespace AdventOfCode2021;

public class Day11 : IAdvent
{
    [Fact]
    public void Run()
    {
        int[][] input = Advent.ReadInputLines()
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();
        int yMax = input.Length;
        int xMax = input[0].Length;
        int flashCount = 0;

        int iteration = 0;
        while (true)
        {
            iteration++;

            for (int y = 0; y < yMax; y++)
                for (int x = 0; x < xMax; x++)
                    input[y][x]++;

            int oldFlashCount = flashCount;
            bool actOnFlash = true;
            while (actOnFlash)
            {
                actOnFlash = false;
                for (int y = 0; y < input.Length; y++)
                    for (int x = 0; x < input[0].Length; x++)
                    {
                        if (input[y][x] > 9)
                        {
                            flashCount++;
                            actOnFlash = true;
                            Flash(x, y);
                        }
                    }
            }

            if (iteration == 100)
                Advent.AssertAnswer1(flashCount);
            if (oldFlashCount + 100 == flashCount)
            {
                Advent.AssertAnswer2(iteration);
                break;
            }
        }

        void Flash(int xP, int yP)
        {
            input[yP][xP] = 0;
            for (int y = Math.Max(yP - 1, 0); y < Math.Min(yP + 2, yMax); y++)
                for (int x = Math.Max(xP - 1, 0); x < Math.Min(xP + 2, xMax); x++)
                    if (!(x == xP && y == yP) && input[y][x] != 0)
                        input[y][x]++;
        }
    }
}
