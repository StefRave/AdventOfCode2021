namespace AdventOfCode2018;

public class Day20 : IAdvent
{
    public void Run()
    {
        string input = Advent.ReadInput();
        int size = Advent.UseSampleData ? 20 : 210;
        char[][] grid = Enumerable.Range(0, size).Select(i => "".PadRight(size, '#').ToCharArray()).ToArray();
        
        var startPos = (y: grid.Length / 2, x: grid[0].Length / 2);
        grid[startPos.y][startPos.x] = 'X';

        var cache = new HashSet<string>();
        BuildMaze(input[1..^1], startPos);
        Print();

        var (furthest, countGe1000) = FurthestRoomDoorCount(startPos);
        Advent.AssertAnswer1(furthest, 4286, 31);
        Advent.AssertAnswer2(countGe1000, 8638, 0);


        (int furthest, int countGe1000)FurthestRoomDoorCount((int y, int x) pos)
        {
            var queue = new Queue<((int y, int x), int doors)>();
            var visited = new bool[grid.Length, grid[0].Length];
            int countGe1000 = 0;

            queue.Enqueue((pos, 0));
            int maxDoors = 0;
            while (queue.Count > 0)
            {
                (pos, int doors) = queue.Dequeue();
                if (visited[pos.y, pos.x] || grid[pos.y][pos.x] == '#')
                    continue;
                maxDoors = doors;
                visited[pos.y, pos.x] = true;

                if (doors >= 1000)
                    countGe1000++;

                if (grid[pos.y - 1][pos.x] != '#')
                    queue.Enqueue(((pos.y - 2, pos.x), maxDoors + 1));
                if (grid[pos.y + 1][pos.x] != '#')
                    queue.Enqueue(((pos.y + 2, pos.x), maxDoors + 1));
                if (grid[pos.y][pos.x - 1] != '#')
                    queue.Enqueue(((pos.y, pos.x - 2), maxDoors + 1));
                if (grid[pos.y][pos.x + 1] != '#')
                    queue.Enqueue(((pos.y, pos.x + 2), maxDoors + 1));
            }
            return (maxDoors, countGe1000);
        }

        void BuildMaze(string pattern, (int y, int x) pos)
        {
            string cacheKey = $"{pattern},{pos}";
            if (!cache.Add(cacheKey))
                return;

            int index = 0;
            while (index < pattern.Length)
            {
                switch(pattern[index++])
                {
                    case 'N': grid[--pos.y][pos.x] = '-'; grid[--pos.y][pos.x] = '.'; break;
                    case 'S': grid[++pos.y][pos.x] = '-'; grid[++pos.y][pos.x] = '.'; break;
                    case 'W': grid[pos.y][--pos.x] = '|'; grid[pos.y][--pos.x] = '.'; break;
                    case 'E': grid[pos.y][++pos.x] = '|'; grid[pos.y][++pos.x] = '.'; break;
                    case '(':
                        int localIndex = index;
                        int localDepth = 0;
                        var separators = new List<int>();
                        while (pattern[localIndex] != ')' || localDepth != 0)
                        {
                            if (pattern[localIndex] == '|' && localDepth == 0)
                                separators.Add(localIndex);
                            localDepth += pattern[localIndex++] switch { '(' => 1, ')' => -1, _ => 0 };
                        }
                        var remaining = pattern[(localIndex + 1)..];

                        foreach (int separatorIndex in separators)
                        {
                            BuildMaze(string.Concat(pattern[index..separatorIndex], remaining), pos);
                            index = separatorIndex + 1;
                        }
                        BuildMaze(string.Concat(pattern[index..localIndex], remaining), pos);
                        index = pattern.Length;
                        break;
                    default: throw new Exception($"Unexpected char {pattern[index - 1]}");
                }
            }
        }

        void Print()
        {
            for (int y = 0; y < grid.Length; y++)
                Console.WriteLine(new string(grid[y]));
        }
    }
}