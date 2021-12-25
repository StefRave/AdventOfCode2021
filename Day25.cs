namespace AdventOfCode2021;

public class Day25
{
    [Fact]
    public void Run()
    {
        char[][] input = Advent.ReadInputLines()
            .Select(line => line.ToArray())
            .ToArray();
        int maxY = input.Length;
        int maxX = input[0].Length;

        int step = 0;
        var map = input;
        while (true)
        {
            char[][] newMap = new char[maxY][];

            for (int y = 0; y < maxY; y++)
                newMap[y] = "".PadRight(maxX, '.').ToCharArray();

            bool moved = false;
            // the east-facing herd moves first
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (map[y][x] == '>')
                    {
                        if (map[y][(x + 1) % maxX] != '.')
                            newMap[y][x] = '>';
                        else
                        {
                            moved = true;
                            newMap[y][(x + 1) % maxX] = '>';
                        }
                    }
                    else if (map[y][x] == 'v')
                        newMap[y][x] = 'v';
                }
            }
            map = newMap;
            newMap = new char[maxY][];
            for (int y = 0; y < maxY; y++)
                newMap[y] = "".PadRight(maxX, '.').ToCharArray();
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (map[y][x] == 'v')
                    {
                        if (map[(y + 1) % maxY][x] != '.')
                            newMap[y][x] = 'v';
                        else
                        {
                            moved = true;
                            newMap[(y + 1) % maxY][x] = 'v';
                        }
                    }
                    else if (map[y][x] == '>')
                        newMap[y][x] = map[y][x];
                }
            }
            map = newMap;
            step++;
            if (!moved)
                break;
        }
        Advent.AssertAnswer1(step);
    }
}
