namespace AdventOfCode2023;

public class Day12 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.Split(' '))
            .Select(s => (Springs: s[0], Numbers: s[1].Split(',').Select(int.Parse).ToArray()))
            .ToArray();

        long answer1 = input.Sum(line => CountOptions(line.Springs, line.Numbers));
        Advent.AssertAnswer1(answer1, expected: 7169, sampleExpected: 21);

        long answer2 = input.Sum(
            line => CountOptions(
                string.Join("?", Enumerable.Repeat(line.Springs, 5)),
                "12345".SelectMany(_ => line.Numbers).ToArray()));
        Advent.AssertAnswer2(answer2, expected: 1738259948652, sampleExpected: 525152);
    }

    public long CountOptions(string springs, int[] numbers)
    {
        var cache = new Dictionary<(string, int), long>();
        return DoPart(springs, numbers);


        long DoPart(string springs, int[] numbers)
        {
            while (springs.Length > 0 && springs[0] == '.')
                springs = springs[1..];
            if (springs.Length == 0)
                return (numbers.Length == 0) ? 1 : 0;
            if (numbers.Length == 0)
                return springs.Any(c => c == '#') ? 0 : 1;

            var key = (springs, numbers.Length);
            if (cache.TryGetValue(((string, int))key, out long res))
                return res;

            long count = 0;
            if (springs[0] == '?')
                count = DoPart(springs[1..], numbers);
            
            if (numbers.Length != 0
                && springs.Length >= numbers[0]
                && !springs[0..numbers[0]].Any(c => c == '.'))
            {
                springs = springs[numbers[0]..];
                numbers = numbers[1..];

                if (springs.Length > 0)
                {
                    if (springs[0] == '#')
                        return count;
                    springs = springs[1..];
                }
                count += DoPart(springs, numbers);
                
            }
            cache.Add(key, count);
            return count;
        }
    }
}
