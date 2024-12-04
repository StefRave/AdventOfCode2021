namespace AdventOfCode2024;

public class Day04 : IAdvent
{
    static (int dy, int dx)[] directions = new[] { (0, 1), (0, -1), (1, 0), (-1, 0), (1, 1), (1, -1), (-1, 1), (-1, -1) };
    string word = "XMAS";
  
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();
        
        int answer1 = FindXmas();
        Advent.AssertAnswer1(answer1, expected: 2536, sampleExpected: 18);

        int answer2 = FindXmas2();
        Advent.AssertAnswer2(answer2, expected: 1875, sampleExpected: 9);


        int FindXmas()
        {
            int total = 0;
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[y].Length; x++)
                    foreach (var dir in directions)
                        if (FindXmasString((y, x), (dir.dy, dir.dx)))
                            total++;
            return total;


            bool FindXmasString((int y, int x) pos, (int y, int x) letterDelta)
            {
                for (int letterIndex = 0; letterIndex < word.Length; letterIndex++)
                {
                    if (!isInside(pos))
                        return false;

                    char c = input[pos.y][pos.x];
                    if (c != word[letterIndex])
                        return false;

                    pos = (pos.y + letterDelta.y, pos.x + letterDelta.x);
                }
                return true;
            }

            bool isInside((int y, int x) pos) => pos.y >= 0 && pos.y < input.Length && pos.x >= 0 && pos.x < input[0].Length;
        }

        int FindXmas2()
        {
            int total = 0;
            for (int y = 1; y < input.Length - 1; y++)
            {
                for (int x = 1; x < input[y].Length - 1; x++)
                {
                    if (input[y][x] == 'A')
                    {
                        string d1 = input[y - 1][x - 1].ToString() + input[y + 1][x + 1];
                        string d2 = input[y - 1][x + 1].ToString() + input[y + 1][x - 1];
                        if ((d1 == "MS" || d1 == "SM") && (d2 == "MS" || d2 == "SM"))
                            total++;
                    }
                }
            }
            return total;
        }
    }
}



