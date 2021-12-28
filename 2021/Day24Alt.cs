namespace AdventOfCode2021;

public class Day24Alt : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(' '))
            .Select(sl => new Instr(sl[0], sl[1], sl.Length >= 3 ? sl[2] : null))
            .ToArray();
        var instrsArray = Chunked(input, instr => instr.Opcode == "inp").ToArray();
        Assert.Equal(14, instrsArray.Length);

        var cache = new Dictionary<(int instrIndex, int z), string>();
        string result = GetMaxSerial(0, 0, Enumerable.Range(1, 9).Reverse().ToArray());
        Advent.AssertAnswer1(result);

        cache = new();
        result = GetMaxSerial(0, 0, Enumerable.Range(1, 9).ToArray());
        Advent.AssertAnswer2(result);

        string GetMaxSerial(int instrIndex, int z, int[] digits)
        {
            if (z > 1000000)
                return null;
            string result = null;
            if (cache.TryGetValue((instrIndex, z), out result))
                return result;


            foreach (int digit in digits)
            {
                int newZ = Execute(instrsArray[instrIndex], z, digit);
                if (instrIndex == instrsArray.Length - 1)
                {
                    if (newZ == 0)
                    {
                        result = "" + digit;
                        break;
                    }
                }
                else
                {
                    result = GetMaxSerial(instrIndex + 1, newZ, digits);
                    if (result != null)
                    {
                        result = digit + result;
                        break;
                    }
                }
            }
            cache.Add((instrIndex, z), result);
            return result;
        }
    }

    public int Execute(Instr[] instrs, int z, int input)
    {
        int w = 0, x = 0, y = 0;
        foreach (var instr in instrs)
        {
            switch (instr.Opcode)
            {
                case "inp": Set(instr.Var1, input);break; // inp a - Read an input value and write it to variable a.
                case "add": Set(instr.Var1, Get(instr.Var1) + Get(instr.Var2)); break; // add a b - Add the value of a to the value of b, then store the result in variable a.
                case "mul": Set(instr.Var1, Get(instr.Var1) * Get(instr.Var2)); break; // mul a b - Multiply the value of a by the value of b, then store the result in variable a.
                case "div": Set(instr.Var1, Get(instr.Var1) / Get(instr.Var2)); break; // div a b - Divide the value of a by the value of b, truncate the result to an integer, then store the result in variable a. (Here, "truncate" means to round the value toward zero.)
                case "mod": Set(instr.Var1, Get(instr.Var1) % Get(instr.Var2)); break; // mod a b - Divide the value of a by the value of b, then store the remainder in variable a. (This is also called the modulo operation.)
                case "eql": Set(instr.Var1, Get(instr.Var1) == Get(instr.Var2) ? 1 : 0); break; // eql a b - If the value of a and b are equal, then store the value 1 in variable a. Otherwise, store the value 0 in variable a.
                default: throw new ArgumentException(nameof(instr.Opcode), $"Unexpected {instr.Opcode}");
            }
        }
        return z;


        int Get(string varName)
        {
            return varName switch
            {
                "w" => w,
                "x" => x,
                "y" => y,
                "z" => z,
                _ => int.Parse(varName),
            };
        }
        void Set(string varName, int value)
        {
            switch(varName)
            {
                case "w": w = value; break;
                case "x": x = value; break;
                case "y": y = value; break;
                case "z": z = value; break;
                default: throw new ArgumentNullException(nameof(varName), $"Unexpected varname {varName}");
            };
        }
    }

    public static IEnumerable<T[]> Chunked<T>(IEnumerable<T> input, Predicate<T> predicate)
    {
        var list = new List<T>();
        foreach (T val in input)
        {
            if (list.Count == 0)
                list.Add(val);
            else
            {
                if (predicate(val))
                {
                    yield return list.ToArray();
                    list = new();
                }
                list.Add(val);
            }
        }
        if (list.Count > 0)
            yield return list.ToArray();
    }
    public record Instr(string Opcode, string Var1, string Var2);
}
