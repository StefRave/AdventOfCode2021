using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day17
    {
        private readonly ITestOutputHelper output;

        public Day17(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay17()
        {
            var input = File.ReadAllLines("input/input17.txt");
            var grid = new char[1, 1, input.Length, input[0].Length];

            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[0].Length; x++)
                    grid[0, 0, y, x] = input[y][x];

            int result = DoIterations(grid, iterations: 6, fourDimensions: false);
            output.WriteLine($"Part1: {result}");

            result = DoIterations(grid, iterations: 6, fourDimensions: true);
            output.WriteLine($"Part2: {result}");
        }

        private int DoIterations(char[,,,] grid, int iterations, bool fourDimensions)
        {
            for (int i = 0; i < iterations; i++)
            {
                grid = DoIteration(grid, fourDimensions);
                //PrintGrid(grid, i);
            }

            int result = grid.Cast<char>().Count(c => c == '#');
            return result;
        }

        private char[,,,] DoIteration(char[,,,] grid, bool fourDimensions)
        {
            int wl = grid.GetLength(0);
            int zl = grid.GetLength(1);
            int yl = grid.GetLength(2);
            int xl = grid.GetLength(3);
            var newGrid = new char[fourDimensions ? wl + 2 : 1, zl + 2, yl + 2, xl + 2];

            for (int w = 0; w < newGrid.GetLength(0); w++)
                for (int z = 0; z < zl + 2; z++)
                    for (int y = 0; y < yl + 2; y++)
                        for (int x = 0; x < xl + 2; x++)
                        {
                            bool isActive = IsActive(fourDimensions ? w - 1 : w, z - 1, y - 1, x - 1);

                            int neighborsActive = isActive ? -1 : 0;

                            for (int wz = fourDimensions ? - 1 : 1; wz <= 1; wz++)
                                for (int dz = -1; dz <= 1; dz++)
                                    for (int dy = -1; dy <= 1; dy++)
                                        for (int dx = -1; dx <= 1; dx++)
                                            if (IsActive(w + wz - 1, z + dz - 1, y + dy - 1, x + dx - 1))
                                                neighborsActive++;

                            if (isActive)
                                newGrid[w, z, y, x] = neighborsActive == 2 || neighborsActive == 3 ? '#' : '.';
                            else
                                newGrid[w, z, y, x] = neighborsActive == 3 ? '#' : '.';
                        }

            return newGrid;

            bool IsActive(int w, int z, int y, int x)
            {
                if (x < 0 || y < 0 || z < 0 || w < 0 || w >= wl || z >= zl || y >= yl || x >= xl)
                    return false;
                return grid[w, z, y, x] == '#';
            }
        }

        public void PrintGrid(char[,,,] grid, int round)
        {
            output.WriteLine($"Round {round}");
            for (int w = 0; w < grid.GetLength(0); w++)
            {
                for (int z = 0; z < grid.GetLength(1); z++)
                {
                    var sb = new StringBuilder();
                    for (int y = 0; y < grid.GetLength(2); y++)
                    {
                        for (int x = 0; x < grid.GetLength(3); x++)
                            sb.Append(grid[w, z, y, x].ToString());
                        sb.AppendLine();
                    }
                    output.WriteLine(sb.ToString());
                }
            }
        }
    }
}