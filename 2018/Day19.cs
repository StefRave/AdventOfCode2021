using System.Text;

namespace AdventOfCode2018;

public class Day19 : IAdvent
{

    public void Run()
    {
        string[] input = Advent.ReadInputLines();
        int ipIndex = int.Parse(input[0][^1..]);
        var instructions = input
            .Skip(1)
            .Select(line => line.Split(' '))
            .Select(sl => new Instr((Opcode)Enum.Parse(typeof(Opcode), sl[0], true), sl.Skip(1).Take(3).Select(int.Parse).ToArray()))
            .ToArray();
        long[] registers = new long[6];
        Run(ipIndex, instructions, registers);
        Advent.AssertAnswer1(registers[0], 960, 7);

        registers = new long[] { 1, 0, 0, 0, 0, 0};
        //Run(ipIndex, instructions, registers);
        if (Advent.UseSampleData)
            return;

        // find all solutions x*y=10551293 . Sum(x) from those solutions
        // 10551293=53*199081 and 1*10551293
        Advent.AssertAnswer2(53 + 199081 + 1 + 10551293, 10750428, 0);
    }

    private void Run(int ipIndex, Instr[] instructions, long[] registers)
    {
        while (registers[ipIndex] < instructions.Length)
        {
            Execute(registers, instructions[registers[ipIndex]]);
            //Console.WriteLine($"0:{registers[0]} 1:{registers[1]} 2:{registers[2]} 3:{registers[3]} 4:{registers[4]} 5:{registers[5]} ");
            registers[ipIndex]++;
        }
    }

    private void Execute(long[] registers, Instr instr)
    {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
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
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
    }

    public record TestInput(int[] Before, int[] Instr, int[] After);
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