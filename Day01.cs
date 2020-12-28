using System;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public class Day01
    {
        [Theory]
        [InlineData(12,2)]
        [InlineData(14, 2)]
        [InlineData(1969, 654)]
        public void TestExamples(long mass, long expectedFeul)
        {
            Assert.Equal(expectedFeul, MassToFeul(mass));
        }

        [Theory]
        [InlineData(14, 2)]
        [InlineData(1969, 966)]
        [InlineData(100756, 50346)]
        public void TestRecursiveFeul(long mass, long expectedFeul)
        {
            Assert.Equal(expectedFeul, MassToFeulRecursive(mass));
        }

        public static long MassToFeul(long mass) => Math.Max(0L, mass / 3 - 2);
        public static long MassToFeulRecursive(long mass)
        {
            long totalFuel = 0;
            while(mass > 0)
            {
                long fuel = MassToFeul(mass);
                totalFuel += fuel;
                mass = fuel;
            }
            return totalFuel;
        }

        private static long[] GetInput()
        {
            return File.ReadAllLines(@"Input/input01.txt")
                .Select(long.Parse)
                .ToArray();
        }

        [Fact]
        public void DoPart1() => 
            Assert.Equal(3488702L, GetInput().Select(MassToFeul).Sum());

        [Fact]
        public void DoPart2() =>
            Assert.Equal(5230169L, GetInput().Select(MassToFeulRecursive).Sum());

    }
}
