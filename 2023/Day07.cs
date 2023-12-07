namespace AdventOfCode2023;

public class Day07 : IAdvent
{
    class Hand
    {
        private string card;
        public string Cards { get => card; set { card = value; SortValue1 = CalcSortValue1(value); SortValue2 = CalcSortValue2(value); } }
        public string SortValue1 { get; private set; }
        public string SortValue2 { get; private set; }

        private string CalcSortValue1(string value)
        {
            char[] cards = value.ToCharArray();
            Array.Sort(cards);
            var ofAKind = new List<int>();
            char current = '\0';
            int countCurrent = 0;
            foreach (var c in cards)
            {
                if (current == c)
                    countCurrent++;
                else
                {
                    if (countCurrent > 0)
                        ofAKind.Add(countCurrent);
                    countCurrent = 1;
                }
                current = c;
            }
            ofAKind.Add(countCurrent);
            ofAKind.Add(0);
            ofAKind.Sort((a, b) => b.CompareTo(a));

            var sv = new char[6];
            if (Cards == "KKAAK")
                1.ToString();
            sv[0] = (ofAKind[0], ofAKind[1]) switch
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
                    'J' => 'B',
                    'T' => 'A',
                    _ => value[i]
                };
            }
            return new string(sv);
        }
        
        private string CalcSortValue2(string value)
        {
            char[] cards = value.ToCharArray();
            Array.Sort(cards);
            var ofAKind = new List<int>();
            char current = '\0';
            int countCurrent = 0;
            int jokers = 0;
            foreach (var c in cards)
            {
                if (c == 'J')
                {
                    if (countCurrent > 0)
                        ofAKind.Add(countCurrent);
                    countCurrent = 0;
                    jokers++;
                }
                else
                {
                    if (current == c)
                        countCurrent++;
                    else
                    {
                        if (countCurrent > 0)
                            ofAKind.Add(countCurrent);
                        countCurrent = 1;
                    }
                }
                current = c;
            }
            if (current != 'J')
                ofAKind.Add(countCurrent);
            while (ofAKind.Count < 2)
                ofAKind.Add(0);
            ofAKind.Sort((a, b) => b.CompareTo(a));
            ofAKind[0] += jokers;

            var sv = new char[6];
            sv[0] = (ofAKind[0], ofAKind[1]) switch
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
                    'J' => '!',
                    'T' => 'A',
                    _ => value[i]
                };
            }
            return new string(sv);
        }

        public int BidAmount { get; set;  }
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

        Array.Sort(input, (a, b) => a.SortValue1.CompareTo(b.SortValue1));
        long answer1 = 0;
        for (int i = 0; i < input.Length; i++)
            answer1 += (i + 1) * input[i].BidAmount;
        Advent.AssertAnswer1(answer1, expected: 253954294, sampleExpected: 6440);

        Array.Sort(input, (a, b) => a.SortValue2.CompareTo(b.SortValue2));
        long answer2 = 0;
        for (int i = 0; i < input.Length; i++)
            answer2 += (i + 1) * input[i].BidAmount;
        Advent.AssertAnswer2(answer2, expected: 254837398, sampleExpected: 5905);
    }
}
