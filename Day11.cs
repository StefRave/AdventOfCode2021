using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public class Day11
    {
        private static List<long> GetInput() => File.ReadAllText(@"Input/input11.txt").Split(",").Select(long.Parse).ToList();


        const char NoPaint = ' ';
        const char White = '#';
        const char Black = '.';


        [Fact]
        public void DoParts()
        {
            var memory = GetInput();

            var panel = Paint(memory, Black);
            Assert.Equal(2093, panel.GroupSelect().Count(c => c != NoPaint));
            
            memory = GetInput();
            panel = Paint(memory, White);
            string display = panel.AsString();
        }
        record MinMax(int minX, int minY, int maxX, int maxY);


        private static FlexArray2D<char> Paint(List<long> memory, int startColor)
        {
            var intCode = new IntCode(memory, new long[0]);

            var array2D = new FlexArray2D<char>() { Default2D = NoPaint };

            int x = 0, y = 0;

            int outputIndex = 0;
            int angle = 0;
            int current = startColor;
            array2D[y][x] = (char)current;

            intCode.InputProvider = () =>
            {
                if (intCode.Output.Count > 0)
                {
                    long color = intCode.Output[outputIndex++] == 0 ? Black : White;
                    long second = intCode.Output[outputIndex++];

                    current = (int)color;
                    array2D[y][x] = (char)current;

                    angle = (angle + (second == 0 ? 3 : 1)) % 4;
                    switch (angle)
                    {
                        case 0: y--; break;
                        case 1: x++; break;
                        case 2: y++; break;
                        case 3: x--; break;
                    }
                }

                current = array2D[y][x];
                intCode.Input.Enqueue(current == White ? 1 : 0);
            };
            intCode.Run();
            return array2D;
        }
    }
}
