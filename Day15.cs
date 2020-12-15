using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day15
    {
        private readonly ITestOutputHelper output;

        public Day15(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay15()
        {
            var input = "2,15,0,9,1,20".Split(",").Select(int.Parse);

            var lastSpoken = new Dictionary<int, int>();
            int turn = 1;
            int previousFoundTurn = 0;
            foreach (int n in input)
                previousFoundTurn = AddNumber(n);
            
            int result = Play(2020);
            output.WriteLine($"Part1: {result}");

            result = Play(30000000);
            output.WriteLine($"Part2: {result}");

            int AddNumber(int number)
            {
                if (!lastSpoken.TryGetValue(number, out int prevTurn))
                    prevTurn = 0;
                lastSpoken[number] = turn++;
                return prevTurn;
            }

            int Play(int untilRound)
            {
                int number = 0;
                while (turn <= untilRound)
                {
                    number = (previousFoundTurn == 0) ? 0 : turn - previousFoundTurn - 1;
                    previousFoundTurn = AddNumber(number);
                }
                return number;
            }
        }
    }
}