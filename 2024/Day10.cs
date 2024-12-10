
namespace AdventOfCode2024;

public class Day10 : IAdvent
{
    private static (int dy, int dx)[] directions = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();

        int answer1 = 0;
        int answer2 = 0;
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[0].Length; x++)
            {
                if (input[y][x] == '0')
                {
                    var (d1, d2) = CountTrails((y, x));
                    answer1 += d1;
                    answer2 += d2;
                }
            }

        Advent.AssertAnswer1(answer1, expected: 472, sampleExpected: 36);
        Advent.AssertAnswer2(answer2, expected: 4855, sampleExpected: 81);


        (int d1, int d2) CountTrails((int y, int x) startPos)
        {
            HashSet<(int y, int x)> endPoints = new();
            Queue<((int y, int x), char height)> queue = new();
            int d2 = 0;

            queue.Enqueue((startPos, '1'));
            while (queue.Count > 0)
            {
                var (pos, height) = queue.Dequeue();
                foreach (var d in directions)
                {
                    var p = (y: pos.y + d.dy, x: pos.x + d.dx);
                    if (!IsInside(p) || input[p.y][p.x] != height)
                        continue;
                    if (height == '9')
                    {
                        d2++;
                        endPoints.Add(p);
                    }
                    queue.Enqueue((p,  (char)(height + 1)));
                }
            }
            return (endPoints.Count, d2);
        }

        bool IsInside((int y, int x) pos) => pos.x >= 0 && pos.x < input[0].Length && pos.y >= 0 && pos.y < input.Length;
    }
}