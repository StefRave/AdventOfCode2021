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
            var cardsPlayer1 = input[0].SplitByNewLine().Skip(1).Select(v => (char)int.Parse(v));
            var cardsPlayer2 = input[1].SplitByNewLine().Skip(1).Select(v => (char)int.Parse(v));

            output.WriteLine($"Part1: {DoIt(recurse: false)}");
            output.WriteLine($"Part2: {DoIt(recurse: true)}");

            long DoIt(bool recurse)
            {
                var deck1 = new Queue<char>(cardsPlayer1);
                var deck2 = new Queue<char>(cardsPlayer2);
                bool player1Wins = Play(deck1, deck2, recurse);
                long result = (player1Wins ? deck1 : deck2).Reverse().Select((v, i) => v * (i + 1L)).Sum();
                return result;
            }
        }

        private static bool Play(Queue<char> player1, Queue<char> player2, bool recurse)
        {
            HashSet<string> player1History = new();
            HashSet<string> player2History = new();

            bool player1wins = true;
            int round = 0;
            while (player1.Count != 0 && player2.Count != 0)
            {
                string player1String = new string(player1.ToArray());
                string player2String = new string(player2.ToArray());
                if (player1History.Contains(player1String) || player2History.Contains(player2String))
                    return true;
                player1History.Add(player1String);
                player2History.Add(player2String);

                round++;
                char p1v = player1.Dequeue();
                char p2v = player2.Dequeue();
                if (recurse && p1v <= player1.Count && p2v <= player2.Count)
                    player1wins = Play(new Queue<char>(player1.Take(p1v)), new Queue<char>(player2.Take(p2v)), true);
                else
                    player1wins =  p1v > p2v;
                if (player1wins)
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
            return player1wins;
        }
    }
}