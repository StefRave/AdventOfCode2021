namespace AdventOfCode2021;

public class Day12 : IAdvent
{
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split('-'))
            .ToArray();
        var pathsFromPoint = input.Concat(input.Select(line => new[] { line[1], line[0] }))
            .GroupBy(i => i[0], i => i[1])
            .ToDictionary(g => g.Key, g => g.ToArray());

        int pathCount = FindPathTillEnd(pathsFromPoint, mayVisitASmallCaveTwice: false);
        Advent.AssertAnswer1(pathCount);

        pathCount = FindPathTillEnd(pathsFromPoint, mayVisitASmallCaveTwice: true);
        Advent.AssertAnswer2(pathCount);
    }

    private int FindPathTillEnd(Dictionary<string, string[]> pathsFromPoint, bool mayVisitASmallCaveTwice)
    {
        var paths = new Stack<ImmutableStack<string>>();
        GotoNextCave("start", currentPath: ImmutableStack<string>.Empty, visitedSmallCaves: ImmutableHashSet<string>.Empty, mayVisitASmallCaveTwice: mayVisitASmallCaveTwice);

        return paths.Count;

        void GotoNextCave(string currentCave, ImmutableStack<string> currentPath, ImmutableHashSet<string> visitedSmallCaves, bool mayVisitASmallCaveTwice)
        {
            if(visitedSmallCaves.Contains(currentCave))
            {
                if (!mayVisitASmallCaveTwice || currentCave == "start")
                    return;
                mayVisitASmallCaveTwice = false;
            }
            currentPath = currentPath.Push(currentCave);
            if (currentCave == "end")
            {
                paths.Push(currentPath);
                return;
            }
            if (char.IsLower(currentCave[0]))
                visitedSmallCaves = visitedSmallCaves.Add(currentCave);
            
            foreach (var option in pathsFromPoint[currentCave])
                GotoNextCave(option, currentPath, visitedSmallCaves, mayVisitASmallCaveTwice);
        }
    }
}