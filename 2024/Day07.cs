using System.Numerics;

namespace AdventOfCode2024;

public class Day07 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(l =>
            {
                var m = l.Split(": ");
                return (testValue: BigInteger.Parse(m[0]), values: (m[1].Split(' ').Select(BigInteger.Parse).ToArray()
                ));
            })
            .ToArray();
        BigInteger answer1 = SumSuccessFull(part2: false);
        Advent.AssertAnswer1(answer1, expected: 3351424677624, sampleExpected: 3749);


        BigInteger answer2 = SumSuccessFull(part2: true);
        Advent.AssertAnswer2(answer2, expected: 204976636995111, sampleExpected: 11387);


        BigInteger SumSuccessFull(bool part2)
        {
            BigInteger result = 0;
            foreach (var line in input)
            {
                if (Calc(line.testValue, line.values, part2))
                    result += line.testValue;
            }
            return result;
        }
    }
    static BigInteger Sum(BigInteger a, BigInteger b) => a + b;
    static BigInteger Mul(BigInteger a, BigInteger b) => a * b;
    static BigInteger ConCat(BigInteger a, BigInteger b) => BigInteger.Parse(a.ToString() + b.ToString());

    static bool Calc(BigInteger testValue, BigInteger[] values, bool part2)
    {
        List<Func<BigInteger, BigInteger, BigInteger>> operations = [ Sum, Mul ];
        if (part2)
            operations.Add(ConCat);

        return Inner(values[0], values[1], values.AsSpan().Slice(2));


        bool Inner(BigInteger value1, BigInteger value2, Span<BigInteger> other)
        {
            if (other.Length == 0)
                return operations.Any(op => testValue == op(value1, value2));

            foreach (var op in operations)
            {
                var val = op(value1, value2);
                if (Inner(val, other[0], other[1..]))
                    return true;
            }
            return false;
        }
    }
}





