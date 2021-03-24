using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2019
{
    public class Day12
    {
        private readonly ITestOutputHelper output;

        public Day12(ITestOutputHelper output)
        {
            this.output = output;
        }
        private static string[] GetInput() => File.ReadAllLines(@"Input/input12.txt");

        private static Moon[] ParseLines(string[] testInput)
        {
            var moons =
                from line in testInput
                let m = Regex.Matches(line, @"(-?\d+)")
                select new Moon(new Vector(int.Parse(m[0].Value), int.Parse(m[1].Value), int.Parse(m[2].Value)), new Vector(0, 0, 0));
            return moons.ToArray();
        }


        [Fact]
        public void Example()
        {
            string[] testInput = @"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>".SplitByNewLine();

            var moons = ParseLines(testInput);
            for (int i = 0; i < 10; i++)
                moons = DoIteration(moons);

            int totalEnergy = moons.Sum(m => m.Energy);
        }

        private Moon[] DoIteration(Moon[] moons)
        {
            return moons
                .Select(m => m with { Vel = CalculateNewVelocity(moons, m) })
                .Select(m => m with { Pos = m.Pos + m.Vel })
                .ToArray();
        }

        private Vector CalculateNewVelocity(Moon[] moons, Moon m)
        {
            return new Vector(
                m.Vel.X + moons.Sum(nm => Compare1(nm.Pos.X, m.Pos.X)),
                m.Vel.Y + moons.Sum(nm => Compare1(nm.Pos.Y, m.Pos.Y)),
                m.Vel.Z + moons.Sum(nm => Compare1(nm.Pos.Z, m.Pos.Z)));

            int Compare1(int a, int b) => a == b ? 0 : a > b ? 1 : -1;
        }

        private bool MatchX(Moon[] ma, Moon[] mb)
        {
            for (int i = 0; i < ma.Length; i++)
            {
                if (ma[i].Pos.X != mb[i].Pos.X || ma[i].Vel.X != mb[i].Vel.X)
                    return false;
            }
            return true;
        }

        private bool MatchY(Moon[] ma, Moon[] mb)
        {
            for (int i = 0; i < ma.Length; i++)
            {
                if (ma[i].Pos.Y != mb[i].Pos.Y || ma[i].Vel.Y != mb[i].Vel.Y)
                    return false;
            }
            return true;
        }
        private bool MatchZ(Moon[] ma, Moon[] mb)
        {
            for (int i = 0; i < ma.Length; i++)
            {
                if (ma[i].Pos.Z != mb[i].Pos.Z || ma[i].Vel.Z != mb[i].Vel.Z)
                    return false;
            }
            return true;
        }

        [Fact]
        public void DoParts()
        {
            var moons = ParseLines(GetInput());
            for (int j = 0; j < 1000; j++)
                moons = DoIteration(moons);

            int totalEnergy = moons.Sum(m => m.Energy);
            Assert.Equal(10189, totalEnergy);


            int imx = 0, imy = 0, imz = 0;
            int i = 0;
            var moonsOrig = ParseLines(GetInput());
            moons = moonsOrig;
            while (imx == 0 || imy == 0 || imz == 0)
            {
                moons = DoIteration(moons);
                i++;
                if (imx == 0 && MatchX(moonsOrig, moons))
                    imx = i;
                if (imy == 0 && MatchY(moonsOrig, moons))
                    imy = i;
                if (imz == 0 && MatchZ(moonsOrig, moons))
                    imz = i;
            }
            var primes = new int[]{ 2, 3, 5, 7, 11, 13, };
            long possibleResult = (long)imx * imy * imz;
            foreach (var prime in primes)
            {
                while(true)
                {
                    long check = possibleResult / prime;
                    if (possibleResult % prime != 0 || (check % imx) != 0 || (check % imy) != 0 || (check % imz) != 0)
                        break;
                    possibleResult = check;
                }
            }
            Assert.Equal(469671086427712L, possibleResult);
        }

        record Vector(int X, int Y, int Z)
        {
            public static Vector operator +(Vector a, Vector b)
                => new Vector(a.X + b.X, b.Y + a.Y, a.Z + b.Z);
            public int Energy => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }
        record Moon(Vector Pos, Vector Vel)
        {
            public int Energy => Pos.Energy * Vel.Energy;
        }
    }
}
