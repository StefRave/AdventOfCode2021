namespace AdventOfCode2024;

public class Day08 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();

        var dict = new Dictionary<char, List<V2>>();
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
                if (input[y][x] != '.')
                {
                    if (!dict.ContainsKey(input[y][x]))
                        dict[input[y][x]] = new List<V2>();
                    dict[input[y][x]].Add(new V2(x, y));
                }

        var answer1 = CalculateAntinodeCount(1);
        Advent.AssertAnswer1(answer1, expected: 371, sampleExpected: 14);


        int answer2 = CalculateAntinodeCount(2);
        Advent.AssertAnswer2(answer2, expected: 1229, sampleExpected: 34);


        bool IsInside(V2 pos) => pos.y >= 0 && pos.y < input.Length && pos.x >= 0 && pos.x < input[0].Length;

        int CalculateAntinodeCount(int part)
        {
            var antinodes = new HashSet<V2>();

            foreach (var (letter, nodes) in dict.Where(d => d.Value.Count > 1))
                for (int i = 0; i < nodes.Count; i++)
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        Add(nodes[i], nodes[j]);
                        Add(nodes[j], nodes[i]);
                    }

            return antinodes.Count;


            void Add(V2 node1, V2 node2)
            {
                V2 diff = node1 - node2;
                var pos = node1;
                for (int k = 0; IsInside(pos); k++)
                {
                    if (part != 1 || k == 1)
                        antinodes.Add(pos);
                    pos = pos + diff;
                }
            }
        }
    }

    public record V2(int x, int y)
    {
        public static V2 operator -(V2 a, V2 b) => new V2(a.x - b.x, a.y - b.y);
        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
    }
}







