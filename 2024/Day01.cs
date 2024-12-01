namespace AdventOfCode2024;

public class Day01 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(l => l.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            .Select(l => (int.Parse(l[0]), int.Parse(l[1])))
            .ToArray();

        var list1 = input.Select(l => l.Item1).OrderBy(a => a).ToArray();
        var list2 = input.Select(l => l.Item2).OrderBy(a => a).ToArray();
        int answer1 = list1.Zip(list2).Select(l => Math.Abs(l.First - l.Second)).Sum();
        Advent.AssertAnswer1(answer1, expected: 1151792, sampleExpected: 11);

        var appearCountList2 = list2.GroupBy(a => a).ToDictionary(g => g.Key, g => g.Count());
        long answer2 = list1.Select(a => (long)a * appearCountList2.GetValueOrDefault(a)).Sum();
        Advent.AssertAnswer2(answer2, expected: 21790168, sampleExpected: 31);
    }
}
