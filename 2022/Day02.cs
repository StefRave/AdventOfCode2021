namespace AdventOfCode2022;

public class Day02 : IAdvent
{
    enum Rps { Rock, Paper, Scissor};
    enum RoundResult { Lose = 'X', Draw = 'Y', Win = 'Z'};
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(l => (Elf: (Rps)(l[0] - 'A'), Me: (Rps)(l[2] - 'X'), DesiredResult: (RoundResult)l[2]))
            .ToArray();

        int score1 = input
            .Select(e => DoRound(e.Elf, e.Me))
            .Sum();
        Advent.AssertAnswer1(score1, expected: 15572, sampleExpected: 15);

        int score2 = input
            .Select(e => DoRound(e.Elf, GetMyMove(e.Elf, e.DesiredResult)))
            .Sum();
        Advent.AssertAnswer2(score2, expected: 16098, sampleExpected: 12);
    }

    private static Rps GetMyMove(Rps elf, RoundResult desiredResult) 
        => (Rps)(((int)elf + 1 + (int)desiredResult % 3) % 3);

    private static int DoRound(Rps elf, Rps me)
    {
        int score = me switch
        {
            Rps.Rock => 1,
            Rps.Paper => 2,
            Rps.Scissor => 3,
            _ => 0
        };

        if (elf == me)
            score += 3;
        if (me == (Rps)(((int)elf + 1) % 3))
            score += 6;

        return score;
    }
}