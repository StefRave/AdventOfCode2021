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

    [Theory]
    [InlineData(sampleInput1, 23)]
    [InlineData(sampleInput2, 58)]
    public void Part1Sample(string input, int expectedSteps)
    {
        Assert.Equal(expectedSteps, DoPart1(input));
    }

    const string samplePart2Input2 = @"             Z L X W       C                 
             Z P Q B       K                 
  ###########.#.#.#.#######.###############  
  #...#.......#.#.......#.#.......#.#.#...#  
  ###.#.#.#.#.#.#.#.###.#.#.#######.#.#.###  
  #.#...#.#.#...#.#.#...#...#...#.#.......#  
  #.###.#######.###.###.#.###.###.#.#######  
  #...#.......#.#...#...#.............#...#  
  #.#########.#######.#.#######.#######.###  
  #...#.#    F       R I       Z    #.#.#.#  
  #.###.#    D       E C       H    #.#.#.#  
  #.#...#                           #...#.#  
  #.###.#                           #.###.#  
  #.#....OA                       WB..#.#..ZH
  #.###.#                           #.#.#.#  
CJ......#                           #.....#  
  #######                           #######  
  #.#....CK                         #......IC
  #.###.#                           #.###.#  
  #.....#                           #...#.#  
  ###.###                           #.#.#.#  
XF....#.#                         RF..#.#.#  
  #####.#                           #######  
  #......CJ                       NM..#...#  
  ###.#.#                           #.###.#  
RE....#.#                           #......RF
  ###.###        X   X       L      #.#.#.#  
  #.....#        F   Q       P      #.#.#.#  
  ###.###########.###.#######.#########.###  
  #.....#...#.....#.......#...#.....#.#...#  
  #####.#.###.#######.#######.###.###.#.#.#  
  #.......#.......#.#.#.#.#...#...#...#.#.#  
  #####.###.#####.#.#.#.#.###.###.#.###.###  
  #.......#.....#.#...#...............#...#  
  #############.#.#.###.###################  
               A O F   N                     
               A A D   M                     ";

    [Theory]
    [InlineData(samplePart2Input2, 396)]
    public void Part2Sample(string input, int expectedSteps)
    {
        Assert.Equal(expectedSteps, DoPart2(input));
    }

    void IAdvent.Run()
    {
        int result = DoPart1(GetInput());
        Advent.AssertAnswer1(result, expected: 528);

        result = DoPart2(GetInput());
        Advent.AssertAnswer2(result, expected: 6214);
    }

    private static int DoPart1(string input)
    {
        var (maze, portalPostions, startPos) = ParseInput(input);
        return Solve(maze, portalPostions, startPos, doLevels: false);
    }

    private static int DoPart2(string input)
    {
        var (maze, portalPostions, startPos) = ParseInput(input);
        return Solve(maze, portalPostions, startPos, doLevels: true);
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

    private static int Solve(char[][] maze, Dictionary<(int y, int x), ((int y, int x), string id)> portalPositions, (int y, int x) startPos, bool doLevels = false)
    {
        var pathStart = ImmutableQueue<(string, int, int)>.Empty;

        int[,,] visited = new int[1000,maze.Length,maze[0].Length];
        var queue = new PriorityQueue<((int y, int x), int level, ImmutableQueue<(string, int, int)> path), int>();
        queue.Enqueue((startPos, 0, pathStart), 0);
        while (queue.TryDequeue(out var posAndLevel, out int cost))
        {
            var pos = posAndLevel.Item1;
            int level = posAndLevel.level;
            var path = posAndLevel.path;
            char c = maze[pos.y][pos.x];
            if (c == '#' || visited[level, pos.y,pos.x] != 0)
                continue;

            visited[level, pos.y,pos.x] = cost;
            if (char.IsLetter(c))
            {
                var (newPos, id) = portalPositions[pos];
                if ((id == "AA" || id == "ZZ") && (level != 0))
                    continue;
                if (id == "ZZ")
                    return cost - 1; // position before Z portal
                int newLevel = !doLevels ? level : NewLevel(maze, pos, level);
                if (newLevel < 0)
                    1.ToString();
                ImmutableQueue<(string, int, int)> newPath = path.Enqueue((id, level, cost));
                if (newLevel >= 0)
                    queue.Enqueue((newPos, newLevel, newPath), cost);
                continue;
            }
            foreach (var p in GetOffsetsFrom(pos))
                queue.Enqueue((p, level, path), cost + 1);
        }
        throw new Exception("not found");
    }

    private static int NewLevel(char[][] maze, (int y, int x) pos, int currentLevel)
        => pos.x > 4 && pos.y > 4 && pos.x < (maze[0].Length - 4) && pos.y < (maze.Length - 4) ? currentLevel + 1 : currentLevel - 1; 
}