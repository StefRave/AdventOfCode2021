namespace AdventOfCode2021;

public class Day02 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(c => c.Split(' '))
            .Select(s => new { Instr = s[0], Steps = int.Parse(s[1]) })
            .ToArray();

        int hor = 0;
        int dep = 0;
        foreach (var item in input)
        {
            switch(item.Instr)
            {
                case "forward": hor += item.Steps; break;
                case "down": dep += item.Steps; break;
                case "up": dep -= item.Steps; break;
                default: throw new Exception($"unknown {item.Instr}");
            }
        }

        Advent.AssertAnswer1((long)hor * dep);

        hor = 0;
        dep = 0;
        int aim = 0;
        foreach (var item in input)
        {
            switch (item.Instr)
            {
                case "forward": hor += item.Steps; dep += item.Steps * aim; break;
                case "down": aim += item.Steps; break;
                case "up": aim -= item.Steps; break;
                default: throw new Exception($"unknown {item.Instr}");
            }
        }
        Advent.AssertAnswer2((long)hor * dep);
    }
}
