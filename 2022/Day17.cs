using System.Linq;

namespace AdventOfCode2022;

public class Day17 : IAdvent
{
    private const int empty = 0b100000001;

    public void Run()
    {
        string rocksString = @"####

                                .#.
                                ###
                                .#.

                                ..#
                                ..#
                                ###

                                #
                                #
                                #
                                #

                                ##
                                ##";

        var input = Advent.ReadInput();

        var rocks = rocksString.SplitByDoubleNewLine()
            .Select(rock => rock.SplitByNewLine()
                .Select(l =>
                {
                    string value = l.Trim().Replace('#', '1').Replace('.', '0').PadRight(6, '0');
                    return Convert.ToInt32(value, 2);
                }).ToArray())
            .ToArray();

        var field = Enumerable.Repeat(empty, 99).Concat(new[] { 0b111111111 }).ToArray();
        long index = field.Length - 1;
        Draw(field, index);

        long wi = 0;
        long rocknr;
        long cleaned = 0;
        long totalRocks = 0;
        for (rocknr = 0; rocknr < 2022; rocknr++, totalRocks++)
            DoIt(rocknr, ref wi);
        
        long answer1 = field.Where(l => l != empty).Count() + cleaned;
        Advent.AssertAnswer1(answer1, expected: 3149, sampleExpected: 3068);

        Console.WriteLine(input.Length);


        long prevCount = 0;
        int repeatDelta = 0;
        long totalCount = 0;
        List<long> deltas = new List<long>();
        for (int j = 0; j < 1000 && repeatDelta == 0; j++)
        {
            for (; rocknr < rocks.Length * input.Length; rocknr++, totalRocks++)
                DoIt(rocknr, ref wi);

            totalCount = field.Where(l => l != empty).Count() + cleaned;
            deltas.Add(totalCount - prevCount);
            if (deltas.Count > 10)
            {
                for (int i = 0; i < deltas.Count - 6; i++)
                {
                    if (deltas.Skip(deltas.Count - 5).SequenceEqual(deltas.Skip(i).Take(5)))
                    {
                        repeatDelta = deltas.Count - i - 5;
                        break;
                    }
                }
            }

            prevCount = totalCount;
            rocknr = 0;

            Console.WriteLine($"{j,-3} {totalCount,-10:D}  {totalRocks}  {wi % input.Length}");
        }
        long repeateRows = deltas.Skip(deltas.Count - repeatDelta).Sum();
        long repeatRocks = repeatDelta * rocks.Length * input.Length;
        Console.WriteLine($"{repeateRows}    {repeatRocks}");

        long endAt = 1000000000000;
        long add = (endAt - totalRocks) / (repeatRocks);
        totalCount += add * repeateRows;
        cleaned += add * repeateRows;
        totalRocks += add * (repeatRocks);


        Console.WriteLine($"{totalRocks}");

        for (; totalRocks < endAt; totalRocks++)
            DoIt(totalRocks, ref wi);

        long answer2 = field.Where(l => l != empty).Count() + cleaned;
        Advent.AssertAnswer2(answer2, expected: 1553982300884, sampleExpected: 1514285714288);


        void DoIt(long ri, ref long wi)
        {
            var rock = rocks[ri % rocks.Length];
            while (field[index % field.Length] != empty)
                index = (index - 1) % field.Length + field.Length;
            
            int el = (int)(index % field.LongLength + field.LongLength);
            for (int i = 1; i < rock.Length + 5; i++)
            {
                el--;
                if (field[el % field.Length] != empty)
                {
                    if (cleaned != 0 || field[el % field.Length] != 0b111111111)
                        cleaned++;
                    field[el % field.Length] = empty;
                }
            }
            int rockDown = el + 2;

            int windOffset = 0;
            while (true)
            {
                int addOffset = input[(int)(wi++ % input.Length)] == '<' ? -1 : 1;
                bool canMoveByWind = true;
                for (int i = 0; i < rock.Length; i++)
                {
                    int movedRockLine = ((rock[i] << 8) >> (8 + windOffset + addOffset));
                    if ((movedRockLine & field[(rockDown + i) % field.Length]) != 0)
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
                    if ((movedRockLine & field[(rockDown + i + 1) % field.Length]) != 0)
                    {
                        canFall = false;
                        break;
                    }
                }
                if (!canFall)
                    break;
                rockDown++;
            }
            for (int i = 0; i < rock.Length; i++)
            {
                int movedRockLine = (rock[i] << 8) >> (8 + windOffset);
                field[(rockDown + i) % field.Length] |= movedRockLine;
            }
        }
    }

    static void Draw(int[] field, long index)
    {
        for (int i = 0; i < 17; i++)
            Console.WriteLine(Visualize(field[(i + index - 4) % field.Length]));
        Console.WriteLine();
     
        
        static string Visualize(int l, char to = '#')
        {
            return Convert.ToString(l, 2).PadLeft(7, '.').Replace('1', to).Replace('0', '.');
        }
    }
}