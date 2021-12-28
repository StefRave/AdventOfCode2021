using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace AdventOfCode2019
{
    public class Day03
    {
        private static string[] GetInput() => File.ReadAllLines(@"Input/input03.txt");


        [Theory]
        [InlineData("R8,U5,L5,D3", "U7,R6,D4,L4", 6)]
        public void TestPart1(string path1, string path2, int expectedClosestManhatanDistance)
        {
            var dict = new Dictionary<(int, int), (char, int)>();
            DoPath(path1, dict, '1');

            Assert.Equal(expectedClosestManhatanDistance, DoPath(path2, dict, '2').lowestManhatan);
        }

        [Theory]
        [InlineData("R8,U5,L5,D3", "U7,R6,D4,L4", 30)]
        public void TestPart2(string path1, string path2, int expectedClosestManhatanDistance)
        {
            var dict = new Dictionary<(int, int), (char, int)>();
            DoPath(path1, dict, '1');

            Assert.Equal(expectedClosestManhatanDistance, DoPath(path2, dict, '2').lowestSteps);
        }


        private static (int lowestManhatan, int lowestSteps) DoPath(string path, Dictionary<(int, int), (char, int)> dict, char c)
        {
            int x = 0, y = 0;
            dict.TryAdd((y, x), (c, 0));
            int lowestManhatan = int.MaxValue;
            int lowestSteps = int.MaxValue;
            int step = 0;
            foreach (string instr in path.Split(','))
            {
                (int dy, int dx) = instr[0] switch
                {
                    'U' => (-1, 0),
                    'D' => (1, 0),
                    'L' => (0, -1),
                    'R' => (0, 1),
                    char ch => throw new ArgumentOutOfRangeException(nameof(path), ch.ToString())
                };
                for(int i = 0; i < int.Parse(instr[1..]); i++)
                {
                    step++;
                    (y, x) = (y + dy, x + dx);
                    if(dict.TryGetValue((y,x), out var prev))
                    {
                        if (prev.Item1 != c)
                        {
                            lowestManhatan = Math.Min(lowestManhatan, Math.Abs(x) + Math.Abs(y));
                            int totalSteps = prev.Item2 + step;
                            lowestSteps = Math.Min(totalSteps, lowestSteps);
                        }
                    }
                    dict[(y, x)] = (c, step);
                }
            }
            return (lowestManhatan, lowestSteps);
        }



        [Fact]
        public void DoPart1()
        {
            string[] input = GetInput();

            var dict = new Dictionary<(int, int), (char, int)>();
            DoPath(input[0], dict, '1');
            Assert.Equal(870, DoPath(input[1], dict, '2').lowestManhatan);
        }

        [Fact]
        public void DoPart2()
        {
            string[] input = GetInput();

            var dict = new Dictionary<(int, int), (char, int)>();
            DoPath(input[0], dict, '1');
            Assert.Equal(13698, DoPath(input[1], dict, '2').lowestSteps);
        }
    }
}
