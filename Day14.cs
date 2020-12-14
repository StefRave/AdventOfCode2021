using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day14
    {
        private readonly ITestOutputHelper output;

        public Day14(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay14()
        {
            var input =
                from m in Regex.Matches(File.ReadAllText("input/input14.txt"), @"mask = (\w+)\r?\n(?:mem\[(\d+)\] = (\d+)\r?\n?)*")
                let maskString = m.Groups[1].Value.PadLeft(64, 'X')
                select new
                {
                    OrMask = Convert.ToUInt64(maskString.Replace('X', '0'), 2),
                    AndMask = Convert.ToUInt64(maskString.Replace('X', '1'), 2),
                    XMask = Convert.ToUInt64(maskString.Replace("1", "0").Replace('X', '1'), 2),
                    Assignments = m.Groups[2].Captures.Select((c, i) => (loc: int.Parse(c.Value), val: ulong.Parse(m.Groups[3].Captures[i].Value))).ToArray()
                };
            var memory = new Dictionary<ulong, ulong>();
            foreach (var item in input)
                foreach (var assignment in item.Assignments)
                    memory[(ulong)assignment.loc] = assignment.val & item.AndMask | item.OrMask;

            long result = memory.Sum(v => (long)v.Value);
            output.WriteLine($"Part1: {result}");

            memory = new Dictionary<ulong, ulong>();
            foreach (var item in input)
                foreach (var assignment in item.Assignments)
                {
                    var bits = new List<ulong>();
                    for (ulong i = 1; i < (1UL << 36); i <<= 1)
                        if ((item.XMask & i) != 0)
                            bits.Add(i);

                    for (int i = 0; i < 1 << bits.Count; i++)
                    {
                        ulong loc = (uint)assignment.loc | item.OrMask;
                        int j = i;
                        foreach (var bit in bits)
                        {
                            if((j & 1) == 1)
                                loc ^= bit;
                            j >>= 1;
                        }
                        memory[loc] = assignment.val;
                    }
                }

            result = memory.Sum(v => (long)v.Value);
            output.WriteLine($"Part2: {result}");
        }
    }
}