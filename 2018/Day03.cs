using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day03 : IAdvent
{
    public void Run()
    {
        //#1 @ 1,3: 4x4
        var input = Advent.ReadInputLines()
            .Select(line => Regex.Matches(line, @"\d+").Cast<Match>().Select(m => int.Parse(m.Value)).ToArray())
            .Select(d => new Instr(d[0], d[1], d[2], d[3], d[4]))
            .ToArray();

        int maxX = input.Max(i => i.X + i.Wide);
        int maxY = input.Max(i => i.Y + i.Tall);
        int[][] grid = new int[maxY + 1][];
        for (int i = 0; i < grid.Length; i++)
            grid[i] = new int[maxX + 1];

        foreach (var instr in input)
        {
            for (int y = instr.Y; y < instr.Y + instr.Tall; y++)
                for (int x = instr.X; x < instr.X + instr.Wide; x++)
                    grid[y][x]++;
        }
        int overlapped = grid.SelectMany(l => l).Count(v => v >= 2);
        Advent.AssertAnswer1(overlapped, expected: 109785, sampleExpected: 4);


        int noOverlapId = 0;
        foreach (var instr in input)
        {
            bool hasOverlap = false;
            for (int y = instr.Y; y < instr.Y + instr.Tall; y++)
                for (int x = instr.X; x < instr.X + instr.Wide; x++)
                    hasOverlap |= grid[y][x] > 1;
            if (!hasOverlap)
            {
                noOverlapId = instr.Id;
                break;
            }
        }
        Advent.AssertAnswer2(noOverlapId, expected: 504, sampleExpected: 3);
    }

    public record Instr(int Id, int X, int Y, int Wide, int Tall);
}