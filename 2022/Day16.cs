namespace AdventOfCode2022;

public class Day16 : IAdvent
{
    public record Valve(string Id, int Rate, string[] LeadTo);


    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line =>
            {
                var m = Regex.Match(line, @"Valve (\w\w).*rate=(\d+).*valves? (.*)");
                return new Valve(m.Groups[1].Value, int.Parse(m.Groups[2].Value), m.Groups[3].Value.Split(", "));
            })
            .ToDictionary(v => v.Id);

        var fromToPath = BuildPaths(input.Keys);
        var unopened = ImmutableArray.Create(input.Values
                .Where(v => v.Rate > 0)
                .OrderByDescending(v => v.Rate)
                .ToArray());

        int answer1 = DoIt2(unopened, new[] { 30, 0 }, new[] { "AA", "AA" });
        Advent.AssertAnswer1(answer1, expected: 1792, sampleExpected: 1651);


        int answer2 = DoIt2(unopened, new[] { 26, 26} , new[] { "AA", "AA" });
        Advent.AssertAnswer2(answer2, expected: 2587, sampleExpected: 1707);

        int DoIt2(ImmutableArray<Valve> unopened, int[] minutesLeft, string[] cur)
        {
            if (unopened.Length == 0)
                 return 0;

            int best = 0;

            int i = minutesLeft[0] >= minutesLeft[1] ? 0 : 1;
            if (minutesLeft[i] <= 1)
                throw new Exception();

            foreach (var to in unopened)
            {
                var dest = fromToPath[cur[i] + to.Id];

                int newMinutesLeft = Math.Max(minutesLeft[i] - dest.Length / 2  - 1, 0);
                var minAr = new int[] { newMinutesLeft, minutesLeft[1 - i] };
                var newCur = new string[] { to.Id, cur[1 - i] };
                var newUnopened = unopened.Remove(to);

                int score = Math.Max(newMinutesLeft, 0) * to.Rate;
                if (newMinutesLeft > 1 || minutesLeft[1 - i] > 1)
                    score += DoIt2(newUnopened, minAr, newCur);

                best = Math.Max(score, best);
            }
            return best;
        }

        Dictionary<string, string> BuildPaths(IEnumerable<string> keys)
        {
            var fromToPath = new Dictionary<string, string>();
            Build(keys);
            
            return fromToPath;


            void Build(IEnumerable<string> keys)
            {
                var toAdd = new List<string>();
                foreach (var currentPath in keys)
                {
                    string from = currentPath[..2];
                    foreach (string to in input[currentPath[^2..]].LeadTo)
                    {
                        if (fromToPath.ContainsKey(from + to) || from == to)
                            continue;

                        fromToPath.Add(from + to, currentPath[2..] + to);
                        toAdd.Add(currentPath + to);
                    }
                }
                if (toAdd.Count > 0)
                    BuildPaths(toAdd);
            }
        }
    }
}