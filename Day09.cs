namespace AdventOfCode2021;

public class Day09
{
    private readonly ITestOutputHelper output;

    public Day09(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();

        var lowPoints = new List<(int x, int y)>();
        int maxY = input.Length;
        int maxX = input[0].Length;
        for (int y = 0; y < maxY; y++)
            for (int x = 0; x < maxX; x++)
                if (Adjacent(x, y).All(adj => input[y][x] < input[adj.y][adj.x]))
                    lowPoints.Add((x, y));

        int lowPointSum = lowPoints.Select(p => input[p.y][p.x] + 1).Sum();
        Advent.AssertAnswer1(lowPointSum);

        var basinSizes = lowPoints.Select(p => FloodFill(p.x, p.y)).OrderByDescending(p => p).ToArray();
        Advent.AssertAnswer2(basinSizes[0] * basinSizes[1] * basinSizes[2]);

        int FloodFill(int x, int y)
        {
            var pixels = new Stack<(int x, int y)>();
            int pointsChanged = 0;

            pixels.Push((x, y));
            while (pixels.Count > 0)
            {
                var p = pixels.Pop();
                if (input[p.y][p.x] != 9)
                {
                    pointsChanged++;
                    input[p.y][p.x] = 9;
                    foreach (var adj in Adjacent(p.x, p.y))
                        pixels.Push((adj.x, adj.y));
                }
            }
            return pointsChanged;
        }

        IEnumerable<(int x, int y)> Adjacent(int x, int y)
        {
            if (x > 0) yield return (x - 1, y);
            if (y > 0) yield return (x, y - 1);
            if (x < maxX - 1) yield return (x + 1, y);
            if (y < maxY - 1) yield return (x, y + 1);
        }
    }
}