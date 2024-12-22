namespace AdventOfCode2024;

public class Day22 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(long.Parse)
            .ToArray();
        int iterations = 2000;
        var bananas = new int[input.Length,iterations];
        var changes = new int[input.Length, iterations];

        long answer1 = 0;
        for (int i = 0; i < input.Length; i++)
        {
            long secretNumber = input[i];
            long s = secretNumber;
            int last = (int)(s % 10);
            for (int j = 0; j < iterations; j++)
            {
                s = GenerateNextSecretNumber(s);
                int banana = (int)(s % 10);
                int change =  banana - last;
                last = banana;
                bananas[i, j] = banana;
                changes[i, j] = change;
            }
            answer1 += s;
        }
        Advent.AssertAnswer1(answer1, expected: 14180628689, sampleExpected: 37990510);


        var maxBanana = new Dictionary<int, int>();
        for (int i = 0; i < input.Length; i++)
        {
            var foundInCurrent = new HashSet<int>();
            for (int j = 3; j < iterations; j++)
            {
                int pattern = 
                    (changes[i, j - 0] + 10) * 1+ 
                    (changes[i, j - 1] + 10) * 100 +
                    (changes[i, j - 2] + 10) * 10000 +
                    (changes[i, j - 3] + 10) * 1000000;
                if (foundInCurrent.Add(pattern))
                    maxBanana.Update(pattern, old => old + bananas[i, j]);
            }
        }
        int answer2 = maxBanana.Max(x => x.Value);
        Advent.AssertAnswer2(answer2, expected: 1690, sampleExpected: 23);
    }

    static long GenerateNextSecretNumber(long secretNumber)
    {
        secretNumber ^= (secretNumber *= 64);
        secretNumber %= 16777216;
        secretNumber ^= secretNumber / 32;
        secretNumber %= 16777216;
        secretNumber ^= secretNumber * 2048;
        secretNumber %= 16777216;

        return secretNumber;
    }
}

