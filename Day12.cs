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
                .Select(l => (instr: l[0], value: int.Parse(l[1..]);

            var ship = new Ship(0, 0, 0);
            foreach (var item in input)
                ship = ship.Execute(item.instr, item.value);
            output.WriteLine($"Part1: {ship.ManhattanDistance}");

            var shipWp = new ShipWaypoint(0, 0, 10, 1);
            foreach (var item in input)
                shipWp = shipWp.Execute(item.instr, item.value);
            output.WriteLine($"Part2: {shipWp.ManhattanDistance}");
        }

        record Ship(int X, int Y, int Angle)
        {
            public Ship Execute(char instr, int value)
                => instr switch
                {
                    'N' => this with { Y = Y + value },
                    'E' => this with { X = X + value },
                    'S' => this with { Y = Y - value },
                    'W' => this with { X = X - value },
                    'L' => this with { Angle = Angle - value },
                    'R' => this with { Angle = Angle + value },
                    'F' => this with { X = X + value * (int)Cos(Angle * PI / 180), Y = Y - value * (int)Sin(Angle * PI / 180) },
                };
            public int ManhattanDistance
                => Abs(X) + Abs(Y);
        }

        record ShipWaypoint(int X, int Y, int WX, int WY)
        {
            public ShipWaypoint Execute(char instr, int value)
                => instr switch
                {
                    'N' => this with { WY = WY + value },
                    'E' => this with { WX = WX + value },
                    'S' => this with { WY = WY - value },
                    'W' => this with { WX = WX - value },
                    'L' => ShipWaypointRotateWaypointRight(360 - value),
                    'R' => ShipWaypointRotateWaypointRight(value),
                    'F' => this with { X = X + value * WX, Y = Y + value * WY },
                };
            public ShipWaypoint ShipWaypointRotateWaypointRight(int angle)
                => angle switch
                {
                    90 => this with { WX = WY, WY = -WX },
                    180 => this with { WX = -WX, WY = -WY },
                    270 => this with { WX = -WY, WY = WX },
                };

            public int ManhattanDistance
                => Abs(X) + Abs(Y);
        }
    }
}