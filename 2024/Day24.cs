using System.Linq;
using System.Net;

namespace AdventOfCode2024;

public class Day24 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        var registers = input[0].SplitByNewLine()
            .Select(x => x.Split(": "))
            .ToDictionary(s => s[0], s => int.Parse(s[1]));
        var instructions = input[1].SplitByNewLine()
            .Select(x => x.Split(" "))
            .Select(l => new Instr(l[0], l[1], l[2], l[4]))
            .ToArray();

        var toExecute = instructions;
        var orderInstructions = new List<Instr>();
        while (toExecute.Length != 0)
        {
            var newToExecute = new List<Instr>();
            foreach (var ins in toExecute)
            {
                if (!registers.ContainsKey(ins.Reg1) || !registers.ContainsKey(ins.Reg2))
                    newToExecute.Add(ins);
                else
                {
                    ExecuteIns(ins);
                    orderInstructions.Add(ins);
                }
            }
            toExecute = newToExecute.ToArray();
        }

        Console.WriteLine();

        long answer1 = GetRegisterValue('z');
        Advent.AssertAnswer1(answer1, expected: 56278503604006, sampleExpected: 2024);
        if (Advent.UseSampleData)
            return;

        long z = GetRegisterValue('z');
        var zRegisters = registers.Keys.Where(k => k[0] == 'z').OrderBy(r => r).ToArray();
        var insByOut = instructions.ToDictionary(i => i.Out);
        var outs = instructions.Select(i => i.Out).ToArray();
        var good = new HashSet<string>();
        var swaps = new Dictionary<string, string>();
        foreach (var zReg in zRegisters[..^1])
        {
            int expectedCount = int.Parse(zReg[1..]) * 4 - 2;
            var used = new HashSet<string>();
            GetInvolvedRegisters(zReg, used);

            var usedExceptGood = used.Except(good).ToArray();
            Console.Write($"\rReg: {zReg}");

            bool isGood = TestWithSwaps(swaps, zReg);
            if (!isGood)
            {
                var swapCandidates = used.Concat([zReg]).Except(good).ToArray();
                var relevantInstr = instructions.Where(i1 => !good.Contains(i1.Out)).Select(i => i.Out).ToArray();
                var combinations = swapCandidates
                    .SelectMany(a => relevantInstr, (a, b) => a.CompareTo(b) < 0 ? (a,b) : (b,a))
                    .Where(t => t.Item1 != t.Item2)
                    .Distinct();
                bool found = false;
                foreach (var (swap1, swap2) in combinations)
                {
                    var testSwaps = swaps.ToDictionary();
                    testSwaps.Add(swap1, swap2);
                    testSwaps.Add(swap2, swap1);
                    used = new HashSet<string>();
                    GetInvolvedRegisters(zReg, used, testSwaps);
                    if (used.Count == expectedCount)
                    {
                        if (TestWithSwaps(testSwaps, zReg))
                        {
                            usedExceptGood = used.Except(good).ToArray();
                            Console.WriteLine($"\rReg: {zReg} swap {swap1}-{swap2}");
                            found = true;
                            swaps = testSwaps;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    Console.WriteLine($"Fail: No swap found for {zReg}");
                    return;
                }
            }
            foreach (var instr in orderInstructions)
                if (usedExceptGood.Contains(instr.Out) || instr.Out == zReg)
                    good.Add(instr.Out);
        }
        Console.WriteLine("                                  ");
        var answer2 = string.Join(",", swaps.Keys.OrderBy(r => r));
        Advent.AssertAnswer2(answer2, expected: "bhd,brk,dhg,dpd,nbf,z06,z23,z38", sampleExpected: 22222222);

        void ExecuteIns(Instr ins)
        {
            registers[ins.Out] = ins.Ins switch
            {
                "AND" => registers[ins.Reg1] & registers[ins.Reg2],
                "XOR" => registers[ins.Reg1] ^ registers[ins.Reg2],
                "OR" => registers[ins.Reg1] | registers[ins.Reg2],
            };
        }

        long GetRegisterValue(char register)
        {
            long answer1 = 0;
            var zRegisters = registers.Keys.Where(k => k[0] == register);
            foreach (var z in zRegisters)
            {
                answer1 += (long)registers[z] << int.Parse(z[1..]);
            }

            return answer1;
        }

        void GetInvolvedRegisters(string zReg, HashSet<string> hashSet, Dictionary<string, string> swaps = null)
        {
            var queue = new Queue<string>();
            swaps ??= new Dictionary<string, string>();
            var ins = insByOut[swaps.GetValueOrDefault(zReg, zReg)];

            queue.Enqueue(ins.Reg1);
            queue.Enqueue(ins.Reg2);
            while (queue.Count > 0)
            {
                var reg = queue.Dequeue();
                if (hashSet.Contains(reg))
                    continue;
                if (reg[0] == 'x' || reg[0] == 'y')
                    continue;
                hashSet.Add(reg);
                if (swaps != null)
                    reg = swaps.GetValueOrDefault(reg, reg);
                if (insByOut.TryGetValue(reg, out ins))
                {
                    if (!hashSet.Contains(ins.Reg1))
                        queue.Enqueue(ins.Reg1);
                    if (!hashSet.Contains(ins.Reg2))
                        queue.Enqueue(ins.Reg2);
                }
            }
        }

        bool TestWithSwaps(Dictionary<string, string> swaps, string zReg)
        {
            int zRegInt = int.Parse(zReg[1..]);

            for (int i = 0; i < 1000; i++)
            {
                long x = Random.Shared.NextInt64() & 0xffffffffffff;
                long y = Random.Shared.NextInt64() & 0xffffffffffff;
                //x = 0x1c28fb4efecd;
                //y = 0x170660684819;
                var registers = new Dictionary<string, int>();
                for (int j = 0; j < zRegisters.Length; j++)
                {
                    string s = j.ToString("00");
                    registers[$"x" + s] = (int)(x >> j) & 1;
                    registers[$"y" + s] = (int)(y >> j) & 1;
                }
                var toExecute = orderInstructions.ToArray();
                while (toExecute.Length != 0)
                {
                    var newToExecute = new List<Instr>();
                    int toExecuteCount = toExecute.Length;
                    foreach (var ins in toExecute)
                    {
                        if (!registers.ContainsKey(ins.Reg1) || !registers.ContainsKey(ins.Reg2))
                            newToExecute.Add(ins);
                        else
                        {
                            var outReg = swaps.GetValueOrDefault(ins.Out, ins.Out);
                            registers[outReg] = ins.Ins switch
                            {
                                "AND" => registers[ins.Reg1] & registers[ins.Reg2],
                                "XOR" => registers[ins.Reg1] ^ registers[ins.Reg2],
                                "OR" => registers[ins.Reg1] | registers[ins.Reg2],
                            };
                        }
                    }
                    if (toExecuteCount == newToExecute.Count)
                        return false;
                    toExecute = newToExecute.ToArray();
                }
                if ((((x + y) >> zRegInt) & 1) != registers[zReg])
                    return false;
            }
            return true;
        }
    }

    class Instr
    {
        public string Reg1 { get; }
        public string Ins { get; }
        public string Reg2 { get; }
        public string Out { get; }

        public Instr(string reg1, string ins, string reg2, string @out)
        {
            Reg1 = reg1;
            Ins = ins;
            Reg2 = reg2;
            Out = @out;

            if (Reg1.CompareTo(Reg2) > 0)
                (Reg1, Reg2) = (Reg2, Reg1);
        }
        public override string ToString()
        {
            return $"{Reg1} {Ins} {Reg2} -> {Out}";
        }
    }
}