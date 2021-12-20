namespace AdventOfCode2021;

public class Day20
{
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine()
            .ToArray();

        string imageEnhancementAlgorithm = input[0];
        char[][] startImg = input[1]
            .SplitByNewLine()
            .Select(line => line.ToCharArray())
            .ToArray();

        (int dy, int dx)[] offsets = new[] { (-1, -1), (-1, -0), (-1, +1), (-0, -1), (-0, -0), (-0, +1), (+1, -1), (+1, -0), (+1, +1) };

        Advent.AssertAnswer1(DoIterations(startImg, 2));
        Advent.AssertAnswer2(DoIterations(startImg, 50));


        int DoIterations(char[][] img, int numberOfIterations)
        {
            for (int i = 0; i < numberOfIterations; i++)
                img = DoIteration(img, i);

            return img.SelectMany(r => r).Count(c => c == '#');
        }

        char[][] DoIteration(char[][] img, int currentIteration)
        {
            var maxY = img.Length;
            var maxX = img[0].Length;
            var newImg = new char[maxY + 2][];
            for (int y = 0; y < maxY + 2; y++)
            {
                char outOfBoundsChar = currentIteration % 2 == 0 || imageEnhancementAlgorithm[0] != '#' ? '.' : '#';
                var row = newImg[y] = new char[maxX + 2];
                for (int x = 0; x < maxX + 2; x++)
                {
                    int index = 0;
                    foreach (var (dy, dx) in offsets)
                    {
                        var (py, px) = (y + dy - 1, x + dx - 1);
                        bool inBounds = px >= 0 && py >= 0 && px < maxX && py < maxY;
                        char c = inBounds ? img[py][px] : outOfBoundsChar;
                        index = (index << 1) + (c == '#' ? 1 : 0);
                    }
                    row[x] = imageEnhancementAlgorithm[index];
                }
            }
            return newImg;
        }
    }
}
