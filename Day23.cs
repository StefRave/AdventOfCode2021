using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day23
    {
        private readonly ITestOutputHelper output;

        public Day23(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay23()
        {
#if true
            string input = "389125467";
#else
            string input = "538914762";
#endif
            int len = input.Length;
            char max = input.Max();
            for (int i = 0; i < 100; i++)
            {
                int index = i % len;
                char inputChar = input[index];

                string pickUp = (input + input).Substring(index + 1, 3);
                string left = (input + input).Substring(index + 4, len - 4);
                int insertIndex;
                char destination = inputChar;
                do
                {
                    destination--;
                    if (destination == '0')
                        destination = max;
                    insertIndex = left.IndexOf(destination) + 1;
                } while (insertIndex <= 0);
                string leftInsert = left.Substring(0, insertIndex) + pickUp + left.Substring(insertIndex);

                input = leftInsert.Substring(len - 1 - index, index) + inputChar + leftInsert.Substring(0, len - index - 1);
            }
            int indexOf1 = input.IndexOf('1');
            input = input.Substring(indexOf1 + 1) + input.Substring(0, indexOf1);
            output.WriteLine($"Part1: {input}");
        }
    }
}