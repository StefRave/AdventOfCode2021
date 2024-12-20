namespace AdventOfCode2024;

public class Day20 : IAdvent
{
    readonly V2[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(l => l.ToCharArray())
            .ToArray();
        int answer1 = Moves(maxMoves: 2, Advent.UseSampleData ? 40 : 100);
        Advent.AssertAnswer1(answer1, expected: 1372, sampleExpected: 2);

        int answer2 = Moves(maxMoves: 20, leastSaves: Advent.UseSampleData ? 50 : 100);
        Advent.AssertAnswer2(answer2, expected: 979014, sampleExpected: 285);


        int Moves(int maxMoves, int leastSaves)
        {
            V2 start = Find('S');
            V2 end = Find('E');

            var queue = new Queue<(V2 p, int moves)>();
            queue.Enqueue((start, 0));
            var visited = new Dictionary<V2, int>();
            while (queue.Count > 0)
            {
                (V2 p, int moves) = queue.Dequeue();
                if (visited.ContainsKey(p))
                    continue;
                visited.Add(p, moves);
                if (p == end)
                    break;
                foreach (var d in directions)
                {
                    var pn = p + d;
                    if (input[pn.y][pn.x] != '#')
                        queue.Enqueue((pn, moves + 1));
                }
            }
            int result = 0;
            foreach (var (p, moves) in visited)
                result += FindCheats(p, moves, maxMoves);
            return result;


            int FindCheats(V2 po, int moves, int maxCount)
            {
                var visited2 = new HashSet<V2>();

                result = 0;
                for (int y = Math.Max(po.y - maxCount, 0); y <= Math.Min(po.y + maxCount, input.Length - 1); y++)
                    for (int x = Math.Max(po.x - (maxCount - Math.Abs(po.y - y)), 0); x <= Math.Min(po.x + (maxCount - Math.Abs(po.y - y)), input[y].Length - 1); x++)
                        if (x != 0 && y != 0 && input[y][x] != '#')
                        {
                            if (visited.TryGetValue((x, y), out int countShort))
                            {
                                int movesSaved = countShort - moves - Math.Abs(po.y - y) - Math.Abs(po.x - x);
                                if (movesSaved >= leastSaves)
                                    result++;
                            }
                        }
                return result;
            }
        }

        V2 Find(char v)
        {
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[y].Length; x++)
                    if (input[y][x] == v)
                        return new V2(x, y);
            throw new Exception();
        }

    }
}
