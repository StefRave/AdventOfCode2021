namespace AdventOfCode2024;

public class Day06 : IAdvent
{
    enum Direction { Up, Right, Down, Left }

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(l => l.ToCharArray())
            .ToArray();

        (int y, int x) startPos = (0, 0);
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
                if (input[y][x] == '^')
                    startPos = (y, x);
        
        int answer1 = Walk(startPos, Direction.Up).Value;
        Advent.AssertAnswer1(answer1, expected: 5067, sampleExpected: 41);

        int answer2 = 0;
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] != '*')
                    continue;
                input[y][x] = '#';
                if (Walk(startPos, Direction.Up) == null)
                    answer2++;
                input[y][x] = '.';
            }
        Advent.AssertAnswer2(answer2, expected: 1793, sampleExpected: 6);


        int? Walk((int y, int x) startPos, Direction startDir)
        {
            var pos = startPos;
            var dir = startDir;
            var walked = new int[input.Length, input[0].Length];
            int posWalkedCount = 0;
            while (true)
            {
                if ((walked[pos.y, pos.x] & (1 << (int)dir)) != 0)
                    return null;
                if (walked[pos.y, pos.x] == 0)
                    posWalkedCount++;
                walked[pos.y, pos.x] |= 1 << (int)dir;
                input[pos.y][pos.x] = '*';

                (int y, int x) newPos = dir switch
                {
                    Direction.Up => (pos.y - 1, pos.x),
                    Direction.Right => (pos.y, pos.x + 1),
                    Direction.Down => (pos.y + 1, pos.x),
                    Direction.Left => (pos.y, pos.x - 1),
                    _ => throw new Exception()
                };
                if (!IsInside(newPos))
                    break;

                if (input[newPos.y][newPos.x] == '#')
                    dir = (Direction)(((int)dir + 1) % 4);
                else
                    pos = newPos;
            }
            return posWalkedCount;
        }

        bool IsInside((int y, int x) pos) => pos.y >= 0 && pos.y < input.Length && pos.x >= 0 && pos.x < input[0].Length;
    }
}





