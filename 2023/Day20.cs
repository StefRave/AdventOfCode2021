using System.Diagnostics;

namespace AdventOfCode2023;

public class Day20 : IAdvent
{
    [DebuggerDisplay("{Type}{Source}={State} ->{Outputs}  <-{Inputs}")]
    class Module(string Source, char Type, string[] Outputs)
    {
        public string Source { get; } = Source;
        public char Type { get; } = Type;
        public string[] Outputs { get; } = Outputs;
        public bool State { get; set; }
        public bool[] LastPulseOfInput { get; set; }
        public Module[] Inputs { get; set; }

        static public Module Parse(string line)
        {
            var g = Regex.Match(line, @"^([%&])?(\w+) -> (.*)$").Groups.AsArray();
            return new Module(g[2], g[1].Length == 1 ? g[1][0] : ' ' , g[3].Split(", "));
        }
    }

    public static string Hl(bool b) => b ? "high" : "low";
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(Module.Parse)
            .ToArray();
        var dict = input.ToDictionary(l => l.Source);
        var inputsForConjunction = 
            (
            from m in dict.Values
            where m.Type != ' '
            from d in m.Outputs
            group m by d into g
            where dict.ContainsKey(g.Key)
            let targetM = dict[g.Key]
            select (targetM, inputs: g.ToArray())
            ).ToDictionary(l => l.targetM.Source, l => l.inputs);
        foreach (var (targetM, inputs) in inputsForConjunction)
        {
            dict[targetM].Inputs = inputs;
            dict[targetM].LastPulseOfInput = new bool[inputs.Length]; 
        }

        var signals = new Queue<(string, bool, Module)>();
        long countLow = 0;
        long countHigh = 0;
        var rxp = Advent.UseSampleData ? null : input.First(m => m.Outputs[0] == "rx");
        var periods = new Dictionary<string, (int,int)>();
        int periodsComplete = 0;
        long answer2 = 0;
        for (int i = 1; answer2 == 0; i++)
        {
            Enqueue(dict["broadcaster"], false);
            countLow++;
            while (signals.Count > 0)
            {
                var (name, pulse, lastInput) = signals.Dequeue();
                if (!dict.TryGetValue(name, out var module))
                    continue;
                
                if (!Advent.UseSampleData)
                {
                    if (module.Outputs[0] == rxp.Source && !pulse)
                    {
                        if (i == 1205436)
                            1.ToString();
                        if (!periods.TryGetValue(name, out var period))
                            periods[name] = (0, i);
                        else
                        {
                            if (period.Item1 == 0)
                                periodsComplete++;
                            periods[name] = (i - period.Item2, i);
                        }
                        if (periodsComplete == rxp.Inputs.Length)
                        {
                            answer2 = periods.Values.Select(t => (long)t.Item1).Aggregate((a, b) => a * b / GCD(a, b));
                            break;
                        }
                    }
                }
                switch (module.Type)
                {
                    case ' ':
                        module.State = false;
                        Enqueue(module, false);
                        break;
                    case '%': // Flip-flop
                        if (!pulse)
                        {
                            module.State = !module.State;
                            Enqueue(module, module.State);
                        }
                        break;
                    case '&': // Conjunction
                        bool allPulse = true;
                        for (int j = 0; j < module.Inputs.Length; j++)
                        {
                            if (module.Inputs[j] == lastInput)
                                module.LastPulseOfInput[j] = pulse;
                            allPulse &= module.LastPulseOfInput[j];
                        }
                        module.State = !allPulse;
                        Enqueue(module, !allPulse);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (i == 1000)
            {
                Advent.AssertAnswer1(countLow * countHigh, expected: 791120136, sampleExpected: 11687500);
                if (Advent.UseSampleData)
                    break;
            }
        }
        if (!Advent.UseSampleData)
            Advent.AssertAnswer2(answer2, expected: 215252378794009, sampleExpected: 2020);


        void Enqueue(Module module, bool pulse)
        {
            foreach (var output in module.Outputs)
            {
                if (pulse)
                    countHigh++;
                else
                    countLow++;
                signals.Enqueue((output, pulse, module));
            }
            //Console.WriteLine($"{module.Type}{module.Source} -{Hl(module.State)} -> {string.Join(",", module.Outputs)}");
        }
    }

    static long GCD(long a, long b) => b == 0 ? a : GCD(b, a % b);
}
