namespace AdventOfCode2023;

public class Day23 : IAdvent
{
    static V2[] Deltas = [new V2(-1, 0), new V2(0, -1), new V2(1, 0), new V2(0, 1)];
    static char[] Slides = ['<', '^', '>', 'v'];

    [Flags] enum Direction { Forward = 1, Backward = 2, Both = 3 };

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();

        var (maxX, maxY) = (input[0].Length, input.Length);
        var start = new V2(input[0].IndexOf('.'), 0);
        var end = new V2(input[^1].IndexOf('.'), maxY - 1);

        var myVectors = GetVectors(input, start, end);
        var pointToIndex = myVectors.Keys.Select((p, i) => (p, i)).ToDictionary(p => p.p, p => p.i);
        var vectorIndexes = pointToIndex.Keys
            .Select(p => myVectors[p].Select(d => (dest: pointToIndex[d.Key], d.Value.length, d.Value.dir)).ToArray())
            .ToArray();

        int answer1 = LongestPath(pointToIndex[start], pointToIndex[end], vectorIndexes, part: 1);
        Advent.AssertAnswer1(answer1, expected: 2430, sampleExpected: 94);

        var answer2 = LongestPath(pointToIndex[start], pointToIndex[end], vectorIndexes, part: 2);
        Advent.AssertAnswer2(answer2, expected: 6534, sampleExpected: 154);
    }

    static Dictionary<V2, Dictionary<V2, (int length, Direction dir)>> GetVectors(
        string[] input, V2 start, V2 end)
    {
        var map = input.Select(line => line.ToCharArray()).ToArray();
        var vectors = new Dictionary<V2, Dictionary<V2, (int length, Direction dir)>>();

        var queue = new Stack<(V2, V2, Direction dir, int length)>();
        queue.Push((start, start, Direction.Both, 0));
        while (queue.Count > 0)
        {
            var (pos, startPos, dir, length) = queue.Pop();
            if (map[pos.y][pos.x] == 'x')
            {
                AddVectors(startPos, pos, FlipDirection(dir), length);
                continue;
            }
            var options = Deltas
                .Select(d => pos + d)
                .Where(p => p.y > 0 && map[p.y][p.x] != '#' && p != startPos)
                .ToArray();
            if (options.Length > 1)
            {
                map[pos.y][pos.x] = 'x';
                AddVectors(startPos, pos, dir, length);
                foreach (var p in options)
                    if (!vectors.ContainsKey(p))
                        queue.Push((p, pos, Direction.Both, 1));
            }
            else if (options.Length == 1)
            {
                var p = options[0];
                int slideIndex = Array.IndexOf(Slides, map[pos.y][pos.x]);
                if (slideIndex >= 0)
                    dir &= (Deltas[slideIndex] == p - pos) ? Direction.Forward : Direction.Backward;
                map[pos.y][pos.x] = '#';
                if (options[0] == end)
                    AddVectors(startPos, p, Direction.Both, length + 1);
                else
                    queue.Push((p, startPos, dir, length + 1));
            }
        }
        return vectors;


        void AddVectors(V2 start, V2 end, Direction dir, int length)
        {
            AddVector(start, end, dir, length);
            AddVector(end, start, FlipDirection(dir), length);
        }

        void AddVector(V2 start, V2 end, Direction dir, int length)
        {
            if (vectors.TryGetValue(start, out var hashSet))
                hashSet.Add(end, (length, dir));
            else
                vectors.Add(start, new Dictionary<V2, (int, Direction)> { { end, (length, dir) } });
        }
    }

    static Direction FlipDirection(Direction dir)
        => dir switch { Direction.Forward => Direction.Backward, Direction.Backward => Direction.Forward, _ => dir };
    
    int LongestPath(int theStartIndex, int theEndIndex, (int dest, int length, Direction dir)[][] vectorIndexes, int part)
    {
        var queue = new Queue<(int pos, long visited, int length)>();
        queue.Enqueue((theStartIndex, 1L << theStartIndex, 0));
        int minLength = int.MaxValue;
        int maxLength = 0;
        long count = 0;
        while (queue.Count > 0)
        {
            count++;
            var (pos, visited, length) = queue.Dequeue();
            if (pos == theEndIndex)
            {
                minLength = Math.Min(minLength, length);
                maxLength = Math.Max(maxLength, length);
                continue;
            }
            foreach (var (dst, len, dir) in vectorIndexes[pos])
            {
                if ((visited & (1L << dst)) == 0 && (part == 2 || dir.HasFlag(Direction.Forward)))
                    queue.Enqueue((dst, visited | (1L << dst), length + len));
            }
        }
        return maxLength;
    }

    public record V2(int x, int y)
    {
        public static V2 operator -(V2 a, V2 b) => new V2(a.x - b.x, a.y - b.y);
        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
    }
}