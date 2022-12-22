using System.Drawing;

namespace AdventOfCode2022;

public class Day22 : IAdvent
{
    public record V2(int X, int Y)
    {
        public static V2 operator -(V2 a, V2 b) => new V2(a.X - b.X, a.Y - b.Y);
        public static V2 operator +(V2 a, V2 b) => new V2(a.X + b.X, a.Y + b.Y);
    }
    static char[] Facing = new[] { '>', 'v', '<', '^' };
    static V2[] MoveD = new V2[] { new V2(1, 0), new V2(0, 1), new V2(-1, 0), new V2(0, -1) };
    public void Run()
    {
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine()
            .ToArray();
        string[] strings = input[0].SplitByNewLine();
        int maxX = strings.Select(s => s.Length).Max();

        Dictionary<V2, char> maze = strings.Select((line, y) => line.PadRight(maxX).Select((c, x) => (p: new V2(x, y), c)))
            .SelectMany(a => a)
            .ToDictionary(p => p.p, p => p.c);

        string[] instructions = Regex.Matches(input[1], @"\d+|\D+").Select(m => m.Value).ToArray();

        int maxY = maze.Select(m => m.Key.Y).Max() + 1;
        var p = new V2(0, 0);
        while (maze[p] != '.')
            p = p + MoveD[0];
        int cubeSideSize = Math.Max(maxY, maxX) / 4;


        int answer1 = DoIt(maze, instructions, p, part: 1);
        Advent.AssertAnswer1(answer1, expected: 65368, sampleExpected: 6032);


        int answer2 = DoIt(maze, instructions, p, part: 2);
        Advent.AssertAnswer2(answer2, expected: 156166, sampleExpected: 5031);


        int DoIt(Dictionary<V2, char> maze, string[] instructions, V2 p, int part)
        {
            int facing = 0;
            maze[p] = Facing[facing];
            int move = 0;
            foreach (var instr in instructions)
            {
                move++;

                if (instr == "L")
                    facing = (facing + 3) % 4;
                else if (instr == "R")
                    facing = (facing + 1) % 4;
                else
                {
                    int moves = int.Parse(instr);
                    for (int i = 0; i < moves; i++)
                    {

                        (V2 newPos, int newFacing) = part == 1 ? MovePart1(p) : MovePart2(p, facing);
                        if (newPos.Y < 0)
                            1.ToString();
                        if (maze[newPos] == '#')
                            break;
                        p = newPos;
                        facing = newFacing;
                        maze[p] = Facing[facing];
                    }

                    if (Advent.UseSampleData)
                        Draw.DrawYx(maze.Select(m => (m.Key.Y, m.Key.X, m.Value)), move);
                }
            }
            maze[p] = '!';
            if (Advent.UseSampleData)
                Draw.DrawYx(maze.Select(m => (m.Key.Y, m.Key.X, m.Value)), move);

            int answer = 1000 * (p.Y + 1) + 4 * (p.X + 1) + facing;
            return answer;

            (V2, int) MovePart2(V2 p, int facing)
            {
                V2 newPos = p;
                newPos = p + MoveD[facing];
                if (IsInMaze(newPos))
                    return (newPos, facing);

                var (pLeft, facingLeft, distLeft) = DoItLeftRight(p, facing, left: true);
                var (pRigh, facingRigh, distRigh) = DoItLeftRight(p, facing, left: false);

                if (distLeft < distRigh)
                    return (pLeft, facingLeft);
                return (pRigh, facingRigh);


                (V2 p, int facing, int dist) DoItLeftRight(V2 p, int facing, bool left)
                {
                    int turnExtra = left ? 0 : 2;
                    bool foldFound = false;
                    facing = (facing + 3 + turnExtra) % 4;
                    int dist = 0;
                    int distAdd = 1;
                    int totalDistance = 0;
                    string path = "";
                    do
                    {
                        totalDistance++;
                        dist += distAdd;

                        int quadrantOld = p.Y / cubeSideSize * 10 + p.X / cubeSideSize;
                        p = p + MoveD[facing];

                        bool posIn = IsInMaze(p);
                        bool trackIn = IsInMaze(p + MoveD[(facing + 1 + turnExtra) % 4]);
                        if (posIn && !trackIn)
                        {
                            int quadrantNew = p.Y / cubeSideSize * 10 + p.X / cubeSideSize;
                            if (quadrantNew != quadrantOld)
                                path += "-";
                        }
                        else if (trackIn)
                        {
                            dist -= distAdd;
                            facing = (facing + 1 + turnExtra) % 4;
                            distAdd = -1;
                            p = p + MoveD[facing];
                            foldFound = true;
                            path += "c";
                        }
                        else
                        {
                            dist -= distAdd;
                            bool outSide = p.X < 0 || p.X >= maxX || p.Y < 0 || p.Y >= maxY;
                            p = p - MoveD[facing]; // move back
                            facing = (facing + 3 + turnExtra) % 4;
                            distAdd = foldFound & outSide ? -1 : 1;
                            if (!foldFound || outSide)
                                dist += distAdd;
                            path += "o";
                        }
                    }
                    while (dist > 0 && totalDistance < cubeSideSize * 8);
                    facing = (facing + 3 + turnExtra) % 4;

                    if (path == "oco")
                        totalDistance = 1000;
                    return (p, facing, totalDistance);
                }
            }

            bool IsInMaze(V2 p) => (p.X >= 0) && (p.Y >= 0) && (p.X < maxX) && (p.Y < maxY) && maze[p] != ' ';

            (V2, int) MovePart1(V2 p)
            {
                V2 newPos = p;
                do
                {
                    newPos = newPos + MoveD[facing];
                    if (newPos.X < 0)
                        newPos = new V2(maxX - 1, newPos.Y);
                    else if (newPos.Y < 0)
                        newPos = new V2(newPos.X, maxY - 1);
                    else if (newPos.X >= maxX)
                        newPos = new V2(0, newPos.Y);
                    else if (newPos.Y >= maxY)
                        newPos = new V2(newPos.X, 0);
                }
                while (maze[newPos] == ' ');
                return (newPos, facing);
            }
        }
    }
}