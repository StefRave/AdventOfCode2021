namespace AdventOfCode2021;

public class Day03 : IAdvent
{
    [Fact]
    public void Run()
    {
        var inputLines = Advent.ReadInputLines();
        var input = inputLines
            .Select(line => Convert.ToInt64(line, 2))
            .ToArray();
        int lineLength = inputLines[0].Length;

        long gamma = 0;
        for (int j = 0; j < lineLength; j++)
        {
            long bit = 1 << (lineLength - 1 - j);
            int ones = input.Count(line => (line & bit) != 0);
            int zeros = input.Length - ones;

            if (ones > zeros)
                gamma += bit;
        }
        long epsilon = ((1 << lineLength) - 1) ^ gamma;
        Advent.AssertAnswer1(gamma * epsilon);

        long oxygen = Remaining(input, match: true);
        long co2 = Remaining(input, match: false);
        Advent.AssertAnswer2(oxygen * co2);



        long Remaining(long[] input, bool match)
        {
            var list = input.AsEnumerable();
            for (int j = 0; j < lineLength; j++)
            {
                long bit = 1 << (lineLength - 1 - j);
                int ones = list.Count(line => (line & bit) != 0);
                int zeros = list.Count() - ones;

                bool oneOrZero = match ? ones >= zeros : zeros > ones;
                var remaining = list.Where(line => ((line & bit) != 0) == oneOrZero).ToArray();

                if (remaining.Count() <= 1)
                    return remaining.First();
                list = remaining;
            }
            throw new Exception("not found");
        }
    }
}
