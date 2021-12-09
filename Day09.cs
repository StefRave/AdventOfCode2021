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
        
        var lowPoints = new List<Point>();
        int maxY = input.Length;
        int maxX = input[0].Length;
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                var points = new List<int>();
                if (x > 0)
                    points.Add(input[y][x - 1]);
                if (y > 0)
                    points.Add(input[y - 1][x]);
                if (y < input.Length - 1)
                    points.Add(input[y + 1][x]);
                if (x < input[0].Length - 1)
                    points.Add(input[y][x + 1]);

                if (points.Min() > input[y][x])
                {
                    lowPoints.Add(new(x, y));
                }
            }
        }
        int lowPointSum = lowPoints.Select(p => input[p.Y][p.X] + 1).Sum();
        Advent.AssertAnswer1(lowPointSum);


        var basinSizes = new List<long>();
        foreach (var lowPoint in lowPoints)
        {
            int pointsChanged = FloodFill(lowPoint);
            basinSizes.Add(pointsChanged);
        }
        basinSizes.Sort();
        Advent.AssertAnswer2(basinSizes[^1] * basinSizes[^2] * basinSizes[^3]);


        int FloodFill(Point pt)
        {
            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(pt);
            int pointsChanged = 0;
            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();
                if (a.X < maxX && a.X >= 0 && a.Y < maxY && a.Y >= 0)
                {

                    if (input[a.Y][a.X] != 9)
                    {
                        pointsChanged++;
                        input[a.Y][a.X] = 9;
                        pixels.Push(new Point(a.X - 1, a.Y));
                        pixels.Push(new Point(a.X + 1, a.Y));
                        pixels.Push(new Point(a.X, a.Y - 1));
                        pixels.Push(new Point(a.X, a.Y + 1));
                    }
                }
            }
            return pointsChanged;
        }
    }

    public record Point(int X, int Y);
}
