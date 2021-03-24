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


        const int White = 1;
        const int Black = 0;


        [Fact]
        public void DoParts()
        {
            var memory = GetInput();

            var dict = Paint(memory, Black);

            Assert.Equal(2093, dict.Count);
            memory = GetInput();
            dict = Paint(memory, White);
            var minMax = dict.Keys.Aggregate(new MinMax(0, 0, 0, 0), (MinMax o, (int x, int y) c) => 
                new MinMax(Math.Min(o.minX, c.x), Math.Min(o.minY, c.y), Math.Max(o.maxX, c.x), Math.Max(o.maxY, c.y)));

            int height = minMax.maxY - minMax.minY + 1;
            int width = minMax.maxX - minMax.minX + 1;
            char[][] panels = Enumerable.Range(0, height).Select(i => ".".PadRight(width).ToCharArray()).ToArray();
            foreach (var kv in dict)
                panels[kv.Key.Item2 - minMax.minY][kv.Key.Item1 - minMax.minX] = kv.Value  == White ? '#' : '.';
            string[] result = panels.Select(ca => new string(ca)).ToArray();
        }
        record MinMax(int minX, int minY, int maxX, int maxY);


        private static Dictionary<(int, int), int> Paint(List<long> memory, int startColor)
        {
            var intCode = new IntCode(memory, new long[0]);

            var dict = new Dictionary<(int, int), int>();

            int x = 0, y = 0;

            int outputIndex = 0;
            int angle = 0;
            int current = startColor;
            dict[(x, y)] = current;

            intCode.InputProvider = () =>
            {
                if (intCode.Output.Count > 0)
                {
                    long color = intCode.Output[outputIndex++];
                    long second = intCode.Output[outputIndex++];

                    current = (int)color;
                    dict[(x, y)] = current;

                    angle = (angle + (second == 0 ? 3 : 1)) % 4;
                    switch (angle)
                    {
                        case 0: y--; break;
                        case 1: x++; break;
                        case 2: y++; break;
                        case 3: x--; break;
                    }
                }

                if (!dict.TryGetValue((x, y), out current))
                    current = Black;

                intCode.Input.Enqueue(current);
            };
            intCode.Run();
            return dict;
        }
    }
}
