namespace AdventOfCode2024;

public class Day23 : IAdvent
{
    static char ToChar(string s) => (char)((s[0] - 'a') * 64 + s[1] - 'a');
    static string FromChar(char c) => new string([(char)(c / 64 + 'a'), (char)((c % 64) + 'a')]);

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(x => x.Split('-').Select(ToChar).ToArray())
            .ToArray();
        var connections = BuildConnections(input);

        var setsOf3 = GetSetsOf3(connections);
        int answer1 = setsOf3.Count(s => s.Any(n => FromChar(n).StartsWith("t")));
        Advent.AssertAnswer1(answer1, expected: 1156, sampleExpected: 7);

        string answer2 = FindSetsWithMore(setsOf3);
        Advent.AssertAnswer2(answer2, expected: "bx,cx,dr,dx,is,jg,km,kt,li,lt,nh,uf,um", sampleExpected: "co,de,ka,ta");


        string FindSetsWithMore(HashSet<string> sets)
        {
            var solutions = new HashSet<string>();
            var setArray = sets.ToArray();
            var removed = sets.ToDictionary(s => s, s => false);
            for (int i = 0; i < setArray.Length; i++)
            {
                var set = setArray[i];
                if (removed[set])
                    continue;
                removed[set] = true;
                var candidates = set.SelectMany(n => connections[n]).ToArray();
                foreach (char candidate in candidates)
                {
                    var candidateConnections = connections[candidate];
                    if (set.All(n => candidateConnections.Contains(n)))
                    {
                        string solution = AddSorted(set, candidate);
                        solutions.Add(solution);
                        for (int j = 0; j < solution.Length - 1; j++)
                            removed[solution[..(j)] + solution[(j + 1)..]] = true;
                    }
                }
            }
            if (solutions.Count > 1)
                return FindSetsWithMore(solutions);
            return string.Join(",", solutions.First().Select(FromChar));
        }
    }

    private static string AddSorted(string s, char c)
        => Sort(s.Concat([c]));
    private static string Sort(IEnumerable<char> s)
        => new string(s.OrderBy(c => c).ToArray());

    private static HashSet<string> GetSetsOf3(Dictionary<char, string> connections)
    {
        var solutions = new HashSet<string>();
        foreach (var (node, connected) in connections)
        {
            foreach (var node2 in connected)
            {
                foreach (var node3 in connected)
                {
                    if (node2 == node3)
                        continue;
                    if (connections[node2].Contains(node3))
                    {
                        solutions.Add(Sort([node, node2, node3]));
                    }

                }
            }
        }
        return solutions;
    }

    private static Dictionary<char, string> BuildConnections(char[][] input)
    {
        var connections = new Dictionary<char, HashSet<char>>();

        foreach (var line in input)
        {
            Add(line[0], line[1]);
            Add(line[1], line[0]);
        }
        return connections.ToDictionary(kv => kv.Key, kv => Sort(kv.Value));


        void Add(char c1, char c2)
        {
            connections.Update(c1, v =>
            {
                v ??= new HashSet<char>();
                v.Add(c2);
                return v;
            });
        }
    }
}