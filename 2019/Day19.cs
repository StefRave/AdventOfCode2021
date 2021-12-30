#nullable enable
namespace AdventOfCode2019;

public class Day19 : IAdvent
{
    private static List<long> GetInput() => File.ReadAllText(@"Input/input19.txt").Split(",").Select(long.Parse).ToList();

    void IAdvent.Run()
    {
        var memory = GetInput();
        int count = 0;
        (int y, int x) last = default;
        for (int y = 0; y < 50; y++)
        {
            for (int x = 0; x < 50; x++)
            {
                if (Try(y, x))
                {
                    count++;
                    last = (y, x);
                }
            }
        }
        Advent.AssertAnswer1(count, 162);

        for (int y = last.y + 1; ; y++)
        {
            for (int x = last.x + 1; Try(y, x); x++)
                last = (y, x);
            if (last.x >= 100 && Try(last.y, last.x - 99) && Try(last.y + 99, last.x - 99))
                break;
        }
        Advent.AssertAnswer1((last.x - 99) * 10000 + last.y, 13021056);


        bool Try(int y, int x)
        {
            var intCode = new IntCode(memory.ToArray(), new long[] { x, y });
            intCode.Run();
            return intCode.Output[0] != 0;
        }
    }
}