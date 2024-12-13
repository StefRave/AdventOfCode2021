namespace AdventOfCode2024;

public class Day13 : IAdvent
{
    public void Run()
    {
        var games = ParseInput(Advent.ReadInput());

        long answer1 = Calc(games);
        Advent.AssertAnswer1(answer1, expected: 39290, sampleExpected: 480);

        long answer2 = Calc(games, 10000000000000);
        Advent.AssertAnswer2(answer2, expected: 73458657399094, sampleExpected: 875318608908);
    }

    private static Game[] ParseInput(string file)
    {
        var input = file.SplitByDoubleNewLine();
        return input
            .Select(game => game.SplitByNewLine())
            .Select(a => new Game(Parse(a[2]), Parse(a[0]), Parse(a[1])))
            .ToArray();
    }
    public static Xy Parse(string line)
    {
        var m = Regex.Matches(line, @"\d+").Select(m => int.Parse(m.Value)).ToArray();
        return new Xy(m[0], m[1]);
    }

    private static long Calc(Game[] games, long toAdd = 0)
    {
        long answer = 0;
        foreach (Game g in games)
        {
            long px = g.Prize.X + toAdd;
            long py = g.Prize.Y + toAdd;

            long bMax = Math.Min(1 + (px - 1) / g.B.X, 1 + (py - 1) / g.B.Y);
            long prevSolA = 0, prevSolB = 0;
            for (long b = bMax - 1000; b <= bMax; b++)
            {
                long xRest = px - b * g.B.X;
                if (xRest % g.A.X == 0)
                {
                    long a = xRest / g.A.X;
                    if (prevSolA == 0)
                    {
                        prevSolA = a; prevSolB = b;
                    }
                    else
                    {
                        var nextSolA = a + (a - prevSolA);
                        var nextSolB = b + (b - prevSolB);

                        long da = prevSolA - a;
                        long db = prevSolB - b;
                        long dy = da * g.A.Y + db * g.B.Y;

                        long missing = py - (b * g.B.Y + a * g.A.Y);
                        if ((missing % dy) == 0)
                        {
                            long turns = missing / dy;

                            var winA = a - turns * (a - prevSolA);
                            var winB = b - turns * (b - prevSolB);
                            answer += winA * 3 + winB;
                            break;
                        }
                    }
                }
            }
        }
        return answer;
    }

    public record Game(Xy Prize, Xy A, Xy B)
    {
        public Xy Points(long a, long b) => new Xy(A.X * a + B.X * b, A.Y * a + B.Y * b);
    }

    public record Xy(long X, long Y);
}












