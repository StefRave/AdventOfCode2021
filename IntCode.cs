using System;
using System.Collections.Generic;

namespace AdventOfCode2019
{
    public class IntCode
    {
        public ExtendedMemory Memory { get; }
        public List<long> Output { get; } = new List<long>();
        public Queue<long> Input { get; }
        public int ProgramCounter { get; private set; } = 0;
        public long RelativeBase { get; set; }

        public IntCode(ICollection<long> memory, IEnumerable<long> input)
        {
            this.Input = new Queue<long>(input ?? Array.Empty<long>());
            Memory = new ExtendedMemory(memory);
        }



        public static List<long> ExecuteProgramm(ICollection<long> memory, long[] input = null)
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

        private (int index, bool halt) ExecuteInstruction(int opcodeOffset)
        {
            int index = opcodeOffset;
            long opcode = Memory[index++];

            int mode = (int)(opcode / 100);
            opcode %= 100;
            if (opcode == 1)
            {
                long tmp = GetInput(mode) + GetInput(mode / 10);
                SetMemory(tmp, mode / 100);
            }
            else if (opcode == 2)
            {
                long tmp = GetInput(mode) * GetInput(mode / 10);
                SetMemory(tmp, mode / 100);
            }
            else if (opcode == 3)
            {
                if (!Input.TryDequeue(out long tmp))
                    throw new InputNeededException();
                SetMemory(tmp, mode);
            }
            else if (opcode == 4)
                Output.Add(GetInput(mode));
            else if (opcode == 5 || opcode == 6)
            {
                long tmp = GetInput(mode);
                long jmpPos = GetInput(mode / 10);
                if ((opcode == 5 && tmp != 0) || (opcode == 6 && tmp == 0))
                    index = (int)jmpPos;
            }
            else    if (opcode == 7 || opcode == 8)
            {
                long input1 = GetInput(mode);
                long input2 = GetInput(mode / 10);
                bool success = (opcode == 7 && input1 < input2) || (opcode == 8 && input1 == input2);
                SetMemory(success ? 1 : 0, mode / 100);
            }
            else if (opcode == 9) // add to relative base
            {
                long input1 = GetInput(mode);
                RelativeBase += input1;
            }
            else if (opcode == 99)
                return(index, halt: true);
            else
                throw new ArgumentOutOfRangeException(nameof(index), $"Unknown opcode {opcode}");

            return (index, halt: false);

            void SetMemory(long value, int opcodeDiv)
            {
                long input = Memory[index++];
                
                if (opcodeDiv == 0)
                    Memory[input] = value;
                else if(opcodeDiv == 2)
                    Memory[RelativeBase + input] = value;
                else
                    throw new NotImplementedException();
            }

            long GetInput(int opcodeDiv)
            {
                long input = Memory[index++];
                return (opcodeDiv % 10) switch
                {
                    0 => Memory[input],
                    1 => input,
                    2 => Memory[RelativeBase + input],
                    int n => throw new ArgumentOutOfRangeException(nameof(opcodeDiv), $"{n}")
                };
            }
        }
    }

    public class ExtendedMemory
    {
        private IList<long> memory;

        public int Length => memory.Count;

        public ExtendedMemory(ICollection<long> memory)
        {
            this.memory = memory as List<long> ?? new List<long>(memory);
        }

        public long this[long location]
        {
            get 
            {
                if (location >= memory.Count)
                    return 0;
                return memory[(int)location];
            }
            set 
            {
                while (location >= memory.Count)
                    memory.Add(0);
                memory[(int)location] = value;
            }
        }
    }

    public class InputNeededException : Exception
    {

    }
}
