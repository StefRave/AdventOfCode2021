namespace AdventOfCode2024;


public class Day15 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        var map = input[0].SplitByNewLine().Select(l => l.ToCharArray()).ToArray();
        var instructions = input[1].Replace("\n", "");
        var map2 = input[0].SplitByNewLine().Select(l =>
                l.Replace("#", "##")
                .Replace("O", "[]")
                .Replace(".", "..")
                .Replace("@", "@.")
                .ToCharArray()
            )
            .ToArray();

        int answer1 = DoIt(map, instructions);
        Advent.AssertAnswer1(answer1, expected: 1516281, sampleExpected: 10092);

        int answer2 = DoIt(map2, instructions);
        Advent.AssertAnswer2(answer2, expected: 1527969, sampleExpected: 9021);

        int DoIt(char[][] map, string instructions)
        {
            V2 start = (0, 0);
            for (int y = 0; y < map.Length; y++)
                for (int x = 0; x < map[0].Length; x++)
                    if (map[y][x] == '@')
                        start = (x, y);
            var p = start;

            foreach (var instruction in instructions)
            {
                var dict = new Dictionary<V2, char>();
                dict.Add(p, '.');
                var queue = new Queue<(V2, char)>();
                queue.Enqueue((p.Move(instruction), '@'));

                V2 pn = p.Move(instruction);
                if (CanMove())
                {
                    foreach (var (ps, c) in dict)
                        Set(ps, c);
                    p = pn;
                }

                bool CanMove()
                {
                    while (queue.Any())
                    {
                        var (p, pc) = queue.Dequeue();
                        if (dict.ContainsKey(p))
                            continue;

                        char c = Get(p);
                        if (c == '#')
                            return false;
                        dict.Add(p, pc);
                        if (c == '.')
                            continue;
                        if ((c == ']' && instruction == '^') || (c == '[' && instruction == 'v'))
                            queue.Enqueue((p.MoveLeft(instruction), '.'));
                        else if ((c == '[' && instruction == '^') || (c == ']' && instruction == 'v'))
                            queue.Enqueue((p.MoveRight(instruction), '.'));
                        queue.Enqueue((p.Move(instruction), c));
                    }
                    return true;
                }
            }
            int answer = 0;
            for (int y = 0; y < map.Length; y++)
                for (int x = 0; x < map[0].Length; x++)
                {
                    if (map[y][x] == 'O')
                        answer += 100 * y + x;
                    else if (map[y][x] == '[')
                        answer += 100 * y + x;
                }
            return answer;

            char Get(V2 p) => map[p.y][p.x];
            char Set(V2 p, char c) => map[p.y][p.x] = c;
        }
    }
}

public static class V2Extensions
{
    public static V2 Move(this V2 p, char move)
    {
        return p + move switch
        {
            '^' => (0, -1),
            '>' => (1, 0),
            'v' => (0, 1),
            '<' => (-1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(move))
        };
    }
    public static V2 MoveLeft(this V2 p, char move)
    {
        return p + move switch
        {
            '^' => (-1, 0),
            '>' => (0, -1),
            'v' => (1, 0),
            '<' => (0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(move))
        };
    }
    public static V2 MoveRight(this V2 p, char move)
    {
        return p + move switch
        {
            '^' => (1, 0),
            '>' => (0, 1),
            'v' => (-1, 0),
            '<' => (0, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(move))
        };
    }
}