using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day25 : IAdvent
{
    static Regex parseLine = new Regex(@"^(?<Unit>\d+) units each with (?<hitPoints>\d+) hit points (\((?<weakImmune>.*)\) )?" +
        @"with an attack that does (?<damage>\d+) (?<damageType>\w+) damage at initiative (?<initiative>\d+)", RegexOptions.Multiline);
    static Regex parseWeakImumune = new Regex(@"(?:(\w+) to (?:(\w+),? ?)+)");

    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(',').Select(int.Parse).ToArray())
            .Select(line => new Vector4(line[0], line[1], line[2], line[3]))
            .ToArray();

        List<List<Vector4>> galaxies = new List<List<Vector4>>();
        foreach (var v in input)
        {
            List<Vector4> foundGalaxy = null;
            foreach (var g in galaxies.ToArray())
            {
                if (g.Any(gv => (v - gv).ManhattanDistance() <= 3))
                {
                    if (foundGalaxy != null)
                    {
                        // if found in multiple galaxied, than merge those galaxies
                        foundGalaxy.AddRange(g);
                        galaxies.Remove(g);
                    }
                    else
                    {
                        foundGalaxy = g;
                        g.Add(v);
                    }
                }
            }
            if (foundGalaxy == null)
                galaxies.Add(new List<Vector4> { v });
        }
        Advent.AssertAnswer1(galaxies.Count, 388, 8);
    }

    public record Vector4(int W, int X, int Y, int Z)
    {
        public int ManhattanDistance() => Math.Abs(W) + Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public static Vector4 operator -(Vector4 a, Vector4 b) => new Vector4(a.W - b.W, a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
}