using System.Text;

namespace AdventOfCode2018;

public class Day22 : IAdvent
{
    const int rocky = 0;
    const int wet = 1;
    const int narrow = 2;
    const int climbingGear = 0;
    const int torch = 1;
    const int neither = 2;

    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(' ').Last().Split(',').Select(int.Parse).ToArray())
            .ToArray();
        int depth = input[0][0];
        var target = (x: input[1][0], y: input[1][1]);
        long modulo = 20183;
        long[,] erosionLevels = new long[target.x + 20, target.y + 20];
        Console.WriteLine($"depth:{depth} target:{target}");


        for (int y = 0; y < erosionLevels.GetLength(1); y++)
            erosionLevels[0,y] = (depth + 48271 * y) % modulo;
        for (int x = 0; x < erosionLevels.GetLength(0); x++)
            erosionLevels[x, 0] = (depth + 16807 * x) % modulo;
        for (int y = 1; y < erosionLevels.GetLength(1); y++)
            for (int x = 1; x < erosionLevels.GetLength(0); x++)
            {
                if (x == target.x && y == target.y)
                    erosionLevels[x, y] = depth % modulo;
                else
                    erosionLevels[x, y] = (erosionLevels[x - 1, y] * erosionLevels[x, y - 1] + depth) % modulo;
            }
        int[,] cave = new int[erosionLevels.GetLength(0),erosionLevels.GetLength(1)];
        for (int y = 0; y < erosionLevels.GetLength(1); y++)
            for (int x = 0; x < erosionLevels.GetLength(0); x++)
                cave[x, y] = ((int)(erosionLevels[x, y] % 3)) switch { 0 => rocky, 1 => wet, _ => narrow };
        Print();

        int riskInArea = 0;
        for (int y = 0; y <= target.y; y++)
            for (int x = 0; x <= target.x; x++)
                riskInArea += cave[x, y];

        Advent.AssertAnswer1(riskInArea, 9940, 114);
        int totalTime = TotalTimeToTarget();
        Advent.AssertAnswer2(totalTime, 944, 45);

        int TotalTimeToTarget()
        {
            int[,,] time = new int[erosionLevels.GetLength(0), erosionLevels.GetLength(1),3];
            for (int y = 0; y < erosionLevels.GetLength(1); y++)
                for (int x = 0; x < erosionLevels.GetLength(0); x++)
                    for (int equipment = 0; equipment < 3; equipment++)
                        time[x, y, equipment] = int.MaxValue;

            var queue = new Queue<(int x, int y, int equipment, int time)>();
            queue.Enqueue((0, 0, torch, 0));
            while (queue.Count > 0)
            {
                (int x, int y, int equipment, int currentTime) = queue.Dequeue();
                if (x < 0 || y < 0 || x >= time.GetLength(0) || y >= time.GetLength(1))
                    continue;

                if (currentTime >= time[x, y, equipment])
                    continue;
                if (cave[x, y] == rocky && equipment == neither)
                    continue;
                if (equipment == torch && cave[x, y] == wet)
                    continue;
                if (equipment == climbingGear && cave[x, y] == narrow)
                    continue;

                time[x, y, equipment] = currentTime;

                queue.Enqueue((x - 1, y, equipment, currentTime + 1));
                queue.Enqueue((x + 1, y, equipment, currentTime + 1));
                queue.Enqueue((x, y - 1, equipment, currentTime + 1));
                queue.Enqueue((x, y + 1, equipment, currentTime + 1));
                queue.Enqueue((x, y, (equipment + 1) % 3, currentTime + 7));
                queue.Enqueue((x, y, (equipment + 2) % 3, currentTime + 7));
            }
            return time[target.x, target.y, torch];
        }

        void Print()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < erosionLevels.GetLength(1); y++)
            {
                for (int x = 0; x < erosionLevels.GetLength(0); x++)
                    sb.Append(cave[x, y] switch {0 => '.', 1 => '=', _ => '|'});
                sb.Append('\n');
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
