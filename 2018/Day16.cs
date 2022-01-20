using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day16 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().Split("\n\n\n");
        var part1Input = input[0].SplitByDoubleNewLine()
            .Select(inp => Regex.Matches(inp, @"(?:(\d+),? ?){4}").Cast<Match>().Select(m => m.Groups[1].Captures.Select(c => int.Parse(c.Value)).ToArray()).ToArray())
            .Select(inp => new TestInput(inp[0], inp[1], inp[2]))
            .ToArray();
        var part2Input = input[1]
            .SplitByNewLine()
            .Select(line => line.Split(' ').Select(int.Parse).ToArray())
            .ToArray();

        var allOpcodes = (Opcode[])Enum.GetValues(typeof(Opcode));
        var opcodeOptions = Enumerable.Range(0, 16).ToDictionary(o => o, o => allOpcodes.ToHashSet());

        int answer1 = 0;
        foreach (var test in part1Input)
        {
            int success = 0;
            foreach (Opcode o in Enum.GetValues(typeof(Opcode)))
            {
                var inp = test.Before;
                var instr = test.Instr;
                var outp = Execute(inp, instr, o);

                bool equals = outp.SequenceEqual(test.After);
                if (equals)
                    success++;
                else
                    opcodeOptions[test.Instr[0]].Remove(o);
            }
            if (success >= 3)
                answer1++;
        }
        Advent.AssertAnswer1(answer1, 509, 1);

        if (Advent.UseSampleData)
            return;

        Opcode[] instrOpcode = GetOpcodeForInstructionNumber(opcodeOptions);
        var registers = new int[4];
        foreach (var instr in part2Input)
            registers = Execute(registers, instr, instrOpcode[instr[0]]);

        Advent.AssertAnswer2(registers[0], 496, 1);
    }

    private static Opcode[] GetOpcodeForInstructionNumber(Dictionary<int, HashSet<Opcode>> opcodeOptions)
    {
        var instrOpcode = new Opcode[16];
        while (opcodeOptions.Count > 0)
        {
            foreach (var (index, set) in opcodeOptions)
            {
                if (set.Count == 1)
                {
                    instrOpcode[index] = set.Single();
                    foreach (var setToRemove in opcodeOptions.Values)
                        setToRemove.Remove(instrOpcode[index]);
                    opcodeOptions.Remove(index);
                    break;
                }
            }
        }

        return instrOpcode;
    }

    private int[] Execute(int[] before, int[] instr, Opcode opcode)
    {
        var result = before.ToArray();
        switch (opcode)
        {
            case Opcode.Addr: result[instr[3]] = before[instr[1]] + before[instr[2]]; break;
            case Opcode.Addi: result[instr[3]] = before[instr[1]] + instr[2]; break;
            case Opcode.Mulr: result[instr[3]] = before[instr[1]] * before[instr[2]]; break;
            case Opcode.Muli: result[instr[3]] = before[instr[1]] * instr[2];  break;
            case Opcode.Banr: result[instr[3]] = before[instr[1]] & before[instr[2]]; break;
            case Opcode.Bani: result[instr[3]] = before[instr[1]] & instr[2]; break;
            case Opcode.Borr: result[instr[3]] = before[instr[1]] | before[instr[2]]; break;
            case Opcode.Bori: result[instr[3]] = before[instr[1]] | instr[2]; break;
            case Opcode.Setr: result[instr[3]] = before[instr[1]];  break;
            case Opcode.Seti: result[instr[3]] = instr[1]; break;
            case Opcode.Gtir: result[instr[3]] = instr[1] > before[instr[2]] ? 1 : 0; break;
            case Opcode.Gtri: result[instr[3]] = before[instr[1]] > instr[2] ? 1 : 0; break;
            case Opcode.Gtrr: result[instr[3]] = before[instr[1]] > before[instr[2]] ? 1 : 0; break;
            case Opcode.Eqir: result[instr[3]] = instr[1] == before[instr[2]] ? 1 : 0; break;
            case Opcode.Eqri: result[instr[3]] = before[instr[1]] == instr[2] ? 1 : 0; break;
            case Opcode.Eqrr: result[instr[3]] = before[instr[1]] == before[instr[2]] ? 1 : 0; break;
        }
        return result;
    }

    public record TestInput(int[] Before, int[] Instr, int[] After);

    public enum Opcode
    {
        Addr, // (add register) stores into register C the result of adding register A and register B.
        Addi, // (add immediate) stores into register C the result of adding register A and value B.
        Mulr, // (multiply register) stores into register C the result of multiplying register A and register B.
        Muli, // (multiply immediate) stores into register C the result of multiplying register A and value B.
        Banr, // (bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
        Bani, // (bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
        Borr, // (bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
        Bori, // (bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
        Setr, // (set register) copies the contents of register A into register C. (Input B is ignored.)
        Seti, // (set immediate) stores value A into register C. (Input B is ignored.)
        Gtir, // (greater-than immediate/register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
        Gtri, // (greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
        Gtrr, // (greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
        Eqir, // (equal immediate/register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
        Eqri, // (equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
        Eqrr, // (equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
    }
}