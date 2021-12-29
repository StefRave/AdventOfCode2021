#nullable enable
using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2019;

public class Day17 : IAdvent
{
    private static List<long> GetInput() => File.ReadAllText(@"Input/input17.txt").Split(",").Select(long.Parse).ToList();
    private static (int dy, int dx)[] offsets = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
    private static char[] TurtleCharacter = new[] { '>', 'v', '<', '^' };
    string part1SampleInput = @"..#..........
..#..........
#######...###
#.#...#...#.#
#############
..#...#...#..
..#####...^..";
    string part2SampleInput = @"#######...#####
#.....#...#...#
#.....#...#...#
......#...#...#
......#...###.#
......#.....#.#
^########...#.#
......#.#...#.#
......#########
........#...#..
....#########..
....#...#......
....#...#......
....#...#......
....#####......";

    [Fact]
    public void Part1Sample()
    {
        var (maze, totalAllignment, turtle) = ToMazeWithIntersections(part1SampleInput);
        Console.WriteLine(string.Join("\n", maze.Select(ca => new string(ca))));
        PrintMaze(maze);
        Assert.Equal(76, totalAllignment);

    }

    private (char[][] maze, int totalAllignment, Turtle turtle) ToMazeWithIntersections(string input)
    {
        var maze = input.SplitByNewLine()
            .Select(c => c.ToCharArray())
            .ToArray();
        int maxX = maze[0].Length;
        int maxY = maze.Length;
        Turtle? turtle = null;

        int totalAllignment = 0;
        for (int y = 0; y < maxY; y++)
            for (int x = 0; x < maxX; x++)
                if (maze[y][x] == '#')
                {
                    if (x > 0 && y > 0 && x < maxX - 1 && y < maxY - 1)
                        if (offsets.All(d => maze[y + d.dy][x + d.dx] == '#'))
                        {
                            maze[y][x] = 'O';
                            totalAllignment += x * y;
                        }
                }
                else if (maze[y][x] != '.')
                {
                    turtle = new Turtle(y, x, Array.IndexOf(TurtleCharacter, maze[y][x]));
                }


        if (turtle == null)
            throw new Exception("Turtle missing");
        return (maze, totalAllignment, turtle);
    }

    void IAdvent.Run()
    {
        var memory = GetInput();
        var intCode = new IntCode(memory, new long[0]);

        var output = intCode.Output;

        intCode.Run();

        string input = new string(output.Select(c => (char)c).ToArray());
        var (maze, totalAllignment, turtle) = ToMazeWithIntersections(input);
        //PrintMaze(maze);

        Advent.AssertAnswer1(totalAllignment, 3920);

        (maze, totalAllignment, turtle) = ToMazeWithIntersections(input);
        var instructions = "";
        //PrintMaze(maze);
        while (true)
        {
            int rot = 0;
            for (; rot < 4; rot++)
            {
                if (turtle.CanMove(maze, 1))
                    break;
                turtle = turtle.Right(maze);
            }
            if (rot == 4)
                break;
            instructions += rot == 1 ? "R," : "L,";

            int moves = 0;
            while (turtle.CanMove(maze, 1))
            {
                turtle = turtle.Move(maze, 1);
                moves++;
            }
            instructions += moves + ",";
        }
        //PrintMaze(maze);
        //Console.WriteLine(instructions);
        var (instrs, calls) = FunctInstructionsAndCalls(instructions);

        string intCodeInput = calls.TrimEnd(',') + '\n';
        foreach (var (instr, _) in instrs.OrderBy(y => y.Value))
            intCodeInput += instr.TrimEnd(',') + '\n';
        intCodeInput += "N\n";
        //Console.WriteLine(intCodeInput);
        
        memory = GetInput();
        memory[0] = 2;
        foreach (var i in intCodeInput)
            intCode.Input.Enqueue(i);

        intCode.Output.Clear();
        intCode.Run();
        string result = new(intCode.Output.Select(i => (char)i).ToArray());
        
        Advent.AssertAnswer2(intCode.Output[^1], 673996);
    }

    private static (Dictionary<string, char> instr, string calls) FunctInstructionsAndCalls(string instructions)
    {
        Dictionary<string, char> instrDict = new();
        string calls = "";
        for (int x = 20; x >= 1; x--)
            for (int y = 20; y >= 1; y--)
                for (int z = 20; z >= 1; z--)
                {
                    var take = new[] { x, y, z };
                    instrDict.Clear();
                    calls = "";
                    int functionsUsed = 0;
                    bool error = false;
                    for (int i = 0; i < instructions.Length; )
                    {
                        var instr = instrDict.FirstOrDefault(kv => kv.Key.Length <= instructions.Length - i && instructions[i..(i+kv.Key.Length)] == kv.Key);
                        if (instr.Key != null)
                        {
                            calls += instr.Value + ",";
                            i += instr.Key.Length;
                        }
                        else
                        {
                            int tryToTake = take[Math.Min(functionsUsed, 2)] + 1; // + 1 for the ,
                            if (functionsUsed == 3 || tryToTake > instructions.Length - i || instructions[i + tryToTake - 1] != ',')
                            {
                                error = true;
                                break;
                            }

                            char c = (char)('A' + functionsUsed++);
                            instrDict[instructions[i..(i + tryToTake)]] = c;
                            calls += c + ",";
                            i += tryToTake;
                        }
                    }
                    if (!error)
                        return (instrDict, calls);
                }
        throw new Exception("not found");
    }

    private static void PrintMaze(char[][] maze)
        => Console.WriteLine("\n" + string.Join("\n", maze.Select(ca => new string(ca))));

    public record Turtle(int Y, int X, int Direction)
    {
        public Turtle Right(char[][] maze)
        {
            var result = new Turtle(Y, X, (Direction + 1) % 4);
            maze[Y][X] = TurtleCharacter[result.Direction];
            return result;
        }
        public Turtle Left(char[][] maze)
        {
            var result = new Turtle(Y, X, (Direction + 3) % 4);
            maze[Y][X] = TurtleCharacter[result.Direction];
            return result;
        }
        public bool CanMove(char[][] maze, int length)
        {
            var (x, y) = (X, Y);
            for (int i = 0; i < length; i++)
            {
                (y, x) = (y + offsets[Direction].dy, x + offsets[Direction].dx);
                if (x < 0 || y < 0 || x >= maze[0].Length || y >= maze.Length)
                    return false;
                if (maze[y][x] == '.')
                    return false;
            }
            return true;
        }
        public Turtle Move(char[][] maze, int length)
        {
            var (x, y) = (X, Y);
            for (int i = 0; i < length; i++)
            {
                if (maze[y][x] != 'O')
                    maze[y][x] = '.';
                (y, x) = (y + offsets[Direction].dy, x + offsets[Direction].dx);
                if (maze[y][x] == '#')
                    maze[y][x] = '.';
            }
            var result = new Turtle(y, x, Direction);

            if (maze[y][x] != 'O')
                maze[result.Y][result.X] = TurtleCharacter[Direction];
            return result;
        }

    }
}