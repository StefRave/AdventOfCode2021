#nullable enable
namespace AdventOfCode2019;

public class Day23 : IAdvent
{
    private static List<long> GetInput() => File.ReadAllText(@"Input/input23.txt").Split(",").Select(long.Parse).ToList();
  
    void IAdvent.Run()
    {
        var computers = Make(50, i => new IntCode(GetInput(), new long[] { i, -1L }));
        var prevC0Y = 0L;
        var natXY = (x: 0L, y: 0L);
        long answer1 = 0;
        long answer2 = 0;
        while (answer2 == 0)
        {
            foreach (var computer in computers)
            {
                try
                {
                    computer.Run();
                }
                catch (InputNeededException)
                {
                }
            }
            var inputs = Make(computers.Length, i => new List<long>());
            foreach (var computer in computers)
            {
                for (int i = 0; i < computer.Output.Count; )
                {
                    long target = computer.Output[i++];
                    long x = computer.Output[i++];
                    long y = computer.Output[i++];
                    if (target == 255)
                    {
                        if (answer1 == 0)
                            answer1 = y;
                        natXY = (x, y);
                    }
                    else
                    {
                        inputs[(int)target].Add(x);
                        inputs[(int)target].Add(y);
                    }
                }
                computer.Output.Clear();
            }
            bool allAreIdle = inputs.All(inp => inp.Count == 0);
            if (allAreIdle)
            {
                inputs[0].Add(natXY.x);
                inputs[0].Add(natXY.y);
            }
            for (int i = 0; i < inputs[0].Count; i += 2)
            {
                if (prevC0Y == inputs[0][i + 1])
                    answer2 = prevC0Y;
                prevC0Y = inputs[0][i + 1];
            }
            for (int i = 0; i < computers.Length; i++)
                if (inputs[i].Count == 0)
                    inputs[i].Add(-1L);
            for (int i = 0;i < computers.Length; i++)
                foreach (long input in inputs[i])
                    computers[i].Input.Enqueue(input);
        }
        Advent.AssertAnswer1(answer1, 27846);
        Advent.AssertAnswer2(answer2, 0);

    }

    private static T[] Make<T>(int count, Func<int, T> maker)
        => Enumerable.Range(0, count).Select(i => maker(i)).ToArray();
}
