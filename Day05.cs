using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public partial class Day05
    {
        private static string GetInput() => File.ReadAllText(@"Input/input05.txt");

        private static int[] ParseInput(string v) => v.Split(",").Select(int.Parse).ToArray();


        [Theory]
        [InlineData("3,9,8,9,10,9,4,9,99,-1,8", 8, 1)]
        [InlineData("3,9,8,9,10,9,4,9,99,-1,8", 7, 0)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8", 7, 1)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8", 9, 0)]
        [InlineData("3,3,1108,-1,8,3,4,3,99", 8, 1)]
        [InlineData("3,3,1108,-1,8,3,4,3,99", 7, 0)]
        [InlineData("3,3,1107,-1,8,3,4,3,99", 7, 1)]
        [InlineData("3,3,1107,-1,8,3,4,3,99", 9, 0)]
        [InlineData(@"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 7, 999)]
        [InlineData(@"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 8, 1000)]
        [InlineData(@"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 9, 1001)]
        public void DoTests(string programm, int input, int expectedResult)
        {
            int[] memory = ParseInput(programm);

            var result = IntCode.ExecuteProgramm(memory, new int[] { input });

            Assert.Equal(expectedResult, result[0]);
        }


        [Fact]
        public void Test34()
        {
            var memory = ParseInput("3,0,4,0,99");
            var input = new int[] { 1234 };
            var result = IntCode.ExecuteProgramm(memory, input);
            Assert.Equal(result[0], memory[0]);
        }

        [Fact]
        public void DoPart1()
        {
            var memory = ParseInput(GetInput());
            var input = new int[] { 1 };
            var result = IntCode.ExecuteProgramm(memory, input);

            Assert.Equal(6069343, result.Last());
        }

        [Fact]
        public void DoPart2()
        {
            var memory = ParseInput(GetInput());
            var input = new int[] { 5 };
            var result = IntCode.ExecuteProgramm(memory, input);

            Assert.Equal(3188550, result.Last());
        }
    }
}
