namespace AdventOfCode2023;

public class Day07 : IAdvent
{
    class Hand
    {
        private string card;
        public string Cards { get => card; set { card = value; SortValue1 = CalcSortValue(value, part: 1); SortValue2 = CalcSortValue(value, part: 2); } }
        public int BidAmount { get; set; }
        public string SortValue1 { get; private set; }
        public string SortValue2 { get; private set; }

        private string CalcSortValue(string value, int part)
        {
            var cardCount = value.GroupBy(c => c).ToDictionary(c => c.Key, c => c.Count());
            cardCount.TryGetValue('J', out int jokers);
            if (part == 2 && jokers > 0 && jokers != 5)
                cardCount.Remove('J');
            var countOrder = cardCount.Select(kv => kv.Value).OrderByDescending(c => c).ToArray();
            if (part == 2 && jokers != 5)
                countOrder[0] += jokers;
            var sv = new char[6];
            sv[0] = (countOrder[0], countOrder.Skip(1).FirstOrDefault()) switch
            {
                (5, _) => '6',
                (4, _) => '5',
                (3, 2) => '4',
                (3, _) => '3',
                (2, 2) => '2',
                (2, _) => '1',
                _ => '0',
            };
            for (int i = 0; i < value.Length; i++)
            {
                sv[i + 1] = value[i] switch
                {
                    'A' => 'E',
                    'K' => 'D',
                    'Q' => 'C',
                    'J' => part == 2 ? '!' : 'B',
                    'T' => 'A',
                    _ => value[i]
                };
            }
            return new string(sv);
        }
    }

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line =>
            {
                var split = line.Split(' ');
                return new Hand { Cards = split[0], BidAmount = int.Parse(split[1]) };
            })
            .ToArray();

        long answer1 = input.OrderBy(h => h.SortValue1).Select((h, i) => (i + 1L) * h.BidAmount).Sum();
        Advent.AssertAnswer1(answer1, expected: 253954294, sampleExpected: 6440);

        long answer2 = input.OrderBy(h => h.SortValue2).Select((h, i) => (i + 1L) * h.BidAmount).Sum();
        Advent.AssertAnswer2(answer2, expected: 254837398, sampleExpected: 5905);
    }
}
