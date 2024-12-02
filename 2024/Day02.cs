namespace AdventOfCode2024;

public class Day02 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(l => l.Split(' ').Select(int.Parse).ToArray())
            .ToArray();

        int answer1 = input.Count(l => IssueIndex(l, true) == null || IssueIndex(l, false) == null);
        Advent.AssertAnswer1(answer1, expected: 269, sampleExpected: 2);


        int answer2 = input.Count(l => HasOneOrLessIssues(l, true) || HasOneOrLessIssues(l, false));
        Advent.AssertAnswer2(answer2, expected: 337, sampleExpected: 4);
    }

    public static bool HasOneOrLessIssues(int[] line, bool up)
    {
        int? issueIndex = IssueIndex(line, up);
        if (issueIndex == null)
            return true;
        return
            IssueIndex(line.Take(issueIndex.Value).Concat(line.Skip(issueIndex.Value + 1)), up) == null ||
            IssueIndex(line.Take(issueIndex.Value - 1).Concat(line.Skip(issueIndex.Value)), up) == null;
    }

    public static int? IssueIndex(IEnumerable<int> line, bool up)
    {
        int index = 0;
        int prev = line.First();
        foreach (int curr in line.Skip(1))
        {
            index++;
            int diff = up ? curr - prev : prev - curr;
            if (diff < 1 || diff > 3)
                return index;
            else
                prev = curr;
        }
        return null;
    }
}


