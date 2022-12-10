using FluentAssertions.Equivalency.Tracing;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2022;

public class Day10 : IAdvent
{
    static HashSet<int> measure = new HashSet<int>(new int[] { 20, 60, 100, 140, 180, 220 });
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split())
            .ToArray();
        var screen = Init.Array(() => Init.Array('.', 40), 6);

        int crtC = 0;
        int crtR = 0;

        int regX = 1;
        int cycle = 1;
        long total = 0;
        for (int i = 0; i < input.Length; i++)
        {
            string[] instr = input[i];
            for (int j = (instr[0] == "noop" ? 1 : 2); j > 0 ; j--)
            {
                var oldRegX = regX;
                if (j == 1)
                {
                    if (instr[0] == "addx")
                        regX += int.Parse(instr[1]);
                }

                var spriteLine = Init.Array('.', 40);
                for (int x = -1; x < 2; x++)
                    if (x + oldRegX >= 0 && x + oldRegX < 40)
                        spriteLine[x + oldRegX] = '#';
                screen[crtR][crtC] = spriteLine[crtC];
                
                crtC = (crtC + 1) % 40;
                if (crtC == 0)
                    crtR++;

                if (measure.Contains(cycle))
                {
                    long val = cycle * oldRegX;
                    total += val;
                }
                cycle++;
            }
        }
        for (int r = 0; r < screen.Length; r++)
            Console.WriteLine(new string(screen[r]));

        Advent.AssertAnswer1(total, expected: 14060, sampleExpected: 13140);

        string answer2 = "PAPKFKEJ";
    }
}