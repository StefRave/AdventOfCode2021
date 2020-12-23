using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day23
    {
        private readonly ITestOutputHelper output;

        public Day23(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay23()
        {
            var input = "538914762";

            var cup1Node = Play(input.Select(c => c - '0'), 100);
            string result = GetNumbersAfter1(cup1Node);
            output.WriteLine($"Part1: {result}");

            cup1Node = Play(input.Select(c => c - '0').Concat(Enumerable.Range(10, 1_000_000 - input.Length)), 10_000_000);
            output.WriteLine($"Part2: {(long)cup1Node.Next.Value * cup1Node.Next.Next.Value}");
        }

        private static string GetNumbersAfter1(LinkedListNode<int> cup)
        {
            string result = "";
            while (true)
            {
                cup = cup.Next ?? cup.List.First;
                if (cup.Value == 1)
                    break;
                result += cup.Value.ToString();
            }
            return result;
        }

        private LinkedListNode<int> Play(IEnumerable<int> input, int iterations)
        {
            var cups = new LinkedList<int>(input);
            LinkedListNode<int> current = cups.First;

            var nodesByValue = new Dictionary<int, LinkedListNode<int>>();
            while(current != null)
            {
                nodesByValue.Add(current.Value, current);
                current = current.Next;
            }

            current = cups.First;
            int max = input.Max();
            List<LinkedListNode<int>> pickUp = new();
            for (int i = 0; i < iterations; i++)
            {
                int currentValue = current.Value;
                for (int j = 0; j < 3; j++)
                {
                    if (current.Next != null)
                    {
                        pickUp.Add(current.Next);
                        cups.Remove(current.Next);
                    }
                    else
                    {
                        pickUp.Add(cups.First);
                        cups.Remove(cups.First);
                    }
                }
                int destination = currentValue;
                do
                {
                    destination--;
                    if (destination == 0)
                        destination = max;
                } while (pickUp.Any(n => n.Value == destination));

                var pos = nodesByValue[destination];
                for (int j = 2; j >= 0; j--)
                {
                    cups.AddAfter(pos, pickUp[j]);
                }
                pickUp.Clear();

                if (current.Next == null)
                    current = cups.First;
                else
                    current = current.Next;
            }
            return nodesByValue[1];
        }
    }
}