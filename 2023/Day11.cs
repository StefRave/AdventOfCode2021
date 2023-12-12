namespace AdventOfCode2023;

public class Day11 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.ToCharArray())
            .ToArray();

        var markEmptyY = input.Select(line => line.All(c => c == '.')).ToArray();
        var markEmptyX = input[0].Select((_, x) => input.All(line => line[x] == '.')).ToArray();

        var coords = new List<(int X, int Y)>();
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
                if (input[y][x] == '#')
                    coords.Add((x, y));

        long answer1 = CalculateTotalDistance(2);

        Advent.AssertAnswer1(answer1, expected: 9521550, sampleExpected: 374);

        long answer2 = CalculateTotalDistance(Advent.UseSampleData ? 100 : 1000000);
        Advent.AssertAnswer2(answer2, expected: 298932923702, sampleExpected: 8410);


        long CalculateTotalDistance(long distanceForEmptySpace)
        {
            long totalDist = 0;
            for (int i = 0; i < coords.Count; i++)
                for (int j = i + 1; j < coords.Count; j++)
                {
                    AddDistance(coords[i].X, coords[j].X, markEmptyX);
                    AddDistance(coords[i].Y, coords[j].Y, markEmptyY);
                }
            return totalDist;


            void AddDistance(int c1, int c2, bool[] markEmpty)
            {
                if (c1 == c2)
                    return;
                var dx = (c2 - c1) / Math.Abs(c2 - c1);
                for (int x = c1; x != c2; x += dx)
                    totalDist += markEmpty[x] ? distanceForEmptySpace : 1;
            }
        }
    }
}
