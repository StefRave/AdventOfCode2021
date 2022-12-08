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
        int previousHeight;
        for (int y = 0; y < input.Length; y++)
        {
            previousHeight = -1;
            for (int x = 0; x < input[0].Length; x++)
            {
                if (input[y][x] > previousHeight)
                    visible[y][x] = true;
                if (previousHeight < input[y][x])
                    previousHeight = input[y][x];
            }
        }
        for (int y = 0; y < input.Length; y++)
        {
            previousHeight = -1;
            for (int x = input[0].Length - 1; x >= 0; x--)
            {
                if (input[y][x] > previousHeight)
                    visible[y][x] = true;
                if (previousHeight < input[y][x])
                    previousHeight = input[y][x];
            }
        }

        for (int x = 0; x < input[0].Length; x++)
        {
            previousHeight = -1;
            for (int y = 0; y < input.Length; y++)
            {
                if (input[y][x] > previousHeight)
                    visible[y][x] = true;
                if (previousHeight < input[y][x])
                    previousHeight = input[y][x];
            }
        }
        for (int x = 0; x < input[0].Length; x++)
        {
            previousHeight = -1;
            for (int y = input.Length - 1; y >= 0; y--)
            {
                if (input[y][x] > previousHeight)
                    visible[y][x] = true;
                if (previousHeight < input[y][x])
                    previousHeight = input[y][x];
            }
        }

        int answer1 = visible.Select(line => line.Where(t => t).Count()).Sum();
        return answer1;
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
            int previousHeight;
            previousHeight = input[yStart][xStart];
            int countXr = 0;
            for (int x = xStart + 1; x < input[0].Length; x++)
            {
                countXr++;
                if (input[yStart][x] >= previousHeight)
                    break;
                if (previousHeight < input[yStart][x])
                    previousHeight = input[yStart][x];
            }
            previousHeight = input[yStart][xStart];
            int countXl = 0;
            for (int x = xStart - 1; x >= 0; x--)
            {
                countXl++;
                if (input[yStart][x] >= previousHeight)
                    break;
                if (previousHeight < input[yStart][x])
                    previousHeight = input[yStart][x];
            }

            previousHeight = input[yStart][xStart];
            int countYd = 0;
            for (int y = yStart + 1; y < input.Length; y++)
            {
                countYd++;
                if (input[y][xStart] >= previousHeight)
                    break;
                if (previousHeight < input[y][xStart])
                    previousHeight = input[y][xStart];
            }
            previousHeight = input[yStart][xStart];
            int countYu = 0;
            for (int y = yStart - 1; y >= 0; y--)
            {
                countYu++;
                if (input[y][xStart] >= previousHeight)
                    break;
                if (previousHeight < input[y][xStart])
                    previousHeight = input[y][xStart];
            }
            return countXl * countXr * countYd * countYu;
        }
    }
}