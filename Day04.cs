using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2021
{
    public class Day04
    {
        private readonly ITestOutputHelper output;

        public Day04(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Run()
        {
            var input = Advent.ReadInput()
                .SplitByDoubleNewLine();
            int[] drawnNumbers = input[0].Split(',').Select(int.Parse).ToArray();
            var boards = input.Skip(1).Select(line => line.SplitByNewLine()
                .Select(row => row.Trim().Replace("  ", " ").Split(' ').Select(num => new BoardPlace(false, int.Parse(num))).ToArray()
                ).ToArray()).ToHashSet();

            int lastDrawnNumber = 0;
            var finishedBoards = new List<(BoardPlace[][] board, int winningNumber)>();
            foreach (var drawnNumber in drawnNumbers)
            {
                lastDrawnNumber = drawnNumber;
                foreach (var board in boards.ToArray())
                {
                    MarkNumberInBoard(board, drawnNumber);
                    if(HasCompleteRowOrColumn(board))
                    {
                        finishedBoards.Add((board, drawnNumber));
                        boards.Remove(board);
                    }
                }
                if (boards.Count == 0)
                    break;
            }
            var firstBoard = finishedBoards[0];
            int unMarkedTotal = AddUnmarked(firstBoard.board);
            Advent.AssertAnswer1(unMarkedTotal * firstBoard.winningNumber);

            var lastBoard = finishedBoards[^1];
            unMarkedTotal = AddUnmarked(lastBoard.board);
            Advent.AssertAnswer2(unMarkedTotal * lastBoard.winningNumber);
        }

        private int AddUnmarked(BoardPlace[][] board)
            => board.SelectMany(row => row).Where(bp => !bp.Marked).Sum(bp => bp.Number);

        private bool HasCompleteRowOrColumn(BoardPlace[][] board)
        {
            for (int i = 0; i < board.Length; i++)
                if (board[i].All(bp => bp.Marked))
                    return true;

            for (int i = 0; i < board[0].Length; i++)
                if (board.All(bp => bp[i].Marked))
                    return true;
            return false;
        }

        private void MarkNumberInBoard(BoardPlace[][] board, int drawnNumber)
        {
            for (int i = 0; i < board.Length; i++)
                for (int j = 0; j < board[0].Length; j++)
                    if (board[i][j].Number == drawnNumber)
                        board[i][j] = board[i][j] with { Marked = true };
        }
    }

    public record BoardPlace(bool Marked, int Number);
}
