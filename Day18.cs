using System.Text;

namespace AdventOfCode2021;

public class Day18
{
    private readonly ITestOutputHelper output;

    public Day18(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => Parse(line))
            .ToArray();

        var prevLine = Reduce(input[0]);
        foreach (var line in input.Skip(1))
            prevLine = AddLines(prevLine, Reduce(line));
        Advent.AssertAnswer1(Calc(prevLine));

        long max = 0;
        for (int i = 0; i < input.Length; i++)
            for (int j = 0; j < input.Length; j++)
                if (i != j)
                    max = Math.Max(max, Calc(AddLines(input[i], input[j])));
        Advent.AssertAnswer2(max);
    }

    private static List<NumLevel> AddLines(List<NumLevel> prevLine, List<NumLevel> line)
    {
        var addedLine = new List<NumLevel>();
        foreach (var numLevel in prevLine)
            addedLine.Add(new NumLevel(numLevel.Num, numLevel.Level + 1, numLevel.CloseAfter));
        foreach (var numLevel in line)
            addedLine.Add(new NumLevel(numLevel.Num, numLevel.Level + 1, numLevel.CloseAfter));
        addedLine[^1].CloseAfter++;
        Reduce(addedLine);
        return addedLine;
    }

    private static long Calc(List<NumLevel> parsedLine)
    {
        while (parsedLine.Count > 1)
        {
            bool modified = false;
            for (int i = 0; i < parsedLine.Count - 1; i++)
            {
                if (parsedLine[i].Level != parsedLine[i + 1].Level || parsedLine[i].CloseAfter != 0)
                    continue;

                if (i < parsedLine.Count - 1)
                {
                    parsedLine[i + 1].Num = parsedLine[i].Num  * 3 + parsedLine[i + 1].Num * 2;
                    parsedLine[i + 1].Level--;
                    parsedLine[i + 1].CloseAfter--;
                    parsedLine.RemoveAt(i);
                    modified = true;
                    break;
                }
            }
            if (!modified)
                throw new Exception("No modification");
        }
        return parsedLine[0].Num;
    }

    private static List<NumLevel> Reduce(List<NumLevel> parsedLine)
    {
        while (true)
        {
            bool modified = false;
            for (int i = 0; i < parsedLine.Count; i++)
            {
                if (parsedLine[i].Level == 5)
                {
                    if (parsedLine[i].CloseAfter != 0)
                        throw new Exception();

                    if (i > 0)
                        parsedLine[i - 1].Num += parsedLine[i].Num;
                    if (i < parsedLine.Count - 2)
                        parsedLine[i + 2].Num += parsedLine[i + 1].Num;
                    parsedLine.RemoveAt(i);
                    parsedLine[i].Level--;
                    parsedLine[i].Num = 0;
                    parsedLine[i].CloseAfter--;
                    modified = true;
                    break;
                }
            }
            if (!modified)
            {
                for (int i = 0; i < parsedLine.Count; i++)
                {
                    long num = parsedLine[i].Num;
                    if (num >= 10)
                    {
                        parsedLine[i].Level++;
                        parsedLine[i].CloseAfter++;
                        parsedLine[i].Num = (num + 1) / 2;
                        parsedLine.Insert(i, new NumLevel(num / 2, parsedLine[i].Level));
                        modified = true;
                        break;
                    }
                }
            }
            if (!modified)
                break;
        }
        return parsedLine;
    }

    private void Print(List<NumLevel> parsedLine)
    {
        var sb = new StringBuilder();
        int level = 0;
        foreach (var numLevel in parsedLine)
        {
            if (sb.Length > 0 && (char.IsDigit(sb[^1]) || sb[^1] == ']'))
                sb.Append(',');
            while (level < numLevel.Level)
            {
                sb.Append('[');
                level++;
            }
            sb.Append(numLevel.Num);
            for (int i = 0; i < numLevel.CloseAfter; i++)
            {
                sb.Append(']');
                level--;
            }
        }
        output.WriteLine(sb.ToString());
    }

    private static List<NumLevel> Parse(string line)
    {
        var parsedLine = new List<NumLevel>();
        NumLevel last = null;
        int level = 0;
        foreach (char c in line)
        {
            switch (c)
            {
                case '[': level++; break;
                case ']': level--; last.CloseAfter++; break;
                case ',': break;
                default: last = new NumLevel(c - '0', level); parsedLine.Add(last); break;
            }
        }

        return parsedLine;
    }

    public class NumLevel
    {
        public long Num { get; set; }
        public int Level { get; set; }
        public int CloseAfter { get; set; }

        public NumLevel(long num, int level, int closeAfter = 0)
        {
            Num = num;
            Level = level;
            CloseAfter = closeAfter;
        }
    }
}
