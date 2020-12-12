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
                .Select(l => new Instr(l[0], int.Parse(l[1..])));

            var ship = new Ship(0, 0, 0);
            foreach (var item in input)
                ship = ship.Execute(item);
            output.WriteLine($"Part1: {ship.ManhattanDistance}");

            var shipWp = new ShipWaypoint(0, 0, 10, 1);
            foreach (var item in input)
                shipWp = shipWp.Execute(item);
            output.WriteLine($"Part2: {shipWp.ManhattanDistance}");
        }

        record Instr(char C, int Value);
        record Ship(int X, int Y, int Angle)
        {
            public Ship Execute(Instr instr)
                => instr.C switch
                {
                    'N' => this with { Y = Y + instr.Value },
                    'E' => this with { X = X + instr.Value },
                    'S' => this with { Y = Y - instr.Value },
                    'W' => this with { X = X - instr.Value },
                    'L' => this with { Angle = Angle - instr.Value },
                    'R' => this with { Angle = Angle + instr.Value },
                    'F' => this with { X = X + instr.Value * (int)Cos(Angle * PI / 180), Y = Y - instr.Value * (int)Sin(Angle * PI / 180) },
                };
            public int ManhattanDistance
                => Abs(X) + Abs(Y);
        }

        record ShipWaypoint(int X, int Y, int WX, int WY)
        {
            public ShipWaypoint Execute(Instr instr)
                => instr.C switch
                {
                    'N' => this with { WY = WY + instr.Value },
                    'E' => this with { WX = WX + instr.Value },
                    'S' => this with { WY = WY - instr.Value },
                    'W' => this with { WX = WX - instr.Value },
                    'L' => ShipWaypointRotateWaypointRight(360 - instr.Value),
                    'R' => ShipWaypointRotateWaypointRight(instr.Value),
                    'F' => this with { X = X + instr.Value * WX, Y = Y + instr.Value * WY },
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