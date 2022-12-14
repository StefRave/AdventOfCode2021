using System.Runtime.CompilerServices;

namespace AdventOfCode2022;

public class Day14 : IAdvent
{
    public void Run()
    {
        int add = 150;
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(" -> ").Select(pair => pair.Split(",").Select(int.Parse).ToArray()).Select(pair => (x: pair[0] + add, y: pair[1])).ToArray())
            .ToArray();

        int maxX = input.SelectMany(line => line).Select(pair => pair.x).Max();
        int maxY = input.SelectMany(line => line).Select(pair => pair.y).Max();
        int minX = input.SelectMany(line => line).Select(pair => pair.x).Min();
        int minY = input.SelectMany(line => line).Select(pair => pair.y).Min();
        
        var grid = Init.Array(() => Init.Array('.', maxX + add), maxY + 3);
        Fill(input, grid);
        var answer1 = Drip();
        Advent.AssertAnswer1(answer1, expected: 1199, sampleExpected: 24);

        grid = Init.Array(() => Init.Array('.', maxX + add), maxY + 3);
        Fill(input, grid);
        AddFloor(grid, maxY);
        var answer2 = Drip();
        Advent.AssertAnswer2(answer2, expected: 23925, sampleExpected: 93);


        int Drip()
        {
            var d = (x: 500 + add, y: 0);

            for (int step = 0; true; step++)            
            {
                var (x, y) = d;
                if (grid[y][x] != '.')
                    return step;

                while (true)
                {
                    if (++y == grid.Length)
                        return step;
                    if (grid[y][x] == '.')
                        continue;
                    else if (grid[y][x - 1] == '.')
                        x--;
                    else if (grid[y][x + 1] == '.')
                        x++;
                    else
                        break;
                }
                grid[y - 1][x] = 'o';
            }
        }
    }

    private static void AddFloor(char[][] grid, int maxY)
    {
        for (int x = 0; x < grid[0].Length; x++)
            grid[maxY + 2][x] = '#';
    }

    private static void Fill((int x, int y)[][] input, char[][] grid)
    {
        foreach (var line in input)
        {
            for (int j = 1; j < line.Length; j++)
            {
                var (x, y) = line[j - 1];
                var (xe, ye) = line[j];

                int steps = Math.Abs(xe - x + ye - y);
                var dx = (xe - x) / steps;
                int dy = (ye - y) / steps;

                for (int i = 0; i <= steps; i++)
                    grid[y + i * dy][x + i * dx] = '#';
            }
        }
    }
}