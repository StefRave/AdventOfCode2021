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
                p = ExecuteInstruction(map, p, instruction);
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
        }
    }

    static V2 ExecuteInstruction(char[][] map, V2 p, char instruction)
    {
        var newValues = new Dictionary<V2, char>();
        newValues.Add(p, '.');
        var positionsToCheck = new Queue<(V2, char)>();
        positionsToCheck.Enqueue((p.Move(instruction), '@'));

        V2 pn = p.Move(instruction);
        if (CanMove())
        {
            foreach (var (ps, c) in newValues)
                map[ps.y][ps.x] = c;
            p = pn;
        }
        return p;


        bool CanMove()
        {
            while (positionsToCheck.Any())
            {
                var (p, pc) = positionsToCheck.Dequeue();
                if (newValues.ContainsKey(p))
                    continue;

                char c = map[p.y][p.x];
                if (c == '#')
                    return false;
                newValues.Add(p, pc);
                if (c == '.')
                    continue;
                if (instruction == '^' || instruction == 'v')
                {
                    if (c == ']')
                        positionsToCheck.Enqueue((p + (-1, 0), '.'));
                    else if (c == '[')
                        positionsToCheck.Enqueue((p + (1, 0), '.'));
                }
                positionsToCheck.Enqueue((p.Move(instruction), c));
            }
            return true;
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
}