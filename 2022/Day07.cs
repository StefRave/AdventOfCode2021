using AdventOfCode2022.Helpers;

namespace AdventOfCode2022;

public class Day07 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .ToArray();

        string currrentDir = "";
        var dirSize = new Dictionary<string, long>();
        long total = 0;
        foreach (var line in input)
        {
            var s = line.TrimStart().Split(" ");
            if (s[0] == "$")
            {
                if (s[1] == "cd")
                {
                    if (s[2] == "/")
                        currrentDir = "/";
                    else if (s[2] == "..")
                        currrentDir = currrentDir.Substring(0, currrentDir.Substring(0, currrentDir.Length - 1).LastIndexOf("/") + 1);
                    else
                        currrentDir += s[2] + "/";
                }
            }
            else
            {
                if (s[0] == "dir")
                {
                }
                else
                {
                    long siz = long.Parse(s[0]);
                    total += siz;

                    string dir = currrentDir;
                    while (true)
                    {
                        dirSize.Update(dir.TrimEnd('/'), s => s + siz);
                        if (dir == "/")
                            break;
                        dir = dir.Substring(0, dir.Substring(0, dir.Length - 1).LastIndexOf("/") + 1);
                    }
                }
            }
        }



        long answer1 = dirSize
            .Where(l => l.Value <= 100000)
            .Select(l => l.Value).Sum();
        Advent.AssertAnswer1(answer1, expected: 2031851, sampleExpected: 95437);

        long answer2 = long.MaxValue;
        long usedSpace = 70000000 - dirSize[""];
        foreach (var (dir, size) in dirSize)
        {
            
            long newUnusedSpace = usedSpace + dirSize[dir];
            if (newUnusedSpace >= 30000000 && (size < answer2))
                answer2 = size;
        }
        Advent.AssertAnswer2(answer2, expected: 2, sampleExpected: 24933642);
    }
}