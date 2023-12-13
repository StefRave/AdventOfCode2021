namespace AdventOfCode2023;

public class Day13 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine()
            .Select(pattern => pattern.SplitByNewLine())
            .ToArray();

        int answer1 = FindAnswer(input, part: 1);
        Advent.AssertAnswer1(answer1, expected: 35360, sampleExpected: 405);

        int answer2 = FindAnswer(input, part: 2);
        Advent.AssertAnswer2(answer2, expected: 36755, sampleExpected: 400);


        int FindAnswer(string[][] input, int part)
        {
            int answer = 0;
            int expectedSmudges = part == 1 ? 0 : 1;
            foreach (string[] pattern in input)
            {
                for (var my = 0; my < pattern.Length - 1; my++)
                {
                    int smudgeCount = 0;
                    for (var y = 0; y < Math.Min(my + 1, pattern.Length - my - 1); y++)
                        for (int x = 0; x < pattern[0].Length; x++)
                            smudgeCount += pattern[my - y][x] != pattern[my + y + 1][x] ? 1 : 0;
                    if (smudgeCount == expectedSmudges)
                        answer += (my + 1) * 100;
                }
                for (var mx = 0; mx < pattern[0].Length - 1; mx++)
                {
                    int smudgeCount = 0;
                    for (var x = 0; x < Math.Min(mx + 1, pattern[0].Length - mx - 1); x++)
                        for (int y = 0; y < pattern.Length; y++)
                            smudgeCount += pattern[y][mx - x] != pattern[y][mx + x + 1] ? 1 : 0;
                    if (smudgeCount == expectedSmudges)
                        answer += mx + 1;
                }
            }
            return answer;
        }
    }
}












