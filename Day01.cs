using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2021
{
    public class Day01
    {
        private readonly ITestOutputHelper output;

        public Day01(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Run()
        {
            var input = Advent.ReadInputLines()
                .Select(c => int.Parse(c))
                .ToArray();

            int prev = input[0];
            int countHigher = 0;
            for (int i = 1; i < input.Length; i++)
            {
                if (prev < input[i])
                    countHigher++;
                prev = input[i];
            }
            Advent.AssertAnswer1(countHigher);


            long prev3 = input[0] + input[1] + input[2];
            countHigher = 0;
            for (int i = 3; i < input.Length; i++)
            {
                long current3 = prev3 - input[i - 3] + input[i];
                if (prev3 < current3)
                    countHigher++;
                prev3 = current3;
            }
            Advent.AssertAnswer2(countHigher);
        }
    }
}
