
namespace AdventOfCode2024;

public class Day16 : IAdvent
{
    readonly V2[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();
        V2 start = Find('S');
        V2 end = Find('E');

        var (answer1, answer2) = ShortestRoute();
        Advent.AssertAnswer1(answer1, expected: 109496, sampleExpected: 7036);
        Advent.AssertAnswer2(answer2, expected: 551, sampleExpected: 45);


        (int answer1, int answer2) ShortestRoute()
        {

            var scores = new Dictionary<(V2 p, V2 d), int>();
            var prevPos = new Dictionary<(V2 p, V2 d), (V2 p, int score)>();
            int recordScore;

            var queue = new PriorityQueue<(V2 p, V2 d, int score, int steps), int>();

            foreach (var d in PossibleDirections(start))
                queue.Enqueue((start + d, d, 1001, 1), 1);
            while (queue.Count > 0)
            {
                var (p, d, score, step) = queue.Dequeue();
                if (scores.TryGetValue((p, d), out recordScore))
                    if (recordScore < score)
                        continue;
                foreach (var dp in PossibleDirections(p))
                {
                    if (dp == (V2.V0 - d))
                        continue;
                    int tScore = score + ((dp == d) ? 1 : 1001);
                    var pn = p + dp;
                    if (scores.TryGetValue((pn, dp), out recordScore) && recordScore <= tScore)
                        continue;
                    queue.Enqueue((pn, dp, tScore, step + 1), step);
                    scores[(pn, dp)] = tScore;
                    prevPos[(pn, dp)] = (p, score);
                }
            }
            recordScore = int.MaxValue;
            V2 recordDir = V2.V0;
            foreach (var dp in directions)
            {
                if (scores.TryGetValue((end, dp), out int endScore) && endScore < recordScore)
                {
                    recordScore = endScore;
                    recordDir = dp;
                }
            }
            int result = CountPositionsOnAllRoutes(scores, recordScore);
            return (recordScore, result);
        }

        IEnumerable<V2> PossibleDirections(V2 p)
        {
            foreach (var d in directions)
            {
                if (Get(p + d) != '#')
                    yield return d;
            }
        }
        char Get(V2 p) => input[p.y][p.x];

        V2 Find(char v)
        {
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[y].Length; x++)
                    if (input[y][x] == v)
                        return new V2(x, y);
            throw new Exception();
        }

        int CountPositionsOnAllRoutes(Dictionary<(V2 p, V2 d), int> scores, int recordScore)
        {
            var queue = new Queue<(V2 p, V2 d, int score)>();
            var visited = new HashSet<(V2, V2)>();

            foreach (var d in PossibleDirections(end))
                queue.Enqueue((end, d, recordScore));
            while (queue.Any())
            {
                var (p, d, score) = queue.Dequeue();
                visited.Add((p, d));

                foreach (var dp in PossibleDirections(p))
                {
                    var np = p + dp;
                    foreach (var ds in directions)
                    {
                        if (scores.TryGetValue((np, ds), out var pScore))
                        {
                            var tScore = pScore + ((dp == V2.V0 - ds) ? 1 : 1001);
                            if (score == tScore)
                                queue.Enqueue((np, V2.V0 - ds, pScore));
                        }
                    }
                }
            }

            return visited.Select(v => v.Item1).Distinct().Count() + 2; // + start and end
        }
    }
}















