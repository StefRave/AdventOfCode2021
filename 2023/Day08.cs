namespace AdventOfCode2023;

public class Day08 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .ToArray();
        string instructions = input[0];
        var paths = input[1].SplitByNewLine()
            .Select(line => Regex.Matches(line, @"\w+").Select(m => m.Value).ToArray())
            .ToDictionary(words => words[0], words => words[1..]);

        long answer1 = Advent.UseSampleData ?
            CountSteps("22A", "22Z") :
            CountSteps("AAA", "ZZZ");
        Advent.AssertAnswer1(answer1, expected: 18827, sampleExpected: 3);

        long answer2 = 
            paths.Keys.Where(k => k[^1] == 'A')
            .Select(pos => CountSteps(pos, "Z"))
            .Aggregate((a, b) => a * b / GCD(a, b));
        Advent.AssertAnswer2(answer2, expected: 20220305520997, sampleExpected: 6);


        long CountSteps(string pos, string endsWith)
        {
            long steps = 0;
            do
            {
                pos = paths[pos][instructions[(int)((steps++) % instructions.Length)] == 'L' ? 0 : 1];
            }
            while (!pos.EndsWith(endsWith));

            return steps;
        }
    }


    static long GCD(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
