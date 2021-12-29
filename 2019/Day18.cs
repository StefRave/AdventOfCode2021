#nullable enable
using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2019;

public class Day18 : IAdvent
{
    private static string GetInput() => File.ReadAllText(@"Input/input18.txt");
    private static (int dy, int dx)[] offsets = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };

    const string sampleInput1 = @"########################
#...............b.C.D.f#
#.######################
#.....@.a.B.c.d.A.e.F.g#
########################";
    const string sampleInput2 = @"#################
#i.G..c...e..H.p#
########.########
#j.A..b...f..D.o#
########@########
#k.E..a...g..B.n#
########.########
#l.F..d...h..C.m#
#################";
    const string sampleInput3 = @"########################
#@..............ac.GI.b#
###d#e#f################
###A#B#C################
###g#h#i################
########################";

    [Theory]
    [InlineData(sampleInput1, 132)]
    [InlineData(sampleInput2, 136)]
    [InlineData(sampleInput3, 81)]
    public void Part1Sample(string input, int expectedSteps)
    {
        Assert.Equal(expectedSteps, DoPart1(input));

    }

    void IAdvent.Run()
    {
        var input = GetInput();


        int result = DoPart1(input);
        Advent.AssertAnswer1(result, expected: 3546);
    }

    private static int DoPart1(string input)
    {
        char[][] maze = input.SplitByNewLine().Select(l => l.ToCharArray()).ToArray();
        var tmpDict = new Dictionary<char, (int y, int x)>();
        for (int y = 0; y < maze.Length; y++)
            for (int x = 0; x < maze[0].Length; x++)
            {
                char c = maze[y][x];
                if (c != '#' && c != '.')
                    tmpDict.Add(c, (y, x));
            }
        var pos = tmpDict['@'];
        tmpDict.Remove('@');


        var cache = new Dictionary<State, int>();

        int result = GetMinimalCost(pos, new string(tmpDict.Keys.Where(c => c != '@').ToArray()));
        return result;


        int GetMinimalCost((int y, int x) pos, string toFind)
        {
            var state = new State(toFind, pos);
            if (cache.TryGetValue(state, out var cachedResult))
                return cachedResult;

            var found = GetOptionsWithCost(pos, toFind);
            if (found.Count == 0)
                return 0;

            int minValue = int.MaxValue;
            foreach (var option in found)
            {
                var newPos = tmpDict[option.c];
                var newToFind = RemoveCharacterFromStrin(toFind, option.c);
                int newValue = option.cost + GetMinimalCost(newPos, newToFind);
                minValue = Math.Min(minValue, newValue);
            }

            cache.Add(state, minValue);
            return minValue;
        }

        static string RemoveCharacterFromStrin(string input, char c)
        {
            int i = input.IndexOf(c);
            return input[..i] + input[(i + 1)..];
        }

        List<(char c, int cost)> GetOptionsWithCost((int y, int x) startPos, string toFind)
        {
            var found = new List<(char c, int cost)>();
            int[,] visited = new int[maze.Length,maze[0].Length];
            var queue = new Queue<(int y, int x, int cost)>();
            queue.Enqueue((startPos.y, startPos.x, 0));
            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                char c = maze[pos.y][pos.x];
                if (c == '#' || (char.IsUpper(c) && toFind.Contains(char.ToLower(c))) || visited[pos.y,pos.x] != 0)
                    continue;

                visited[pos.y,pos.x] = pos.cost;
                if (char.IsLower(c) && toFind.Contains(c))
                {
                    found.Add((c, pos.cost));
                    continue;
                }
                foreach (var d in offsets)
                    queue.Enqueue((pos.y + d.dy, pos.x + d.dx, pos.cost + 1));
            }
            return found;
        }
    }

    public class State
    {
        string toFind;
        (int y, int x) startPos;

        public State(string toFind, (int y, int x) startPos)
        {
            this.toFind = toFind;
            this.startPos = startPos;
        }

        public override bool Equals(object? obj)
        {
            var y = obj as State ?? throw new InvalidCastException();
            return
                startPos == y.startPos &&
                toFind ==y.toFind;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(startPos, toFind);
        }
    }
    




    //private static void PrintMaze(char[][] maze)
    //    => Console.WriteLine("\n" + string.Join("\n", maze.Select(ca => new string(ca))));


}