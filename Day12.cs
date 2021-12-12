namespace AdventOfCode2021;

public class Day12
{
    private readonly ITestOutputHelper output;

    public Day12(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split('-'))
            .ToArray();
        var pathsFromPoint = input.Concat(input.Select(i => i.Reverse().ToArray()))
            .GroupBy(i => i[0])
            .ToDictionary(g => g.Key, g => g.Select(i => i[1]).ToArray());

        int pathCount = FindPathTillEnd(pathsFromPoint, mayVisitASmallCaveTwice: false);
        Advent.AssertAnswer1(pathCount);

        pathCount = FindPathTillEnd(pathsFromPoint, mayVisitASmallCaveTwice: true);
        Advent.AssertAnswer2(pathCount);
    }

    private int FindPathTillEnd(Dictionary<string, string[]> pathsFromPoint, bool mayVisitASmallCaveTwice)
    {
        var paths = new List<List<string>>();
        GotoNextCave("start", currentPath: new List<string>(), visitedSmallCaves: new HashSet<string>(), mayVisitASmallCaveTwice: mayVisitASmallCaveTwice);

        return paths.Count;

        void GotoNextCave(string currentCave, List<string> currentPath, HashSet<string> visitedSmallCaves, bool mayVisitASmallCaveTwice)
        {
            if(visitedSmallCaves.Contains(currentCave))
            {
                if (!mayVisitASmallCaveTwice || currentCave == "start")
                    return;
                mayVisitASmallCaveTwice = false;
            }
            currentPath = new List<string>(currentPath);
            currentPath.Add(currentCave);
            if (currentCave == "end")
            {
                paths.Add(currentPath);
                return;
            }
            if (char.IsLower(currentCave[0]))
            {
                visitedSmallCaves = new HashSet<string>(visitedSmallCaves);
                visitedSmallCaves.Add(currentCave);
            }
            foreach (var option in pathsFromPoint[currentCave])
                GotoNextCave(option, currentPath, visitedSmallCaves, mayVisitASmallCaveTwice);
        }
    }
}
