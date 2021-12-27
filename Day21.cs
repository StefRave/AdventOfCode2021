namespace AdventOfCode2021;

public class Day21 : IAdvent
{
    private static readonly int[] occurencesFor3Rolls = new int[] { 0, 0, 0, 1, 3, 6, 7, 6, 3, 1 };

    [Fact]
    public void Run()
    {
        var playerStartingPos = Advent.ReadInputLines()
            .Select(line => new PosScore(int.Parse(line[^1..]), 0))
            .ToArray();

        (int throws, int losingPlayerScore) = PlayNormal(playerStartingPos[0], playerStartingPos[1]);
        Advent.AssertAnswer1(losingPlayerScore * throws);

        var (player1Wins, player2Wins) = QuantumPlay(playerStartingPos[0], playerStartingPos[1]);
        Advent.AssertAnswer2(Math.Max(player1Wins, player2Wins));
    }
    private static (long playerAWins, long playerBWins) QuantumPlay(PosScore playerA, PosScore playerB)
    {
        long playerAWins = 0;
        long playerBWins = 0;
        for (int i = 3; i <= 9; i++)
        {
            var newPos = ((playerA.Pos + i - 1) % 10) + 1;
            if (playerA.Score + newPos >= 21)
                playerAWins += occurencesFor3Rolls[i];
            else
            {
                var (wp2, wp1) = QuantumPlay(playerB, new PosScore(newPos, playerA.Score + newPos));
                playerAWins += wp1 * occurencesFor3Rolls[i];
                playerBWins += wp2 * occurencesFor3Rolls[i];
            }
        }
        return (playerAWins, playerBWins);
    }

    private static (int throws, int losingPlayerScore) PlayNormal(PosScore playerA, PosScore playerB, int currentThrow = 0)
    {
        int moveSpaces = (++currentThrow + ++currentThrow + ++currentThrow) % 10;
        int newPos = ((playerA.Pos + moveSpaces - 1) % 10) + 1;
        int newScore = playerA.Score + newPos;
        if (newScore >= 1000)
            return (currentThrow, playerB.Score);
        return PlayNormal(playerB, new PosScore(newPos, newScore), currentThrow);
    }

    public record PosScore(int Pos, int Score);
}