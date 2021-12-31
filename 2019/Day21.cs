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
        Advent.AssertAnswer1(Execute(code), 19349722);



        code = @"NOT D T
NOT T J
NOT A T
NOT T T
AND B T
AND C T
NOT T T
AND T J
NOT E T
NOT T T
OR H T
AND T J
NOT A T
OR T J
RUN
";

        Advent.AssertAnswer2(Execute(code), 1141685254);


        long Execute(string code)
        {
            var intInput = code.Replace("\r", "").Select(c => (long)c).ToArray();
            var intCode = new IntCode(GetInput(), intInput);
            intCode.Run();
            var result = intCode.Output.Aggregate("", (s, c) => s + (char)c);
            //WriteLine(result);
            return intCode.Output[^1];
        }
    }
}
