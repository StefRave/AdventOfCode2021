using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static System.Math;

namespace AdventOfCode2020
{
    public class Day12
    {
        private readonly ITestOutputHelper output;

        public Day12(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay12()
        {
            var input = File.ReadAllLines("input/input12.txt")
                .Select(l => (instr: l[0], value: int.Parse(l[1..])));

            var ship = input.Aggregate(
                new Ship(new Coord(0, 0), new Coord(1, 0)),
                (s, item) => s.Execute(item.instr, item.value, Ship.MoveShip));
            output.WriteLine($"Part1: {ship.Pos.ManhattanDistance}");

            ship = input.Aggregate(
                new Ship(new Coord(0, 0), new Coord(10, 1)),
                (s, item) => s.Execute(item.instr, item.value, Ship.MoveWaypoint));
            output.WriteLine($"Part2: {ship.Pos.ManhattanDistance}");
        }

        record Coord(int X, int Y)
        {
            public Coord Move(char instr, int value)
                => instr switch
                {
                    'N' => this with { Y = Y + value },
                    'E' => this with { X = X + value },
                    'S' => this with { Y = Y - value },
                    'W' => this with { X = X - value },
                };

            public Coord Rotate(int angle)
                => angle switch
                {
                    90 => new Coord(Y, -X),
                    180 => new Coord(-X, -Y),
                    270 => new Coord(-Y, X),
                };

            public int ManhattanDistance
                => Abs(X) + Abs(Y);
        }

        record Ship(Coord Pos, Coord WayPoint)
        {
            public Ship Execute(char instr, int value, MoveDelegate move)
                => instr switch
                {
                    'N' or 'E' or 'S' or 'W' => move(this, instr, value),
                    'L' => this with { WayPoint = WayPoint.Rotate(360 - value) },
                    'R' => this with { WayPoint = WayPoint.Rotate(value) },
                    'F' => this with { Pos = new Coord(Pos.X + value * WayPoint.X, Pos.Y + value * WayPoint.Y) },
                };

            public static Ship MoveWaypoint(Ship ship, char instr, int value)
                => ship with { WayPoint = ship.WayPoint.Move(instr, value) };

            public static Ship MoveShip(Ship ship, char instr, int value)
                => ship with { Pos = ship.Pos.Move(instr, value) };


            public delegate Ship MoveDelegate(Ship ship, char instr, int value);
        }
    }
}