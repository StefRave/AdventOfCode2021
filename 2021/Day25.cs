namespace AdventOfCode2021;

public class Day25 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines();
        Dictionary<(int y, int x), char> dict = input
            .Select((line, y) => line.Select((c, x) => (x, y, c)))
            .SelectMany(l => l)
            .Where(l => l.c != '.')
            .ToDictionary(kv => (kv.y, kv.x), kv => kv.c);
        int maxY = input.Length;
        int maxX = input[0].Length;
        
        int step = 0;
        Print();
        
        while(step++ < 1000)
        {
            (dict, bool hasChangeX) = Move(dict, '>', yx => (yx.y, (yx.x + 1) % maxX));
            (dict, bool hasChangeY) = Move(dict, 'v', yx => ((yx.y + 1) % maxY, yx.x));
            if (!hasChangeX && !hasChangeY)
                break;
        }
        Print();
        Advent.AssertAnswer1(step);


        static (Dictionary<(int y, int x), char>, bool hasChanged) Move(Dictionary<(int y, int x), char> dict, char cType, Func<(int y, int x), (int y, int x)> movement)
        {
            var oldDict = dict;
            dict = new();
            bool hasChanged = false;
            foreach (var (yx, c) in oldDict)
            {
                var yxNew = movement(yx);
                bool canMove = c == cType && !oldDict.ContainsKey(yxNew);
                dict[canMove ? yxNew : yx] = c;
                hasChanged = hasChanged || canMove;
            }
            return (dict, hasChanged);
        }

        void Print()
        {
            Console.WriteLine($"\nStep {step}");
            for (int y = 0; y < maxY; y++)
            {
                string line = "";
                for (int x = 0; x < maxX; x++)
                    line += dict.ContainsKey((y, x)) ? dict[(y, x)] : '.';
                Console.WriteLine(line);
            }
        }
    }
}