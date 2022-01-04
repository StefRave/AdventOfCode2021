#nullable enable

namespace AdventOfCode2019;

public class Day24 : IAdvent
{
    private static string GetInput() => File.ReadAllText(@"Input/input24.txt");

    private static (int dy, int dx)[] offsets = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
    public static IEnumerable<(int z, int y, int x)> GetOffsets2D(int y, int x)
    {
        return offsets
            .Select(o => (z: 0, y: o.dy + y, x: o.dx + x))
            .Where(p => p.x >= 0 && p.y >= 0 && p.x < 5 && p.y < 5);
    }

    private static IEnumerable<(int z, int y, int x)> GetOffsetsFrom3D(int y, int x)
    {
        if (x == 2 && y == 2)
            yield break;

        // left
        if (x == 3 && y == 2)
        {
            for (int i = 0; i < 5; i++)
                yield return (-1, i, 4);
        }
        else if (x >= 1)
            yield return (0, y, x - 1);
        else
            yield return (1, 2, 1);

        // right
        if (x == 1 && y == 2)
        {
            for (int i = 0; i < 5; i++)
                yield return (-1, i, 0);
        }
        else if (x < 4)
            yield return (0, y, x + 1);
        else
            yield return (1, 2, 3);

        // top
        if (x == 2 && y == 3)
        {
            for (int i = 0; i < 5; i++)
                yield return (-1, 4, i);
        }
        else if (y >= 1)
            yield return (0, y - 1, x);
        else
            yield return (1, 1, 2);

        // bottom
        if (x == 2 && y == 1)
        {
            for (int i = 0; i < 5; i++)
                yield return (-1, 0, i);
        }
        else if (y < 4)
            yield return (0, y + 1, x);
        else
            yield return (1, 3, 2);
    }

    void IAdvent.Run()
    {
        var input = GetInput();
        //        input = @"....#
        //#..#.
        //#..##
        //..#..
        //#....";
        var grid0 = input
            .SplitByNewLine()
            .Select(line => line.ToArray())
            .ToArray();

        char[][][] grid = new char[][][] { grid0 };
        var previous = new HashSet<string>();
        while (true)
        {
            if (!previous.Add(ToString(grid[0])))
                break;

            grid = DoIteration(grid, singleDimension: true);
        }
        long total = CountPart1(grid[0]);
        Advent.AssertAnswer1(total, 32506911);


        grid = NewGrid(400);
        grid[grid.Length / 2] = grid0; // in the middle

        for (int i = 0; i < 200; i++)
            grid = DoIteration(grid, singleDimension: false);

        int bugCount = grid.SelectMany(l1 => l1.SelectMany(l2 => l2)).Count(c => c == '#');

        Advent.AssertAnswer2(bugCount, 2025);



        static char[][][] NewGrid(int depth)
        {
            var grid = new char[depth][][];
            for (int z = 0; z < grid.Length; z++)
            {
                grid[z] = new char[5][];
                for (int y = 0; y < 5; y++)
                    grid[z][y] = new char[] { '.', '.', '.', '.', '.' };
            }
            return grid;
        }

        static char[][][] DoIteration(char[][][] grid, bool singleDimension)
        {
            var newGrid = NewGrid(grid.Length);
            int minZ = singleDimension ? 0 : 1;
            int maxZ = singleDimension ? 0 : grid.Length - 2;
            for (int z = minZ; z <= maxZ; z++)
                for (int y = 0; y < 5; y++)
                    for (int x = 0; x < 5; x++)
                    {
                        var neighbours = singleDimension ? GetOffsets2D(y, x) : GetOffsetsFrom3D(y, x);
                        int adjecentBugs = neighbours.Sum(p => grid[z - p.z][p.y][p.x] == '#' ? 1 : 0);
                        newGrid[z][y][x] = grid[z][y][x] switch
                        {
                            '#' when adjecentBugs != 1 => '.',
                            '.' when adjecentBugs == 1 || adjecentBugs == 2 => '#',
                            var current => current
                        };
                    }
            return newGrid;
        }
    }

    private static long CountPart1(char[][] grid)
    {
        long total = 0;
        long toAdd = 1;
        for (int y = 0; y < grid.Length; y++)
            for (int x = 0; x < grid[0].Length; x++)
            {
                if (grid[y][x] == '#')
                    total += toAdd;
                toAdd <<= 1;
            }
        return total;
    }

    private static void PrintGrid(char[][] maze)
        => WriteLine("\n" + ToString(maze));

    private static string ToString(char[][] maze) 
        => string.Join("\n", maze.Select(ca => new string(ca)));
}
