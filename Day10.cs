namespace AdventOfCode2021;

public class Day10
{
    private readonly ITestOutputHelper output;

    record Day10Data(char Open, char Close, int ScoreClose, int ScoreCorrupt);
    private static Day10Data[] day10data = new[]
    {
        new Day10Data('(', ')', 1, 3),
        new Day10Data('[', ']', 2, 57),
        new Day10Data('{', '}', 3, 1197),
        new Day10Data('<', '>', 4, 25137),
    };
    private static Dictionary<char, Day10Data> dataPerOpenChar = day10data.ToDictionary(x => x.Open);

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines();
        int totalCorruptPoints = 0;
        var lineScores = new List<long>();

        foreach (var line in input)
        {
            var stack = new Stack<Day10Data>();
            bool error = false;

            foreach (char c in line)
            {
                bool isOpen = dataPerOpenChar.TryGetValue(c, out var data);
                if (isOpen)
                    stack.Push(data);
                else
                {
                    data = stack.Pop();
                    if (c != data.Close)
                    {
                        totalCorruptPoints += day10data.First(d => d.Close == c).ScoreCorrupt;
                        error = true;
                        break;
                    }
                }
            }
            if (!error && stack.Count > 0)
            {
                long totalLineScore = 0;
                foreach (var day10Data in stack)
                    totalLineScore = totalLineScore * 5 + day10Data.ScoreClose;
                lineScores.Add(totalLineScore);
            }
        }
        Advent.AssertAnswer1(totalCorruptPoints);

        lineScores.Sort();
        Advent.AssertAnswer2(lineScores[lineScores.Count / 2]);
    }
}
