#nullable enable
using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode2019;

public class Day22 : IAdvent
{
    private static string GetInput() => File.ReadAllText(@"Input/input22.txt");

    void IAdvent.Run()
    {
        long index = 2019;
        long cardCount = 10007;

        Line[] lines = Parse(GetInput());
        long answer1 = Execute(lines, index, cardCount);
        Advent.AssertAnswer1(answer1, 3377);

        // Try same using mul,add
        MulAdd[] mulAdd = lines.Select(l => l.ToMulAdd(cardCount)).ToArray();
        MulAdd combined = MulAdd.Combine(cardCount, mulAdd);
        Assert.Equal(answer1, combined.Execute(index, cardCount));

        // check if reverse operation works
        mulAdd = lines.Select(l => l.ToMulAddReverse(cardCount)).Reverse().ToArray();
        combined = MulAdd.Combine(cardCount, mulAdd);
        Assert.Equal(index, combined.Execute(answer1, cardCount));


        index = 2020;
        cardCount = 119315717514047;
        long times = 101741582076661;
        long fac2TableFactor = 1;
        var fact2Table = new List<MulAdd>();
        var reverseCombined = MulAdd.Combine(cardCount, lines.Reverse().Select(l => l.ToMulAddReverse(cardCount)).ToArray());
        while (fac2TableFactor < times)
        {
            if ((fac2TableFactor & times) != 0)
                fact2Table.Add(reverseCombined);
            fac2TableFactor *= 2;
            reverseCombined = MulAdd.Combine(cardCount, reverseCombined, reverseCombined);
        }
        reverseCombined = MulAdd.Combine(cardCount, fact2Table.ToArray());
        
        long answer2 = reverseCombined.Execute(index, cardCount);
        Advent.AssertAnswer1(answer2, 29988879027217);
    }

    private static long Execute(Line[] commands, long index, long cardCount)
    {
        foreach (var command in commands)
            index = command.Execute(index, cardCount);
        return index;
    }

    static long ModInverse(long a, long m)
    {
        long m0 = m;
        long y = 0, x = 1;

        if (m == 1)
            return 0;
        while (a > 1) // Euclid's algo
            (m, a,x, y) = (a % m, m, y, x - (a / m) * y);
        if (x < 0)
            x += m0;
        return x;
    }

    private static Line[] Parse(string input)
    {
        var matches = Regex.Matches(input.ReplaceLineEndings("\n"), @"^(cut|deal) .*?([-\d]+)?$", RegexOptions.Multiline | RegexOptions.ECMAScript);
        var result = matches
            .Select(m => new Line(ToEnum(m), m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : 0))
            .ToArray();
        Assert.Equal(input.SplitByNewLine().Length, result.Length);
        return result;

        ShuffleMethod ToEnum(Match m)
            => (m.Groups[1].Value == "cut") ? ShuffleMethod.Cut : m.Groups[2].Success ? ShuffleMethod.DealInto : ShuffleMethod.DealNew;
    }

    public enum ShuffleMethod { Cut, DealInto, DealNew };

    public record Line(ShuffleMethod Method, int Number)
    {
        public long Execute(long index, long cardCount)
            => Method switch
            {
                ShuffleMethod.DealNew => cardCount - index - 1,
                ShuffleMethod.DealInto => (index * Number) % cardCount,
                ShuffleMethod.Cut => (index + cardCount - Number) % cardCount,
                _ => throw new ArgumentOutOfRangeException(),
            };

        public MulAdd ToMulAdd(long cardCount)
        {
            return Method switch
            {
                ShuffleMethod.DealNew => new MulAdd(cardCount - 1, cardCount - 1), // reverse deck is same as DealInto(cardCount-1) and then change index - 1
                ShuffleMethod.DealInto => new MulAdd(Number, 0),
                ShuffleMethod.Cut => new MulAdd(1, cardCount - Number),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public long ExecuteReverse(long index, long cardCount)
            => Method switch
            {
                ShuffleMethod.DealNew => cardCount - index - 1,
                ShuffleMethod.DealInto => (index * ModInverse(Number, cardCount)) % cardCount,
                ShuffleMethod.Cut => (index + cardCount + Number) % cardCount,
                _ => throw new ArgumentOutOfRangeException(),
            };
        
        public MulAdd ToMulAddReverse(long cardCount)
        {
            return Method switch
            {
                ShuffleMethod.DealNew => new MulAdd(cardCount - 1, cardCount - 1),
                ShuffleMethod.DealInto => new MulAdd(ModInverse(Number, cardCount), 0),
                ShuffleMethod.Cut => new MulAdd(1, cardCount + Number),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    public record MulAdd(BigInteger Mul, BigInteger Add)
    {
        // ((a*X + b) * c + d) == (a*c)X + b*c + d
        public static MulAdd Combine(long cardCount, params MulAdd[] input)
            => input.Aggregate((a, b) => new MulAdd((a.Mul * b.Mul) % cardCount, (a.Add * b.Mul + b.Add) % cardCount));

        public long Execute(long index, long cardCount)
            => (long)(((index * Mul) % cardCount) + Add) % cardCount;
    }
}
