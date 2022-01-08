namespace AdventOfCode2018;

public class Day14 : IAdvent
{
    public void Run()
    {
        int inputInt = 681901;
        byte[] input = inputInt.ToString().Select(c => (byte)(c - '0')).ToArray();

        var list = new List<byte>();
        list.Add(3);
        list.Add(7);
        int index1 = 0;
        int index2 = 1;

        string answer1 = null;
        int answer2 = 0;
        for (int i = 0; answer1 == null || answer2 == 0; i++)
        {
            if (i == inputInt + 10)
                answer1 = list.Skip(inputInt).Take(10).Aggregate("", (x, b) => x + b);

            int reciepe = list[index1] + list[index2];
            if (reciepe >= 10)
                list.Add((byte)(reciepe / 10));
            list.Add((byte)(reciepe % 10));

            index1 = (index1 + 1 + list[index1]) % list.Count;
            index2 = (index2 + 1 + list[index2]) % list.Count;
            if (index1 == index2)
                throw new Exception("huh?");


            if (list.Count > 6)
            {
                int? index = SequenceEqual(input, list, list.Count - input.Length) ??
                    (reciepe < 10 ? null : SequenceEqual(input, list, list.Count - input.Length - 1));
                if (index.HasValue)
                    answer2 = index.Value;
            }
        }
        Advent.AssertAnswer1(answer1, expected: 1617111014);
        Advent.AssertAnswer2(answer2, expected: 20321495);
    }

    public int? SequenceEqual(byte[] a, List<byte> b, int index)
    {
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i + index])
                return null;
        return index;
    }
}