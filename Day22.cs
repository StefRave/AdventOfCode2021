using System.ComponentModel.DataAnnotations;

namespace AdventOfCode2021;

public class Day22
{
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine()
            .Select(data => data.SplitByNewLine().Select(ParseLine).ToArray())
            .ToArray();

        Advent.AssertAnswer1(Solve(input[0].Where(i => Math.Abs(i.Xs) < 50).ToArray()));
        Advent.AssertAnswer2(Solve(input[^1]));


        static Instr ParseLine(string linea)
        {
            var s = linea.Split(new[] { " x=", ",y=", ",z=", ".." }, 0);

            return new Instr(
                On: s[0] == "on",
                Xs: int.Parse(s[1]),
                Xe: int.Parse(s[2]),
                Ys: int.Parse(s[3]),
                Ye: int.Parse(s[4]),
                Zs: int.Parse(s[5]),
                Ze: int.Parse(s[6]));
        }
    }

    public static long Solve(Instr[] input)
    {
        int[] xv = input.Select(line => line.Xs).Union(input.Select(line => line.Xe + 1)).Distinct().OrderBy(x => x).ToArray();
        int[] yv = input.Select(line => line.Ys).Union(input.Select(line => line.Ye + 1)).Distinct().OrderBy(y => y).ToArray();
        int[] zv = input.Select(line => line.Zs).Union(input.Select(line => line.Ze + 1)).Distinct().OrderBy(y => y).ToArray();

        var array = new bool[xv.Length, yv.Length, zv.Length];
        foreach (var line in input)
            Fill(line);
        return Count();

        void Fill(Instr a)
        {
            int xs = Array.BinarySearch(xv, a.Xs);
            int ys = Array.BinarySearch(yv, a.Ys);
            int zs = Array.BinarySearch(zv, a.Zs);
            int xe = Array.BinarySearch(xv, a.Xe + 1);
            int ye = Array.BinarySearch(yv, a.Ye + 1);
            int ze = Array.BinarySearch(zv, a.Ze + 1);

            for (var x = xs; x < xe; x++)
                for (var y = ys; y < ye; y++)
                    for (var z = zs; z < ze; z++)
                        array[x, y, z] = a.On;
        }

        long Count()
        {
            long count = 0;
            for (var xi = 0; xi < xv.Length - 1; xi++)
                for (var yi = 0; yi < yv.Length - 1; yi++)
                    for (var zi = 0; zi < zv.Length - 1; zi++)
                        if (array[xi, yi, zi])
                            count += ((long)xv[xi + 1] - xv[xi]) * ((long)yv[yi + 1] - yv[yi]) * ((long)zv[zi + 1] - zv[zi]);
            return count;
        }
    }

    public static int IndexOf<TSource>(IReadOnlyList<TSource> source, Func<TSource, bool> predicate)
    {
        for (int index = 0; index < source.Count; index++)
            if (predicate.Invoke(source[index]))
                return index;
        return -1;
    }

    public record Instr(bool On, int Xs, int Xe, int Ys, int Ye, int Zs, int Ze);
}