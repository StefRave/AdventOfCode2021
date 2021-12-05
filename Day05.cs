using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2021
{
    public class Day05
    {
        private readonly ITestOutputHelper output;

        public Day05(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Run()
        {
            var input = Advent.ReadInputLines()
                .Select(c => Regex.Matches(c, @"\d+").Select(m => int.Parse(m.Value)).ToArray())
                .Select(m => new Vector(m[0], m[1], m[2], m[3]))
                .ToArray();

            char[][] array = Draw(input, includeDiagonal: false);
            int count = array.SelectMany(line => line).Count(c => c > '1');
            Advent.AssertAnswer1(count);

            array = Draw(input, includeDiagonal: true);
            count = array.SelectMany(line => line).Count(c => c > '1');
            Advent.AssertAnswer2(count);
        }

        private static char[][] Draw(Vector[] input, bool includeDiagonal)
        {
            int maxX = input.Max(v => Math.Max(v.X1, v.X2)) + 1;
            int maxY = input.Max(v => Math.Max(v.Y1, v.Y2)) + 1;

            var array = Enumerable.Range(0, maxY).Select(i => "".PadRight(maxX, '.').ToArray()).ToArray();
            foreach (var vector in input)
            {
                if (vector.Y1 == vector.Y2)
                {
                    int add = vector.X1 < vector.X2 ? 1 : -1;
                    for (int i = 0; i <= Math.Abs(vector.X1 - vector.X2); i++)
                        SetPixel(vector.Y1, vector.X1 + i * add);
                }
                else if (vector.X1 == vector.X2)
                {
                    int add = vector.Y1 < vector.Y2 ? 1 : -1;
                    for (int i = 0; i <= Math.Abs(vector.Y1 - vector.Y2); i++)
                        SetPixel(vector.Y1 + i * add, vector.X1);
                }
                else if(includeDiagonal)
                {
                    int addX = vector.X1 < vector.X2 ? 1 : -1;
                    int addY = vector.Y1 < vector.Y2 ? 1 : -1;
                    for (int i = 0; i <= Math.Abs(vector.Y1 - vector.Y2); i++)
                        SetPixel(vector.Y1 + i * addY, vector.X1 + i * addX);
                }
            }

            return array;

            void SetPixel(int y, int x)
            {
                if (array[y][x] == '.')
                    array[y][x] = '1';
                else
                    array[y][x]++;
            }
        }
    }
    public record Vector(int X1, int Y1, int X2, int Y2);
}
