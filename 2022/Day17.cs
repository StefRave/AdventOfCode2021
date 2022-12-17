using System.Linq;

namespace AdventOfCode2022;

public class Day17 : IAdvent
{
    private const int empty = 0b100000001;

    public void Run()
    {
        string rocksString = @"1111

                                010
                                111
                                010

                                001
                                001
                                111

                                1
                                1
                                1
                                1

                                11
                                11";

        var input = Advent.ReadInput();
        var rocks = rocksString.SplitByDoubleNewLine()
            .Select(rock => rock.SplitByNewLine()
                .Select(l => Convert.ToInt32(l.Trim().PadRight(6, '0'), 2)).ToArray())
            .ToArray();

        var field = new int[50000000];
        field[^1] = 0b111111111;
        int index = field.Length - 2;

        int wi = 0;
        long rocknr;
        for (rocknr = 0; rocknr != 2022; rocknr++)
            DoIt(rocknr);
        
        long answer1 = field.Length - 2 - index;
        Advent.AssertAnswer1(answer1, expected: 3149, sampleExpected: 3068);

        field = new int[50000000];
        field[^1] = 0b111111111;
        index = field.Length - 2;
        wi = 0;

        long totalCount = 0;
        int firstWi = 0;
        long firstTotal = 0;
        long totalRocks = 0;
        bool first = true;
        while (true)
        {
            long until = ((totalRocks / rocks.Length / input.Length) + 1) * rocks.Length * input.Length;
            for (; totalRocks < until; totalRocks++)
                DoIt(totalRocks);
            
            totalCount = field.Length - 2 - index;

            wi %= input.Length;
            if (first)
            {
                first = false;
                firstWi = wi;
                firstTotal = totalCount;
            }
            else if (firstWi == wi)
                break;
        }
        long repeateRows = totalCount - firstTotal;
        long repeatRocks = totalRocks - rocks.Length * input.Length;

        long endAt = 1000000000000;
        long add = (endAt - totalRocks) / repeatRocks;
        totalRocks += add * repeatRocks;



        for (; totalRocks < endAt; totalRocks++)
            DoIt(totalRocks);

        long answer2 = field.Length - 2 - index + add * repeateRows;
        Advent.AssertAnswer2(answer2, expected: 1553982300884, sampleExpected: 1514285714288);


        void DoIt(long ri)
        {
            var rock = rocks[ri % rocks.Length];
            int rockDown = index - rock.Length - 2;

            int windOffset = 0;
            while (true)
            {
                int addOffset = input[(int)(wi++ % input.Length)] == '<' ? -1 : 1;
                bool canMoveByWind = true;
                for (int i = 0; i < rock.Length; i++)
                {
                    int movedRockLine = ((rock[i] << 8) >> (8 + windOffset + addOffset));
                    int v = field[(rockDown + i)] | empty;
                    if ((movedRockLine & v) != 0)
                    {
                        canMoveByWind = false;
                        break;
                    }
                }
                if (canMoveByWind)
                    windOffset += addOffset;

                bool canFall = true;
                for (int i = 0; i < rock.Length; i++)
                {
                    int movedRockLine = (rock[i] << 8) >> (8 + windOffset);
                    if ((movedRockLine & field[rockDown + i + 1]) != 0)
                    {
                        canFall = false;
                        break;
                    }
                }
                if (!canFall)
                    break;
                rockDown++;
            }
            index = Math.Min(rockDown - 1, index);
            for (int i = 0; i < rock.Length; i++)
            {
                int movedRockLine = (rock[i] << 8) >> (8 + windOffset);
                field[rockDown + i] |= movedRockLine;
            }
        }
    }

    static void Draw(int[] field, int index)
    {
        for (int i = index - 3; i < Math.Min(field.LongLength, index + 17); i++)
            Console.WriteLine(Visualize(field[(i)]));
        Console.WriteLine();
     
        
        static string Visualize(int l, char to = '#')
        {
            return Convert.ToString(l | empty, 2).PadLeft(7, '.').Replace('1', to).Replace('0', '.');
        }
    }
}