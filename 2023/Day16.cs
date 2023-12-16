
namespace AdventOfCode2023;
using static Day16.Movement;

public class Day16 : IAdvent
{

    public enum Movement { Right, Left, Up, Down }

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();

        int answer1 = DoIt((x: 0, y: 0), Right);
        Advent.AssertAnswer1(answer1, expected: 8551, sampleExpected: 46);

        int answer2 = 0;
        for (int x = 0; x < input[0].Length; x++)
        {
            answer2 = Math.Max(answer2, DoIt((x, 0), Down));
            answer2 = Math.Max(answer2, DoIt((x, input.Length - 1), Up));
        }
        for (int y = 0; y < input.Length; y++)
        {
            answer2 = Math.Max(answer2, DoIt((0, y), Right));
            answer2 = Math.Max(answer2, DoIt((input[0].Length - 1, y), Left));
        }
        Advent.AssertAnswer2(answer2, expected: 8754, sampleExpected: 51);


        int DoIt((int x, int y) pos, Movement m)
        {
            var visited = new Dictionary<(int x, int y), int>();
            var toGo = new Stack<((int x, int y) p, Movement m)>();
            toGo.Push((pos, m));

            while (toGo.Count > 0)
            {
                (pos, m) = toGo.Pop();
                var c = input[pos.y][pos.x];
                visited.TryGetValue(pos, out int prev);
                int mask = prev | (1 << (int)m);
                if (mask == prev)
                    continue;
                visited[pos] = mask;

                if (c == '|' && (m == Left || m == Right) ||
                   (c == '-' && (m == Up   || m == Down)))
                {
                    Process(pos, m, '\\');
                    Process(pos, m, '/');
                }
                else
                    Process(pos, m, c);
            }
            return visited.Count;


            void Process((int x, int y) pos, Movement m, char c)
            {
                m = NewDirection(m, c);
                pos = Move(pos, m);
                if (pos.x >= 0 && pos.x < input[0].Length && pos.y >= 0 && pos.y < input.Length)
                    toGo.Push((pos, m));
            }

            Movement NewDirection(Movement m, char c)
            {
                return (c, m) switch
                {
                    ('/', Right) => Up,
                    ('/', Left) => Down,
                    ('/', Up) => Right,
                    ('/', Down) => Left,
                    ('\\', Right) => Down,
                    ('\\', Left) => Up,
                    ('\\', Up) => Left,
                    ('\\', Down) => Right,
                    _ => m,
                };
            }

            (int x, int y) Move((int x, int y) pos, Movement m)
            {
                return m switch
                {
                    Right => (pos.x + 1, pos.y),
                    Left => (pos.x - 1, pos.y),
                    Up => (pos.x, pos.y - 1),
                    Down => (pos.x, pos.y + 1),
                    _ => throw new Exception("Invalid movement"),
                };
            }
        }
    }
}