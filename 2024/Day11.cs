using System.Numerics;

namespace AdventOfCode2024;

public class Day11 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().Split(' ').Select(BigInteger.Parse).ToList();

        long answer1 = Count(25);
        Advent.AssertAnswer1(answer1, expected: 199946, sampleExpected: 55312);

        long answer2 = Count(75);
        Advent.AssertAnswer2(answer2, expected: 237994815702032, sampleExpected: 65601038650482);


        long Count(int toGo)
        {
            var dict = new Dictionary<(BigInteger, int), long>();
            return input.Sum(item => CountStones(item, toGo));


            long CountStones(BigInteger item, int toGo)
            {
                if (dict.TryGetValue((item, toGo), out var result))
                    return result;
                if (toGo == 0)
                    return 1;

                long count;
                if (item == 0)
                {
                    count = CountStones(1, toGo - 1);
                }
                else
                {
                    string s = item.ToString();
                    if (s.Length % 2 == 0)
                    {
                        count =
                            CountStones(BigInteger.Parse(s.Substring(0, s.Length / 2)), toGo - 1) +
                            CountStones(BigInteger.Parse(s.Substring(s.Length / 2)), toGo - 1);
                    }
                    else
                    {
                        count = CountStones(item * 2024, toGo - 1);
                    }
                }
                dict[(item, toGo)] = count;
                return count;
            }
        }
    }
}