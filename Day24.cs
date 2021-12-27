namespace AdventOfCode2021;

public class Day24 : IAdvent
{
    [Fact]
    public void Run()
    {
        string[] input = Advent.ReadInputLines();
        if (input.Length <= 1)
            return; // no sample data

        var code = new List<List<Instr>>();
        foreach (string line in input)
        {
            if (line.StartsWith("inp"))
                code.Add(new List<Instr>());
            var instr = Instr.Parse(line);
            code[^1].Add(instr);
        }

#if false
        var possibleInputs = new Dictionary<int, List<(int digit, int zin)>>();
        possibleInputs.Add(0, null);
        var inToOut = new Dictionary<int, List<(int digit, int zin)>>[code.Count];
        for (int codeIndex = 0; codeIndex < code.Count; codeIndex++)
        {
            var outputs = new Dictionary<int, List<(int digit, int zin)>>();
            foreach (int possibleInput in possibleInputs.Keys)
                for (int digit = 1; digit <= 9; digit++)
                {
                    int result = Execute(code[codeIndex], digit, possibleInput);
                    if (result < 1000000)
                    {
                        if(outputs.TryGetValue(result, out var list))
                            list.Add((digit, possibleInput));
                        else
                            outputs.Add(result, new List<(int digit, int zin)> { (digit, possibleInput) });
                    }
                }
            possibleInputs = outputs;
            inToOut[codeIndex] = outputs;
        }

        var r = new Dictionary<int, (string serialMin, string serialMax)>();
        r[0] = ("", "");
        for (int codeIndex = code.Count - 1; codeIndex >= 0; codeIndex--)
        {
            r = GetPossibleZ2(r, codeIndex);
        }
        1.ToString();

        Dictionary<int, (string serialMin, string serialMax)> GetPossibleZ2(Dictionary<int, (string serialMin, string serialMax)> possibleOutputs, int codeIndex)
        {
            return possibleOutputs
                .SelectMany(
                    a => inToOut[codeIndex][a.Key],
                    (s, c) => (serialMin: c.digit + s.Value.serialMin, serialMax: c.digit + s.Value.serialMax, c.zin))
                .GroupBy(g => g.zin)
                .ToDictionary(g => g.Key, g => (g.Min(s => s.Item1), g.Max(s => s.serialMax)));
        }
#else

        var r = new Dictionary<int, (string serialMin, string serialMax)>() { [0] = ("", "") };
        for (int codeIndex = code.Count - 1; codeIndex >= 0; codeIndex--)
            r = GetPossibleZ(r, codeIndex);
#endif
        Advent.AssertAnswer1(r.Values.Min(s => s.serialMax));
        Advent.AssertAnswer2(r.Values.Min(s => s.serialMin));

        Dictionary<int, (string serialMin, string serialMax)> GetPossibleZ(Dictionary<int, (string serialMin, string serialMax)> possibleOutputs, int codeIndex, int high = 1000000)
        {
            var candidates = new Dictionary<int, (string serialMin, string serialMax)>();
            int minResult = possibleOutputs.Min(a => a.Key);
            int maxResult = possibleOutputs.Max(a => a.Key);
            int notFound = 0;
            for (int z = 0; z <= high; z++)
            {
                int resultTooHighCount = 0;
                string min = null;
                string max = null;
                int digitMin = 0;
                int digitMax = 0;
                for (int digit = 1; digit <= 9; digit++)
                {
                    int result = Execute(code[codeIndex], digit, z);
                    
                    if (result >= minResult && result <= maxResult && possibleOutputs.TryGetValue(result, out var serials))
                    {
                        if (max == null || string.Compare(serials.serialMax, max) >= 0)
                        {
                            max = serials.serialMax;
                            digitMax = digit;
                        }
                        if (min == null || string.Compare(serials.serialMin, min) <= 0)
                        {
                            min = serials.serialMin;
                            digitMin = digit;
                        }
                    }
                    if (result > maxResult)
                        resultTooHighCount++;
                }
                if (max != null)
                {
                    if (min == null)
                        throw new Exception();
                    candidates.Add(z, (digitMin + min, digitMax + max));
                    notFound = 0;
                }
                else if (++notFound > 1000000)
                    break;

            }
            //output.WriteLine($"{codeIndex}  {r.Count,-10} r = GetPossibleZ(r, --codeIndex, low: {r.Min(a => (int?)a.Key)}, high: {r.Max(a => (int?)a.Key)});");
            return candidates;
        }
    }

    private static int Execute(IList<Instr> code, int digit, int z)
    {
        int x = 0, y = 0, w = 0;

        void Store(char var, int value)
        {
            switch (var)
            {
                case 'w': w = value; break;
                case 'x': x = value; break;
                case 'y': y = value; break;
                case 'z': z = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(var), $"{var} is not a valid variable");
            };
        }

        foreach (var instr in code)
            ExecuteInstr(instr);
        return z;


        void ExecuteInstr(Instr instr)
        {
            int a, b;
            switch (instr.Op)
            {
                case Op.Inp:// inp a - Read an input value and write it to variable a.
                    Store(instr.V1, digit);
                    break;
                case Op.Add:// add a b - Add the value of a to the value of b, then store the result in variable a.
                    Store(instr.V1, GetV1() + GetV2());
                    break;
                case Op.Mul:// mul a b - Multiply the value of a by the value of b, then store the result in variable a.
                    Store(instr.V1, GetV1() * GetV2());
                    break;
                case Op.Div:// div a b - Divide the value of a by the value of b, truncate the result to an integer, then store the result in variable a. (Here, "truncate" means to round the value toward zero.)
                    a = GetV1();
                    b = GetV2();
                    if (b == 0) throw new Exception();
                    Store(instr.V1, GetV1() / GetV2());
                    break;
                case Op.Mod:// mod a b - Divide the value of a by the value of b, then store the remainder in variable a. (This is also called the modulo operation.)
                    a = GetV1();
                    b = GetV2();
                    if (a < 0) throw new Exception();
                    if (b <= 0) throw new Exception();
                    Store(instr.V1, a % b);
                    break;
                case Op.Eql:// eql a b - If the value of a and b are equal, then store the value 1 in variable a. Otherwise, store the value 0 in variable a.
                    Store(instr.V1, GetV1() == GetV2() ? 1 : 0);
                    break;
            }
            return;


            int GetV1()
            {
                return instr.V1 switch
                {
                    'w' => w,
                    'x' => x,
                    'y' => y,
                    'z' => z,
                };
            }

            int GetV2()
            {
                if (instr.V2 != '\0')
                return instr.V2 switch
                {
                    'w' => w,
                    'x' => x,
                    'y' => y,
                    'z' => z,
                    _ => throw new ArgumentOutOfRangeException(nameof(instr.V2))
                };
                return instr.Const2;
            }
        }
    }
    public enum Op { Inp, Add, Mul, Div, Mod, Eql }

    public record Instr(Op Op, char V1, char V2 = '\0', int Const2 = 0)
    {
        public static Instr Parse(string line)
        {
            var s = line.Split();
            Op op = Enum.Parse<Op>(s[0], ignoreCase: true);
            char v1 = s[1][0];
            if (op == Op.Inp)
                return new Instr(op, v1);
            if (char.IsLetter(s[2][0]))
                return new Instr(op, v1, V2: s[2][0]);
            return new Instr(op, v1, Const2: int.Parse(s[2]));
        }
    }
}
