using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day11
    {
        private readonly ITestOutputHelper output;

        public Day11(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay11()
        {
            var input =
                File.ReadAllLines("input/input11.txt");

            int occupiedSeatsCount = CalculateSeats(maxOccupied: 4, onlyAdjacent: true);
            output.WriteLine($"Part1: {occupiedSeatsCount}");

            
            occupiedSeatsCount = CalculateSeats(maxOccupied: 5, onlyAdjacent: false);
            output.WriteLine($"Part2: {occupiedSeatsCount}");

            int CalculateSeats(int maxOccupied, bool onlyAdjacent)
            {
                var offsets = new (int dx, int dy)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };

                int seatInRowCount = input[0].Length;
                int rowCount = input.Length;

                IList<string> prevList = input;
                List<string> newList;
                bool isStable = false;
                int occupiedSeatsCount = 0;
                while (!isStable)
                {
                    occupiedSeatsCount = 0;
                    isStable = true;
                    newList = new List<string>();
                    for (int y = 0; y < prevList.Count; y++)
                    {
                        var newRow = new char[seatInRowCount];
                        for (int x = 0; x < prevList[0].Length; x++)
                        {
                            char currentSeat = prevList[y][x];
                            if (currentSeat != '.')
                            {
                                int count = 0;
                                foreach (var (dx, dy) in offsets)
                                {
                                    int nx = x, ny = y;
                                    do
                                    {
                                        nx += dx;
                                        ny += dy;
                                        if (nx < 0 || ny < 0 || nx >= seatInRowCount || ny >= rowCount)
                                            break;
                                        if (prevList[ny][nx] == '#')
                                        {
                                            count++;
                                            break;
                                        }
                                        if (prevList[ny][nx] == 'L')
                                            break;
                                    }
                                    while (!onlyAdjacent);
                                }
                                if (currentSeat == 'L')
                                {
                                    if (count == 0)
                                    {
                                        currentSeat = '#';
                                        isStable = false;
                                        occupiedSeatsCount++;
                                    }
                                }
                                else
                                {
                                    if (count >= maxOccupied)
                                    {
                                        currentSeat = 'L';
                                        isStable = false;
                                    }
                                    else
                                        occupiedSeatsCount++;
                                }
                            }
                            newRow[x] = currentSeat;
                        }
                        newList.Add(new string(newRow));
                    }
                    prevList = newList;
                }

                return occupiedSeatsCount;
            }
        }
    }
}