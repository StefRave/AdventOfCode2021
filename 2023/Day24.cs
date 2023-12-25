using FluentAssertions;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace AdventOfCode2023;

public class Day24 : IAdvent
{
    public void Run()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.GetLongs())
            .Select(n => (p: new V3(n[0], n[1], n[2]), v: new V3(n[3], n[4], n[5])))
            .ToArray();

        char variable = 'g';
        for (int i =0; i < 5; i++)
        {
            // equation1 = Eq(2*x + 3*y, y+2)
            var d = input[i];
            char vt = variable++;
            if (i == 0)
            {
                Console.WriteLine($"equation{i * 3 + 1} = Eq(a,{d.p.X}{d.p.X:+0;-0}*{vt})");
                Console.WriteLine($"equation{i * 3 + 2} = Eq(b,{d.p.Y}{d.p.Y:+0;-0}*{vt})");
                Console.WriteLine($"equation{i * 3 + 3} = Eq(c,{d.p.Z}{d.p.Z:+0;-0}*{vt})");
            }
            else
            {
                char vv = variable++;
                Console.WriteLine($"equation{i * 3 + 1} = Eq(a+{vv}*d,{d.p.X}{d.p.X:+0;-0}*{vt})");
                Console.WriteLine($"equation{i * 3 + 2} = Eq(b+{vv}*e,{d.p.Y}{d.p.Y:+0;-0}*{vt})");
                Console.WriteLine($"equation{i * 3 + 3} = Eq(c+{vv}*f,{d.p.Z}{d.p.Z:+0;-0}*{vt})");
            }
        }
        Console.Write("solution = solve((");
        for (int i = 1; i <= 15; i++)
        {
            if (i > 1)
                Console.Write(", ");
            Console.Write($"equation{i}");
        }
        Console.Write("), (");
        for (int i = 0; i < 15; i++)
        {
            if (i >= 1)
                Console.Write(", ");
            Console.Write($"{(char)('a' + i)}");
        }
        Console.WriteLine("))");

        Console.WriteLine();
        Console.WriteLine();
        //int answer1 = Part1(input);
        //Advent.AssertAnswer1(answer1, expected: 19523, sampleExpected: 2);

        var poi = new V3[] {new V3(9, 18, 20),
            new V3(15, 16, 16),
            new V3(12, 17, 18),
            new V3(6, 19, 22),
            new V3(21, 14, 12),
        };
        var dd = LinearRegression3D.CalculateRegression(poi);


        var points = input.Select(i => i.p).ToList();
        (var direction, var deviation) = LinearRegression3D.CalculateRegression(points);
        var deviations = LinearRegression3D.CalculateDeviations(points, direction, points[0]);

        var newPoints = input.Select((pv, i) => pv.p + (input[i].v * (BigInteger)(deviation/ (double)input[i].v.Length))).ToList();
        (var direction2, var deviation2) = LinearRegression3D.CalculateRegression(newPoints);

        1.ToString();
        for (int i = 0; i < input.Length - 2; i++)
        {
            for (int j = i + 1; j < input.Length - 1; j++)
            {
                for (int k = j + 1; k < input.Length; k++)
                {
                    if (i == 0 && j == 1 && k == 4)
                        1.ToString();
                    var p1 = input[i].p;
                    var p2 = input[j].p + input[j].v;
                    var p3 = input[k].p + input[k].v + input[k].v;
                    if ((p1 - p2).ManhattanDistance == (p2 - p3).ManhattanDistance)
                        Console.WriteLine($"{i} {j} {k}");
                }
            }
        }

        Advent.AssertAnswer2(43545, expected: 200024, sampleExpected: 2024);
    }

    private static int Part1((V3 p, V3 v)[] input)
    {
        int answer1 = 0;
        BigInteger[] ins = Advent.UseSampleData ? [7, 27] : [200000000000000, 400000000000000];
        ins = ins.Select(n => n).ToArray();
        for (int i = 0; i < input.Length; i++)
            for (int j = i + 1; j < input.Length; j++)
            {
                var (p1, v1) = input[i];
                var (p2, v2) = input[j];
                var p = V3.XyIntersections(p1, v1, p2, v2);
                if (p != null && p.X >= ins[0] && p.X <= ins[1] && p.Y >= ins[0] && p.Y <= ins[1])
                {
                    var intersectTime1 = (p.X - p1.X) / v1.X;
                    var intersectTime2 = (p.X - p2.X) / v2.X;
                    if (intersectTime1 >= 0 && intersectTime2 >= 0)
                        answer1++;
                }
            }

        return answer1;
    }

    [DebuggerDisplay("{X},{Y},{Z}")]
    public record V3(BigInteger X, BigInteger Y, BigInteger Z)
    {
        public BigInteger DistanceSquared => X * X + Y * Y + Z * Z;
        public double Length => Math.Sqrt((double)DistanceSquared);
        public BigInteger ManhattanDistance() => BigInteger.Abs(X) + BigInteger.Abs(Y) + BigInteger.Abs(Z);

        public static V3 operator -(V3 a, V3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static V3 operator +(V3 a, V3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static V3 operator *(V3 a, BigInteger m) => new(a.X * m, a.Y * m, a.Z * m);
        public static explicit operator Vector3(V3 v) => new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        public static explicit operator V3(Vector3 v) => new V3((BigInteger)v.X, (BigInteger)v.Y, (BigInteger)v.Z);

        public static Vector3 Normalize(V3 v)
        {
            float l = (float)v.Length;
            return new Vector3((float)v.X / l, (float)v.Y / l, (float)v.Z / l);
        }

        public static V3 XyIntersections(V3 p1, V3 v1, V3 p2, V3 v2)
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
                return null;
            x /= d;
            y /= d;
            return new V3(x, y, 0);
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
    }


    public class LinearRegression3D
    {
        public static (Vector3 direction, double deviation) CalculateRegression(ICollection<V3> points)
        {
            // Ensure there are enough points to perform regression
            if (points.Count < 3)
            {
                throw new InvalidOperationException("At least three points are required to calculate linear regression in 3D.");
            }

            // Calculate means for x, y, and z
            BigInteger meanX = 0, meanY = 0, meanZ = 0;
            foreach (var point in points)
            {
                meanX += point.X;
                meanY += point.Y;
                meanZ += point.Z;
            }
            meanX /= points.Count;
            meanY /= points.Count;
            meanZ /= points.Count;

            // Calculate the components of the covariance matrix
            BigInteger xx = 0, yy = 0, zz = 0, xy = 0, xz = 0, yz = 0;
            foreach (var point in points)
            {
                BigInteger rX = point.X - meanX;
                BigInteger rY = point.Y - meanY;
                BigInteger rZ = point.Z - meanZ;
                xx += rX * rX;
                yy += rY * rY;
                zz += rZ * rZ;
                xy += rX * rY;
                xz += rX * rZ;
                yz += rY * rZ;
            }

            // Calculate the coefficients of the plane
            BigInteger detX = yy * zz - yz * yz;
            BigInteger detY = xx * zz - xz * xz;
            BigInteger detZ = xx * yy - xy * xy;

            // Choose the largest determinant for the most stable solution
            BigInteger maxDet = BigInteger.Max(BigInteger.Max(detX, detY), detZ);
            V3 direction;
            if (maxDet == detX)
            {
                direction = new V3(detX, xz * yz - xy * zz, xy * yz - xz * yy);
            }
            else if (maxDet == detY)
            {
                direction = new V3(xz * yz - xy * zz, detY, xy * xz - yz * xx);
            }
            else
            {
                direction = new V3(xy * yz - xz * yy, xy * xz - yz * xx, detZ);
            }

            // Normalize the direction vector
            var dir = V3.Normalize(direction);

            // Calculate the deviation
            BigInteger d = 0;
            foreach (var point in points)
            {
                V3 pointProjected = ProjectPointOnLine(point, meanX, meanY, meanZ, dir);
                d += (point - pointProjected).DistanceSquared;
            }
            double deviation = Math.Sqrt((double)d / points.Count);

            return (dir, deviation);
        }

        public static List<double> CalculateDeviations(List<V3> points, Vector3 lineDirection, V3 linePoint)
        {
            List<double> deviations = new List<double>();
            foreach (var point in points)
            {
                double deviation = DistanceFromPointToLine(point, linePoint, lineDirection);
                deviations.Add(deviation);
            }
            return deviations;
        }

        // Helper method to calculate the perpendicular distance from a point to a line
        private static double DistanceFromPointToLine(V3 point, V3 linePoint, Vector3 lineDirection)
        {
            V3 pointToLineStart = point - linePoint;
            Vector3 crossProduct = Vector3.Cross((Vector3)pointToLineStart, (Vector3)lineDirection);
            return crossProduct.Length() / lineDirection.Length();
        }

        private static V3 ProjectPointOnLine(V3 point, BigInteger lineX, BigInteger lineY, BigInteger lineZ, Vector3 lineDirection)
        {
            V3 linePoint = new V3(lineX, lineY, lineZ);
            V3 v = point - linePoint;
            float d = Vector3.Dot((Vector3)v, lineDirection);
            return linePoint + (V3)(lineDirection * d);
        }
    }
}
