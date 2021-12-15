namespace AdventOfCode2021;

public class Day15
{
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();
        int xMax = input[0].Length;
        int yMax = input.Length;

        Advent.AssertAnswer1(Solve());
        Advent.AssertAnswer2(Solve(tileCount: 5));
        

        int Solve(int tileCount = 1)
        {
            int[][] lengths = new int[yMax * tileCount][];
            for (int y = 0; y < yMax * tileCount; y++)
            {
                lengths[y] = new int[xMax * tileCount];
                for (int x = 0; x < xMax * tileCount; x++)
                    lengths[y][x] = int.MaxValue;
            }

            lengths[0][0] = 0;
            var toDo = new Queue<(int y, int x)>();
            toDo.Enqueue((0, 0));
            while (toDo.Count > 0)
            {
                var (y, x) = toDo.Dequeue();
                int minLength = lengths[y][x];
                foreach (var (yn, xn) in Surrounding(y, x))
                {
                    int possibleLength = minLength + ((input[yn % yMax][xn % xMax] + yn / yMax + xn / xMax - 1) % 9) + 1;
                    if (possibleLength < lengths[yn][xn])
                    {
                        lengths[yn][xn] = possibleLength;
                        toDo.Enqueue((yn, xn));
                    }
                }
            }
            return lengths[^1][^1];


            IEnumerable<(int y, int x)> Surrounding(int y, int x)
            {
                if (y > 0)                    yield return (y - 1, x);
                if (x > 0)                    yield return (y,     x - 1);
                if (y < yMax * tileCount - 1) yield return (y + 1, x);
                if (x < xMax * tileCount - 1) yield return (y,     x + 1);
            }
        }
    }
}
