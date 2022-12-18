namespace AdventOfCode2022;

public class Day18 : IAdvent
{
    record V3(int X, int Y, int Z);

    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line =>
            {
                var ints = line.Split(',').Select(int.Parse).ToArray();
                return new V3(ints[0], ints[1], ints[2]);
            })
            .ToArray();

        var max = new V3(
            input.Select(v => v.X).Max(),
            input.Select(v => v.Y).Max(),
            input.Select(v => v.Z).Max());
        var grid = new bool[max.X + 1, max.Y + 1, max.Z + 1];

        foreach (var line in input)
            grid[line.X, line.Y, line.Z] = true;


        int answer1 = Count() * 2;
        Advent.AssertAnswer1(answer1, expected: 3550, sampleExpected: 64);


        Fill();
        int answer2 = Count() * 2;
        Advent.AssertAnswer2(answer2, expected: 2028, sampleExpected: 58);



        int Count()
        {
            int count = 0;
            for (int x = 0; x <= max.X; x++)
            {
                for (int y = 0; y <= max.Y; y++)
                {
                    bool free = true;
                    for (int z = 0; z <= max.Z; z++)
                    {
                        if (grid[x, y, z])
                            if (free)
                                count++;
                        free = !grid[x, y, z];
                    }
                }
            }

            for (int z = 0; z <= max.Z; z++)
            {
                for (int x = 0; x <= max.X; x++)
                {
                    bool free = true;
                    for (int y = 0; y <= max.Y; y++)
                    {
                        if (grid[x, y, z])
                            if (free)
                                count++;
                        free = !grid[x, y, z];
                    }
                }
            }

            for (int y = 0; y <= max.Y; y++)
            {
                for (int z = 0; z <= max.Z; z++)
                {
                    bool free = true;
                    for (int x = 0; x <= max.X; x++)
                    {
                        if (grid[x, y, z])
                            if (free)
                                count++;
                        free = !grid[x, y, z];
                    }
                }
            }
            return count;
        }

        void Fill()
        {
            var move = new[] { new V3(0, -1, 0), new V3(0, 1, 0), new V3(1, 0, 0), new V3(-1, 0, 0), new V3(0, 0, 1), new V3(0, 0, -1) };

            var outSide = new bool[max.X + 1, max.Y + 1, max.Z + 1];
            FloodOutSide(new V3(0, 0, 0));

            for (int x = 0; x <= max.X; x++)
                for (int y = 0; y <= max.Y; y++)
                    for (int z = 0; z <= max.Z; z++)
                        grid[x, y, z] |= !outSide[x, y, z];

            void FloodOutSide(V3 pos)
            {
                if (outSide[pos.X, pos.Y, pos.Z])
                    return;
                outSide[pos.X, pos.Y, pos.Z] = true;
                foreach (var d in move)
                {
                    var np = new V3(pos.X + d.X, pos.Y + d.Y, pos.Z + d.Z);
                    if (np.X >= 0 && np.Y >= 0 && np.Z >= 0 && np.X <= max.X && np.Y <= max.Y && np.Z <= max.Z)
                        if (!grid[np.X, np.Y, np.Z])
                            FloodOutSide(np);
                }
            }
        }
    }
}