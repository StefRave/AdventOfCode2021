namespace AdventOfCode2022;

public class Day11 : IAdvent
{
    public class Monkey
    {
        public int Id;
        public List<long> Items;
        public char OperationType;
        public string OperationRight;
        public int DivisableBy;
        public int ThrowTrueMonkey;
        public int ThrowFalseMonkey;
        public int Inspections;
    }

    public void Run()
    {
        var answer1 = DoIt(ParseInput(), div3: true, rounds: 20);
        Advent.AssertAnswer1(answer1, expected: 56595, sampleExpected: 10605);


        var answer2 = DoIt(ParseInput(), div3: false, rounds: 10000);
        Advent.AssertAnswer2(answer2, expected: 15693274740, sampleExpected: 2713310158);
    }

    private static Dictionary<int, Monkey> ParseInput()
    {
        return Advent.ReadInput()
            .SplitByDoubleNewLine()
            .Select(mi =>
            {
                string[] ar = mi.SplitByNewLine();
                var m = new Monkey
                {
                    Id = int.Parse(Regex.Match(ar[0], @"\d+").Value),
                    Items = Regex.Matches(ar[1], @"\d+").Select(a => long.Parse(a.Value)).ToList(),
                    OperationType = Regex.Match(ar[2], @"= old (.)").Groups[1].Value[0],
                    OperationRight = Regex.Match(ar[2], @"= old . (\w+)").Groups[1].Value,
                    DivisableBy = int.Parse(Regex.Match(ar[3], @"\d+").Value),
                    ThrowTrueMonkey = int.Parse(ar[4][^2..]),
                    ThrowFalseMonkey = int.Parse(ar[5][^2..]),
                };
                return m;

            })
            .ToDictionary(m => m.Id);
    }

    private static long DoIt(Dictionary<int, Monkey> monkeys, bool div3, int rounds)
    {
        long divTot = monkeys.Values.Select(m => (long)m.DivisableBy).Aggregate((a, b) => a * b);

        for (int round = 1; round <= rounds; round++)
        {
            for (int mi = 0; mi < monkeys.Count; mi++)
            {
                var m = monkeys[mi];
                foreach (var item in m.Items)
                {
                    long right = (m.OperationRight == "old") ? item : int.Parse(m.OperationRight);
                    long worry = m.OperationType switch
                    {
                        '+' => right + item,
                        '*' => right * item,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    long worryDiv = div3 ? worry / 3 : worry % divTot;
                    bool isDisable = (worryDiv % m.DivisableBy) == 0;
                    int throwTo = isDisable ? m.ThrowTrueMonkey : m.ThrowFalseMonkey;
                    monkeys[throwTo].Items.Add(worryDiv);
                    m.Inspections++;
                }
                m.Items.Clear();
            }
        }

        return monkeys
            .Select(kv => (long)kv.Value.Inspections)
            .OrderByDescending(i => i)
            .Take(2)
            .Aggregate((a,b) => a * b);
    }
}