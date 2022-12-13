namespace AdventOfCode2022;

public class Day13 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .Select(line => line.SplitByNewLine())
            .ToArray();
        int answer1 = 0;
        for (int i = 0; i < input.Length; i++)
            if(ParseAndCompare(input[i][0], input[i][1]) <= 0)
                answer1 += i + 1;
        Advent.AssertAnswer1(answer1, expected: 6072, sampleExpected: 13);

        var input2 = input.SelectMany(a => a).Concat(new[] { "[[2]]", "[[6]]" }).ToList();
        input2.Sort(ParseAndCompare);
        int i1 = input2.IndexOf("[[2]]") + 1;
        int i2 = input2.IndexOf("[[6]]") + 1;
        Advent.AssertAnswer2(i1 * i2, expected: 22184, sampleExpected: 140);
    }

    private int ParseAndCompare(string v1, string v2)
    {
        var s1= Split(v1);
        var s2= Split(v2);
        return Comparer(s1, s2);
    }

    private int Comparer(object[] s1, object[] s2)
    {
        for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
        {
            int comp;
            if (s1[i] is int && s2[i] is int)
                comp = ((int)s1[i]).CompareTo((int)s2[i]);
            else
            {
                var l1 = s1[i] as object[] ?? new object[] { s1[i] };
                var l2 = s2[i] as object[] ?? new object[] { s2[i] };
                comp = Comparer(l1, l2);
            }
            if (comp != 0)
                return comp;
        }
        return s1.Length.CompareTo(s2.Length);
    }

    private object[] Split(string inp)
    {
        int i = 0;
        return SplitInner();

        object[] SplitInner()
        {
            var result = new List<object>();
            while (++i < inp.Length)
            {
                if (char.IsDigit(inp[i]))
                {
                    int start = i;
                    while (char.IsDigit(inp[++i]))
                        ;
                    result.Add(int.Parse(inp[start..i]));
                }
                if (inp[i] == '[')
                    result.Add(SplitInner());
                else if (inp[i] == ']')
                    return result.ToArray();
            }
            throw new Exception("unexpected end");
        }
    }
}