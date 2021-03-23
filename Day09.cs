using FluentAssertions;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public partial class Day09
    {
        private static string GetInput() => File.ReadAllText(@"Input/input09.txt");

        private static long[] ToLongs(string v) => v.Split(",").Select(long.Parse).ToArray();


        [Theory]
        [InlineData("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99", "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99")]
        [InlineData("1102,34915192,34915192,7,4,7,99,0", "1219070632396864")]
        [InlineData("104,1125899906842624,99", "1125899906842624")]
        public void DoTests1(string input, string output)
        {
            long[] memory = ToLongs(input);

            var result = IntCode.ExecuteProgramm(memory, new long[] { });

            result.Should().BeEquivalentTo(ToLongs(output));
        }


        [Fact]
        public void DoTests3()
        {
            long[] memory = ToLongs("104,1125899906842624,99");

            var result = IntCode.ExecuteProgramm(memory, new long[] {  });

            Assert.Equal(1125899906842624, result[0]);
        }



        [Fact]
        public void DoPart1()
        {
            long[] memory = ToLongs(GetInput());
            var input = new long[] {1};
            var result = IntCode.ExecuteProgramm(memory, input);

            result.Should().HaveCount(1);
            Assert.Equal(2377080455, result[0]);
        }

        [Fact]
        public void DoPart2()
        {
            long[] memory = ToLongs(GetInput());
            var input = new long[] { 2 };
            var result = IntCode.ExecuteProgramm(memory, input);

            result.Should().HaveCount(1);
            Assert.Equal(74917, result[0]);
        }
    }
}
