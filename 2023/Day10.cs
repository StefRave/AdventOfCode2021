namespace AdventOfCode2023;

public class Day10 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.ToCharArray())
            .ToArray();
        input = Resize(input);

        var start = GetStartingPosition();
        var pos = start;
        char dir = 'N';
        int moves = 0;
        do
        {
            char c = input[pos.Y][pos.X];
            input[pos.Y][pos.X] = '*';

            var newDir = (c, dir) switch
            {
                ('S', 'N') => 'E',
                ('F', 'N') => 'E',
                ('F', 'W') => 'S',
                ('7', 'E') => 'S',
                ('7', 'N') => 'W',
                ('J', 'S') => 'W',
                ('J', 'E') => 'N',
                ('L', 'W') => 'N',
                ('L', 'S') => 'E',
                (_, _) => dir,
            };
            if ((pos.X % 2 != 0) && (pos.Y % 2 != 0))
                moves++;
            
            var newPos = pos.Move(newDir);
            dir = newDir;
            pos = newPos;
        }
        while (pos != start);


        Advent.AssertAnswer1(moves / 2, expected: 6864, sampleExpected: 78);

        int answer2 = Fill(input, start + new V2(1, 1));
        Advent.AssertAnswer2(answer2, expected: 349, sampleExpected: 14);

        V2 GetStartingPosition()
        {
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[0].Length; x++)
                    if (input[y][x] == 'S')
                        return new V2(x, y);
            throw new Exception("not found");
        }        
    }

    private int Fill(char[][] input, V2 start)
    {
        var queue = new Stack<V2>();
        queue.Push(start);
        int count = 0;
        while (queue.Count > 0)
        {
            var p = queue.Pop();
            if (input[p.Y][p.X] == '*')
                continue;
            if ((p.X % 2 != 0) && (p.Y % 2 != 0))
                count++;
            input[p.Y][p.X] = '*';

            foreach (var d in V2.Deltas)
            {
                var pos = p + d;
                if (pos.X >= 0 && pos.Y >= 0 && pos.Y < input.Length && pos.X < input[0].Length)
                    queue.Push(pos);
            }
        }
        return count;
    }

    private char[][] Resize(char[][] input)
    {
        var ni = Enumerable.Repeat(0, input.Length * 2 + 1)
            .Select(i => "".PadRight(input[0].Length * 2 + 1, '.').ToArray())
            .ToArray();
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[0].Length; x++)
                ni[1 + y * 2][1 + x * 2] = input[y][x];

        for (int y = 2; y < ni.Length - 1; y += 2)
            for (int x = 1; x < ni[0].Length; x += 2)
            {
                char n = ni[y - 1][x];
                char s = ni[y + 1][x];
                if ((n == '|' || n == 'F' || n == 'S' || n == '7') && (s == '|' || s == 'L' || s == 'J'))
                    ni[y][x] = '|';
            }
        for (int y = 1; y < ni.Length; y += 2)
            for (int x = 2; x < ni[0].Length - 1; x += 2)
            {
                char w = ni[y][x - 1];
                char e = ni[y][x + 1];
                if ((w == '-' || w == 'F' || w == 'S' || w == 'L') && (e == '-' || e == 'J' || e == '7'))
                    ni[y][x] = '-';
            }
        return ni;
    }

    public record V2(int X, int Y)
    {
        public static V2 operator -(V2 a, V2 b) => new V2(a.X - b.X, a.Y - b.Y);
        public static V2 operator +(V2 a, V2 b) => new V2(a.X + b.X, a.Y + b.Y);
        public static V2[] Deltas = new[] { new V2(-1, 0), new V2(0, -1), new V2(0, 1), new V2(1, 0) };

        public V2 Move(char direction)
        {
            return direction switch
            {
                'E' => new V2(X + 1, Y),
                'W' => new V2(X - 1, Y),
                'N' => new V2(X, Y - 1),
                'S' => new V2(X, Y + 1),
            };
        }
    }
}
