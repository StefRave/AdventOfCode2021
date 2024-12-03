namespace AdventOfCode2024;

public class Day03 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput();

        long answer1 = Calc(input);
        Advent.AssertAnswer1(answer1, expected: 185797128, sampleExpected: 161);

        long answer2 = Calc(input, dontSupport: true);
        Advent.AssertAnswer2(answer2, expected: 89798695, sampleExpected: 48);
    }

    private static long Calc(string input, bool dontSupport = false)
    {
        MatchCollection matches = new Regex(@"(mul|do|don't)\((\d+,\d+|)\)", RegexOptions.Multiline).Matches(input);
        bool doFlag = true;
        long total = 0;
        foreach (Match match in matches)
        {
            var opcode = match.Groups[1].Value;
            if (opcode == "mul" && doFlag)
            {
                var args = match.Groups[2].Value.Split(',');
                total += int.Parse(args[0]) * int.Parse(args[1]);
            }
            else if (opcode == "don't" && dontSupport)
                doFlag = false;
            else if (opcode == "do")
                doFlag = true;
        }
        return total;
    }
}


