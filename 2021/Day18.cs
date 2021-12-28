using System.Text;

namespace AdventOfCode2021;

public class Day18 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => Reduce(Parse(line)))
            .ToArray();

        var line = input.Aggregate((line1, line2) => AddAndReduce(line1, line2));
        Advent.AssertAnswer1(Calc(line));

        long max = input.Select(line1 => 
            input.Select(line2 => Calc(AddAndReduce(line1, line2))).Max()).Max();
        Advent.AssertAnswer2(max);
    }

    private static List<NumLevel> AddAndReduce(List<NumLevel> lines1, List<NumLevel> lines2)
    {
        var result = lines1.Concat(lines2)
            .Select(numLevel => new NumLevel(numLevel.Num, numLevel.Level + 1, numLevel.CloseAfter))
            .ToList();
        result[^1].CloseAfter++;
        return Reduce(result);
    }

    private static long Calc(List<NumLevel> list)
    {
        while (list.Count > 1)
            for (int i = 0; i < list.Count - 1; i++)
                if (list[i].IsPairWith(list[i + 1]))
                    SumAndReplaceItemsAtIndex(i);
        return list[0].Num;

        void SumAndReplaceItemsAtIndex(int i)
        {
            list[i] = new NumLevel(list[i].Num * 3 + list[i + 1].Num * 2, list[i + 1].Level - 1, list[i + 1].CloseAfter - 1);
            list.RemoveAt(i + 1);
        }
    }

    private static List<NumLevel> Reduce(List<NumLevel> parsedLine)
    {
        DoExplodes();
        if (TrySplit())
            return Reduce(parsedLine);
        return parsedLine;


        void DoExplodes()
        {
            for (int i = 0; i < parsedLine.Count; i++)
                if (parsedLine[i].Level == 5)
                    Explode(i);
        }

        bool TrySplit()
        {
          for (int i = 0; i < parsedLine.Count; i++)
                if (parsedLine[i].Num >= 10)
                {
                    Split(i);
                    return true;
                }
            return false;
        }

        void Explode(int i)
        {
            if (i > 0)
                parsedLine[i - 1].Num += parsedLine[i].Num;
            if (i < parsedLine.Count - 2)
                parsedLine[i + 2].Num += parsedLine[i + 1].Num;
            parsedLine.RemoveAt(i);
            parsedLine[i] = new NumLevel(0, parsedLine[i].Level - 1, parsedLine[i].CloseAfter - 1);
        }

        void Split(int i)
        {
            long num = parsedLine[i].Num;
            parsedLine[i].Level++;
            parsedLine[i].CloseAfter++;
            parsedLine[i].Num = (num + 1) / 2;
            parsedLine.Insert(i, new NumLevel(num / 2, parsedLine[i].Level));
        }
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
        Console.WriteLine(sb.ToString());
    }

    private static List<NumLevel> Parse(string line)
    {
        var result = new List<NumLevel>();
        int level = 0;
        foreach (char c in line)
        {
            switch (c)
            {
                case '[': level++; break;
                case ']': level--; result[^1].CloseAfter++; break;
                case ',': break;
                default: result.Add(new NumLevel(c - '0', level)); break;
            }
        }
        return result;
    }

    public class NumLevel
    {
        public long Num { get; set; }
        public int Level { get; set; }
        public int CloseAfter { get; set; }

        public bool IsPairWith(NumLevel other) => CloseAfter == 0 && Level == other.Level; 

        public NumLevel(long num, int level, int closeAfter = 0)
        {
            Num = num;
            Level = level;
            CloseAfter = closeAfter;
        }
    }
}