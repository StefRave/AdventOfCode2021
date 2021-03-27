using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2019
{
    public class Day14
    {
        private readonly ITestOutputHelper output;

        public Day14(ITestOutputHelper output)
        {
            this.output = output;
        }
        private static string[] GetInput() => File.ReadAllLines(@"Input/input14.txt");

        record Item(int Count, string Name);
        record Ingr(Item Item, Item[] Needs);

        private static Ingr[] ParseLines(string[] input)
        {
            var ingredients =
                from line in input
                let m = Regex.Match(line, @"(?:(\d+) (\w+),? ?)+ => (\d+) (\w+)")
                select new Ingr(new Item(int.Parse(m.Groups[3].Value), m.Groups[4].Value),
                    Enumerable.Range(0, m.Groups[1].Captures.Count).Select(i => new Item(int.Parse(m.Groups[1].Captures[i].Value), m.Groups[2].Captures[i].Value)).ToArray());
            return ingredients.ToArray();
        }


        [Fact]
        public void Example1()
        {
            string[] testInput = @"10 ORE => 10 A
1 ORE => 1 B
7 A, 1 B => 1 C
7 A, 1 C => 1 D
7 A, 1 D => 1 E
7 A, 1 E => 1 FUEL".SplitByNewLine();

            var ingredients = ParseLines(testInput).ToDictionary(ingr => ingr.Item.Name);
            long ore = CalculateNeededOre(ingredients, new Dictionary<string, long>());

            Assert.Equal(31, ore);
        }
        private const long totalOreSupply = 1000000000000;

        [Fact]
        public void Example2()
        {
            string[] testInput = @"157 ORE => 5 NZVS
165 ORE => 6 DCFZ
44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
179 ORE => 7 PSHF
177 ORE => 5 HKGWZ
7 DCFZ, 7 PSHF => 2 XJWVT
165 ORE => 2 GPVTF
3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT".SplitByNewLine();

            var ingredients = ParseLines(testInput).ToDictionary(ingr => ingr.Item.Name);
            var produce = new Dictionary<string, long>();
            long ore = CalculateNeededOre(ingredients, produce, 1);

            Assert.Equal(13312, ore);

            long totalFuel = CalculateMaxFeul(ingredients);

            Assert.Equal(82892753, totalFuel);
        }

        private static long CalculateMaxFeul(Dictionary<string, Ingr> ingredients)
        {
            long totalFuel = 0;
            long ore = 0;
            long makeFeul = 1;
            var produce = new Dictionary<string, long>();

            while (ore < totalOreSupply)
            {
                ore = CalculateNeededOre(ingredients, produce, makeFeul);
                totalFuel += makeFeul;
                
                makeFeul = (totalOreSupply - ore) / (ore / totalFuel + 1) + 1;
            }
            return totalFuel - 1;
        }

        private static long CalculateNeededOre(Dictionary<string, Ingr> ingredients, Dictionary<string, long> produce, long amountOfFeul = 1)
        {
            Produce("FUEL", amountOfFeul);
            void Produce(string name, long count)
            {
                if (name == "ORE")
                {
                    produce.TryGetValue(name, out long ore);
                    ore += count;
                    produce[name] = ore;
                    return;
                }
                Ingr ingr = ingredients[name];
                
                produce.TryGetValue(name, out long currentCount);
                long hasProduced = ((currentCount + ingr.Item.Count - 1) / ingr.Item.Count) * ingr.Item.Count;
                currentCount += count;
                long needsProduced = ((currentCount + ingr.Item.Count - 1) / ingr.Item.Count) * ingr.Item.Count;
                produce[name] = currentCount;

                long needNew = needsProduced - hasProduced;
                if (needNew > 0)
                {
                    foreach (var item in ingr.Needs)
                        Produce(item.Name, needNew * item.Count / ingr.Item.Count);
                }
            }

            return produce["ORE"];
        }



        [Fact]
        public void DoParts()
        {

            var ingredients = ParseLines(GetInput()).ToDictionary(ingr => ingr.Item.Name);
            long ore = CalculateNeededOre(ingredients, new Dictionary<string, long>());

            Assert.Equal(892207, ore);

            long totalFuel = CalculateMaxFeul(ingredients);

            Assert.Equal(82892753, totalFuel);
        }
    }
}
