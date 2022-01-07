using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day09 : IAdvent
{
    public void Run()
    {
        var inputNumbers = Regex.Matches(Advent.ReadInput(), @"\d+").Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();
        int players = inputNumbers[0];
        int lastMarble = inputNumbers[1];

        long result = Play(players, lastMarble);

        Advent.AssertAnswer1(result, expected: 404611, sampleExpected: 32);

        result = Play(players, lastMarble * 100);
        Advent.AssertAnswer1(result, expected: 3350093681, sampleExpected: 22563);
    }

    private static long Play(int players, int lastMarble)
    {
        int[] field = new int[lastMarble + 1];
        int[] fieldC = new int[lastMarble + 1];
        long[] playerScores = new long[players];

        field[0] = 0;
        fieldC[0] = 0;
        int index = 0;
        for (int i = 1; i <= lastMarble; i++)
        {
            if (i % 23 == 0)
            {
                index = fieldC[index];
                index = fieldC[index];
                index = fieldC[index];
                index = fieldC[index];
                index = fieldC[index];
                index = fieldC[index];
                playerScores[i % players] += i + fieldC[index];
                fieldC[index] = fieldC[fieldC[index]];
                field[fieldC[index]] = index;
            }
            else
            {
                index = field[index];

                field[i] = field[index];
                field[index] = i;

                fieldC[i] = index;
                fieldC[field[i]] = i;

                index = i;
            }
            //PrintField(field);
        }
        return playerScores.Max();


        void PrintField(int[] field)
        {
            int i = 0;
            do
            {
                if (i == index)
                    Console.Write('(');
                Console.Write(i.ToString());
                if (i == index)
                    Console.Write(')');
                Console.Write(' ');

                i = field[i];
            }
            while (i != 0);
            
            Console.WriteLine("");
        }
    }
}