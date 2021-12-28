using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public class Day15
    {
        private static List<long> GetInput() => File.ReadAllText(@"Input/input15.txt").Split(",").Select(long.Parse).ToList();
        private Random rnd = new Random();
        [Fact]
        public void DoParts()
        {
            var memory = GetInput();
            var intCode = new IntCode(memory, new long[0]);

            var output = intCode.Output;
            int outputIndex = 0;
            var screen = new FlexArray2D<char>() { Default2D = ' ' };
            var pathLength = new FlexArray2D<int?>();
            int x = 0; int y = 0;
            int move = 0;
            screen[0][0] = 's';
            int steps = 0;
            pathLength[0][0] = steps;
            var moveOptions = new List<int>(4);
            var (oy, ox) = (0, 0);
            int oxygenSteps = 0;
            intCode.InputProvider = () =>
            {
                if (output.Count > outputIndex)
                {
                    var (ny, nx) = move switch
                    {
                        1 => (y - 1, x),
                        2 => (y + 1, x),
                        3 => (y, x - 1),
                        4 => (y, x + 1),
                        _ => throw new Exception("invalid")
                    };

                    int outVal = (int)output[outputIndex++];
                    if (ny != 0 || nx != 0)
                        screen[ny][nx] = outVal switch { 0 => '#', 1 => '.', 2 => 'O', _ => 'E' };
                    string display = screen.AsString();

                    if (outVal != 0)
                    {
                        steps++;
                        (x, y) = (nx, ny);
                        pathLength[y][x] = steps;
                    }
                    if (outVal == 2)
                    {
                        (oy, ox) = (y, x);
                        oxygenSteps = steps;
                    }
                }
                moveOptions.Clear();
                if (CanMove(x, y - 1)) moveOptions.Add(1);
                if (CanMove(x, y + 1)) moveOptions.Add(2);
                if (CanMove(x - 1, y)) moveOptions.Add(3);
                if (CanMove(x + 1, y)) moveOptions.Add(4);
                if (moveOptions.Count == 0)
                {
                    screen[y][x] = '*';

                    steps--;
                    if (pathLength[y - 1][x] == steps) moveOptions.Add(1);
                    else if (pathLength[y + 1][x] == steps) moveOptions.Add(2);
                    else if (pathLength[y][x - 1] == steps) moveOptions.Add(3);
                    else if (pathLength[y][x + 1] == steps) moveOptions.Add(4);
                    else
                    {
                        intCode.Halt();
                        return;
                    }
                    steps--;
                }

                move = moveOptions[rnd.Next(0, moveOptions.Count)]; // north (1), south (2), west (3), and east (4)
                intCode.Input.Enqueue(move);
            };
            intCode.Run();

            bool CanMove(int x, int y) => (screen[y][x] == ' ' || screen[y][x] == '.') && (!pathLength[y][x].HasValue || pathLength[y][x] > steps);


            Assert.Equal(212, oxygenSteps);

            pathLength = new FlexArray2D<int?>();
            screen[oy][ox] = 'O';
            pathLength[oy][ox] = 0;
            steps = 0;
            bool isActive = true;
            while (isActive)
            {
                isActive = false;

                steps++;
                var (minX, maxX) = (screen[0].Min, screen[0].Max);
                var (minY, maxY) = (screen.Min, screen.Max);
                for (x = minX; x < maxX; x++)
                    for (y = minY; y < maxY; y++)
                    {
                        if (screen[y][x] != '*')
                            continue;
                        if (pathLength[y - 1][x] == steps - 1 ||
                            pathLength[y + 1][x] == steps - 1 ||
                            pathLength[y][x + 1] == steps - 1 ||
                            pathLength[y][x - 1] == steps - 1)
                        {
                            isActive = true;
                            screen[y][x] = 'O';
                            pathLength[y][x] = steps;
                        }
                    }

            }
            Assert.Equal(358, steps - 1);
        }
    }
}
