using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day20
    {
        private readonly ITestOutputHelper output;

        public Day20(ITestOutputHelper output)
        {
            this.output = output;
        }

        private record Piece(int id, string[] lines)
        {
            public IEnumerable<char> IterateSide(int sideIdex)
            {
                (int y, int yd, int x, int xd) = move[sideIdex];
                if (y == 1)
                    y = lines.Length - 1;
                if (x == 1)
                    x = lines.Length - 1;
                for (int i = 0; i < lines.Length; i++, y += yd, x += xd)
                    yield return lines[y][x];
            }

            static (int y0, int yd, int x0, int xd)[] move = { (0, 0, 0, 1), (0, 1, 1, 0), (1, 0, 1, -1), (1, -1, 0, 0), (1, 0, 0, 1), (1, -1, 1, 0), (0, 0, 1, -1), (0, 1, 0, 0) };
        }

        [Fact]
        public void DoDay20()
        {
            var input =
                File.ReadAllText("input/input20.txt").SplitByDoubleNewLine()
                .Select(ParsePiece).ToArray();

            var solutions = new int[input.Length];
            for (int i1 = 0; i1 < input.Length - 1; i1++)
                for (int i2 = i1+1; i2 < input.Length; i2++)
                    for (int s1 = 0; s1 < 4; s1++)
                        for (int s2 = 0; s2 < 8; s2++)
                            if (input[i1].IterateSide(s1).SequenceEqual(input[i2].IterateSide(s2)))
                            {
                                solutions[i1]++;
                                solutions[i2]++;
                            }


            long result = Enumerable.Range(0, solutions.Length)
                .Where(i => solutions[i] == 2)
                .Aggregate(1L, (a, b) => a * input[b].id);
            output.WriteLine($"Part1: {result}");
        }

        private static Piece ParsePiece(string pp)
        {
            var lines = pp.SplitByNewLine();
            int piece = int.Parse(lines[0].Split(' ')[1].TrimEnd(':'));
            lines = lines.Skip(1).ToArray();
            return new Piece(piece, lines);
        }
    }
}