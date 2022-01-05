using System.Globalization;
using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day04 : IAdvent
{
    public void Run()
    {
        // [1518-05-04 00:19] falls asleep
        List<(int guard, DateTime startSleep, DateTime wakeUp)> list = ParseInput();

        var guardAndSleep = list
            .GroupBy(g => g.guard)
            .Select(g => (guard: g.Key, sleep: GetMostAsleepIndexAndTimes(g, g.Key)))
            .ToArray();

        var sleepiestGuard = guardAndSleep.OrderByDescending(gs => gs.sleep.totalMinutesSleep).First();
        int answer1 = sleepiestGuard.guard * sleepiestGuard.sleep.maxSleepIndex;
        Advent.AssertAnswer1(answer1, expected: 87681, sampleExpected: 240);


        var maxTimesAsleepGuard = guardAndSleep.OrderByDescending(gs => gs.sleep.maxTimesSleep).First();
        int answer2 = maxTimesAsleepGuard.guard * maxTimesAsleepGuard.sleep.maxSleepIndex;
        Advent.AssertAnswer2(answer2, expected: 136461, sampleExpected: 4455);
    }

    private static List<(int guard, DateTime startSleep, DateTime wakeUp)> ParseInput()
    {
        var input = Advent.ReadInputLines()
            .Select(line => new Event(DateTime.ParseExact(line[1..17], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), line[19..]))
            .OrderBy(line => line.Timestamp)
            .ToArray();

        var list = new List<(int guard, DateTime startSleep, DateTime wakeUp)>();

        int guard = -1;
        DateTime? sleepingSince = null;
        foreach (var (time, log, g) in input)
        {
            if (g != 0)
            {
                guard = g;
                sleepingSince = null;
            }
            else if (log == "falls asleep")
                sleepingSince = time;
            else if (log == "wakes up" && sleepingSince != null)
                list.Add((guard, sleepingSince.Value, time));
        }

        return list;
    }

    private static (int maxSleepIndex, int maxTimesSleep, int totalMinutesSleep) GetMostAsleepIndexAndTimes(IEnumerable<(int guard, DateTime startSleep, DateTime wakeUp)> list, int sleepiestGuard)
    {
        var asleep = new int[60];
        int totalMinutesSleep = 0;
        foreach (var sleep in list.Where(l => l.guard == sleepiestGuard))
        {
            int sleepMinuteStart = sleep.startSleep.Hour * 60 + sleep.startSleep.Minute;
            int sleepMinuteEnd = sleep.wakeUp.Hour * 60 + sleep.wakeUp.Minute;
            for (int i = sleepMinuteStart; i != sleepMinuteEnd; i = (i + 1) % 60)
                asleep[i]++;
            totalMinutesSleep += (int)sleep.wakeUp.Subtract(sleep.startSleep).TotalMinutes;
        }
        int maxTimesSleep = 0;
        int maxSleepIndex = 0;
        for (int i = 0; i < asleep.Length; i++)
        {
            if (asleep[i] > maxTimesSleep)
            {
                maxTimesSleep = asleep[i];
                maxSleepIndex = i;
            }
        }
        return (maxSleepIndex, maxTimesSleep, totalMinutesSleep);
    }

    public record Event
    {
        public DateTime Timestamp { get; }
        public string Log { get; }
        public int Guard { get; }

        public void Deconstruct(out DateTime timeStamp, out string log, out int guard)
            => (timeStamp, log, guard) = (Timestamp, Log, Guard);

        public Event(DateTime timestamp, string log)
        {
            Timestamp = timestamp;
            Log = log;
            var m = Regex.Match(Log, @"\d+");
            if (m.Success)
                Guard = int.Parse(m.Value);
        }
    }
}