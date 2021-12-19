using System.Numerics;
using System.Reflection;

namespace AdventOfCode2021;

public class Day19
{
    [Fact]
    public void Run()
    {
        Vector3[][] scannerRows = Advent.ReadInput()
            .SplitByDoubleNewLine()
            .Select(ParseCoords)
            .ToArray();

        var beacons = scannerRows[0];
        var toFind = Enumerable.Range(1, scannerRows.Length - 1).ToList();
        var scannerOffsets = new List<Vector3> { new Vector3(0, 0, 0) };
        while (toFind.Any())
        {
            foreach (int srb in toFind.ToArray())
            {
                var result = FindSolution(beacons.ToArray(), scannerRows[srb]);
                if (result != null)
                {
                    beacons = beacons.Concat(
                        scannerRows[srb].Select(v => v.Flip(result.Value.flip) + result.Value.offset)
                        ).Distinct().ToArray();
                    scannerOffsets.Add(result.Value.offset);
                    toFind.Remove(srb);
                }
            }
        }

        Advent.AssertAnswer1(beacons.Length);

        int maxDistance = MaxManhatanDistance(scannerOffsets);
        Advent.AssertAnswer2(maxDistance);
    }

    private static int MaxManhatanDistance(List<Vector3> scannerOffsets)
    {
        int maxDistance = 0;
        for (int i = 0; i < scannerOffsets.Count - 1; i++)
        {
            for (int j = 1; j < scannerOffsets.Count; j++)
                maxDistance = Math.Max(maxDistance, scannerOffsets[i].ManhattanDistance(scannerOffsets[j]));
        }

        return maxDistance;
    }

    private static (int flip, Vector3 offset)? FindSolution(Vector3[] sra, Vector3[] srb)
    {

        for (int a1 = 0; a1 < sra.Length; a1++)
        {
            var solutions = new List<(int flip, Vector3 offset)>();

            for (int a2 = 0; a2 < sra.Length; a2++)
            {
                if (a1 == a2) continue;

                Vector3 a = sra[a1] - sra[a2];
                float aLength = a.Length();
                for (int b1 = 0; b1 < srb.Length - 1; b1++)
                {
                    for (int b2 = b1 + 1; b2 < srb.Length; b2++)
                    {
                        Vector3 b = srb[b1] - srb[b2];
                        float bLength = b.Length();
                        if (Math.Abs(aLength - bLength) < 0.00001)
                        {
                            bool found = false;
                            for (int flip = 0; flip < 24; flip++)
                            {
                                if ((a - b.Flip(flip)).Length() < 0.00001)
                                {
                                    var offset = sra[a1] - srb[b1].Flip(flip);
                                    solutions.Add((flip, offset));
                                    var test1 = srb[b1].Flip(flip) + offset;
                                    var test2 = srb[b2].Flip(flip) + offset;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                1.ToString();
                        }
                    }
                }

            }
            var result = solutions.GroupBy(s => s).Where(g => g.Count() > 4).FirstOrDefault()?.Key;
            if (result != null)
                return result;
        }
        return null;
    }

    private Vector3[] ParseCoords(string scannerLines)
    {
        return scannerLines
            .SplitByNewLine()
            .Skip(1)
            .Select(line => line.Split(",").Select(int.Parse).ToArray())
            .Select(nums => new Vector3(nums[0], nums[1], nums[2]))
            .ToArray();
    }
}

public static class Vector3Extensions
{
    public static int ManhattanDistance(this Vector3 vector1, Vector3 vector2)
        => (int)Math.Round(Math.Abs(vector1.X - vector2.X) + Math.Abs(vector1.Y - vector2.Y) + Math.Abs(vector1.Z - vector2.Z), 0);
    public static Vector3 Flip(this Vector3 v, int flip)
    {
        return flip switch
        {
            0 => new Vector3(v.X, v.Y, v.Z),
            1 => new Vector3(v.Y, -v.X, v.Z),
            2 => new Vector3(-v.X, -v.Y, v.Z),
            3 => new Vector3(-v.Y, v.X, v.Z),
            4 => new Vector3(v.X, -v.Y, -v.Z),
            5 => new Vector3(-v.Y, -v.X, -v.Z),
            6 => new Vector3(-v.X, v.Y, -v.Z),
            7 => new Vector3(v.Y, v.X, -v.Z),
            8 => new Vector3(v.Z, v.Y, -v.X),
            9 => new Vector3(v.Y, -v.Z, -v.X),
            10 => new Vector3(-v.Z, -v.Y, -v.X),
            11 => new Vector3(-v.Y, v.Z, -v.X),
            12 => new Vector3(v.Z, -v.Y, v.X),
            13 => new Vector3(-v.Y, -v.Z, v.X),
            14 => new Vector3(-v.Z, v.Y, v.X),
            15 => new Vector3(v.Y, v.Z, v.X),
            16 => new Vector3(v.X, -v.Z, v.Y),
            17 => new Vector3(-v.Z, -v.X, v.Y),
            18 => new Vector3(-v.X, v.Z, v.Y),
            19 => new Vector3(v.Z, v.X, v.Y),
            20 => new Vector3(v.X, v.Z, -v.Y),
            21 => new Vector3(v.Z, -v.X, -v.Y),
            22 => new Vector3(-v.X, -v.Z, -v.Y),
            23 => new Vector3(-v.Z, v.X, -v.Y),
        };
    }
}