namespace AdventOfCode2022;

public class Day05 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine()
            .ToArray();
        var stackStartInput = input[0].SplitByNewLine();
        var stackInput = new List<string>();
        for (int i = 1; i < stackStartInput[0].Length; i += 4)
        {
            var line = stackStartInput.Select(line => line[i]).Reverse().ToArray();
            stackInput.Add(new string(line).Trim());
        }

        var instructions = input[1].SplitByNewLine()
            .Select(intstr =>
            {
                var r = Regex.Matches(intstr, @"\d+").Select(r => int.Parse(r.Captures[0].Value)).ToArray();
                return (move: r[0], from: r[1], to: r[2]);
            })
            .ToArray();

        string answer1 = Shuffle(moveMultiple: false);
        Advent.AssertAnswer1(answer1, expected: "SPFMVDTZT", sampleExpected: "CMZ");

        string answer2 = Shuffle(moveMultiple: true);
        Advent.AssertAnswer2(answer2, expected: "ZFSJBPRFP", sampleExpected: "MCD");


        string Shuffle(bool moveMultiple)
        {
            var stacks = stackInput.Select(line => new Stack<char>(line)).ToArray();
            foreach (var instr in instructions)
            {
                var tmp = moveMultiple ? new Stack<char>() : stacks[instr.to - 1];
                for (int i = 0; i < instr.move; i++)
                {
                    var letter = stacks[instr.from - 1].Pop();
                    tmp.Push(letter);
                }
                if (moveMultiple)
                    foreach (var letter in tmp)
                        stacks[instr.to - 1].Push(letter);
            }
            return new string(stacks.Select(s => s.Peek()).ToArray());
        }
    }
}