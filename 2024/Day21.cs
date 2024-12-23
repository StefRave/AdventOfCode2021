using CommandLine;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2024;

public class Day21 : IAdvent
{
    public void Run()
    {
        string[] keypad1 = "789\n456\n123\n 0A".SplitByNewLine();
        string[] keypad2 = " ^A\n<v>".SplitByNewLine();
        //string[] len1 = ["A03", "0A2", "142", "21530", "36"
        var input = Advent.ReadInput().SplitByNewLine();

        var keypad1Lengths = GetKeyMovements(keypad1);
        var keypad2Lengths = GetKeyMovements(keypad2);

        long answer1 = 0;
        foreach (var line in input)
        {
            long solutionLength = Solve(line, 2);
            answer1 += long.Parse(line[0..^1]) * solutionLength;
        }
        Advent.AssertAnswer1(answer1, expected: 157908, sampleExpected: 126384);
       
        long answer2 = 0;
        foreach (var line in input)
        {
            long solutionLength = Solve(line, 25);
            answer2 += long.Parse(line[0..^1]) * solutionLength;
        }
        Advent.AssertAnswer2(answer2, expected: 196910339808654, sampleExpected: 154115708116294);


        long Solve(string line, int iterations)
        {
            Movements[] solution1s = CalculateKeyPressSequence(line, keypad1Lengths).ToArray();
            Console.WriteLine($"\n{line}");
            Console.WriteLine($"solutions:{solution1s.Length}");
            solution1s = Movements.RemoveLargeResults(solution1s);
            Console.WriteLine($"solutions:{solution1s.Length} after cleanup");

            var z = solution1s;
            int count = 0;
            for (int i = 0; i < iterations; i++)
            {
                z = Movements.ReturnBest(DoIt(z));
                count = z.Length;
                Console.WriteLine($"{line} {i} {z.Length}");
            }
            
            
            return z[0].Length;


        }

        Movements[] DoIt(Movements[] solution1s)
        {
            var solution2s = new List<Movements>();
            foreach (var solution in solution1s)
            {
                var newSolutions = new List<Movements>([new Movements()]);
                foreach (var (k, v) in solution.Dict)
                {
                    Movements[] tmp = CalculateKeyPressSequence(k, keypad2Lengths).ToArray();
                    Movements[] oldSolutionsList = newSolutions.ToArray();
                    foreach (var hoi in tmp.Skip(1))
                    {
                        foreach (Movements m in oldSolutionsList)
                        {
                            newSolutions.Add(m.CopyWith(hoi, v));
                        }
                    }
                    foreach (Movements m in oldSolutionsList)
                    {
                        m.With(tmp[0], v);
                    }
                }
                solution2s.AddRange(newSolutions);
            }
            return solution2s.ToArray();
        }
    }

    private static Dictionary<(char, char), string[]> GetKeyMovements(string[] keypad)
    {
        string keys = string.Join("", keypad);
        var dict = new Dictionary<(char, char), string[]>();
        foreach (char a in keys)
            foreach (char b in keys)
            {
                var p1 = Pos(keypad, a);
                var p2 = Pos(keypad, b);
                var presses = "";
                if (p2.y > p1.y)
                    presses = "".PadLeft(p2.y - p1.y, 'v');
                else
                    presses = "".PadLeft(p1.y - p2.y, '^');
                if (p2.x > p1.x)
                    presses += "".PadLeft(p2.x - p1.x, '>');
                else
                    presses += "".PadLeft(p1.x - p2.x, '<');
                var solutions = new List<string>();
                if (keypad[p2.y][p1.x] != ' ')
                    solutions.Add(presses);
                if (keypad[p1.y][p2.x] != ' ' && p1 != p2 && presses.Length > 0 && presses[0] != presses[^1])
                    solutions.Add(new string(presses.Reverse().ToArray()));
                dict.Add((a, b), solutions.ToArray());
            }

        (int x, int y) Pos(string[] keyboard, char c)
        {
            for (int y = 0; y < keyboard.Length; y++)
                for (int x = 0; x < keyboard[0].Length; x++)
                    if (c == keyboard[y][x])
                        return (x, y);
            throw new Exception();
        }
        return dict;
    }
    Dictionary<string, List<Movements>> CalculateKeyPressSequenceCache = new();

    IEnumerable<Movements> CalculateKeyPressSequence(string line, Dictionary<(char, char), string[]> keys)
    {
        if (CalculateKeyPressSequenceCache.TryGetValue(line, out var result))
            return result;
        result = new List<Movements>();
        var values = Calculate(line, 'A', '\0');
        foreach (var segmentList in values)
        {
            var dict = new Movements();
            foreach (var segment in segmentList)
                dict.Add(segment, 1);

            result.Add(dict);
        }
        CalculateKeyPressSequenceCache.Add(line, result);
        return result;

        IEnumerable<IEnumerable<string>> Calculate(string line, char current, char lastPresses)
        {
            var c = line[0];
            string[] toPresses = keys[(current, c)];
            if (line.Length == 1)
            {
                yield return [toPresses[0] + 'A'];
                yield break;
            }
            foreach (var toPress in toPresses)
            {
                if (line.Length == 1)
                    yield return [toPress + 'A'];
                else
                {
                    foreach (var e in Calculate(line[1..], c, toPress.Length > 0 ? toPress[^1] : lastPresses))
                    {

                        var l = new List<string>();
                        l.Add(toPress + "A");
                        l.AddRange(e);
                        yield return l;
                    }
                }
            }
        }
    }


    [DebuggerDisplay("Length:{Length} Items:{Dict.Count}")]
    public class Movements
    {
        public Dictionary<string, long> Dict { get; }
        public long Length => Dict.Sum(v => v.Key.Length * v.Value);

        public Movements(Dictionary<string, long> dict = null)
        {
            Dict = dict ?? new Dictionary<string, long>();
        }
        public static Movements[] RemoveLargeResults(Movements[] solutions)
        {
            long min = solutions.Min(s => s.Length);
            return solutions.Where(s => s.Length == min).ToArray();
        }

        public static Movements[] RemoveDuplicates(Movements[] solutions)
        {
            return solutions.Distinct(MovementsComparer.Instance).ToArray();
        }

        public static Movements[] ReturnBest(Movements[] solutions)
        {
            var s = RemoveLargeResults(solutions);
            return RemoveDuplicates(s);
        }

        public void Add(string segment, long count)
            => Dict.Update(segment, v => v + count);

        public Movements CopyWith(Movements tmp, long count)
        {
            var result = new Movements(Dict.ToDictionary());
            return result.With(tmp, count);
            throw new NotImplementedException();
        }

        public Movements With(Movements tmp, long count)
        {
            foreach (var (k, v) in tmp.Dict)
                Add(k, v * count);
            return this;
        }

        private class MovementsComparer : IEqualityComparer<Movements>
        {
            public static MovementsComparer Instance { get; } = new MovementsComparer();

            public bool Equals(Movements x, Movements y)
            {
                if (x.Length != y.Length)
                    return false;
                bool same = true;
                foreach (var (k, v) in x.Dict)
                {
                    if(!y.Dict.TryGetValue(k, out long value) || value != v)
                    {
                        same = false;
                        break;
                    }
                }
                return same;
            }

            public int GetHashCode([DisallowNull] Movements obj)
            {
                int hash = (int)obj.Length;
                foreach (var (k, v) in obj.Dict)
                    hash ^= k.GetHashCode() ^ v.GetHashCode();
                return hash;
            }
        }
    }
}
