namespace AdventOfCode2021;

public class Day10
{
    private static int ScoreCorrupted(char c) => c switch { ')' => 3, ']' => 57, '}' => 1197, '>' => 25137};
    private static int ScoreClose(char c) => c switch { ')' => 1, ']' => 2, '}' => 3, '>' => 4 };
    private static char? CloseForOpen(char c) => c switch { '(' => ')', '[' => ']', '{' => '}', '<' => '>', _ => null };

    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines();
        int totalCorruptPoints = 0;
        var lineScores = new List<long>();

        foreach (var line in input)
        {
            var stack = new Stack<char>();
            char? corruptCharacter = null;
            foreach (char c in line)
            {
                char? matchingClose = CloseForOpen(c);
                if (matchingClose.HasValue)
                    stack.Push(matchingClose.Value);
                else
                {
                    char expectedClose = stack.Pop();
                    if (c != expectedClose)
                    {
                        corruptCharacter = c;
                        break;
                    }
                }
            }
            if(corruptCharacter.HasValue)
                totalCorruptPoints += ScoreCorrupted(corruptCharacter.Value);
            else if (stack.Count > 0)
                lineScores.Add(stack.Aggregate(0, (acc, c) => acc * 5 + ScoreClose(c)));
        }
        Advent.AssertAnswer1(totalCorruptPoints);

        lineScores.Sort();
        Advent.AssertAnswer2(lineScores[lineScores.Count / 2]);
    }
}