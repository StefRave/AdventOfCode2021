namespace AdventOfCode2023;

public class Day10 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.ToCharArray())
            .ToArray();
        var start = GetStartingPosition();
        var pos = start;
        char dir = 'N';
        int moves = 0;
        do
        {
            char c = input[pos.Y][pos.X];
            //Console.WriteLine($"{moves} {dir} {c} {pos}");
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
            var newPos = pos.Move(newDir);
            {
                if (pos.Y == 1 && pos.X == 4)
                    1.ToString();
                var toOuter = DirToOuter(dir);
                TryMark(pos + toOuter, 'O');
                if (c == '|' || c == '-')
                    TryMark(pos - toOuter, 'I');
                else
                {
                    var toOuter2 = DirToOuter(newDir);
                    if (newPos + toOuter2 == pos)
                    {
                        TryMark(pos - toOuter2, 'I');
                        TryMark(pos - toOuter2 - toOuter, 'I');
                    }
                    else
                    {
                        TryMark(pos + toOuter2, 'O');
                        TryMark(pos + toOuter2 + toOuter, 'O');
                    }
                }
            }
            dir = newDir;
            pos = newPos;
            moves++;
        }
        while (pos != start);

        foreach (var line in input)
            Console.WriteLine(new string(line));

        //Advent.AssertAnswer1(moves / 2, expected: 100010, sampleExpected: 8);




        bool marked = true;
        while (marked)
        {
            marked = false;
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[0].Length; x++)
                    if (input[y][x] == 'I')
                        foreach (var d in V2.Deltas)
                            marked |= TryMark(new V2(x, y) + d, 'I');
        }
        var chars = input.SelectMany(line => line);
        int countI = chars.Count(c => c == 'I');
        int countO = chars.Count(c => c == 'O');
        int countP = chars.Count(c => c == '.');
        Console.WriteLine($"I{countI} O{countO} P{countP}");
        Advent.AssertAnswer2(5432, expected: 200010, sampleExpected: 2010);


        V2 GetStartingPosition()
        {
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[0].Length; x++)
                    if (input[y][x] == 'S')
                        return new V2(x, y);
            throw new Exception("not found");
        }

        V2 DirToOuter(char dir)
        {
            return dir switch
            {
                'N' => new V2(-1, 0),
                'E' => new V2(0, -1),
                'S' => new V2(1, 0),
                'W' => new V2(0, 1),
            };
        }

        bool TryMark(V2 pos, char c)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.Y >= input.Length || pos.X >= input[0].Length || input[pos.Y][pos.X] != '.')
                return false;
            input[pos.Y][pos.X] = c;
            return true;
        }

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
