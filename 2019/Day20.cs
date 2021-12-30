#nullable enable
namespace AdventOfCode2019;

public class Day20 : IAdvent
{
    private static string GetInput() => File.ReadAllText(@"Input/input20.txt");
    private static (int dy, int dx)[] offsets = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
    public static IEnumerable<(int y, int x)> GetOffsetsFrom((int y, int x) p) => offsets.Select(o => (o.dy + p.y, o.dx + p.x));

    const string sampleInput1 = @"         A           
         A           
  #######.#########  
  #######.........#  
  #######.#######.#  
  #######.#######.#  
  #######.#######.#  
  #####  B    ###.#  
BC...##  C    ###.#  
  ##.##       ###.#  
  ##...DE  F  ###.#  
  #####    G  ###.#  
  #########.#####.#  
DE..#######...###.#  
  #.#########.###.#  
FG..#########.....#  
  ###########.#####  
             Z       
             Z       ";
    const string sampleInput2 = @"                   A               
                   A               
  #################.#############  
  #.#...#...................#.#.#  
  #.#.#.###.###.###.#########.#.#  
  #.#.#.......#...#.....#.#.#...#  
  #.#########.###.#####.#.#.###.#  
  #.............#.#.....#.......#  
  ###.###########.###.#####.#.#.#  
  #.....#        A   C    #.#.#.#  
  #######        S   P    #####.#  
  #.#...#                 #......VT
  #.#.#.#                 #.#####  
  #...#.#               YN....#.#  
  #.###.#                 #####.#  
DI....#.#                 #.....#  
  #####.#                 #.###.#  
ZZ......#               QG....#..AS
  ###.###                 #######  
JO..#.#.#                 #.....#  
  #.#.#.#                 ###.#.#  
  #...#..DI             BU....#..LF
  #####.#                 #.#####  
YN......#               VT..#....QG
  #.###.#                 #.###.#  
  #.#...#                 #.....#  
  ###.###    J L     J    #.#.###  
  #.....#    O F     P    #.#...#  
  #.###.#####.#.#####.#####.###.#  
  #...#.#.#...#.....#.....#.#...#  
  #.#####.###.###.#.#.#########.#  
  #...#.#.....#...#.#.#.#.....#.#  
  #.###.#####.###.###.#.#.#######  
  #.#.........#...#.............#  
  #########.###.###.#############  
           B   J   C               
           U   P   P               ";
    const string sampleInput3 = @"";

    [Theory]
    [InlineData(sampleInput1, 23)]
    [InlineData(sampleInput2, 58)]
    public void Part1Sample(string input, int expectedSteps)
    {
        Assert.Equal(expectedSteps, DoPart1(input));
    }

    const string samplePart2Input2 = @"";

    [Theory]
    [InlineData(samplePart2Input2, 72)]
    public void Part2Sample(string input, int expectedSteps)
    {
        //Assert.Equal(expectedSteps, DoPart2(input));
    }


    void IAdvent.Run()
    {
        int result = DoPart1(GetInput());
        Advent.AssertAnswer1(result, expected: 23);

        //result = DoPart2(GetInput());
        //Advent.AssertAnswer2(result, expected: 3546);
    }

    private static int DoPart1(string input)
    {
        var (maze, portalPostions, startPos) = ParseInput(input);
        return Solve(maze, portalPostions, startPos);
    }

    private static int DoPart2(string input)
    {
        var (maze, portalPostions, startPos) = ParseInput(input);
        return 0;
    }

    private static (char[][] maze, Dictionary<(int y, int x), ((int y, int x), string id)>, (int y, int x) startPos) ParseInput(string input)
    {
        var maze = input.SplitByNewLine().Select(l => l.ToCharArray()).ToArray();

        var list = new List<(string, (int y, int x))>();
        var portals = new Dictionary<string, (int y, int x)>();
        for (int y = 1; y < maze.Length - 1; y++)
            for (int x = 1; x < maze[0].Length - 1; x++)
                if (char.IsLetter(maze[y][x]))
                {
                    string? id = null;
                    bool hasOpenPosition = false;

                    foreach (var p in GetOffsetsFrom((y, x)))
                    {
                        if (maze[p.y][p.x] == '.')
                            hasOpenPosition = true;
                        if (char.IsLetter(maze[p.y][p.x]))
                        {
                            if (p.y < y || p.x < x)
                                id = "" + maze[p.y][p.x] + maze[y][x];
                            else
                                id = "" + maze[y][x] + maze[p.y][p.x];

                        }
                    }
                    if (id != null && hasOpenPosition)
                        list.Add((id, (y, x)));
                }
        
        var portalPositions = new Dictionary<(int y, int x), ((int y, int x), string id)>();
        foreach (var positions in list.GroupBy(l => l.Item1))
        {
            string id = positions.Key;
            var pos = positions.First();
            var pos2 = positions.Last();
            portalPositions.Add(pos.Item2, (GetTransportingPosition(maze, pos2.Item2), id));
            if (pos != pos2)
                portalPositions.Add(pos2.Item2, (GetTransportingPosition(maze, pos.Item2), id));
        }
        return (maze, portalPositions, portalPositions.First(kv => kv.Value.id == "AA").Value.Item1);
    }

    private static (int y, int x) GetTransportingPosition(char[][] maze, (int y, int x) pos)
    {
        return GetOffsetsFrom(pos)
            .Where(p => p.x >= 0 && p.y >= 0 && p.x < maze[0].Length && p.y < maze.Length)
            .Single(p => maze[p.y][p.x] == '.');
    }

    private static int Solve(char[][] maze, Dictionary<(int y, int x), ((int y, int x), string id)> portalPositions, (int y, int x) startPos)
    {
        var cache = new Dictionary<State, int>();
        var optionsCache = new Dictionary<OptionState, List<(char c, int cost)>>();

        int result = GetMinimalCost(startPos);
        return result;

        int GetMinimalCost((int y, int x) startPos)
        {
            //var optionsState = new OptionState(startPos, toFind);
            //if (optionsCache.TryGetValue(optionsState, out var found))
            //    return found;

            int[,] visited = new int[maze.Length,maze[0].Length];
            var queue = new PriorityQueue<(int y, int x), int>();
            queue.Enqueue(startPos, 0);
            while (queue.TryDequeue(out var pos, out int cost))
            {
                char c = maze[pos.y][pos.x];
                if (c == '#' || visited[pos.y,pos.x] != 0)
                    continue;

                visited[pos.y,pos.x] = cost;
                if (char.IsLetter(c))
                {
                    if (c == 'Z')
                        return cost - 1; // position before Z portal
                    queue.Enqueue(portalPositions[pos].Item1, cost);
                    continue;
                }
                foreach (var d in offsets)
                    queue.Enqueue((pos.y + d.dy, pos.x + d.dx), cost + 1);
            }
            //optionsCache.Add(optionsState, found);
            throw new Exception("not found");
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