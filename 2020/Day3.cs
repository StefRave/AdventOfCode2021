using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day3
    {
        private readonly ITestOutputHelper output;

        public Day3(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay3()
        {
            var input = File.ReadAllLines("input/input3.txt");
            int width = input[0].Length;

            long result = CountTrees(3, 1);
            output.WriteLine($"Part1: {result}");

            result =
                CountTrees(1, 1) *
                CountTrees(3, 1) *
                CountTrees(5, 1) *
                CountTrees(7, 1) *
                CountTrees(1, 2);
            output.WriteLine($"Part2: {result}");


            long CountTrees(int dx, int dy)
            {
                int trees = 0;
                int x = 0;
                for (int y = 0; y < input.Length; y += dy)
                {
                    if (input[y][x % width] == '#')
                        trees++;
                    x += dx;
                }
                return trees;
            }

        }
    }
}