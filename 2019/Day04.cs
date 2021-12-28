using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public class Day04
    {
        private static int[] GetInput() => new[] { 240298, 784956 };


        [Fact]
        public void DoPart1()
        {
            var input = GetInput();
            int i;
            int count = 0;
            for(i = input[0]; i < input[1]; i++)
            {
                string test = i.ToString();
                bool hasDouble = false;
                bool increases = true;
                char prev = '\0';
                foreach(char c in test)
                {
                    if (prev == c)
                        hasDouble = true;
                    if (c < prev)
                    {
                        increases = false;
                        break;
                    }
                    prev = c;
                }
                if (increases && hasDouble)
                    count++;
            }

            Assert.Equal(1150, count);
        }

        [Fact]
        public void DoPart2()
        {
            var input = GetInput();
            int i;
            int count = 0;
            for(i = input[0]; i < input[1]; i++)
            {
                string test = i.ToString();
                bool hasDouble = false;
                bool increases = true;
                char prev = '\0';
                foreach(char c in test)
                {
                    if (prev == c && test.Count(cc => cc == c) == 2)
                        hasDouble = true;
                    if (c < prev)
                    {
                        increases = false;
                        break;
                    }
                    prev = c;
                }
                if (increases && hasDouble)
                    count++;
            }

            Assert.Equal(748, count);
        }
    }
}
