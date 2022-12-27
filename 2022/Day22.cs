using System.IO;

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

                return MoveNearEdge(p, facing);


                (V2 p, int facing) MoveNearEdge(V2 p, int facing)
                {
                    var direction = new Vector3(1, 0, 0);
                    var normal = new Vector3(0, 0, 1);
                    var start = new Vector3(0, 0, 0);
                    var pos = start;

                    facing = (facing + 1) % 4;
                    do
                    {
                        pos = pos + direction;

                        int quadrantOld = p.Y / cubeSideSize * 10 + p.X / cubeSideSize;
                        p = p + MoveD[facing];

                        bool posIn = IsInMaze(p);
                        bool trackIn = IsInMaze(p + MoveD[(facing + 3) % 4]);
                        if (posIn && !trackIn)
                        {
                            int quadrantNew = p.Y / cubeSideSize * 10 + p.X / cubeSideSize;
                            if (quadrantNew != quadrantOld)
                            {
                                (direction, normal) = CubeMoveStraightCorner(direction, normal);
                                pos = pos + direction;
                            }
                        }
                        else if (trackIn)
                        {
                            facing = (facing + 3) % 4;
                            p = p + MoveD[facing];
                            (direction, normal) = CubeMoveClosedCorner(direction, normal);
                            pos = pos + direction;
                        }
                        else
                        {
                            p = p - MoveD[facing]; // move back
                            facing = (facing + 1) % 4;
                            (direction, normal) = CubeMoveOpenCorner(direction, normal);
                            pos = pos + direction;
                        }
                    }
                    while (pos != start);
                    facing = (facing + 1) % 4;

                    return (p, facing);
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

    static (Vector3 m, Vector3 n) CubeMoveClosedCorner(Vector3 m, Vector3 n)
    {
        if (m.X != 0)
            n = n.RotateX(90 * m.X);
        else if (m.Y != 0)
            n = n.RotateY(90 * m.Y);
        else
            n = n.RotateZ(90 * m.Z);
        return (m.Reverse(), n);
    }

    static (Vector3 m, Vector3 n) CubeMoveOpenCorner(Vector3 m, Vector3 n)
    {
        if (n.X != 0)
            return (m.RotateX(-90 * n.X), n);
        else if (n.Y != 0)
            return (m.RotateY(-90 * n.Y), n);
        else
            return (m.RotateZ(-90 * n.Z), n);
    }

    static (Vector3 m, Vector3 n) CubeMoveStraightCorner(Vector3 m, Vector3 n)
    {
        return (n, m.Reverse());
    }

    public record Vector3(int X, int Y, int Z)
    {
        public int Length() => X * X + Y * Y + Z * Z;
        public int ManhattanDistance() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static (int cos, int sin) GetRotationValues(int angle)
        {
            angle = angle % 360;
            angle = angle < 0 ? angle + 360 : angle;
            return angle switch
            {
                0 => (1, 0),
                90 => (0, 1),
                180 => (-1, 0),
                270 => (0, -1),
                _ => throw new ArgumentException("Rotation must be a multiple of 90 degrees.")
            };
        }

        public Vector3 Reverse() => new Vector3(-X, -Y, -Z);

        public Vector3 RotateX(int angle)
        {
            (int cos, int sin) = GetRotationValues(angle);
            return new Vector3(X, Y * cos - Z * sin, Y * sin + Z * cos);
        }

        public Vector3 RotateY(int angle)
        {
            (int cos, int sin) = GetRotationValues(angle);
            return new Vector3(Z * sin + X * cos, Y, Z * cos - X * sin);
        }

        public Vector3 RotateZ(int angle)
        {
            (int cos, int sin) = GetRotationValues(angle);
            return new Vector3(X * cos - Y * sin, X * sin + Y * cos, Z);
        }
    }
}