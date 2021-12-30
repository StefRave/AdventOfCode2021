#nullable enable
using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
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

    const string samplePart2Input2 = @"###############
#d.ABC.#.....a#
###### . ######
######.@.######
###### . ######
#b.....#.....c#
###############";

    [Theory]
    [InlineData(samplePart2Input2, 72)]
    public void Part2Sample(string input, int expectedSteps)
    {
        Assert.Equal(expectedSteps, DoPart2(input));
    }


    void IAdvent.Run()
    {
        int result = DoPart1(GetInput());
        Advent.AssertAnswer1(result, expected: 3546);

        result = DoPart2(GetInput());
        Advent.AssertAnswer2(result, expected: 3546);
    }

    private static int DoPart1(string input)
    {
        var (maze, tmpDict, startPos) = ParseInput(input);

        return Solve(maze, tmpDict, startPos);
    }

    private static int DoPart2(string input)
    {
        var (maze, tmpDict, pos) = ParseInput(input);
        maze[pos.y+0][pos.x+0] = '#';
        maze[pos.y-1][pos.x+0] = '#';
        maze[pos.y+1][pos.x+0] = '#';
        maze[pos.y+0][pos.x-1] = '#';
        maze[pos.y+0][pos.x+1] = '#';
        var posses = new[]
        {
            (pos.y - 1, pos.x - 1),
            (pos.y - 1, pos.x + 1),
            (pos.y + 1, pos.x - 1),
            (pos.y + 1, pos.x + 1),
        };
        return Solve(maze, tmpDict, posses);
    }

    private static (char[][] maze, Dictionary<char, (int y, int x)> tmpDict, (int y, int x) startPos) ParseInput(string input)
    {
        var maze = input.SplitByNewLine().Select(l => l.ToCharArray()).ToArray();
        var dict = new Dictionary<char, (int y, int x)>();
        for (int y = 0; y < maze.Length; y++)
            for (int x = 0; x < maze[0].Length; x++)
            {
                char c = maze[y][x];
                if (c != '#' && c != '.')
                    dict.Add(c, (y, x));
            }
        var startPos = dict['@'];
        dict.Remove('@');

        return (maze, dict, startPos);
    }

    private static int Solve(char[][] maze, Dictionary<char, (int y, int x)> tmpDict, (int y, int x) startPos)
    {
        return Solve(maze, tmpDict, new[] { startPos });
    }

    private static int Solve(char[][] maze, Dictionary<char, (int y, int x)> tmpDict, (int y, int x)[] posses)
    {
        var cache = new Dictionary<State, int>();
        var optionsCache = new Dictionary<OptionState, List<(char c, int cost)>>();

        int result = GetMinimalCost(
            posses.ToImmutableArray(),
            new string(tmpDict.Keys.Where(c => c != '@').ToArray()));
        
        return result;


        int GetMinimalCost(ImmutableArray<(int y, int x)> posses, string toFind)
        {
            var state = new State(toFind, posses);
            if (cache.TryGetValue(state, out var cachedResult))
                return cachedResult;

            bool noResults = true;
            int minValue = int.MaxValue;
            for (int index = 0; index < posses.Length; index++)
            {
                var pos = posses[index];
                var found = GetOptionsWithCost(pos, toFind);

                foreach (var option in found)
                {
                    var newPos = tmpDict[option.c];
                    var newPosses = posses.SetItem(index, newPos);
                    var newToFind = RemoveCharacterFromStrin(toFind, option.c);
                    int newValue = option.cost + GetMinimalCost(newPosses, newToFind);
                    minValue = Math.Min(minValue, newValue);
                    noResults = false;
                }
            }
            if (noResults)
                minValue = 0;
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
            var optionsState = new OptionState(startPos, toFind);
            if (optionsCache.TryGetValue(optionsState, out var found))
                return found;

            found = new List<(char c, int cost)>();
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
            optionsCache.Add(optionsState, found);
            return found;
        }
    }

    public record OptionState((int,int) p1, string p2);

    public class State
    {
        string toFind;
        ImmutableArray<(int y, int x)> startPos;

        public State(string toFind, ImmutableArray<(int y, int x)> startPos)
        {
            this.toFind = toFind;
            this.startPos = startPos;
        }

        public override bool Equals(object? obj)
        {
            var y = obj as State ?? throw new InvalidCastException();
            return
                startPos.SequenceEqual(y.startPos) &&
                toFind == y.toFind;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(startPos.Aggregate(0, (x,y) => HashCode.Combine(x, y)), toFind);
        }
    }

    private static void PrintMaze(char[][] maze)
        => Console.WriteLine("\n" + string.Join("\n", maze.Select(ca => new string(ca))));


}