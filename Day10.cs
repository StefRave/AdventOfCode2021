using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public partial class Day10
    {
        private static string[] GetInput() => File.ReadAllLines(@"Input/input10.txt");

        
        private static int[] primes = { 2, 3, 5, 7, 11, 13, 17 };
        
        static bool CanBeReduced(int n1, int n2)
        {
            if (n1 == 0 && n2 == 0)
                return true;
            if (n1 == 0 && n2 != 1)
                return true;
            if (n2 == 0 && n1 != 1)
                return true;

            foreach (var prime in primes)
            {
                if (n1 % prime == 0 && n2 % prime == 0)
                    return true;
            }
            return false;
        }

        [Theory]
        [InlineData(1,0, false)]
        [InlineData(1,1, false)]
        [InlineData(0,0, true)]
        [InlineData(2,0, true)]
        [InlineData(2,2, true)]
        private void ShouldBeSkipped(int dx, int dy, bool shouldSkip)
        {
            CanBeReduced(dx, dy).Should().Be(shouldSkip);
        }

        private IEnumerable<(int dx, int dy)> AllAngles90()
        {
            for (int x = 1; x < 34; x++)
                for (int y = 0; y < 34; y++)
                {
                    if (!CanBeReduced(x, y))
                        yield return (x, y);
                }
        }
        private (int dx, int dy) Rot90((int dx, int dy) v) => (v.dy, -v.dx);

        private IEnumerable<(int dx, int dy)> AllAngles()
        {
            foreach (var angle in AllAngles90())
            {
                var tmp = angle;
                for (int i = 0; i < 4; i++)
                {
                    yield return tmp;
                    tmp = Rot90(tmp);
                }
            }
        }

        private (int dx, int dy)[] GetAllCoordsSortedByAngle()
        {
            return AllAngles()
                .OrderBy(coord => GetAngle(coord.dx, coord.dy))
                .ToArray();

            static object GetAngle(int x, int y)
            {
                double a = Math.Atan2(x, y);
                return a >= 0 ? a : 2 * Math.PI + a;
            }
        }

        [Fact]
        public void DoTests1()
        {
            const string example = @".#..#
.....
#####
....#
...##";

            char[][] matrix = example.SplitByNewLine()
                .Select(s => s.ToCharArray())
                .ToArray();

            for(int x = 0; x < matrix[0].Length; x++)
                for(int y = 0; y < matrix.Length; y++)
                {
                    if (matrix[y][x] == '#')
                        matrix[y][x] = (char)('0' + CountInSight(matrix, x, y));
                }

            string[] result = matrix.Select(ca => new string(ca)).ToArray();
        }

        [Fact]
        public void DoTests2()
        {
            const string example = @".#....#####...#..
##...##.#####..##
##...#...#.#####.
..#.....X...###..
..#.#.....#....##";

            char[][] matrix = example.SplitByNewLine()
                .Select(s => s.ToCharArray())
                .ToArray();
            int x = 8, y = 3;
            Assert.Equal('X', matrix[y][x]);

            LocateAsteroids(matrix, x, y);

            var locator = LocateAsteroids(matrix, x, y).GetEnumerator();
            for (int j = 0; j < 4; j++)
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (!locator.MoveNext())
                        break;
                    var coord = locator.Current;
                    matrix[coord.y][coord.x] = (char)('0' + i);
                }
                string[] result = matrix.Select(ca => new string(ca)).ToArray();
                ClearNumbers();
            }

            void ClearNumbers()
            {
                for (int x = 0; x < matrix[0].Length; x++)
                    for (int y = 0; y < matrix.Length; y++)
                    {
                        if (char.IsDigit(matrix[y][x]))
                            matrix[y][x] = '.';
                    }
            }
        }

        private IEnumerable<(int x, int y)> LocateAsteroids(char[][] matrix, int x, int y)
        {
            var angles = GetAllCoordsSortedByAngle();
            while (true)
            {
                foreach (var angle in angles)
                {
                    int nx = x, ny = y;
                    while (true)
                    {
                        nx += angle.dx;
                        ny -= angle.dy;
                        if (nx < 0 || ny < 0 || nx >= matrix[0].Length || ny >= matrix.Length)
                            break;
                        if (matrix[ny][nx] == '#')
                        {
                            yield return (nx, ny);
                            break;
                        }
                    }
                }
            }
        }

        private int CountInSight(char[][] matrix, int x, int y)
        {
            int found = 0;

            foreach(var angle in AllAngles())
            {
                int nx = x, ny = y;
                while(true)
                {
                    nx += angle.dx;
                    ny += angle.dy;
                    if (nx < 0 || ny < 0 || nx >= matrix[0].Length || ny >= matrix.Length)
                        break;
                    if(matrix[ny][nx] != '.')
                    {
                        found++;
                        break;
                    }
                }
            }
            return found;
        }



        

        private ((int x, int y), int max) GetMaxCoord(char[][] matrix)
        {
            int max = 0;
            (int, int) coord = (0, 0);

            for (int x = 0; x < matrix[0].Length; x++)
                for (int y = 0; y < matrix.Length; y++)
                {
                    if (matrix[y][x] == '#')
                    {
                        int count = CountInSight(matrix, x, y);
                        if (count > max)
                        {
                            max = count;
                            coord = (x, y);
                        }
                    }
                }
            return (coord, max);
        }

        [Fact]
        public void DoTests3()
        {
            const string example = @".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##";

            char[][] matrix = example.SplitByNewLine()
                .Select(s => s.ToCharArray())
                .ToArray();
            var result = GetMaxCoord(matrix);

            Assert.Equal(210, result.max);

            var locator = LocateAsteroids(matrix, result.Item1.x, result.Item1.y).GetEnumerator();

            (int x, int y) coord = result.Item1;
            for (int i = 1; i <= 200; i++)
            {
                if (!locator.MoveNext())
                    break;
                coord = locator.Current;
                matrix[coord.y][coord.x] = '.';
            }

            Assert.Equal(802, coord.x * 100 + coord.y);
        }

        [Fact]
        public void DoParts()
        {
            char[][] matrix = GetInput()
               .Select(s => s.ToCharArray())
               .ToArray();
            var result = GetMaxCoord(matrix);

            Assert.Equal(256, result.max);

            var locator = LocateAsteroids(matrix, result.Item1.x, result.Item1.y).GetEnumerator();

            (int x, int y) coord = (0, 0);
            for (int i = 0; i < 200; i++)
            {
                if (!locator.MoveNext())
                    break;
                coord = locator.Current;
                matrix[coord.y][coord.x] = '.';
            }

            Assert.Equal(1707, (coord.x * 100) + coord.y);
        }
    }
}
