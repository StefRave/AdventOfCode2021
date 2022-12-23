namespace AdventOfCode2022;

public class Day23 : IAdvent
{
    public record V2(int X, int Y)
    {
        public static V2 operator -(V2 a, V2 b) => new V2(a.X - b.X, a.Y - b.Y);
        public static V2 operator +(V2 a, V2 b) => new V2(a.X + b.X, a.Y + b.Y);
    }
    static V2[][] Nswe = new V2[][]
        {
            new V2[]{ new V2(-1, -1), new V2(0, -1), new V2(1, -1) },
            new V2[]{ new V2(-1, 1), new V2(0, 1), new V2(1, 1) },
            new V2[]{ new V2(-1, -1), new V2(-1, 0), new V2(-1, 1) },
            new V2[]{ new V2(1, -1), new V2(1, 0), new V2(1, 1) },
            new V2[]{ new V2(-1, -1), new V2(0, -1), new V2(1, -1) },
            new V2[]{ new V2(-1, 1), new V2(0, 1), new V2(1, 1) },
            new V2[]{ new V2(-1, -1), new V2(-1, 0), new V2(-1, 1) },
        };
    static V2[] Adjecent = new V2[]
        {
            new V2(-1, -1), new V2(0, -1), new V2(1, -1),
            new V2(-1, 1), new V2(0, 1), new V2(1, 1),
            new V2(-1, 0),
            new V2(1, 0),
        };
    public void Run()
    {
        var field = Advent.ReadInputLines()
            .Select((line, y) => line.Select((c, x) => (p: new V2(x, y), c)))
            .SelectMany(a => a)
            .Where(p => p.c == '#')
            .Select(p => p.p)
            .ToHashSet();

        int iteration;
        int directionIndex = 0;
        bool relativePositionsChanged = false;

        for (iteration = 0; iteration < 10; iteration++)
            (field, relativePositionsChanged) = DoIteration(field, directionIndex++);

        int minX = field.Select(m => m.X).Min();
        int maxX = field.Select(m => m.X).Max();
        int minY = field.Select(m => m.Y).Min();
        int maxY = field.Select(m => m.Y).Max();
        int answer1 = (maxX - minX + 1) * (maxY - minY + 1) - field.Count;
        Advent.AssertAnswer1(answer1, expected: 4082, sampleExpected: 110);


        for (; relativePositionsChanged; iteration++)
            (field, relativePositionsChanged) = DoIteration(field, directionIndex++);
        Advent.AssertAnswer2(iteration, expected: 1065, sampleExpected: 20);
    }

    private static (HashSet<V2> newField, bool relativePositionsChanged) DoIteration(HashSet<V2> field, int directionIndex)
    {
        bool relativePositionsChanged = false;
        Dictionary<V2, V2> newField = new();
        V2 lastMove = null;
        foreach (V2 elve in field)
        {
            bool alone = Adjecent.All(c => !field.Contains(elve + c));
            if (alone)
            {
                newField.Add(elve, elve);
                continue;
            }
            V2[] move = Nswe.Skip(directionIndex % 4).Take(4).FirstOrDefault(l => l.All(c => !field.Contains(elve + c)));
            if (move == null)
            {
                newField.Add(elve, elve);
                continue;
            }
            newField.Add(elve, elve + move[1]);
            if (lastMove == null)
                lastMove = move[1];
            else if (lastMove != move[1])
                relativePositionsChanged = true;
        }

        bool hasDuplicates;
        do
        {
            var duplicates = newField.GroupBy(k => k.Value).Where(g => g.Count() > 1);
            hasDuplicates = false;
            foreach (var kv in duplicates)
            {
                hasDuplicates = true;
                foreach (var oldPos in kv.Select(kv => kv.Key))
                {
                    newField.Remove(oldPos);
                    newField.Add(oldPos, oldPos);
                }
            }
        }
        while (hasDuplicates);


        return (new HashSet<V2>(newField.Values), relativePositionsChanged);
    }
}