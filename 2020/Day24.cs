using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day24
    {
        private readonly ITestOutputHelper output;
        private static (int dy, int dx)[] adjd = new[] { (-1, -1), (0, -2), (-1, 1), (1, 1), (0, 2), (1, -1) };

        public Day24(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay24()
        {
            var input = File.ReadAllLines("input/input24.txt");

            (int y, int x)[] flippedCoords = input
                .Select(line => ParseLine(line))
                .Select(movements => movements.Aggregate((0, 0), (n, o) => (n.Item1 + o.y, n.Item2 + o.x)))
                .GroupBy(coord => coord)
                .Where(g => g.Count() % 2 == 1)
                .Select(g => g.Key)
                .ToArray();
            output.WriteLine($"Part1: {flippedCoords.Count()}");

            bool[,] array = CoordsToArray(flippedCoords.ToArray());
            for (int a = 0; a < 100; a++)
                array = DoIteration(array);

            int result = array.Cast<bool>().Count(b => b);
            output.WriteLine($"Part2: {result}");

            static IEnumerable<(int y, int x)> ParseLine(string line)
            {
                int index = 0;
                while (index < line.Length)
                {
                    char c1 = line[index++];
                    char c2 = (c1 == 's' || c1 == 'n') ? line[index++] : '\0';
                    yield return (c1, c2) switch
                    {
                        ('e', _) => (0, 2),
                        ('s', 'e') => (1, 1),
                        ('s', 'w') => (1, -1),
                        ('w', _) => (0, -2),
                        ('n', 'w') => (-1, -1),
                        ('n', 'e') => (-1, 1),
                        _ => throw new ArgumentOutOfRangeException("instruction")
                    };
                }
            }

            static bool[,] DoIteration(bool[,] array)
            {
                var newArray = new bool[array.GetLength(0), array.GetLength(1)];

                for (int y = 2; y < array.GetLength(0) - 2; y++)
                    for (int x = 2; x < array.GetLength(1) - 2; x++)
                    {
                        int adjCount = adjd.Count(d => array[y + d.dy, x + d.dx]);

                        newArray[y, x] = (array[y, x], adjCount) switch
                        {
                            (true, int c) when c == 0 || c > 2 => false,
                            (false, 2) => true,
                            (bool cur, _) => cur
                        };
                    }
                return newArray;
            }

            static bool[,] CoordsToArray((int y, int x)[] coords)
            {
                int minX = coords.Select(c => c.x).Min();
                int minY = coords.Select(c => c.y).Min();
                int maxX = coords.Select(c => c.x).Max();
                int maxY = coords.Select(c => c.y).Max();
                const int extra = 100;
                var array = new bool[maxY - minY + extra * 2, maxX - minX + extra * 2];
                foreach (var (y, x) in coords)
                    array[y - minY + extra, x - minX + extra] = true;
                return array;
            }
        }
    }
}