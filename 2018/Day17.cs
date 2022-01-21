using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day17 : IAdvent
{
    public void Run()
    {
        // x=495, y=2..7
        var input = Advent.ReadInputLines()
            .Select(line => Regex.Match(line, @"(\w)=(\d+), (\w)=(\d+)\.\.(\d+)"))
            .Select(m => m.Groups.Cast<Group>().Select(g => g.Value).ToArray())
            .Select(gv => new Instr(gv[1][0], int.Parse(gv[2]), int.Parse(gv[4]), int.Parse(gv[5])))
            .ToArray();
        var (maxX, maxY, minX, minY) = GetBoundaries(input);

        var grid = new char[maxY, maxX];
        InitializeGrid(grid, input);

        Fill(spring: (minY - 1, 500));
        PrintGrid();

        int answer1 = grid.Cast<char>().Count(c => c == '|' || c == '~');
        Advent.AssertAnswer1(answer1, 34291, 57);

        int answer2 = grid.Cast<char>().Count(c => c == '~');
        Advent.AssertAnswer1(answer2, 28487, 29);


        void Fill((int y, int x) spring)
        {
            var stack = new Stack<(int y, int x)>();
            grid[spring.y, spring.x] = '+';
            stack.Push((spring.y + 1, spring.x));
            bool printGrid = false;
            while (stack.Count > 0)
            {
                var pos = stack.Pop();
                if (printGrid)
                    PrintGrid(pos);

                if (grid[pos.y, pos.x] == '~')
                    continue;
                if ((pos.y < maxY - 1) && (grid[pos.y + 1, pos.x] == '#' || grid[pos.y + 1, pos.x] == '~'))
                {
                    bool canLeak = false;
                    int rowMinX, rowMaxX;
                    grid[pos.y, pos.x] = '|';
                    for (rowMinX = pos.x - 1; rowMinX > minX; rowMinX--)
                    {
                        if (grid[pos.y + 1, rowMinX] == '|')
                        {
                            canLeak = true;
                            break;
                        }
                        if (grid[pos.y, rowMinX] == '#')
                        {
                            rowMinX++;
                            break;
                        }
                        grid[pos.y, rowMinX] = '|';
                        if (grid[pos.y + 1, rowMinX] == '.')
                        {
                            canLeak = true;
                            if (pos.y < maxY - 1)
                                stack.Push((pos.y + 1, rowMinX));
                            break;
                        }
                    }
                    for (rowMaxX = pos.x + 1; rowMaxX < maxX; rowMaxX++)
                    {
                        if (grid[pos.y + 1, rowMaxX] == '|')
                        {
                            canLeak = true;
                            break;
                        }
                        if (grid[pos.y, rowMaxX] == '#')
                        {
                            rowMaxX--;
                            break;
                        }
                        grid[pos.y, rowMaxX] = '|';
                        if (grid[pos.y + 1, rowMaxX] == '.')
                        {
                            canLeak = true;
                            if (pos.y < maxY - 1)
                                stack.Push((pos.y + 1, rowMaxX));
                            break;
                        }
                    }
                    if (!canLeak)
                    {
                        for (int x = rowMinX; x <= rowMaxX; x++)
                            grid[pos.y, x] = '~';
                        stack.Push((pos.y - 1, pos.x));
                    }
                }
                else if (grid[pos.y, pos.x] == '.')
                {
                    grid[pos.y, pos.x] = '|';
                    if (pos.y < maxY - 1)
                        stack.Push((pos.y + 1, pos.x));
                }
            }
        }

        void PrintGrid((int y, int x) pos = default)
        {
            int yStart = pos.y - 25;
            int yEnd = pos.y + 25;
            if (yStart < 0)
            {
                yEnd -= yStart;
                yStart = 0;
            }
            if (yEnd > maxY)
            {
                yStart -= yEnd - maxY;
                yEnd = maxY;
            }
            if (pos == (0, 0))
            {
                yStart = 0;
                yEnd = maxY;
            }
            var sb = new StringBuilder();
            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = minX; x < maxX; x++)
                    sb.Append(grid[y, x]);
                sb.Append('\n');
            }
            Console.Clear();
            Console.WriteLine(sb.ToString());
            if (pos != (0, 0))
            {
                Console.CursorLeft = pos.x - minX;
                Console.CursorTop = pos.y - yStart;
            }
        }
    }

    private static void InitializeGrid(char[,] grid, Instr[] input)
    {
        for (int y = 0; y < grid.GetLength(0); y++)
            for (int x = 0; x < grid.GetLength(1); x++)
                grid[y, x] = '.';

        foreach (var instr in input)
        {
            if (instr.Xy == 'x')
            {
                for (int y = instr.RangeStart; y <= instr.RangeEnd; y++)
                    grid[y, instr.Pos] = '#';
            }
            else
            {
                for (int x = instr.RangeStart; x <= instr.RangeEnd; x++)
                    grid[instr.Pos, x] = '#';
            }
        }
    }

    private static (int maxX, int maxY, int minX, int minY) GetBoundaries(Instr[] input)
    {
        int maxX = 0;
        int maxY = 0;
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        foreach (var instr in input)
        {
            if (instr.Xy == 'x')
            {
                maxX = Math.Max(maxX, instr.Pos + 1);
                maxY = Math.Max(maxY, instr.RangeEnd + 1);
                minX = Math.Min(minX, instr.Pos);
                minY = Math.Min(minY, instr.RangeStart);
            }
            else
            {
                maxY = Math.Max(maxY, instr.Pos + 1);
                maxX = Math.Max(maxX, instr.RangeEnd + 1);
                minY = Math.Min(minY, instr.Pos);
                minX = Math.Min(minX, instr.RangeStart);
            }
        }
        minX--;
        maxX++;

        return (maxX, maxY, minX, minY);
    }

    public record Instr(char Xy, int Pos, int RangeStart, int RangeEnd);
}