using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static AdventOfCode2021.Helper;

namespace AdventOfCode2021
{
    public class Day02
    {
        private readonly ITestOutputHelper output;

        public Day02(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Run()
        {
            var input1 = ReadInputLines()
                .Select(c => int.Parse(c))
                .ToArray();

            AssertAnswer1("answer1");

            AssertAnswer2(2);
        }
    }
}
