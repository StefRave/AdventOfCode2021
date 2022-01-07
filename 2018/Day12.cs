using System.Text;

namespace AdventOfCode2018;

public class Day12 : IAdvent
{
    public void Run()
    {
        // initial state: .##..##.
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        string initial = input[0][15..];
        HashSet<string> a1 = input[1].SplitByNewLine()
            .Where(line => line[9] == '#')
            .Select(line => line[0..5])
            .ToHashSet();

        int sum1 = 0;
        long sum2 = 0;

        int toAdd = 150;
        var num = "".PadLeft(150, '.') + initial + "".PadLeft(toAdd, '.');
        Console.WriteLine(num);
        string last = "";
        for (int i2 = 0; i2 < 220; i2++)
        {
            if (i2 == 20)
                sum1 = SumIt(toAdd, num);
            string current = num.Trim('.');
            if (current == last)
            {
                sum2 = SumIt(toAdd, num);
                int posCount = num.Where(c => c == '#').Count();
                sum2 += posCount * (50000000000 - i2);
                break;
            }
            last = current;

            num = DoIt(a1, num);
            Console.WriteLine(num);
        }

        Advent.AssertAnswer1(sum1, expected: 2823, sampleExpected: 325);
        Advent.AssertAnswer1(sum2, expected: 2900000001856, sampleExpected: 999999999374);
    }

    private static int SumIt(int toAdd, string num)
    {
        int sum = 0;
        for (int i = 0; i < num.Length; i++)
        {
            if (num[i] == '#')
                sum += -toAdd + i;
        }

        return sum;
    }

    string DoIt(HashSet<string> a, string numbers)
    {
        int side = a.First().Length / 2;
        var sb = new StringBuilder("".PadRight(side, '.'));
        for (int i = side; i < numbers.Length - side; i++)
            sb.Append(a.Contains(numbers[(i - side)..(i + side + 1)]) ? '#' : '.');
        sb.Append("".PadRight(side, '.'));
        return sb.ToString();
    }
    static int BitCount(long number)
    {
        int bitCount = 0;
        for (int i = 0; i < 64; i++)
            if ((number & (1L << i)) != 0)
                bitCount++;
        return bitCount;
    }

    void Print(byte[] numbers)
    {
        foreach (var b in numbers)
            Console.Write(Convert.ToString(b, 2).PadLeft(8, '0'));
        Console.Write('\n');
    }

    private static byte ToNumber(string line) => Convert.ToByte(line, 2);
    private static byte[] ToBytes(string line) => line.Chunk(8).Select(b => Convert.ToByte(new string(b).PadRight(8, '0'), 2)).ToArray();
}