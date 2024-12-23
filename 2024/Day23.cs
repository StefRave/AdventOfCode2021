
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode2024;

public class Day23 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(x => x.Split('-'))
            .ToArray();
        var connections = BuildConnections(input);

        var setsOf3 = GetSetsOf3(connections);
        int answer1 = setsOf3.Count(s => s.Nodes.Any(n => n.StartsWith("t")));
        Advent.AssertAnswer1(answer1, expected: 1156, sampleExpected: 7);

        string answer2 = FindSetsWithMore(setsOf3);
        Advent.AssertAnswer2(answer2, expected: "bx,cx,dr,dx,is,jg,km,kt,li,lt,nh,uf,um", sampleExpected: "co,de,ka,ta");


        string FindSetsWithMore(NodeSet[] sets)
        {
            var solutions = new HashSet<NodeSet>();
            for (int i = 0; i < sets.Length; i++)
            {
                var set = sets[i];
                var candidates = set.Nodes.SelectMany(n => connections[n]).ToArray();
                foreach (string candidate in candidates)
                {
                    var candidateConnections = connections[candidate];
                    if (set.Nodes.All(n => candidateConnections.Contains(n)))
                    {
                        solutions.Add(new NodeSet(set.Nodes.Concat([candidate])));
                    }
                }
            }
            if (solutions.Count > 1)
                return FindSetsWithMore(solutions.ToArray());
            return string.Join(",", solutions.First().Nodes);
        }
    }

    private static NodeSet[] GetSetsOf3(Dictionary<string, HashSet<string>> connections)
    {
        var solutions = new HashSet<NodeSet>();
        foreach (var (node, connected) in connections)
        {
            foreach (string node2 in connected)
            {
                foreach (string node3 in connected)
                {
                    if (node2 == node3)
                        continue;
                    if (connections[node2].Contains(node3))
                    {
                        if (solutions.Add(new NodeSet([node, node2, node3])))
                        {
                            //Console.WriteLine($"{node} {node2} {node3}");
                        }
                    }

                }
            }
        }
        return solutions.ToArray();
    }

    private static Dictionary<string, HashSet<string>> BuildConnections(string[][] input)
    {
        var connections = new Dictionary<string, HashSet<string>>();

        foreach (var line in input)
        {
            connections.Update(line[0], v =>
            {
                v ??= new HashSet<string>();
                v.Add(line[1]);
                return v;
            });
            connections.Update(line[1], v =>
            {
                v ??= new HashSet<string>();
                v.Add(line[0]);
                return v;
            });
        }
        return connections;
    }

    [DebuggerDisplay("Nodes:{Nodes.Length} {string.Join(',', Nodes)}")]
    public class NodeSet : IEquatable<NodeSet>
    {
        public string[] Nodes { get; }

        public NodeSet(IEnumerable<string> nodes)
            => Nodes = nodes.OrderBy(x => x).ToArray();

        public override bool Equals(object obj) 
            => Equals(obj as NodeSet);

        public bool Equals(NodeSet other)
        {
            if (other == null)
                return false;
            if (Nodes.Length != other.Nodes.Length)
                return false;
            for (int i = 0; i < Nodes.Length; i++)
                if (Nodes[i] != other.Nodes[i])
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (var node in Nodes)
                hashCode ^= node.GetHashCode();
            return hashCode;
        }

        public bool Match(NodeSet set2)
        {
            string[] sortedStringsA = Nodes;
            string[] sortedStringsB = set2.Nodes;
            // count matches of sortedStringsA in sortedStringsB
            int matches = 0;
            int i = 0;
            int j = 0;
            while (i < sortedStringsA.Length && j < sortedStringsB.Length)
            {
                int compare = sortedStringsA[i].CompareTo(sortedStringsB[j]);
                if (compare == 0)
                {
                    matches++;
                    i++;
                    j++;
                }
                else if (compare < 0)
                {
                    i++;
                }
                else
                {
                    j++;
                }
            }
            return matches == Nodes.Length - 1;
        }
    }
}






















