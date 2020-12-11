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
            IList<string> input = File.ReadAllLines("input/input11.txt");
            int seatInRowCount = input[0].Length;
            int rowCount = input.Count;

            int occupiedSeatsCount = CalculateSeats(input, maxOccupied: 4, onlyAdjacent: true);
            output.WriteLine($"Part1: {occupiedSeatsCount}");

            occupiedSeatsCount = CalculateSeats(input, maxOccupied: 5, onlyAdjacent: false);
            output.WriteLine($"Part2: {occupiedSeatsCount}");

            int CalculateSeats(IList<string> seats, int maxOccupied, bool onlyAdjacent)
            {

                bool isStable = false;
                int occupiedSeatsCount = 0;
                while (!isStable)
                    (seats, isStable, occupiedSeatsCount) = DoSeatIteration(seats, maxOccupied, onlyAdjacent);

                return occupiedSeatsCount;
            }

            (List<string> newList, bool isStable, int occupiedSeatsCount) DoSeatIteration(IList<string> seats, int maxOccupied, bool onlyAdjacent)
            {
                List<string> newSeats = new List<string>();
                int occupiedSeatsCount = 0;
                bool isStable = true;

                for (int y = 0; y < seats.Count; y++)
                {
                    var newRow = new char[seatInRowCount];
                    for (int x = 0; x < seats[0].Length; x++)
                    {
                        char currentSeat = seats[y][x];
                        if (currentSeat != '.')
                        {
                            int count = CountVisibleSeats(seats, onlyAdjacent, y, x);
                            if (currentSeat == 'L' && count == 0)
                                currentSeat = '#';
                            else if(currentSeat == '#' && count >= maxOccupied)
                                currentSeat = 'L';
                        }
                        if (currentSeat == '#')
                            occupiedSeatsCount++;
                        if(seats[y][x] != currentSeat)
                            isStable = false;

                        newRow[x] = currentSeat;
                    }
                    newSeats.Add(new string(newRow));
                }

                return (newSeats, isStable, occupiedSeatsCount);
            }

            int CountVisibleSeats(IList<string> seats, bool onlyAdjacent, int y, int x)
            {
                var offsets = new (int dx, int dy)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };

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
                        if (seats[ny][nx] == '#')
                        {
                            count++;
                            break;
                        }
                        if (seats[ny][nx] == 'L')
                            break;
                    }
                    while (!onlyAdjacent);
                }

                return count;
            }
        }
    }
}