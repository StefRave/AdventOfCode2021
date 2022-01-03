#nullable enable
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AdventOfCode2019;

public class Day24 : IAdvent
{
    private static string GetInput() => File.ReadAllText(@"Input/input24.txt");

    private static (int dy, int dx)[] offsets = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
    public static IEnumerable<(int y, int x)> GetOffsetsFrom((int y, int x) p) => offsets.Select(o => (o.dy + p.y, o.dx + p.x));
    void IAdvent.Run()
    {
        var input = GetInput();
//        input = @"....#
//#..#.
//#..##
//..#..
//#....";
        var field = input
            .SplitByNewLine()
            .Select(line => line.ToArray())
            .ToArray();

        var previous = new HashSet<string>();

        while (true)
        {
            string item = ToString(field);
            if (previous.Contains(item))
                break;
            previous.Add(item);

            var newField = new char[field.Length][];
            for (int y = 0; y < field.Length; y++)
            {
                newField[y] = new char[field[0].Length];
                for (int x = 0; x < field[0].Length; x++)
                {
                    int adjecentBugs = GetOffsetsFrom((y, x))
                        .Where(p => p.x >= 0 && p.y >= 0 && p.x < field[0].Length && p.y < field.Length)
                        .Sum(p => field[p.y][p.x] == '#' ? 1 : 0);
                    newField[y][x] = field[y][x] switch
                    {
                        '#' when adjecentBugs != 1 => '.',
                        '.' when adjecentBugs == 1 || adjecentBugs == 2 => '#',
                        var current => current
                    };
                }
            }
            field = newField;
        }

        long total = 0;
        long mask = 1;
        for (int y = 0; y < field.Length; y++)
        {
            for (int x = 0; x < field[0].Length; x++)
            {
                if (field[y][x] == '#')
                    total += mask;
                mask <<= 1;
            }
        }
        Advent.AssertAnswer1(total, 32506911);
        //Advent.AssertAnswer2(index, 2020);




    }
    private static void PrintMaze(char[][] maze)
        => WriteLine("\n" + ToString(maze));

    private static string ToString(char[][] maze) 
        => string.Join("\n", maze.Select(ca => new string(ca)));
}
