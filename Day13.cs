using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2021;

public class Day13
{
    private readonly ITestOutputHelper output;

    public Day13(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine();
        var coords = input[0].SplitByNewLine()
            .Select(line => line.Split(',').Select(n => int.Parse(n)).ToArray())
            .Select(line => (y: line[1], x: line[0]))
            .ToArray();
        var instrs = input[1]
            .SplitByNewLine()
            .Select(line => Regex.Match(line, @"([xy])=(\d+)"))
            .Select(m => (xory: m.Groups[1].Value[0], pos: int.Parse(m.Groups[2].Value)))
            .ToArray();

        coords = Fold(coords, instrs[0].xory, instrs[0].pos);
        Advent.AssertAnswer1(coords.Length);

        foreach (var (xory, pos) in instrs.Skip(0))
            coords = Fold(coords, xory, pos);

        DisplayCharacters(coords);

        //Advent.AssertAnswer2(word);
    }

    private (int y, int x)[] Fold((int y, int x)[] coords, char xory, int pos)
    {
        return coords
            .Select(coord => xory == 'x' ? FoldX(coord, pos) : FoldY(coord, pos))
            .Distinct()
            .ToArray();
    }

    private static (int y, int x) FoldX((int y, int x) coord, int pos) => (coord.y, Fold(coord.x, pos));
    private static (int y, int x) FoldY((int y, int x) coord, int pos) => (Fold(coord.y, pos), coord.x);
    private static int Fold(int n, int pos) => (n < pos) ? n : pos - (n - pos);

    private void DisplayCharacters((int y, int x)[] coords)
    {
        int xMax = coords.Max(coord => coord.x);
        int yMax = coords.Max(coord => coord.y);
        var array = Enumerable.Range(0, yMax + 1)
            .Select(i => new StringBuilder(new string("".PadRight(xMax + 1, ' ').ToArray())))
            .ToArray();
        foreach (var (y, x) in coords)
            array[y][x] = '#';
        output.WriteLine(string.Join('\n', array.Select(sb => sb.ToString())));
    }
}
