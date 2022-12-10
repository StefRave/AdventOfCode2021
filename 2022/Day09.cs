using AdventOfCode2022.Helpers;

namespace AdventOfCode2022;

public class Day09 : IAdvent
{
    record Motion(char Dir, int Steps);

    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line =>
            {
                //D 19
                var s = line.Split(' ');
                return new Motion(s[0][0], int.Parse(s[1]));
            })
            .ToArray();

        int answer1 = DoIt(input, 1);
        Advent.AssertAnswer1(answer1, expected: 5981, sampleExpected: 88);


        int answer2 = DoIt(input, 9);
        Advent.AssertAnswer2(answer2, expected: 2352, sampleExpected: 36);
    }

    static int DoIt(Motion[] input, int tailLength)
    {
        var tailLocations = new HashSet<(int y, int x)>();
        var rope = Init.Array((y: 0, x: 0), tailLength + 1);

        foreach (var move in input)
        {
            for (int step = 0; step < move.Steps; step++)
            {
                rope[0] = move.Dir switch
                {
                    'U' => (rope[0].y + 1, rope[0].x),
                    'D' => (rope[0].y - 1, rope[0].x),
                    'R' => (rope[0].y, rope[0].x + 1),
                    'L' => (rope[0].y, rope[0].x - 1),
                    _ => throw new ArgumentOutOfRangeException()
                };
                for (int i = 1; i < rope.Length; i++)
                {
                    var h = rope[i - 1];
                    var t = rope[i];
                    if (Math.Abs(h.x - t.x) > 1 || Math.Abs(h.y - t.y) > 1)
                        rope[i] = (Towards(t.y, h.y), Towards(t.x, h.x));
                }
                tailLocations.Add(rope[^1]);
            }
        }

        Draw.DrawYx(
            tailLocations.Select(l => (l.y, l.x, '#'))
            .Union(rope.Select((l, i) => (l.y, l.x, i == 0 ? 'H' : (char)('0' + i))))
            .Union(new[] { (0, 0, 's') }), step: 1);

        return tailLocations.Count;
    }

    public static int Towards(int t, int h) => t == h ? h : (t > h ? t - 1 : t + 1);
}