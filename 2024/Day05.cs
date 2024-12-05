namespace AdventOfCode2024;

public class Day05 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        var rules = input[0].SplitByNewLine()
            .Select(line => line.Split('|').Select(int.Parse).ToArray())
            .GroupBy(x => x[0])
            .ToDictionary(x => x.Key, x => x.ToArray());
        var pagesList = input[1].SplitByNewLine()
            .Select(line => line.Split(',').Select(int.Parse).ToList())
            .ToArray();

        int answer1 = 0;
        int answer2 = 0;
        foreach (var pages in pagesList)
        {
            if (FixOrder(pages))
                answer1 += pages[pages.Count / 2];
            else
                answer2 += pages[pages.Count / 2];
        }

        Advent.AssertAnswer1(answer1, expected: 5509, sampleExpected: 143);
        Advent.AssertAnswer2(answer2, expected: 4407, sampleExpected: 123);


        bool FixOrder(List<int> pages)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                int page = pages[i];
                var mustOccurAfter = rules.GetValueOrDefault(page);
                for (int j = 0; j < i; j++)
                {
                    if (mustOccurAfter.Any(x => x[1] == pages[j]))
                    {
                        pages.RemoveAt(i);
                        pages.Insert(j, page);
                        FixOrder(pages);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}




