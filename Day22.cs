using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day22
    {
        private readonly ITestOutputHelper output;

        public Day22(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay22()
        {
            var input =
                File.ReadAllText("input/input22.txt").SplitByDoubleNewLine();
            var player1 = new Queue<int>(input[0].SplitByNewLine().Skip(1).Select(int.Parse));
            var player2 = new Queue<int>(input[1].SplitByNewLine().Skip(1).Select(int.Parse));

            while(player1.Count != 0 && player2.Count != 0)
            {
                string p1C = string.Join(", ", player1);
                string p2C = string.Join(", ", player2);
                int p1v = player1.Dequeue();
                int p2v = player2.Dequeue();
                if(p1v > p2v)
                {
                    player1.Enqueue(p1v);
                    player1.Enqueue(p2v);
                }
                else
                {
                    player2.Enqueue(p2v);
                    player2.Enqueue(p1v);
                }
            }
            var winner = ((player1.Count > 0) ? player1 : player2).Reverse().ToArray();
            long result = winner.Select((v, i) => v * (i + 1L)).Sum();

            output.WriteLine($"Part1: {result}");

            //output.WriteLine($"Part2: {dangerousIngredients}");
        }


    }
}