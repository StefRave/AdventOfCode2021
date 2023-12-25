using System.Diagnostics;
using System.Numerics;

namespace AdventOfCode2023;

public class Day25 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => (src: line[..3], targets: line[5..].Split( )))
            .ToArray();

        var components = input.Select(i => i.src).Concat(input.SelectMany(i => i.targets))
            .Distinct()
            .Select((c, i) => (c, i))
            .ToDictionary(c => c.c, c => c.i);
        var connections = new Dictionary<int, HashSet<int>>();
        var sourceTargets = new List<(string src, string target)>();
        foreach (var (src, targets) in input)
        {
            int srcNr = components[src];
            foreach (var target in targets)
            {
                sourceTargets.Add((src, target));
                AddToConnections(srcNr, components[target]);
                AddToConnections(components[target], srcNr);
            }
        }
        foreach (var kv in connections)
        {
            if (kv.Value.Count < 4)
                Console.WriteLine(kv.Key);
        }
        var nodes = sourceTargets.Select(s => s.src).Union(sourceTargets.Select(s => s.target)).Distinct()
            .Select(n => new Node
        {
            Position = new Vector2((float)Random.Shared.NextDouble() * 2 - 1, (float)Random.Shared.NextDouble() * 2 - 1),
            Velocity = new Vector2(0, 0),
            Name = n
        }).ToList();
        var nodeDict = nodes.ToDictionary(n => n.Name, n => n);
        var edges = sourceTargets.Select(v => new Edge { From = nodeDict[v.src], To = nodeDict[v.target] }).ToList();
        foreach (var node in nodes)
        {
            node.Connections = edges.Where(e => e.From == node).ToList();
        }
        ForceDirectedGraph graph = new ForceDirectedGraph
        {
            Nodes = nodes,
            Edges = edges
        };

        int toRemove = 3;
        int iteration = 0;
        while (toRemove > 0)
        {
            // Simulation loop
            for (int i = 0; i < 50; i++) // Run for a certain number of iterations
            {
                iteration++;
                graph.Update(0.01f); // Update with a fixed delta time
                                     // Render or output your graph here
            }
            edges = graph.Edges.OrderByDescending(e => e.Length).ToList();
            while (edges[0].Length > edges[toRemove].Length * 1.5)
            {
                Console.WriteLine($"Remove {iteration}  {edges[0].From.Name} - {edges[0].To.Name}");
                connections[components[edges[0].From.Name]].Remove(components[edges[0].To.Name]);
                connections[components[edges[0].To.Name]].Remove(components[edges[0].From.Name]);

                graph.Edges = edges = edges.Skip(1).ToList();
                toRemove--;
            }
        }

        int[] numbers = components.Values.ToArray();
        bool[] visited = new bool[components.Count];
        int nr1 = numbers.First(n => !visited[n]);
        int count1 = FloodFill(nr1, visited);
        int nr2 = numbers.First(n => !visited[n]);
        int count2 = FloodFill(nr2, visited);
        Advent.AssertAnswer1(count1 * count2, expected: 562772, sampleExpected: 54);


        void AddToConnections(int src, int target)
        {
            if (!connections.TryGetValue(src, out var set))
            {
                set = new HashSet<int>();
                connections.Add(src, set);
            }
            set.Add(target);
        }

        int FloodFill(int start, bool[] visited)
        {
            var queue = new Queue<int>([start]);
            int count = 0;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                if (visited[current])
                    continue;
                visited[current] = true;
                count++;
                if (connections.TryGetValue(current, out var nexts))
                {
                    foreach (var next in nexts)
                        queue.Enqueue(next);
                }
            }
            return count;
        }
    }

    [DebuggerDisplay("{Name} ({Position.X}, {Position.Y})")]
    public class Node
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public List<Edge> Connections;
        public string Name;
    }

    [DebuggerDisplay("{From.Name} ({To.Name}, {Length})")]
    public class Edge
    {
        public Node From;
        public Node To;
        public float Length => (From.Position - To.Position).Length();
    }

    public class ForceDirectedGraph
    {
        public List<Node> Nodes;
        public List<Edge> Edges;

        // Constants for forces
        public float RepulsiveForceConstant = 5000;
        public float SpringLength = 10; // Ideal spring length
        public float SpringConstant = 0.02f;

        public void Update(float deltaTime)
        {
            ApplyForces(deltaTime);
            UpdatePositions(deltaTime);
        }

        private void ApplyForces(float deltaTime)
        {
            // Apply Repulsive Forces
            foreach (var node in Nodes)
            {
                node.Velocity = new Vector2(0, 0); // Reset the velocity
                foreach (var other in Nodes)
                {
                    if (node != other)
                    {
                        Vector2 direction = node.Position - other.Position;
                        float distance = direction.Length();
                        Vector2 normalizedDirection = direction / distance;
                        node.Velocity += normalizedDirection * (RepulsiveForceConstant / (distance * distance));
                    }
                }
            }

            // Apply Spring Forces
            foreach (var edge in Edges)
            {
                Vector2 direction = edge.To.Position - edge.From.Position;
                float distance = direction.Length();
                Vector2 normalizedDirection = direction / distance;

                // Hooke's Law for spring force: F = -kx
                float springForceMagnitude = SpringConstant * (distance - SpringLength);
                Vector2 springForce = normalizedDirection * springForceMagnitude;

                edge.From.Velocity += springForce / deltaTime;
                edge.To.Velocity -= springForce / deltaTime;
            }
        }

        private void UpdatePositions(float deltaTime)
        {
            // Update positions based on velocity
            foreach (var node in Nodes)
            {
                node.Position += node.Velocity * deltaTime;
            }
        }
    }
}
