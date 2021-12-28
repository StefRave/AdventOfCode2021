namespace AdventOfCode2021;

public class Day19 : IAdvent
{
    public void Run()
    {
        Vector3[][] scannerRows = Advent.ReadInput()
            .SplitByDoubleNewLine()
            .Select(ParseCoords)
            .ToArray();

        var identifiers = scannerRows.Select(CreateIdentities).ToArray();
        var beacons = identifiers[0];
        var toFind = identifiers.Skip(1).ToHashSet();
        var scannerOffsets = new List<Vector3> { new Vector3(0, 0, 0) };

        while (toFind.Any())
        {
            foreach (var idb in toFind.ToArray())
            {
                var result = FindSolution(beacons, idb);
                if (result != null)
                {
                    foreach (var item in idb)
                        beacons[item.Key] = new VectorOffset(
                            item.Value.Vector.Flip(result.Value.flip) + result.Value.offset,
                            item.Value.Offset.Flip(result.Value.flip));

                    scannerOffsets.Add(result.Value.offset);
                    toFind.Remove(idb);
                }
            }
        }
        var answer1 = beacons.Select(b => b.Value.Vector).Distinct().Count();
        Advent.AssertAnswer1(answer1);

        int maxDistance = MaxManhatanDistance(scannerOffsets);
        Advent.AssertAnswer2(maxDistance);
    }

    private static (int flip, Vector3 offset)? FindSolution(Dictionary<Identifier, VectorOffset> ida, Dictionary<Identifier, VectorOffset> idb)
    {
        var solutions = new List<(int flip, Vector3 offset)>();

        foreach (var identifier in ida.Keys)
        {
            if (!idb.TryGetValue(identifier, out var kv))
                continue;

            var (va, oa) = ida[identifier];
            var (vb, ob) = kv;

            var flip = FindFlip(oa, ob);
            if (flip != null)
                solutions.Add((flip.Value, offset: va - vb.Flip(flip.Value)));
        }
        return solutions
            .GroupBy(s => s)
            .OrderBy(g => g.Count())
            .FirstOrDefault(g => g.Count() >= 6)?.Key;
    }
    private static int? FindFlip(Vector3 v1, Vector3 v2)
    {
        for (int i = 0; i < 24; i++)
            if (v1 == v2.Flip(i))
                return i;
        return null;
    }

    private Dictionary<Identifier, VectorOffset> CreateIdentities(Vector3[] vectors)
    {
        var lengthsForVertors = new List<(int length, int i1, int i2)>();

        for (int i1 = 0; i1 < vectors.Length - 1; i1++)
            for (int i2 = i1 + 1; i2 < vectors.Length; i2++)
                lengthsForVertors.Add(((vectors[i1] - vectors[i2]).Length(), i1, i2));
        lengthsForVertors = lengthsForVertors.OrderBy(i => i.length).ToList();

        var identifiers = new List<(Identifier identifier, VectorOffset vectorOffset)>[vectors.Length];
        int maxLength = lengthsForVertors[^1].length / 3; // Don't use vectors that are far from each other
        foreach (var (length, i1, i2) in lengthsForVertors.Where(v => v.length < maxLength))
        {
            // The whole array must have a value, or beacons will be missing
            if (identifiers[i1] == null)
            {
                identifiers[i1] = new();
                identifiers[i1].Add((new Identifier(length, CreateIdentier(vectors[i1] - vectors[i2])), new VectorOffset(vectors[i1], vectors[i1] - vectors[i2])));
            }
            else
            {
                if (identifiers[i2] == null)
                    identifiers[i2] = new();
                identifiers[i2].Add((new Identifier(length, CreateIdentier(vectors[i1] - vectors[i2])), new VectorOffset(vectors[i2], vectors[i2] - vectors[i1])));
            }
        }
        return identifiers.SelectMany(l => l).ToDictionary(i => i.identifier, i => i.vectorOffset);
    }

    private static Vector3 CreateIdentier(Vector3 v)
    {
        Vector3 good = null;
        Vector3 perfect = null;
        for (int i = 0; i < 24; i++)
        {
            var rot = v.Flip(i);
            if (rot.X >= 0 && rot.Y >= 0 && rot.Z >= 0 && rot.X <= rot.Y)
            {
                good = rot;
                if (rot.Y <= rot.Z)
                    perfect = rot;
            }
        }
        return perfect ?? good;
    }

    private static int MaxManhatanDistance(List<Vector3> scannerOffsets)
    {
        int maxDistance = 0;
        for (int i = 0; i < scannerOffsets.Count - 1; i++)
        {
            for (int j = 1; j < scannerOffsets.Count; j++)
                maxDistance = Math.Max(maxDistance, (scannerOffsets[i] - scannerOffsets[j]).ManhattanDistance());
        }

        return maxDistance;
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

    record Identifier(int Length, Vector3 Vector);
    record VectorOffset(Vector3 Vector, Vector3 Offset);

    public record Vector3(int X, int Y, int Z)
    {
        public int Length() => X * X + Y * Y + Z * Z;
        public int ManhattanDistance() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public Vector3 Flip(int flip)
        {
            return flip switch
            {
                0 => new Vector3(X, Y, Z),
                1 => new Vector3(Y, -X, Z),
                2 => new Vector3(-X, -Y, Z),
                3 => new Vector3(-Y, X, Z),
                4 => new Vector3(X, -Y, -Z),
                5 => new Vector3(-Y, -X, -Z),
                6 => new Vector3(-X, Y, -Z),
                7 => new Vector3(Y, X, -Z),
                8 => new Vector3(Z, Y, -X),
                9 => new Vector3(Y, -Z, -X),
                10 => new Vector3(-Z, -Y, -X),
                11 => new Vector3(-Y, Z, -X),
                12 => new Vector3(Z, -Y, X),
                13 => new Vector3(-Y, -Z, X),
                14 => new Vector3(-Z, Y, X),
                15 => new Vector3(Y, Z, X),
                16 => new Vector3(X, -Z, Y),
                17 => new Vector3(-Z, -X, Y),
                18 => new Vector3(-X, Z, Y),
                19 => new Vector3(Z, X, Y),
                20 => new Vector3(X, Z, -Y),
                21 => new Vector3(Z, -X, -Y),
                22 => new Vector3(-X, -Z, -Y),
                23 => new Vector3(-Z, X, -Y),
            };
        }
    }
}