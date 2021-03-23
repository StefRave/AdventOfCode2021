using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2019
{
    public class Day08
    {
        public Day08(ITestOutputHelper output)
        {
            this.output = output;
        }

        int width = 25;
        int tall = 6;
        private readonly ITestOutputHelper output;

        private static string GetInput()
        {
            return File.ReadAllLines(@"Input/input08.txt")[0];
        }

        [Fact]
        public void DoPart1()
        {
            string input = GetInput();
            string[] layerStrings = GetLayers(input);
            var layerFewest0 = layerStrings
                .Select((l, layer) => (cnt: l.Count(c => c == '0'), layer))
                .OrderBy(c => c.cnt)
                .First().layer;
            int cnt1 = layerStrings[layerFewest0].Count(c => c == '1');
            int cnt2 = layerStrings[layerFewest0].Count(c => c == '2');
            Assert.Equal(1596, cnt1 * cnt2);
        }

        private  string[] GetLayers(string input)
        {
            int numberLayers = input.Length / width / tall;

            string[] layerStrings =
                Enumerable.Range(0, numberLayers)
                .Select(i => input.Substring(i * width * tall, width * tall))
                .ToArray();
            return layerStrings;
        }

        [Fact]
        public void DoPart2()
        {
            string input = GetInput();
            string[] layerStrings = GetLayers(input);
            var sb = new StringBuilder(layerStrings[0]);
            foreach (var layer in layerStrings.Skip(1))
            {
                for (int i = 0; i < layer.Length; i++)
                    sb[i] = sb[i] != '2' ? sb[i] : layer[i];
            }
            string result = sb.ToString();
            for (int i = 0; i < tall; i++)
                output.WriteLine(result.Substring(i * width, width).Replace("1", "░").Replace("0", "█"));
        }

    }
}
