namespace AdventOfCode2021;

public class Day16 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput();
        var parser = new Parser(input);

        int totalV = 0;
        long result2 = ReadPacket();
        Advent.AssertAnswer1(totalV);
        Advent.AssertAnswer2(result2);


        long ReadPacket()
        {
            int v = parser.GetBits(3);
            totalV += v;
            int t = parser.GetBits(3);

            if (t == 4)
            {
                long result = 0;
                while (true)
                {
                    int g = parser.GetBits(5);
                    result = (result << 4) + (g & 0xf);
                    if ((g & 0x10) == 0)
                        return result;
                }
            }
            var values = new List<long>();
            int lengthType = parser.GetBits(1);
            if (lengthType == 0)
            {
                int subPacketBitsLength = parser.GetBits(15);
                int readUntil = parser.Position + subPacketBitsLength;
                while (parser.Position < readUntil)
                    values.Add(ReadPacket());
            }
            else
            {
                int numberOfSubPackets = parser.GetBits(11);
                for (int i = 0; i < numberOfSubPackets; i++)
                    values.Add(ReadPacket());
            }
            return t switch
            {
                0 => values.Sum(),
                1 => values.Aggregate(1L, (acc, val) => acc * val),
                2 => values.Aggregate(long.MaxValue, (acc, val) => Math.Min(acc, val)),
                3 => values.Aggregate(long.MinValue, (acc, val) => Math.Max(acc, val)),
                5 => values[0] > values[1] ? 1 : 0,
                6 => values[0] < values[1] ? 1 : 0,
                7 => values[0] == values[1] ? 1 : 0,
                _ => throw new InvalidOperationException($"unexpected {t}")
            };
        }
    }

    public class Parser
    {
        private string input;
        int index = 0;
        int bits;
        int bitsLeft;

        public int Position => index * 4 - bitsLeft;

        public Parser(string input) => this.input = input;

        public int GetBits(int numberOfBitsNeeded)
        {
            while (bitsLeft < numberOfBitsNeeded)
            {
                bits = (bits << 4) + Convert.ToInt32(input.Substring(index++, 1), 16);
                bitsLeft += 4;
            }
            bitsLeft -= numberOfBitsNeeded;
            return (bits >> bitsLeft) & (0xffff >> (16 - numberOfBitsNeeded));
        }
    }
}
