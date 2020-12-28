using System;
using System.Collections.Generic;

namespace AdventOfCode2019
{
    public class IntCode
    {
        public int[] Memory { get; }
        public List<int> Output { get; } = new List<int>();
        public Queue<int> Input { get; }
        public int ProgramCounter { get; private set; } = 0;

        public IntCode(int[] memory, IEnumerable<int> input)
        {
            this.Input = new Queue<int>(input ?? Array.Empty<int>());
            Memory = memory;
        }



        public static List<int> ExecuteProgramm(int[] memory, int[] input = null)
        {
            var intCode = new IntCode(memory, input);
            intCode.Run();
            return intCode.Output;
        }

        public void Run()
        {
            bool halt = false;

            while (!halt && ProgramCounter < Memory.Length)
            {
                (ProgramCounter, halt) = ExecuteInstruction(ProgramCounter);
            }
        }

        private (int index, bool halt) ExecuteInstruction(int index)
        {
            int opcode = Memory[index++];

            int mode = opcode / 100;
            opcode %= 100;
            if (opcode == 1)
            {
                int tmp = GetInput(mode) + GetInput(mode / 10);
                Memory[Memory[index++]] = tmp;
            }
            else if (opcode == 2)
            {
                int tmp = GetInput(mode) * GetInput(mode / 10);
                Memory[Memory[index++]] = tmp;
            }
            else if (opcode == 3)
            {
                if (!Input.TryDequeue(out int tmp))
                    throw new InputNeededException();
                Memory[Memory[index++]] = tmp;
            }
            else if (opcode == 4)
                Output.Add(GetInput(mode));
            else if (opcode == 5 || opcode == 6)
            {
                int tmp = GetInput(mode);
                int jmpPos = GetInput(mode / 10);
                if ((opcode == 5 && tmp != 0) || (opcode == 6 && tmp == 0))
                    index = jmpPos;
            }
            else if (opcode == 7 || opcode == 8)
            {
                int input1 = GetInput(mode);
                int input2 = GetInput(mode / 10);
                bool success = (opcode == 7 && input1 < input2) || (opcode == 8 && input1 == input2);
                Memory[Memory[index++]] = success ? 1 : 0;
            }
            else if (opcode == 99)
                return(index, halt: true);
            else
                throw new ArgumentOutOfRangeException(nameof(index), $"Unknown opcode {opcode}");

            return (index, halt: false);

            int GetInput(int opcodeDiv)
            {
                int input = Memory[index++];
                return (opcodeDiv % 10) switch
                {
                    0 => Memory[input],
                    1 => input,
                    int n => throw new ArgumentOutOfRangeException(nameof(opcodeDiv), $"{n}")
                };
            }
        }
    }

    public class InputNeededException : Exception
    {

    }
}
