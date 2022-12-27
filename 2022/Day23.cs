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
        for (iteration = 0; iteration < 10; iteration++)
            (field, _) = DoIteration(field, iteration);

        int minX = field.Select(m => m.X).Min();
        int maxX = field.Select(m => m.X).Max();
        int minY = field.Select(m => m.Y).Min();
        int maxY = field.Select(m => m.Y).Max();
        int answer1 = (maxX - minX + 1) * (maxY - minY + 1) - field.Count;
        Advent.AssertAnswer1(answer1, expected: 4082, sampleExpected: 110);

        bool moveMade = true;
        while (moveMade)
            (field, moveMade) = DoIteration(field, iteration++);
        Advent.AssertAnswer2(iteration, expected: 1065, sampleExpected: 20);
    }

    private static (HashSet<V2> newField, bool noMove) DoIteration(HashSet<V2> field, int directionIndex)
    {
        bool allAlone = true;
        Dictionary<V2, V2> newField = new();
        foreach (V2 elve in field)
        {
            bool alone = Adjecent.All(c => !field.Contains(elve + c));
            if (alone)
            {
                newField.Add(elve, elve);
                continue;
            }
            allAlone = false;
            V2[] move = Nswe.Skip(directionIndex % 4).Take(4).FirstOrDefault(l => l.All(c => !field.Contains(elve + c)));
            if (move == null)
                newField.Add(elve, elve);
            else
                newField.Add(elve, elve + move[1]);
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


        return (new HashSet<V2>(newField.Values), !allAlone);
    }
}