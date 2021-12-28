using AdventOfCode2019.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public partial class Day06
    {
        private static string[][] GetInput() => ParseInput(File.ReadAllText(@"Input/input06.txt"));

        private static string[][] ParseInput(string v) => v.SplitByNewLine().Select(l => l.Split(')')).ToArray();


        private static string[][] GetTestInput() => ParseInput(@"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L
");

        [Fact]
        public void DoTests()
        {
            var input = GetTestInput();

            long total = Do(input).Item1;

            Assert.Equal(42, total);
        }

        private static (long, Dictionary<string, string>) Do(string[][] input)
        {
            var dict = input.ToDictionary(i => i[1], i => i[0]);

            int total = 0;
            foreach (var item in dict)
            {
                string val = item.Value;
                while (val != null)
                {
                    total++;
                    dict.TryGetValue(val, out val);
                }
            }
            return (total, dict);
        }

        [Fact]
        public void DoPart1()
        {
            var input = GetInput();

            long total = Do(input).Item1;

            Assert.Equal(417916, total);
        }

        [Fact]
        public void DoPart2()
        {
            var input = GetInput();

            var dict = Do(input).Item2;
            var b1 = GetFrom("YOU").Reverse().ToArray();
            var b2 = GetFrom("SAN").Reverse().ToArray();
            int same = 0;
            while (b1[same] == b2[same])
                same++;

            Assert.Equal(523, b1.Length + b2.Length - same * 2);

            IEnumerable<string> GetFrom(string val)
            {
                while (true)
                {
                    dict.TryGetValue(val, out val);
                    if (val == null)
                        break;
                    yield return val;
                }
            }
        }
    }
}
