namespace AdventOfCode2022;

public class Day16 : IAdvent
{
    public record Valve(int Id, int Rate, int[] LeadTo);

    Dictionary<string, int> stringToId = new();
    int GetIdForString(string val)
    {
        if (!stringToId.TryGetValue(val, out int result))
        {
            result = stringToId.Count;
            stringToId[val] = result;
        }
        return result;
            
    }

    public void Run()
    {
        GetIdForString("AA");
        var input = Advent.ReadInputLines()
            .Select(line =>
            {
                var m = Regex.Match(line, @"Valve (\w\w).*rate=(\d+).*valves? (.*)");
                Valve valve = new Valve(GetIdForString(m.Groups[1].Value), int.Parse(m.Groups[2].Value), m.Groups[3].Value.Split(", ").Select(GetIdForString).ToArray());
                return valve;
            })
            .ToDictionary(a => a.Id);

        var fromToPath = BuildPaths(input.Keys);
        var unopened = ImmutableArray.Create(input.Values
                .Where(v => v.Rate > 0)
                .OrderByDescending(v => v.Rate)
                .ToArray());
        

        int answer1 = DoIt2(unopened, new[] { 30, 0 }, new[] { 0, 0 });
        Advent.AssertAnswer1(answer1, expected: 1792, sampleExpected: 1651);


        int answer2;
        if (!Advent.UseSampleData)
            answer2 = UsePossiblePaths(unopened, 0, 26); // much faster, but does not work with sample data
        else
            answer2 = DoIt2(unopened, new[] { 26, 26}, new[] {0, 0});
        Advent.AssertAnswer2(answer2, expected: 2587, sampleExpected: 1707);


        int DoIt2(ImmutableArray<Valve> unopened, int[] minutesLeft, int[] cur)
        {
            if (unopened.Length == 0)
                 return 0;

            int best = 0;

            int i = minutesLeft[0] >= minutesLeft[1] ? 0 : 1;
            foreach (var to in unopened)
            {
                var dest = fromToPath[(cur[i], to.Id)];

                int newMinutesLeft = Math.Max(minutesLeft[i] - dest  - 1, 0);
                var minAr = new int[] { newMinutesLeft, minutesLeft[1 - i] };
                var newCur = new int[] { to.Id, cur[1 - i] };
                var newUnopened = unopened.Remove(to);

                int score = Math.Max(newMinutesLeft, 0) * to.Rate;
                if (newMinutesLeft > 1 || minutesLeft[1 - i] > 1)
                    score += DoIt2(newUnopened, minAr, newCur);

                best = Math.Max(score, best);
            }
            return best;
        }

        int UsePossiblePaths(ImmutableArray<Valve> unopened, int cur, int minutesLeft)
        {
            var list = new List<(ImmutableHashSet<int> s, int total)>();

            DoIt(ImmutableHashSet.Create<int>(), 0, unopened, cur, minutesLeft);

            int answer1 = list.Select(l => l.Item2).Max();


            var bitmapped = list.Select(item => (s: item.s.Select(n => 1L << n).Sum(), total: item.total))
                .OrderByDescending(item => item.total)
                .Take(list.Count / 1000)
                .ToArray();
            int max = 0;
            for (int i = 0; i < bitmapped.Length - 1; i++)
            {
                var s1 = bitmapped[i].s;
                for (int j = i + 1; j < bitmapped.Length; j++)
                {
                    var s2 = bitmapped[j].s;
                    if ((s1 & s2) == 0)
                        max = Math.Max(max, bitmapped[i].total + bitmapped[j].total);
                }
            }
            return max;

            void DoIt(ImmutableHashSet<int> path, int total, ImmutableArray<Valve> unopened, int cur, int minutesLeft)
            {
                bool maxedOut = true;
                foreach (var to in unopened)
                {
                    var dest = fromToPath[(cur, to.Id)];

                    int newMinutesLeft = minutesLeft - dest - 1;
                    if (newMinutesLeft >= 1)
                    {
                        var newCur = to.Id;
                        var newUnopened = unopened.Remove(to);
                        var newPath = path.Add(newCur);

                        DoIt(newPath, total + newMinutesLeft * to.Rate, newUnopened, newCur, newMinutesLeft);
                        maxedOut = false;
                    }
                }
                if (maxedOut)
                    list.Add((path, total));
            }
        }

        Dictionary<(int, int), int> BuildPaths(IEnumerable<int> keys)
        {
            var fromToPath = new Dictionary<(int, int), int>();
            Build(keys.Select(p => new int[] {p}));

            return fromToPath;


            void Build(IEnumerable<int[]> paths)
            {
                var toAdd = new List<int[]>();
                foreach (int[] path  in paths)
                {
                    int from = path[0];
                    foreach (int to in input[path[^1]].LeadTo)
                    {
                        if (fromToPath.ContainsKey((from, to)) || from == to)
                            continue;

                        fromToPath.Add((from, to), path.Length);
                        toAdd.Add(path.Concat(new[] { to }).ToArray());
                    }
                }
                if (toAdd.Count > 0)
                    Build(toAdd);
            }
        }
    }
}