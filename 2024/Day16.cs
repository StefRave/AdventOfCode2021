
namespace AdventOfCode2024;

public class Day16 : IAdvent
{
    enum Dir { S, E, N, W }
    readonly V2[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];

    Dir Left(Dir d) => (Dir)(((int)d + 1) % 4);
    Dir Right(Dir d) => (Dir)(((int)d + 3) % 4);
    V2 Move(Dir d) => directions[(int)d];

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

            var scores = new Dictionary<(V2 p, Dir d), int>();
            var queue = new PriorityQueue<(V2 p, Dir, int score, int steps), int>();

            queue.Enqueue((start, Dir.E, 0, 0), 0);

            while (queue.Count > 0)
            {
                var (p, d, score, step) = queue.Dequeue();
                if (scores.TryGetValue((p, d), out int recordScore))
                    if (recordScore < score)
                        continue;
                scores[(p, d)] = score;

                TryQueue(p + Move(d), d, score + 1, step + 1);
                TryQueue(p, Left(d), score + 1000, step);
                TryQueue(p, Right(d), score + 1000, step);

                void TryQueue(V2 p, Dir d, int score, int step)
                {
                    if (input[p.y][p.x] == '#')
                        return;
                    if (scores.TryGetValue((p, d), out recordScore) && recordScore <= score)
                        return;
                    queue.Enqueue((p, d, score, step), step);
                }
            }
            
            (int endScore, Dir recordDir) = GetEndScore(scores);
            int positions = CountPositionsOnAllRoutes(scores, endScore, recordDir);
            
            return (endScore, positions);
        }
        
        (int endScore, Dir recordDir) GetEndScore(Dictionary<(V2 p, Dir d), int> scores)
        {
            var endScore = int.MaxValue;
            Dir recordDir = 0;
            foreach (var dp in Enum.GetValues<Dir>())
            {
                if (scores.TryGetValue((end, dp), out int recordScore) && recordScore < endScore)
                {
                    endScore = recordScore;
                    recordDir = dp;
                }
            }
            return (endScore, recordDir);
        }

        V2 Find(char v)
        {
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[y].Length; x++)
                    if (input[y][x] == v)
                        return new V2(x, y);
            throw new Exception();
        }

        int CountPositionsOnAllRoutes(Dictionary<(V2 p, Dir d), int> scores, int recordScore, Dir lastDir)
        {
            var queue = new Queue<(V2 p, Dir d, int score)>();
            var visited = new HashSet<(V2, Dir)>();

            queue.Enqueue((end, lastDir, recordScore));
            while (queue.Any())
            {
                var (p, d, score) = queue.Dequeue();
                visited.Add((p, d));

                TryQueue(p - Move(d), d, score - 1);
                TryQueue(p, Left(d), score - 1000);
                TryQueue(p, Right(d), score - 1000);

                void TryQueue(V2 p, Dir d, int score)
                {
                    if (scores.TryGetValue((p, d), out recordScore) && recordScore == score)
                        queue.Enqueue((p, d, score));
                }
            }
            return visited.Select(v => v.Item1).Distinct().Count();
        }
    }
}















