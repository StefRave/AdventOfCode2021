namespace AdventOfCode2023;

public class Day17 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();

        int answer1 = DoIt(input, minSameDir: 1, maxSameDir: 3);
        Advent.AssertAnswer1(answer1, expected: 1138, sampleExpected: 102);

        int answer2 = DoIt(input, minSameDir: 4, maxSameDir: 10);
        Advent.AssertAnswer2(answer2, expected: 1312, sampleExpected: 94);
    }

    private int DoIt(int[][] input, int minSameDir, int maxSameDir)
    {
        var (endX, endY) = (input[0].Length, input.Length);
        var lowestTotal = new (int total, bool queued)[endX, endY, 2];
        var stack = new Queue<(int x, int y, bool vert)>();
        Enqueue(0, 0, (1, 0), 0);
        Enqueue(0, 0, (0, 1), 0);

        while (stack.Count > 0)
        {
            var (x, y, vert) = stack.Dequeue();
            int total = lowestTotal[x, y, vert ? 0 : 1].total;
            lowestTotal[x, y, vert ? 0 : 1].queued = false;
            Enqueue(x, y, vert ? (0, 1) : (1, 0), total);
            Enqueue(x, y, vert ? (0, -1) : (-1, 0), total);
        }
        return Math.Min(lowestTotal[endX - 1, endY - 1, 0].total, lowestTotal[endX - 1, endY - 1, 1].total);


        void Enqueue(int x, int y, (int x, int y) d, int total)
        {
            for (int i = 1; i <= maxSameDir; i++)
            {
                (x, y) = (x + d.x, y + d.y);
                if (x < 0 || x >= endX || y < 0 || y >= endY)
                    break;
                total += input[y][x];
                if (i >= minSameDir)
                {
                    bool vert = d.y == 0;
                    var prev = lowestTotal[x, y, vert ? 0 : 1];
                    if (prev.total != 0 && prev.total <= total)
                        continue;
                    lowestTotal[x, y, vert ? 0 : 1] = (total, true);
                    if (!prev.queued)
                        stack.Enqueue((x, y, vert));
                }
            }
        }
    }
}
