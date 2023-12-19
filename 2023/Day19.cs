namespace AdventOfCode2023;

public class Day19 : IAdvent
{
    record Range(int Min, int Max);
    record Rule(char C, char Op, int Count, string To)
    {
        public static Rule[] FromStrings(string s)
            => s.Split(',').Select(FromString).ToArray();

        public static Rule FromString(string s)
        {
            var m = Regex.Match(s, @"^(\w+)([><])(\d+):(\w+)").Groups.AsArray();
            return new Rule(m[1][0], m[2][0], int.Parse(m[3]), m[4]);
        }
    }
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();
        // example: px{a<2006:qkq,m>2090:A,rfg}
        Dictionary<string, (string last, Rule[] comp)> workflows = input[0].SplitByNewLine()
            .Select(line => Regex.Match(line, @"^(\w+){(.*),(\w+)}$"))
            .ToDictionary(m => m.Groups[1].Value, m => (last: m.Groups[3].Value, comp: Rule.FromStrings(m.Groups[2].Value)));

        // example: {x=787,m=2655,a=1222,s=2876}
        Dictionary<char, int>[] ratings = input[1].SplitByNewLine()
            .Select(line => 
                line[1..^1].Split(',')
                .Select(r => r.Split('='))
                .ToDictionary(s => s[0][0], s => int.Parse(s[1]))
            ).ToArray();

        long answer1 = ratings.Where(r => ShouldAccept(r, "in")).Sum(r => r.Sum(r => r.Value));
        Advent.AssertAnswer1(answer1, expected: 386787, sampleExpected: 19114);

        var r = new Range(1, 4000);
        long answer2 = CountOptions([r, r, r, r], "in");
        Advent.AssertAnswer2(answer2, expected: 131029523269531, sampleExpected: 167409079868000);


        long CountOptions(Range[] ranges, string variable)
        {
            if (variable == "R")
                return 0;
            if (variable == "A")
                return ranges.Aggregate(1L, (a, r) => a * (r.Max - r.Min + 1));

            long count = 0;
            var workflow = workflows[variable];
            foreach (var comp in workflow.comp)
            {
                int index = comp.C switch {'x' => 0, 'm' => 1, 'a' => 2, 's' => 3};
                var r = ranges[index];
                var (oMin, oMax) = (comp.Op == '<') ? (1, comp.Count - 1) : (comp.Count + 1, 4000);
                var nr = new Range(Math.Max(r.Min, oMin), Math.Min(r.Max, oMax));
                if (nr.Min <= nr.Max)
                {
                    var rs = ranges.ToArray();
                    rs[index] = nr;
                    count += CountOptions(rs, comp.To);
                }
                ranges[index] = (r.Min < nr.Min)
                    ? new Range(r.Min, Math.Min(nr.Min - 1, r.Max))
                    : new Range(Math.Max(nr.Max + 1, r.Min), r.Max);
            }
            return count + CountOptions(ranges, workflow.last);
        }

        bool ShouldAccept(Dictionary<char, int> rating, string variable)
        {
            if (variable == "R")
                return false;
            if (variable == "A")
                return true;
            var workflow = workflows[variable];

            foreach (var comp in workflow.comp)
                if ((comp.Op == '<' && rating[comp.C] < comp.Count) ||
                    (comp.Op == '>' && rating[comp.C] > comp.Count))
                    return ShouldAccept(rating, comp.To);

            return ShouldAccept(rating, workflow.last);
        }
    }
}
