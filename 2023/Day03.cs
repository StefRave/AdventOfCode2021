namespace AdventOfCode2023;

public class Day03 : IAdvent
{
    (int dy, int dx)[] Deltas = new[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)  };

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine().ToArray();

        var numbers = new Dictionary<(int y, int x), int>();
        var numberAtCoord = new Dictionary<(int y, int x), ((int y, int x), int)>();
        var parts = new Dictionary<(int y, int x), char>();
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[0].Length; x++)
            {
                char c = input[y][x];
                if (char.IsDigit(input[y][x]))
                {
                    int xStart = x;
                    while (++x < input[0].Length)
                    {
                        c = input[y][x];
                        if (!char.IsDigit(c))
                            break;
                    }
                    int number = int.Parse(input[y][xStart..x]);
                    for (int xi = xStart; xi < x; xi++)
                        numberAtCoord.Add((y, xi), ((y, xStart), number));
                    numbers.Add((y, xStart), number);
                }
                if (c != '.' && !char.IsDigit(c))
                    parts.Add((y, x), c);
            }
        }

        var numbersNextToPart = new Dictionary<(int y, int x), int>();
        long totalGearRatios = 0;
        foreach (var (coord, part) in parts)
        {
            var found = new HashSet<((int y, int x), int)>();
            foreach (var (dy, dx) in Deltas)
            {
                if (numberAtCoord.TryGetValue((coord.y + dy, coord.x + dx), out var coordNum))
                    found.Add(coordNum);
            }
            foreach(var coordNum in found)
                numbersNextToPart[(coordNum.Item1)] = coordNum.Item2;
            if (part == '*' && found.Count == 2)
                totalGearRatios += found.Aggregate(1L, (o, cn) => o * cn.Item2);
        }

        int sumOfNumbersNextToPart = numbersNextToPart.Sum(kv => kv.Value);
        Advent.AssertAnswer1(sumOfNumbersNextToPart, expected: 527364, sampleExpected: 4361);

        Advent.AssertAnswer2(totalGearRatios, expected: 79026871, sampleExpected: 467835);
    }
}
