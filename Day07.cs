using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public partial class Day07
    {
        private static string GetInput() => File.ReadAllText(@"Input/input07.txt");

        private static int[] ParseInput(string v) => v.Split(",").Select(int.Parse).ToArray();


        
        [Theory]
        [InlineData("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0", "4,3,2,1,0", 43210)]
        [InlineData("3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0", "0,1,2,3,4", 54321)]
        [InlineData("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5", "9,8,7,6,5", 139629729)]
        public void DoTests1(string programm, string phaseString, int expectedResult)
        {
            int[] memory = ParseInput(programm);
            int[] phases = ParseInput(phaseString);

            int maxThrusterSignal = GetMaxThrusterSignal(memory, phases);
            Assert.Equal(expectedResult, maxThrusterSignal);
        }


        private static int GetMaxThrusterSignal(int[] memory, int[] phases)
        {
            int maxThrusterSignal = int.MinValue;
            var orders = GetPermutations(phases, phases.Length).ToArray();
            int[] bestOrder = null;
            foreach (var order in orders.Select(o => o.ToArray()))
            {
                int thrusterSignal = DoLoop(memory, order, 0);
                if (thrusterSignal > maxThrusterSignal)
                {
                    maxThrusterSignal = thrusterSignal;
                    bestOrder = order;
                }
            }

            return maxThrusterSignal;
        }

        private static int DoLoop(int[] memory, int[] order, int thrusterSignal = 0)
        {
            bool allHalted = false;
            var intCode = order.Select(val => new IntCode(memory.ToArray(), new int[] { val })).ToArray();

            while (!allHalted)
            {
                bool anyAborted = false;
                for (int i = 0; i < order.Length; i++)
                {
                    intCode[i].Input.Enqueue(thrusterSignal);
                    try
                    {
                        intCode[i].Run();
                    }
                    catch(InputNeededException)
                    {
                        anyAborted = true;
                    }
                    thrusterSignal = intCode[i].Output[^1];
                }
                allHalted = !anyAborted;
            }

            return thrusterSignal;
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        [Fact]
        public void DoPart1()
        {
            var memory = ParseInput(GetInput());
            var phases = new int[] { 0,1,2,3,4 };

            int maxThrusterSignal = GetMaxThrusterSignal(memory, phases);
            Assert.Equal(225056, maxThrusterSignal);
        }

        [Fact]
        public void DoPart2()
        {
            var memory = ParseInput(GetInput());
            var phases = new int[] { 5,6,7,8,9 };

            int maxThrusterSignal = GetMaxThrusterSignal(memory, phases);
            Assert.Equal(14260332, maxThrusterSignal);
        }
    }
}
