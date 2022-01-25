using System.Diagnostics;
using System.Text;

namespace AdventOfCode2018;

public class Day21 : IAdvent
{

    public void Run()
    {
        if (Advent.UseSampleData)
            return;
        
        string[] input = Advent.ReadInputLines();
        int ipIndex = int.Parse(input[0][^1..]);
        var instructions = input
            .Skip(1)
            .Where(line => !line.StartsWith("#"))
            .Select(line => line.Split(' '))
            .Select(sl => new Instr((Opcode)Enum.Parse(typeof(Opcode), sl[0], true), sl.Skip(1).Take(3).Select(int.Parse).ToArray()))
            .ToArray();
        long[] registers = new long[] { 0, 0, 0, 0, 0, 0 };
        var (firstValue, minValue) = Run(ipIndex, instructions, registers);
        Advent.AssertAnswer1(firstValue, 7216956, 0);
        Advent.AssertAnswer2(minValue, 14596916, 0);
    }

    private (long firstValue, long minValue) Run(int ipIndex, Instr[] instructions, long[] registers)
    {
        var hist = new HashSet<long>();
        long firstValue = 0;
        long prev = 0;
        while (registers[ipIndex] < instructions.Length)
        {
            string regStr = string.Join(" ", registers.Select((r, i) => $"{i}: {r}"));
            Instr instr = instructions[registers[ipIndex]];
            Execute(registers, instr);
            if (registers[ipIndex] == 17)
                registers[4] = registers[2] / 256; // shortcut
            regStr = string.Join(" ", registers.Select((r, i) => $"{i}: {r}"));
            if (registers[1] == 27)
            {
                if (firstValue == 0)
                    firstValue = registers[3];
                if (!hist.Add(registers[3]))
                    return (firstValue, prev);
                prev = registers[3];
            }
            registers[ipIndex]++;
        }
        return default;
    }

    private void Execute(long[] registers, Instr instr)
    {
        switch (instr.Opcode)
        {
            case Opcode.Addr: registers[instr[3]] = registers[instr[1]] + registers[instr[2]]; break;
            case Opcode.Addi: registers[instr[3]] = registers[instr[1]] + instr[2]; break;
            case Opcode.Mulr: registers[instr[3]] = registers[instr[1]] * registers[instr[2]]; break;
            case Opcode.Muli: registers[instr[3]] = registers[instr[1]] * instr[2]; break;
            case Opcode.Banr: registers[instr[3]] = registers[instr[1]] & registers[instr[2]]; break;
            case Opcode.Bani: registers[instr[3]] = registers[instr[1]] & instr[2]; break;
            case Opcode.Borr: registers[instr[3]] = registers[instr[1]] | registers[instr[2]]; break;
            case Opcode.Bori: registers[instr[3]] = registers[instr[1]] | instr[2]; break;
            case Opcode.Setr: registers[instr[3]] = registers[instr[1]]; break;
            case Opcode.Seti: registers[instr[3]] = instr[1]; break;
            case Opcode.Gtir: registers[instr[3]] = instr[1] > registers[instr[2]] ? 1 : 0; break;
            case Opcode.Gtri: registers[instr[3]] = registers[instr[1]] > instr[2] ? 1 : 0; break;
            case Opcode.Gtrr: registers[instr[3]] = registers[instr[1]] > registers[instr[2]] ? 1 : 0; break;
            case Opcode.Eqir: registers[instr[3]] = instr[1] == registers[instr[2]] ? 1 : 0; break;
            case Opcode.Eqri: registers[instr[3]] = registers[instr[1]] == instr[2] ? 1 : 0; break;
            case Opcode.Eqrr: registers[instr[3]] = registers[instr[1]] == registers[instr[2]] ? 1 : 0; break;
        }
    }

    
    [DebuggerDisplay("Indtr {Opcode} {Data[0]} {Data[1]} {Data[2]}")]
    public record Instr(Opcode Opcode, int[] Data)
    {
        public int this[int index] => Data[index - 1];
    }

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
