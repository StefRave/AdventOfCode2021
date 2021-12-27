using System.Text.RegularExpressions;

namespace AdventOfCode2021;

public class Day17 : IAdvent
{
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput();
        var m = Regex.Match(input, @"target area: x=(\d+)\.\.(\d+), y=(-\d+)..(-\d+)");
        int x0 = int.Parse(m.Groups[1].Value);
        int x1 = int.Parse(m.Groups[2].Value);
        int y0 = int.Parse(m.Groups[3].Value);
        int y1 = int.Parse(m.Groups[4].Value);

        int maxy = int.MinValue;
        int win = 0;
        for (int dx = 1; dx <= x1; dx++)
        {
            for (int dy = y0; dy < -y0; dy++)
            {
                var (hit, isOvershoot, turnMaxY) = Try(dy, dx);
                if (hit)
                {
                    win++;
                    maxy = Math.Max(turnMaxY, maxy);
                }
                if (isOvershoot)
                    break;
            }
        }
        Advent.AssertAnswer1(maxy);
        Advent.AssertAnswer2(win);


        (bool hit, bool overshoot, int maxy) Try(int dy, int dx)
        {
            int x = 0, y = 0, maxY = 0;

            while (y > y0)
            {
                x += dx;
                y += dy;
                maxY = Math.Max(maxY, y);

                if (x >= x0 && x <= x1 && y >= y0 && y <= y1)
                    return (true, false, maxY);

                dx = Math.Max(0, dx - 1);
                dy--;
            }
            return (false, x > x1, maxY);
        }
    }
}
