namespace AdventOfCode2023;

public class Day14 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.ToCharArray())
            .ToArray();

        MoveNorth();
        Advent.AssertAnswer1(CountWeight(), expected: 108889, sampleExpected: 136);


        var list = new Dictionary<string, int>();
        int targetCycles = 1000000000;
        int cycle = 0;
        int repeat = 0;
        while(true)
        {
            MoveCycle();
            cycle++;

            string snap = input.Aggregate("", (a, l) => a += new string(l));
            if (list.TryGetValue(snap, out int previousCycle))
            {
                repeat = cycle - previousCycle;
                break;
            }
            list.Add(snap, cycle);
        }
        cycle += ((targetCycles - cycle) / repeat) * repeat;

        for ( ; cycle < targetCycles; cycle++)
            MoveCycle();

        Advent.AssertAnswer2(CountWeight(), expected: 104671, sampleExpected: 64);


        void MoveCycle()
        {
            MoveNorth();
            MoveWest();
            MoveSouth();
            MoveEast();
        }

        void MoveNorth()
        {
            for (int y = 1; y < input.Length; y++)
                for (int x = 0; x < input[0].Length; x++)
                {
                    if (input[y][x] == 'O')
                    {
                        input[y][x] = '.';
                        int ym = y;
                        while (ym-- > 0)
                        {
                            if (input[ym][x] != '.')
                                break;
                        }
                        input[++ym][x] = 'O';
                    }
                }
        }

        void MoveWest()
        {
            for (int x = 1; x < input[0].Length; x++)
                for (int y = 0; y < input.Length; y++)
                {
                    if (input[y][x] == 'O')
                    {
                        input[y][x] = '.';
                        int xm = x;
                        while (xm-- > 0)
                        {
                            if (input[y][xm] != '.')
                                break;
                        }
                        input[y][++xm] = 'O';
                    }
                }
        }

        void MoveSouth()
        {
            for (int y = input.Length - 1; y >= 0; y--)
                for (int x = 0; x < input[0].Length; x++)
                {
                    if (input[y][x] == 'O')
                    {
                        input[y][x] = '.';
                        int ym = y;
                        while (ym++ < input.Length - 1)
                        {
                            if (input[ym][x] != '.')
                                break;
                        }
                        input[--ym][x] = 'O';
                    }
                }
        }

        void MoveEast()
        {
            for (int x = input[0].Length - 1; x >= 0; x--)
                for (int y = 0; y < input.Length; y++)
                {
                    if (input[y][x] == 'O')
                    {
                        input[y][x] = '.';
                        int xm = x;
                        while (xm++ < input[0].Length - 1)
                        {
                            if (input[y][xm] != '.')
                                break;
                        }
                        input[y][--xm] = 'O';
                    }
                }
        }

        int CountWeight()
        {
            int answer = 0;
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[y].Length; x++)
                    if (input[y][x] == 'O')
                        answer += input.Length - y;
            return answer;
        }
    }
}