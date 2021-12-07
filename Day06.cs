namespace AdventOfCode2021;

public class Day06
{
    private readonly ITestOutputHelper output;

    public Day06(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput().Split(',')
            .Select(c => int.Parse(c))
            .ToArray();
        var numberOfFishOnCycle = new long[9];
        foreach (var timerVal in input)
            numberOfFishOnCycle[timerVal]++;

        for (int i = 0; i < 256; i++)
        {
            if(i == 80)
                Advent.AssertAnswer1(numberOfFishOnCycle.Sum());

            long newFishBorn = numberOfFishOnCycle[0];

            for (int j = 1; j < numberOfFishOnCycle.Length; j++)
                numberOfFishOnCycle[j - 1] = numberOfFishOnCycle[j];

            numberOfFishOnCycle[6] += newFishBorn;
            numberOfFishOnCycle[^1] = newFishBorn;
        }

        Advent.AssertAnswer2(numberOfFishOnCycle.Sum());
    }
}
