using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day23 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => Regex.Match(line, "pos=<(.+),(.+),(.+)>, r=(.+)").Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray())
            .Select(num => (pos: new Vector3(num[0], num[1], num[2]), r: num[3]))
            .ToArray();

        var (pos, range) = input.OrderByDescending(b => b.r).First();
        int countInRange = input.Select(b => (b.pos - pos).ManhattanDistance()).Where(dist => dist <= range).Count();

        Advent.AssertAnswer1(countInRange, 248, 6);

        var sum = input.Select(b => b.pos).Aggregate((a, b) => a + b);
        pos = new Vector3(sum.X / input.Length, sum.Y / input.Length, sum.Z / input.Length);
        int offset = 100000000;
        int stepSize = 2;
        while (offset > 1)
        {
            pos = Search(pos, offset, stepSize);
            offset /= stepSize;
        }
        Advent.AssertAnswer2(pos.ManhattanDistance(), 124623002, 36);


        Vector3 Search(Vector3 avg, int offset, int steps)
        {
            int stepSize = Math.Max(1, offset / steps);
            int mostInRange = 3;
            Vector3 bestPos = new Vector3(0, 0, 0);
            int bestDistance = int.MaxValue;

            for (int x = avg.X - offset; x <= avg.X + offset; x += stepSize)
                for (int y = avg.Y - offset; y <= avg.Y + offset; y += stepSize)
                    for (int z = avg.Z - offset; z <= avg.Z + offset; z += stepSize)
                    {
                        Vector3 v = new Vector3(x, y, z);
                        countInRange = input.Where(b => (b.pos - v).ManhattanDistance() <= b.r + stepSize - 1).Count();
                        int v1 = v.ManhattanDistance();
                        if (countInRange > mostInRange || (countInRange == mostInRange && v1 < bestDistance))
                        {
                            mostInRange = countInRange;
                            bestPos = v;
                            bestDistance = v.ManhattanDistance();
                        }
                    }
            //Console.WriteLine($"{offset} {bestPos} {mostInRange}");
            return bestPos;
        }
    }

    public record Vector3(int X, int Y, int Z)
    {
        public int Length() => X * X + Y * Y + Z * Z;
        public int ManhattanDistance() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
}
