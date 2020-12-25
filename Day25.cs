using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day25
    {
        private readonly ITestOutputHelper output;
        public Day25(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay25()
        {
            var input = File.ReadAllLines("input/input25.txt");
            int cardKey = int.Parse(input[0]);
            int doorKey = int.Parse(input[1]);

            long s = 1;
            long result = 1;
            do
            {
                s = s * 7 % 20201227;
                result = result * doorKey % 20201227;
            }
            while (s != cardKey);

            output.WriteLine($"Part1: {result}");
        }
    }
}