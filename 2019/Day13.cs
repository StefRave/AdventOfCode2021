using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public class Day13
    {
        private static List<long> GetInput() => File.ReadAllText(@"Input/input13.txt").Split(",").Select(long.Parse).ToList();


        [Fact]
        public void DoParts()
        {
            int count = CountBlocks();
            Assert.Equal(260, count);

            var memory = GetInput();
            memory[0] = 2;
            var intCode = new IntCode(memory, new long[0]);

            var output = intCode.Output;
            int outputIndex = 0;
            var screen = new FlexArray2D<char>();
            int highScore = 0;
            int ballPosX = 0;
            int ballPosY = 0;
            bool ballUp = true;
            bool ballRight = true;
            int paddlePosX = 0;
            int paddlePosY = 0;
            string display;

            intCode.InputProvider = () =>
            {
                DoTurn();
            };
            intCode.Run();

            DoTurn();

            Assert.Equal(12952, highScore);

            void DoTurn()
            {
                while (outputIndex < output.Count)
                {
                    int x = (int)output[outputIndex];
                    int y = (int)output[outputIndex + 1];
                    int t = (int)output[outputIndex + 2];
                    outputIndex += 3;

                    if (x == -1)
                        highScore = Math.Max(highScore,t);
                    else
                        screen[y][x] = t switch
                        {
                            0 => ' ', // 0 is an empty tile.No game object appears in this tile.
                            1 => '#', // 1 is a wall tile.Walls are indestructible barriers.
                            2 => 'x', // 2 is a block tile.Blocks can be broken by the ball.
                            3 => 'W', // 3 is a horizontal paddle tile. The paddle is indestructible.
                            4 => 'o', // 4 is a ball tile.The ball moves diagonally and bounces off objects.
                            _ => throw new ArgumentOutOfRangeException("type of object")
                        };
                    switch (t)
                    {
                        case 4:
                            ballUp = ballPosY > y;
                            ballRight = ballPosX < x;
                            ballPosX = x;
                            ballPosY = y;
                            break;
                        case 3:
                            paddlePosX = x;
                            paddlePosY = y;
                            break;
                    }
                }
                int wallMin = screen[0].Min + 1;
                int wallMax = screen[0].Max - 1;

                display = screen.AsString();

                if (paddlePosX == ballPosX)
                    intCode.Input.Enqueue(0);
                else if (paddlePosX > ballPosX)
                    intCode.Input.Enqueue(-1);
                else
                    intCode.Input.Enqueue(1);
            }
        }

        private static int CountBlocks()
        {
            var memory = GetInput();
            //memory[0] = 2;
            var intCode = new IntCode(memory, new long[0]);
            intCode.InputProvider = () => { intCode.Input.Enqueue(0); };
            intCode.Run();

            int count = 0;
            var output = intCode.Output;
            var screen = new FlexArray2D<char>();
            for (int i = 0; i < output.Count; i += 3)
            {
                if (output[i + 2] == 2)
                    count++;
            }

            return count;
        }
    }
}
