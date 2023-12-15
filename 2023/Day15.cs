namespace AdventOfCode2023;

public class Day15 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()[0].Split(',');

        int answer1 = input.Sum(Hash);
        Advent.AssertAnswer1(answer1, expected: 513643, sampleExpected: 1320);

        var boxes = Enumerable.Range(0, 256).Select(i => new List<(string seq, int num)>()).ToArray();
        foreach (var sequence in input)
        {
            var si = sequence.IndexOfAny(['-', '=']);
            var seq = sequence[0..si];
            var op = sequence[si++];
            var num = int.Parse("0" + sequence[si..]);
            int currentValue = Hash(seq);
            if (op == '=')
            {
                var found = boxes[currentValue].Select((b, i) => (b, i)).FirstOrDefault(b => b.b.seq == seq);
                if (found.b.seq == null)
                    boxes[currentValue].Add((seq, num));
                else
                    boxes[currentValue][found.i] = (seq, num);
            }
            else
                boxes[currentValue].RemoveAll(b => b.seq == seq);
        }
        long answer2 = 0;
        for (int i = 0; i < boxes.Length; i++)
            for (int j = 0; j < boxes[i].Count; j++)
                answer2 += (j + 1) * boxes[i][j].num * ((long)i + 1);
        Advent.AssertAnswer2(answer2, expected: 265345, sampleExpected: 145);
    }

    private static int Hash(string sequence)
        => sequence.Aggregate(0, (a, c) => ((a + c) * 17) % 256);
}
