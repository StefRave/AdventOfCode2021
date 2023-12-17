namespace AdventOfCode2023;

using static AdventOfCode2023.Day17.Direction;

public class Day17 : IAdvent
{
    public enum Direction { Right, Down, Left, Up }

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
        var lowestTotal = new int[input[0].Length,input.Length, 4];
        var stack = new Queue<(int x, int y, bool vert, int total)>();
        var (endX, endY) = (input[0].Length - 1, input.Length - 1);
        Enqueue(0, 0, Right, 0);
        Enqueue(0, 0, Down, 0);

        while (stack.Count > 0)
        {
            var (x, y, vert, total) = stack.Dequeue();

            int prevTotal = lowestTotal[x, y, vert ? 0 : 1];
            if (prevTotal != 0 && prevTotal <= total)
                continue;
            lowestTotal[x, y, vert ? 0 : 1] = total;
            Enqueue(x, y, vert ? Down : Right, total);
            Enqueue(x, y, vert ? Up : Left, total);
        }
        return Math.Min(lowestTotal[endX, endY, 0], lowestTotal[endX, endY, 1]);


        void Enqueue(int x, int y, Direction m, int total)
        {
            for (int i = 1; i <= maxSameDir; i++)
            {
                (x, y) = Move((x, y), m);
                if (x < 0 || x > endX || y < 0 || y > endY)
                    break;
                total += input[y][x];
                if (i >= minSameDir)
                    stack.Enqueue((x, y, m == Right || m == Left, total));
            }
        }

        (int x, int y) Move((int x, int y) pos, Direction m)
        {
            return m switch
            {
                Right => (pos.x + 1, pos.y),
                Left => (pos.x - 1, pos.y),
                Up => (pos.x, pos.y - 1),
                Down => (pos.x, pos.y + 1),
                _ => throw new Exception("Invalid movement"),
            };
        }
    }
}
