namespace AdventOfCode2023;

public class Day11 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.ToCharArray())
            .ToArray();
        var markEmptyY = new bool[input.Length];
        var markEmptyX = new bool[input[0].Length];

        for (int y = 1; y < input.Length - 1; y++)
            if (input[y].All(c => c == '.'))
                markEmptyY[y] = true;
        for (int x = 1; x < input[0].Length - 1; x++)
            if (input.All(line => line[x] == '.' || line[x] == ' '))
                markEmptyX[x] = true;

        var coords = new List<(int X, int Y)>();
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[0].Length; x++)
                if (input[y][x] != '.')
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
                    var (cx1, cx2) = (coords[i].X, coords[j].X);
                    var (cy1, cy2) = (coords[i].Y, coords[j].Y);
                    var dx = (cx2 - cx1) == 0 ? 0 : (cx2 - cx1) / Math.Abs(cx2 - cx1);
                    var dy = (cy2 - cy1) == 0 ? 0 : (cy2 - cy1) / Math.Abs(cy2 - cy1);
                    for (int x = cx1; x != cx2; x += dx)
                        totalDist += markEmptyX[x] ? distanceForEmptySpace : 1;
                    for (int y = cy1; y != cy2; y += dy)
                        totalDist += markEmptyY[y] ? distanceForEmptySpace : 1;
                }
            return totalDist;
        }
    }
}
