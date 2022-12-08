using System.Runtime.CompilerServices;

namespace AdventOfCode2022;

public class Day08 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.ToArray())
            .ToArray();

        int answer1 = DoPart1(input);
        Advent.AssertAnswer1(answer1, expected: 1809, sampleExpected: 21);

        int answer2 = DoPart2(input);
        Advent.AssertAnswer2(answer2, expected: 479400, sampleExpected: 8);
    }

    private static int DoPart1(char[][] input)
    {
        bool[][] visible = Enumerable.Repeat(1, input.Length)
            .Select(_ => new bool[input[0].Length])
            .ToArray();

        for (int y = 0; y < input.Length; y++)
        {
            Move((y, 0), (0, 1));
            Move((y, input[0].Length - 1), (0, -1));
        }
        for (int x = 0; x < input[0].Length; x++)
        {
            Move((0, x), (1, 0));
            Move((input.Length - 1, x), (-1, 0));
        }

        int answer1 = visible.Select(line => line.Where(t => t).Count()).Sum();
        return answer1;


        void Move((int y, int x) start, (int y, int x) delta, int previousHeight = -1)
        {
            var (y, x) = start;

            if (y >= 0 && x >= 0 && x < input[0].Length && y < input.Length)
            {
                if (input[y][x] > previousHeight)
                    visible[y][x] = true;
                if (previousHeight < input[y][x])
                    previousHeight = input[y][x];

                Move((y + delta.y, x + delta.x), delta, previousHeight);
            }
        }
    }

    private static int DoPart2(char[][] input)
    {
        int[][] d2 = Enumerable.Repeat(1, input.Length)
            .Select(_ => new int[input[0].Length])
            .ToArray();

        int answer2 = 0;
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[0].Length; x++)
            {
                int val2 = CountTreesFromCoord(y, x);
                d2[y][x] = val2;
                answer2 = Math.Max(answer2, val2);
            }
        return answer2;


        int CountTreesFromCoord(int yStart, int xStart)
        {
            return
                CountVisible((0, 1)) *
                CountVisible((1, 0)) *
                CountVisible((0, -1)) *
                CountVisible((-1, 0));

            int CountVisible((int y, int x) delta)
            {
                var (y, x) = (yStart + delta.y, xStart + delta.x);
                int count = 0;
                while (y >= 0 && x >= 0 && x < input[0].Length && y < input.Length)
                {
                    count++;
                    if (input[y][x] >= input[yStart][xStart])
                        break;
                    (y, x) = (y + delta.y, x + delta.x);
                }
                return count;
            }
        }
    }
}