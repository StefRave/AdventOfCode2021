namespace AdventOfCode2024;

public class Day12 : IAdvent
{
    static (int dy, int dx)[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();

        int answer1 = CalculateCost(input);
        Advent.AssertAnswer1(answer1, expected: 1522850, sampleExpected: 1930);

        int answer2 = CalculateCost(input, discount: true);
        Advent.AssertAnswer2(answer2, expected: 953738, sampleExpected: 1206);
    }

    private static int CalculateCost(string[] input, bool discount = false)
    {
        Dictionary<(int y, int x), char> garden = new();
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[0].Length; x++)
                garden[(y, x)] = input[y][x];

        Dictionary<char, (int, int)> ap = new();
        HashSet<(int y, int x)> done = new();

        int ans1 = 0;
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[0].Length; x++)
            {
                if (done.Contains((y, x)))
                    continue;

                ans1 += FloodFill(y, x);
            }

        int FloodFill(int y, int x)
        {
            var queue = new Queue<(int y, int x)>();
            queue.Enqueue((y, x));
            var p = input[y][x];

            int area = 0;
            int perimeter = 0;

            while (queue.Any())
            {
                var (cy, cx) = queue.Dequeue();
                if (done.Contains((cy, cx)))
                    continue;
                done.Add((cy, cx));

                area++;
                perimeter += AddPerimeter(cy, cx);

                foreach (var d in directions)
                {
                    var (ny, nx) = (cy + d.dy, cx + d.dx);
                    if (garden.GetValueOrDefault((ny, nx)) == p)
                        queue.Enqueue((ny, nx));
                }
            }
            return area * perimeter;


            int AddPerimeter(int cy, int cx)
            {
                int localPerimiter = 0;
                for (int i = 0; i < directions.Length; i++)
                {
                    var d = directions[i];
                    var pn = garden.GetValueOrDefault((cy + d.dy, cx + d.dx), '.');
                    if (pn != p)
                    {
                        if (!discount)
                            localPerimiter++;
                        else
                        {
                            var dd = directions[(i + 1) % 4];
                            if (garden.GetValueOrDefault((cy + dd.dy, cx + dd.dx), '.') != p)
                                localPerimiter++;
                            else if (garden.GetValueOrDefault((cy + d.dy + dd.dy, cx + d.dx + dd.dx), '.') == p)
                                localPerimiter++;
                        }
                    }
                }
                return localPerimiter;
            }
        }

        return ans1;
    }
}











