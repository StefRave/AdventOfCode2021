#nullable enable
namespace AdventOfCode2019;

public class Day21 : IAdvent
{
    private static List<long> GetInput() => File.ReadAllText(@"Input/input21.txt").Split(",").Select(long.Parse).ToList();
    // true if hole
    //   J   
    // FFFtttFFF
    //   J   
    // ###oo ###
    // J     
    // #T#TT##


    // J     
    // FFFtFttFFF
    // !(a & b & c) & D 
    // 

    void IAdvent.Run()
    {
        var memory = GetInput();

        string code = @"NOT D T
NOT T J
NOT A T
NOT T T
AND B T
AND C T
NOT T T
AND T J
WALK
";

        var intInput = code.Replace("\r", "").Select(c => (long)c).ToArray();
        var intCode = new IntCode(memory.ToArray(), intInput);
        intCode.Run();
        var result = intCode.Output.Aggregate("", (s, c) => s + (char)c);
        WriteLine($"Len {intCode.Output.Count,8} {intCode.Output[^1]}");
        Advent.AssertAnswer1(intCode.Output[^1], 19349722);

    }
}
