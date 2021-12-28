using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day7
    {
        private readonly ITestOutputHelper output;

        public Day7(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay7()
        {
            var input = File.ReadAllText("input/input7.txt");

            var matches = Regex.Matches(input, @"^(\w+ \w+) bags contain(?: (\d*) ?(\w+ \w+) bags?[,.])+", RegexOptions.Multiline);

            var itemsPerBag = matches.ToDictionary(m =>
                m.Groups[1].Value,
                m => m.Groups[2].Captures.Select((c, i) => new { Count = int.Parse("0" + c.Value), Bag = m.Groups[3].Captures[i].Value }).ToArray());

            output.WriteLine($"Part1: {CountBagsContaining("shiny gold")}");
            output.WriteLine($"Part2: {CountAllBagsInsideBayWith("shiny gold")}");

            int CountBagsContaining(string bag)
                => itemsPerBag.Count(item => HasBagOrBagInside(item.Key, bag));

            bool HasBagOrBagInside(string bag, string lookFor) 
                => itemsPerBag[bag].Any(item => item.Bag == lookFor || (item.Bag != "no other" && HasBagOrBagInside(item.Bag, lookFor)));

            int CountAllBagsInsideBayWith(string bag)
            {
                return itemsPerBag[bag]
                    .Where(i => i.Bag != "no other")
                    .Sum(i => i.Count * (1 + CountAllBagsInsideBayWith(i.Bag)));
            }
        }
    }
}