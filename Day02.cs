using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public class Day02
    {


        private static int[] GetTestInput() => ParseInput("1,9,10,3,2,3,11,0,99,30,40,50");

        private static int[] GetInput() => ParseInput(File.ReadAllText(@"Input/input02.txt"));

        private static int[] ParseInput(string v) => v.Split(",").Select(int.Parse).ToArray();

        [Fact]
        public void DoPart1Test()
        {
            int[] memory = GetTestInput();

            IntCode.ExecuteProgramm(memory);

            Assert.Equal(3500, memory[0]);
        }

        [Fact]
        public void DoPart1()
        {
            int[] memory = GetInput().ToArray();
            memory[1] = 12;
            memory[2] = 2;
            IntCode.ExecuteProgramm(memory);

            Assert.Equal(5434663, memory[0]);
        }

        [Fact]
        public void DoPart2()
        {
            int i, j = 0;
            for (i = 0; i < 100; i++)
                for (j = 0; j < 100; j++)
                {
                    int[] memory = GetInput().ToArray();
                    memory[1] = i;
                    memory[2] = j;
                    IntCode.ExecuteProgramm(memory, null);

                    if (memory[0] == 19690720)
                        goto found;
                }
            found:
            Assert.Equal(4559, i * 100 + j);
        }

        
    }
}
