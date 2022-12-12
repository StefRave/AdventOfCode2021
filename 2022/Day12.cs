namespace AdventOfCode2022;

public class Day12 : IAdvent
{
    static (int y, int x, char dir)[] M = new (int,int, char)[] {(-1, 0, 'V'), (1, 0, '^'), (0, -1, '<'), (0, 1, '>')};

    public void Run()
    {
        var input = Advent.ReadInput();
        int width = input.IndexOf('\n');
        input = input.Replace("\n", "");
        int height = input.Length / width;

        int start = input.IndexOf('S');
        (int y, int x) s = (start / width, start % width);
        int end = input.IndexOf('E');
        (int y, int x) e = (end / width, end % width);

        var answer1 = DoIt(input.ToArray(), s, goUp: true);
        Advent.AssertAnswer1(answer1, expected: 350, sampleExpected: 31);


        var answer2 = DoIt(input.ToArray(), e, goUp: false);
        Advent.AssertAnswer2(answer2, expected: 349, sampleExpected: 29);


        int DoIt(char[] field, (int y, int x) s, bool goUp)
        {
            var toGo = new Queue<(int y, int x, int step, int eliv)>();
            toGo.Enqueue((s.y, s.x, 0, goUp ? 'a' : 'z'));

            while (true)
            {
                var cur = toGo.Dequeue();
                foreach (var m in M)
                {
                    var (y, x) = (cur.y - m.y, cur.x + m.x);
                    if (y < 0 || x < 0 || y >= height || x >= width)
                        continue;
                    char ch = field[y * width + x];
                    if (ch == '<' || ch == '>' || ch == 'V' || ch == '^') // been there
                        continue;
                    if (goUp)
                    {
                        if (ch == 'E' && (cur.eliv == 'z' || cur.eliv == 'y'))
                            return cur.step + 1;
                        if (ch == 'S')
                            continue;
                        if (cur.eliv + 1 < ch || ch == 'E')
                            continue;
                    }
                    else
                    {
                        if (ch == 'E')
                            continue;
                        if (cur.eliv - 1 > ch || ch == 'S')
                            continue;
                        if (ch == 'a')
                            return cur.step + 1;
                    }

                    field[y * width + x] = m.dir;
                    toGo.Enqueue((y, x, cur.step + 1, ch));
                }
            }
        }
    }
}