using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day8
    {
        private readonly ITestOutputHelper output;

        public Day8(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay7()
        {
            var input = 
                File.ReadAllLines("input/input8.txt")
                .Select(l => l.Split())
                .Select(sl => new Instr(Op: sl[0], Arg: int.Parse(sl[1])))
                .ToArray();

            var (completed, result) = Execute(input, 0);
            output.WriteLine($"Part1: {result}");

            int jmpPosition = -1;
            while(!completed)
            {
                while (input[++jmpPosition].Op != "jmp") ;

                (completed, result) = Execute(input, 0, jmpPosition);
            }
            output.WriteLine($"Part2: {result}");

            (bool completed, int acc) Execute(Instr[] instructions, int accumulator, int skipJmpAtPosition = -1)
            {
                bool[] positionsExecuted = new bool[instructions.Length];

                int position = 0;
                while(position < instructions.Length)
                {
                    if (positionsExecuted[position])
                        return (false, accumulator);
                    positionsExecuted[position] = true;

                    var current = instructions[position++];
                    switch(current.Op)
                    {
                        case "nop": break;
                        case "acc": accumulator += current.Arg; break;
                        case "jmp": if(position - 1 != skipJmpAtPosition) position += current.Arg - 1; break;
                    }
                }
                return (true, accumulator);
            }
        }

        record Instr(string Op, int Arg);
    }
}