namespace AdventOfCode2023;

public class Day06 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();
        int[] times = input[0].GetInts();
        int[] distances = input[1].GetInts();

        long answer1 = 1;
        for (int i = 0; i < times.Length; i++)
            answer1 *= CalcMinMaxSpeedOptions(times[i], distances[i]);
        Advent.AssertAnswer1(answer1, expected: 440000, sampleExpected: 288);

        long time = input[0].Replace(" ", "").GetLongs()[0];
        long distance = input[1].Replace(" ", "").GetLongs()[0];
        long answer2 = CalcMinMaxSpeedOptions(time, distance);
        Advent.AssertAnswer2(answer2, expected: 26187338, sampleExpected: 71503);
    }

    public static (double min, double max) CalcMinMaxSpeed(long time, long distanceToBeat)
    {
        var sq = Math.Sqrt(time * time - 4 * distanceToBeat);
        return ((time - sq) / 2, (time + sq) / 2);
    }

    public static long CalcMinMaxSpeedOptions(long time, long distanceToBeat)
    {
        var result = CalcMinMaxSpeed(time, distanceToBeat);
        return (long)(Math.Floor(result.max - 0.000000000001) - Math.Ceiling(result.min + 0.000000000001) + 1);
    }
}
