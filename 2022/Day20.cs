using System.Collections.Generic;

namespace AdventOfCode2022;

public class Day20 : IAdvent
{
    public class Item
    {
        public long Value { get; set; }
        public Item Next { get; set; }
        public Item Prev { get; set; }
    }
    
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(int.Parse)
            .ToArray();

        long answer1 = DoIt(input);
        Advent.AssertAnswer1(answer1, expected: 2203, sampleExpected: 3);
        

        long answer2 = DoIt(input, iterations: 10, decryptionKey: 811589153);
        Advent.AssertAnswer2(answer2, expected: 6641234038999, sampleExpected: 1623178306);
    }

    private static long DoIt(int[] input, int iterations = 1, long decryptionKey = 1)
    {
        var nodes = Init.Array(() => new Item(), input.Length);

        for (int i = 0; i < input.Length; i++)
        {
            nodes[i].Value = input[i];
            nodes[i].Next = nodes[(i + 1) % input.Length];
            nodes[i].Prev = nodes[(i + input.Length - 1) % input.Length];
        }

        for (int hoi = 0; hoi < iterations; hoi++)
        {
            for (int step = 0; step < input.Length; step++)
            {
                var current = nodes[step];
                long val = current.Value;
                if (val == 0)
                    continue;
                long moves = val * decryptionKey;
                moves = moves % (input.Length - 1);
                if (moves < 0)
                    moves += input.Length - 1;

                var moveTo = current.Next;
                current.Prev.Next = moveTo;
                moveTo.Prev = current.Prev;

                for (int j = 1; j < moves; j++)
                    moveTo = moveTo.Next;

                current.Next = moveTo.Next;
                moveTo.Next.Prev = current;
                current.Prev = moveTo;
                moveTo.Next = current;
            }
        }
        long sum = 0;
        var c = nodes.First(n => n.Value == 0);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 1000; j++)
                c = c.Next;
            sum += c.Value * decryptionKey;
        }
        return sum;
    }
}