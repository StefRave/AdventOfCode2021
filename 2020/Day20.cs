using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        private class Piece
        {
            public int Id { get; init; }
            public string[] Lines { get; private set; }
            public int?[] Neigbours { get; } = new int?[4];

            public Piece(int id, string[] lines) => (Id, Lines) = (id, lines);

            public void Mirror() => Lines = Day20.Mirror(Lines);
            public void Rotate() => Lines = Day20.Rotate(Lines);

            public IEnumerable<char> IterateSide(int sideIdex)
            {
                (int y, int yd, int x, int xd) = move[sideIdex];
                if (y == 1)
                    y = Lines.Length - 1;
                if (x == 1)
                    x = Lines.Length - 1;
                for (int i = 0; i < Lines.Length; i++, y += yd, x += xd)
                    yield return Lines[y][x];
            }

            static (int y0, int yd, int x0, int xd)[] move = { (0, 0, 0, 1), (0, 1, 1, 0), (1, 0, 1, -1), (1, -1, 0, 0), (1, 0, 0, 1), (1, -1, 1, 0), (0, 0, 1, -1), (0, 1, 0, 0) };
        }

        [Fact]
        public void DoDay20()
        {
            var input =
                File.ReadAllText("input/input20.txt").SplitByDoubleNewLine()
                .Select(ParsePiece).ToArray();
            
            FindNeighboursAndOrient(input);
            var cornerPieces = input.Where(p => p.Neigbours.Count(n => n.HasValue) == 2);
            long result = cornerPieces.Aggregate(1L, (a, p) => a * p.Id);
            output.WriteLine($"Part1: {result}");


            string[] puzzle = GetPuzzle(input);

            var (count, roughness) = FindSeaMonsters(puzzle);
            output.WriteLine($"Part2: {roughness}");
        }

        private static void FindNeighboursAndOrient(Piece[] input)
        {
            int piecesWithCorrectRotation = 1;
            for (int i1 = 0; i1 < input.Length - 1; i1++)
                for (int i2 = i1 + 1; i2 < input.Length; i2++)
                    for (int s1 = 0; s1 < 4; s1++)
                        for (int s2 = 0; s2 < 8; s2++)
                            if (input[i1].IterateSide(s1).SequenceEqual(input[i2].IterateSide(s2)))
                            {
                                input[i1].Neigbours[s1] = input[i2].Id;
                                input[i2].Neigbours[(s1 + 2) % 4] = input[i1].Id;

                                var n = input[i2];
                                if (s2 < 4)
                                    n.Mirror();
                                for (int i = 0; i < ((s1 + s2) % 4); i++)
                                    n.Rotate();
                                if (piecesWithCorrectRotation < i2)
                                {
                                    input[i2] = input[piecesWithCorrectRotation];
                                    input[piecesWithCorrectRotation++] = n;
                                }
                            }
        }

        private string[] GetPuzzle(Piece[] input)
        {
            Dictionary<int, Piece> pieceDict = input.ToDictionary(p => p.Id);

            Piece[][] puzzlePiece = new Piece[(int)Math.Sqrt(input.Length)][];
            Piece topLeftPiece = input.Single(p => !p.Neigbours[0].HasValue && p.Neigbours[1].HasValue && p.Neigbours[2].HasValue && !p.Neigbours[3].HasValue);
            for (int y = 0; y < puzzlePiece.Length; y++)
            {
                puzzlePiece[y] = new Piece[puzzlePiece.Length];
                puzzlePiece[y][0] = y == 0 ? topLeftPiece : pieceDict[puzzlePiece[y - 1][0].Neigbours[2].Value];

                for (int x = 1; x < puzzlePiece.Length; x++)
                    puzzlePiece[y][x] = pieceDict[puzzlePiece[y][x - 1].Neigbours[1].Value];
            }

            int pieceWidth = topLeftPiece.Lines.Length - 2;

            var p =
                puzzlePiece.Select(row =>
                Enumerable.Range(1, pieceWidth)
                    .Select(y => string.Join("", Enumerable.Range(0, puzzlePiece.Length).Select(i => row[i].Lines[y].Substring(1, pieceWidth)))))
                .SelectMany(row => row)
                .ToArray();
            return p;
        }

        private static (int count, int roughness) FindSeaMonsters(string[] puzzle)
        {
            string[] seaMonster =
                {
                    "                  # ",
                    "#    ##    ##    ###",
                    " #  #  #  #  #  #    "
                };
            var matches =
                (
                from y in Enumerable.Range(0, seaMonster.Length)
                from x in Enumerable.Range(0, seaMonster[0].Length)
                where seaMonster[y][x] == '#'
                select (y, x)
                ).ToArray();


            int count, roughness = 0;

            var p = puzzle;
            for (int mirror = 0; mirror <= 1; mirror++)
            {
                if (mirror == 1) p = Mirror(puzzle);

                for (int rotate = 0; rotate < 4; rotate++)
                {
                    if (rotate > 0)
                        p = Rotate(p);

                    (count, roughness) = CountSeaMonsters(p);
                    if (count > 0)
                        return (count, roughness);
                }
            }
            return (0, 0);

            (int count, int roughness) CountSeaMonsters(string[] p)
            {
                char[][] puzzleArray = p.Select(p => p.ToCharArray()).ToArray();

                int count = 0;
                for (int y = 0; y <= p.Length - seaMonster.Length; y++)
                    for (int x = 0; x <= p.Length - seaMonster[0].Length; x++)
                        if (matches.All(m => p[y + m.y][x + m.x] != '.'))
                        {
                            count++;
                            foreach (var m in matches)
                                puzzleArray[y + m.y][x + m.x] = 'O';
                        }
                return (count, puzzleArray.SelectMany(c => c).Count(c => c == '#'));
            }
        }

        private static Piece ParsePiece(string pp)
        {
            var lines = pp.SplitByNewLine();
            int piece = int.Parse(lines[0].Split(' ')[1].TrimEnd(':'));
            lines = lines.Skip(1).ToArray();
            return new Piece(piece, lines);
        }

        public static string[] Mirror(string[] lines) => lines.Reverse().ToArray();
        public static string[] Rotate(string[] lines)
        {
            int length = lines.Length;
            var rotated = Enumerable.Range(0, length).Select(i => new char[length]).ToArray();
            for (int y = 0; y < length; y++)
                for (int x = 0; x < length; x++)
                    rotated[y][x] = lines[length - x - 1][y];
            return rotated.Select(ca => new string(ca.ToArray())).ToArray();
        }
    }
}