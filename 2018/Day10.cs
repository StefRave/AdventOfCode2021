using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day10 : IAdvent
{
    public void Run()
    {
        IEnumerable<int[]> inputNumbers = Advent.ReadInputLines()
            .Select(line => Regex.Matches(line, @"-?\d+").Cast<Match>().Select(m => int.Parse(m.Value)).ToArray());
        var input = inputNumbers
            .Select(i => new PointVelocity(new Vector(i[0], i[1]), new Vector(i[2], i[3])))
            .ToArray();

        var state = input;
        int seconds = 0;
        int expectedHeight = Advent.UseSampleData ? 8 : 9;
        while (state.Max(pv => pv.Point.Y) - state.Min(pv => pv.Point.Y) > expectedHeight)
        {
            state = Iteration(state);
            seconds++;
        }
        Print(state);

        Advent.AssertAnswer2(seconds, expected: 10105, sampleExpected: 3);
    }

    private void Print(PointVelocity[] state)
    {
        int minX = state.Min(pv => pv.Point.X);
        int maxX = state.Max(pv => pv.Point.X);
        int minY = state.Min(pv => pv.Point.Y);
        int maxY = state.Max(pv => pv.Point.Y);

        for (int y = minY; y <= maxY; y++)
        {
            var line = "";
            for (int x = minX; x <= maxX; x++)
            {
                var p = new Vector(x, y);
                line += state.Any(s => s.Point == p) ? '#' : ' ';
            }
            Console.WriteLine(line);
        }
    }

    private PointVelocity[] Iteration(PointVelocity[] state)
    {
        return state
            .Select(pv => pv with { Point = pv.Point with { X = pv.Point.X + pv.Velocity.X, Y = pv.Point.Y + pv.Velocity.Y } })
            .ToArray();
    }

    public record PointVelocity(Vector Point, Vector Velocity);
    public record Vector(int X, int Y);
}