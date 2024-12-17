using Microsoft.Win32;

namespace AdventOfCode2024;

public class Day17 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        long[] registers = input[0].SplitByNewLine()
            .Select(l => long.Parse(l.Split(": ")[1])).ToArray();
        int[] program = input[1].SplitByNewLine()[0].Split(": ")[1]
            .Split(',').Select(int.Parse).ToArray();


        var answer1 = Execute(program, registers);
        var answer1String = string.Join(",", answer1.Select(i => i.ToString()));
        Advent.AssertAnswer1(answer1String, expected: "2,1,4,7,6,0,3,1,4", sampleExpected: "4,6,3,5,6,3,5,2,1,0");
        if (Advent.UseSampleData)
            return;

        var output = new int[program.Length * 2];
        var reg = new long[3];
        long[] lastDigits = new long[] { 0 };
        long digitMul = 1;
        long answer2 = 0;
        for (int lookingForDigits = 2; lookingForDigits <= 16; lookingForDigits += 2)
        {
            long mask = (1L << (3 * lookingForDigits)) - 1;
            if (lookingForDigits == 16)
                mask = -1;
            var foundValues = new HashSet<long>();
            foreach (var ld in lastDigits)
            { 
                int count = 0;
                for (long i = 0; i < 10000; i += 1)
                {
                    long val = i * digitMul + ld;
                    reg[0] = val;
                    reg[1] = registers[1];
                    reg[2] = registers[2];
                    Execute(program, reg, output);
     
                    bool match = true;
                    for (int j = 0; j < lookingForDigits; j++)
                    {
                        if (output[j] != program[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        foundValues.Add(val & mask);
                        if (count == 20)
                            break;
                    }
                }
            }
            answer2 = foundValues.Min();
            Console.WriteLine($"{lookingForDigits} {foundValues.Count} min={foundValues.Min()}");
            lastDigits = foundValues.ToArray();
            digitMul = mask + 1;
        }
        Advent.AssertAnswer2(answer2, expected: 266932601404433, sampleExpected: 117440);
    }

    public class IntArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x == null || y == null)
                return false;
            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                    return false;
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            if (obj == null)
                return 0;

            int hash = 17;
            foreach (int element in obj)
            {
                hash = hash * 31 + element;
            }
            return hash;
        }
    }


    public int[] Execute(int[] program, long[] registers)
    {
        int[] output = new int[20];
        int length = Execute(program, registers, output);
        return output[0..length];
    }

    public int Execute(int[] program, long[] registers, int[] output)
    {
        int instr = 0;
        int outptr = 0;
        while (instr < program.Length)
        {
            int opcode = program[instr++];
            if (opcode == 0) // div
            {
                long combo = Combo(program[instr++]);
                long denominator = (int)Math.Pow(2, combo);
                if (denominator == 0)
                    return - 1;
                registers[0] = registers[0] / denominator;
            }
            else if (opcode == 1) // bxl
            {
                int literal = program[instr++];
                registers[1] = registers[1] ^ literal;
            }
            else if (opcode == 2) // bst
            {
                long literal = Combo(program[instr++]);
                registers[1] = literal % 8;
            }
            else if (opcode == 3) // jnz
            {
                int literal = program[instr++];
                if (registers[0] != 0)
                {
                    instr = literal;
                }
            }
            else if (opcode == 4) // bxc
            {
                instr++;
                registers[1] = registers[1] ^ registers[2];
            }
            else if (opcode == 5) // out
            {
                long combo = Combo(program[instr++]);
                output[outptr++] = (int)(combo % 8);
            }
            else if (opcode == 6) // bvd
            {
                long combo = Combo(program[instr++]);
                long denominator = (int)Math.Pow(2, combo);
                if (denominator == 0)
                    return -1;
                registers[1] = registers[0] / denominator;
            }
            else if (opcode == 7) // cvd
            {
                long combo = Combo(program[instr++]);
                long denominator = (int)Math.Pow(2, combo);
                if (denominator == 0)
                    return -1;
                registers[2] = registers[0] / denominator;
            }
        }
        return outptr;


        long Combo(int combo)
        {
            return combo switch
            {
                0 => combo,
                1 => combo,
                2 => combo,
                3 => combo,
                4 => registers[0],
                5 => registers[1],
                6 => registers[2],
                _ => throw new Exception("not valid")
            };
        }

    }

}
