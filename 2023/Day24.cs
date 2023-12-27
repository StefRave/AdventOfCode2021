using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace AdventOfCode2023;

public class Day24 : IAdvent
{
    static BigInteger[] ins = Advent.UseSampleData ? [7, 27] : [200000000000000, 400000000000000];
    public void Run()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.GetLongs().Select(n => n).ToArray())
            .Select(n => (p: new V3(n[0], n[1], n[2]), v: new V3(n[3], n[4], n[5])))
            .ToArray();

        var answer1 = CountIntersectionsPart1(input);
        Advent.AssertAnswer1(answer1, expected: 19523, sampleExpected: 2);

        var answer2 = FindRockPosition(input);
        Advent.AssertAnswer2(answer2, expected: 566373506408017, sampleExpected: 47);
    }


    static long FindRockPosition((V3 p, V3 v)[] input)
    {
        var (hs0, hs1, hs2, hs3) = (input[0], input[1], input[2], input[3]);
        for (int x = -600; x < 600; x++)
            for (int y = -600; y < 600; y++)
            {
                var ds = new V3(x, y, 0);
                var (p1, _, t1) = V3.XyIntersections(hs0.p, hs0.v + ds, hs1.p, hs1.v + ds);
                if (p1 == null)
                    continue;
                var (p2, _, t2) = V3.XyIntersections(hs0.p, hs0.v + ds, hs2.p, hs2.v + ds);
                if (p2 == null || (p1.Y, p1.X) != (p2.Y, p2.X))
                    continue;
                var (p3, _, t3) = V3.XyIntersections(hs0.p, hs0.v + ds, hs3.p, hs3.v + ds);
                if (p3 == null || (p1.Y, p1.X) != (p3.Y, p3.X))
                    continue;

                for (int z = -600; z < 600; z++)
                {
                    var z1 = hs1.p.Z + (BigInteger)(t1 * (double)(hs1.v.Z + z));
                    var z2 = hs2.p.Z + (BigInteger)(t2 * (double)(hs2.v.Z + z));
                    if (z1 != z2) continue;
                    var z3 = hs3.p.Z + (BigInteger)(t3 * (double)(hs3.v.Z + z));
                    if (z1 != z3) continue;

                    return (long)(p1.X + p1.Y + z1);
                }
            }
        throw new Exception("Not found");
    }

    private static int CountIntersectionsPart1((V3 p, V3 v)[] input)
    {
        int answer1 = 0;
        ins = ins.Select(n => n).ToArray();
        for (int i = 0; i < input.Length; i++)
            for (int j = i + 1; j < input.Length; j++)
            {
                var (p1, v1) = input[i];
                var (p2, v2) = input[j];
                var (p, t1, t2) = V3.XyIntersections(p1, v1, p2, v2);
                if (p != null && p.X >= ins[0] && p.X <= ins[1] && p.Y >= ins[0] && p.Y <= ins[1])
                {
                    if (t1 >= 0 && t2 >= 0)
                    {
                        answer1++;
                    }
                }
            }
        return answer1;
    }
}
[DebuggerDisplay("{X},{Y},{Z}")]
public record V3(BigInteger X, BigInteger Y, BigInteger Z)
{
    public BigInteger DistanceSquared => X * X + Y * Y + Z * Z;
    public double Length() => Math.Sqrt((double)DistanceSquared);
    public BigInteger ManhattanDistance() => BigInteger.Abs(X) + BigInteger.Abs(Y) + BigInteger.Abs(Z);

    public override string ToString()
        => $"({X},{Y},{Z})";

    public static V3 operator -(V3 a, V3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static V3 operator +(V3 a, V3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static V3 operator *(V3 a, BigInteger m) => new(a.X * m, a.Y * m, a.Z * m);
    public static V3 operator /(V3 a, BigInteger m) => new((a.X + m / 2) / m, (a.Y + m / 2) / m, (a.Z + m / 2) / m);
    public static explicit operator Vector3(V3 v) => new Vector3((float)v.X, (float)v.Y, (float)v.Z);
    public static explicit operator V3(Vector3 v) => new V3((BigInteger)v.X, (BigInteger)v.Y, (BigInteger)v.Z);

    public static Vector3 Normalize(V3 v)
    {
        float l = (float)v.Length();
        return new Vector3((float)v.X / l, (float)v.Y / l, (float)v.Z / l);
    }

    public static (V3 p, double t1, double t2) XyIntersections(V3 p1, V3 v1, V3 p2, V3 v2)
    {
        var x1 = p1.X;
        var x2 = p1.X + v1.X;
        var x3 = p2.X;
        var x4 = p2.X + v2.X;
        var y1 = p1.Y;
        var y2 = p1.Y + v1.Y;
        var y3 = p2.Y;
        var y4 = p2.Y + v2.Y;
        var x = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
        var y = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
        var d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (d == 0)
            return (null, 0, 0);
        x /= d;
        y /= d;

        if (v1.X == 0 || v2.X == 0)
            return (null, 0, 0);
        var t1 = (double)(x - p1.X) / (double)v1.X;
        var t2 = (double)(x - p2.X) / (double)v2.X;
        return (new V3(x, y, p1.Z + v1.Z * (BigInteger)(t1 * 10000) / 10000), t1, t2);
    }

    public V3 Mean(IEnumerable<V3> list)
    {
        BigInteger sumX = 0, sumY = 0, sumZ = 0;
        foreach (var v in list)
        {
            sumX += v.X;
            sumY += v.Y;
            sumZ += v.Z;
        }
        return new V3(sumX / list.Count(), sumY / list.Count(), sumZ / list.Count());
    }
    public static V3 Cross(V3 vector1, V3 vector2)
    {
        return new V3(
            (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y),
            (vector1.Z * vector2.X) - (vector1.X * vector2.Z),
            (vector1.X * vector2.Y) - (vector1.Y * vector2.X)
        );
    }

    public static BigInteger Dot(V3 vector1, V3 vector2)
    {
        return (vector1.X * vector2.X)
                + (vector1.Y * vector2.Y)
                + (vector1.Z * vector2.Z);
    }

    public static V3 ClosestPoint(V3 p1, V3 d1, V3 p2, V3 d2)
    {
        V3 n = V3.Cross(d1, d2);
        V3 n2 = V3.Cross(d2, n);
        V3 numerator = d1 * V3.Dot(p2 - p1, n2);
        BigInteger denominator = V3.Dot(d1, n2);

        if (denominator != 0) // Ensure no division by zero
            return p1 + numerator / denominator;
        // Handle the case where lines are parallel or an error occurs
        throw new InvalidOperationException("Lines are parallel or an error occurred.");
    }
}



