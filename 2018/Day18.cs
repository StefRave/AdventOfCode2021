using System.Text;

namespace AdventOfCode2018;

public class Day18 : IAdvent
{
    (int dy, int dx)[] Deltas = new[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)  };
    const char openGround = '.';
    const char trees = '|';
    const char lumberyard = '#';

    public void Run()
    {
        var grid = Advent.ReadInputLines()
            .Select(line => line.ToCharArray())
            .ToArray();

        PrintGrid();
        for (int i = 0; i < 10; i++)
            grid = DoIteration(grid);
        int treesCount = grid.SelectMany(l => l).Count(c => c == trees);
        int lumberCount = grid.SelectMany(l => l).Count(c => c == lumberyard);
        Advent.AssertAnswer1(treesCount * lumberCount, 480150, 1147);

        var cache = new Dictionary<string, int>();
        for (int i = 10; i < 1000000000; i++)
        {
            string key = GridToString();
            if (!cache.TryAdd(key, i))
            {
                int prevI = cache[key];
                int toAdd = ((1000000000 - i) / (i - prevI)) * (i - prevI);
                int rest = toAdd % (i - prevI);
                i += toAdd;
            }
            grid = DoIteration(grid);
        }
        treesCount = grid.SelectMany(l => l).Count(c => c == trees);
        lumberCount = grid.SelectMany(l => l).Count(c => c == lumberyard);
        Advent.AssertAnswer2(treesCount * lumberCount, 233020, 0);

        void PrintGrid(int iteration = 0)
        {
            Console.WriteLine(iteration == 0 ? "Initial" : $"Iteration {iteration}");
            Console.WriteLine(GridToString());
        }

        string GridToString()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < grid.Length; y++)
                sb.AppendLine(new string(grid[y]));
            return sb.ToString();
        }
        char[][] CopyGrid(char[][] input)
            => input.Select(line => line.ToArray()).ToArray();

        int CountType(char type, int y, int x)
            => GetNeighbours(y, x).Count(c => c == type);

        IEnumerable<char> GetNeighbours(int y, int x)
        {
            foreach (var d in Deltas)
                if (y + d.dy >= 0 && x + d.dx >= 0 && y + d.dy < grid.Length && x + d.dx < grid[0].Length)
                    yield return grid[y + d.dy][x + d.dx];
        }

        char[][] DoIteration(char[][] grid)
        {
            var newGrid = CopyGrid(grid);
            for (int y = 0; y < grid.Length; y++)
            {
                for (int x = 0; x < grid[0].Length; x++)
                {
                    int adjecentTrees = CountType(trees, y, x);
                    int adjectenLumber = CountType(lumberyard, y, x);
                    newGrid[y][x] = grid[y][x] switch
                    {
                        openGround => adjecentTrees >= 3 ? trees : openGround,
                        trees => adjectenLumber >= 3 ? lumberyard : trees,
                        lumberyard => adjectenLumber >= 1 && adjecentTrees >= 1 ? lumberyard : openGround,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }
            return newGrid;
        }
    }
}