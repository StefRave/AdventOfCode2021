using System.Text;

namespace AdventOfCode2018;

public class Day07 : IAdvent
{
    public void Run()
    {
        // Step X must be finished before step C can begin.
        var input = Advent.ReadInputLines()
            .Select(line => (first: line[5], later: line[36]))
            .ToArray();

        var (order, time) = GetOrderAndTime(input, minimalDuration : 0, numberOfWorkers: 1);
        Advent.AssertAnswer1(order, expected: "FMOXCDGJRAUIHKNYZTESWLPBQV", sampleExpected: "CABDFE");

        int minimalDuration = Advent.UseSampleData ? 1 : 61;
        int numberOfWorkers = Advent.UseSampleData ? 2 : 5;
        
        (order, time) = GetOrderAndTime(input, minimalDuration, numberOfWorkers);
        Advent.AssertAnswer2(time, expected: 1053, sampleExpected: 15);
    }

    private static (string order, object time) GetOrderAndTime((char first, char later)[] input, int minimalDuration, int numberOfWorkers)
    {
        var leters = input.Select(l => l.first).Concat(input.Select(l => l.later)).Distinct();
        Dictionary<char, char[]> lettersWithCondition = leters
            .ToDictionary(c => c, c => input.Where(l => l.later == c).Select(l => l.first).ToArray());

        var order = new StringBuilder();
        var workingOnAndFinishTime = new PriorityQueue<char, int>();
        int currentTime = 0;
        while (lettersWithCondition.Count > 0 || workingOnAndFinishTime.Count != 0)
        {
            char c;
            while (lettersWithCondition.Count > 0 && workingOnAndFinishTime.Count < numberOfWorkers)
            {
                c = GetNextToBuild();
                if (c == 0)
                    break;
                workingOnAndFinishTime.Enqueue(c, currentTime + minimalDuration + c - 'A');
            }
            workingOnAndFinishTime.TryDequeue(out c, out int time);
            currentTime = time;
            order.Append(c);
            lettersWithCondition.Remove(c);
        }
        return (order.ToString(), currentTime);

        char GetNextToBuild()
        {
            var done = order.ToString().ToHashSet();
            var onlyFirst = lettersWithCondition.Where(kv => !workingOnAndFinishTime.UnorderedItems.Any(item => item.Element == kv.Key) && kv.Value.All(cond => done.Contains(cond)));

            var c = onlyFirst.Select(kv => kv.Key).OrderBy(c => c).FirstOrDefault();
            return c;
        }
    }
}