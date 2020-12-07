using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day1
    {
        private readonly ITestOutputHelper output;

        public Day1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay1()
        {
            var input1 = File.ReadAllLines(@"input\input1.txt")
                .Select(c => int.Parse(c))
                .ToArray();

            Random rnd = new Random();

            while (true)
            {
                int i1 = rnd.Next(0, input1.Length);
                int i2 = rnd.Next(0, input1.Length);

                if (input1[i1] + input1[i2] == 2020)
                {
                    output.WriteLine("{0}", input1[i1] * input1[i2]);
                    break;
                }
            }

            while (true)
            {
                int i1 = rnd.Next(0, input1.Length);
                int i2 = rnd.Next(0, input1.Length);
                int i3 = rnd.Next(0, input1.Length);

                if (input1[i1] + input1[i2] + input1[i3] == 2020)
                {
                    output.WriteLine("{0}", input1[i1] * input1[i2] * input1[i3]);
                    break;
                }
            }
        }
    }
}
