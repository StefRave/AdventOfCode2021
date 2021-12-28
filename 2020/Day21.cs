using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day21
    {
        private readonly ITestOutputHelper output;

        public Day21(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void DoDay21()
        {
            var input =
                File.ReadAllLines("input/input21.txt")
                .Select(line =>
                {
                    var split = line.Split(" (contains ");
                    return (ingredients: split[0].Split(' '), allergens: split[1][0..^1].Split(", "));
                })
                .ToArray();

            Dictionary<string, string> allergenWithIngredient = GetIngredientsWithAllergen(input);

            var ingredientsWithoutAllergens = input
                .SelectMany(ia => ia.ingredients)
                .Distinct()
                .Except(allergenWithIngredient.Values).ToHashSet();

            int count = input
                .SelectMany(ia => ia.ingredients)
                .Count(i => ingredientsWithoutAllergens.Contains(i));
            output.WriteLine($"Part1: {count}");


            string dangerousIngredients = string.Join(",", allergenWithIngredient.OrderBy(kv => kv.Key).Select(kv => kv.Value));
            output.WriteLine($"Part2: {dangerousIngredients}");
        }

        private static Dictionary<string, string> GetIngredientsWithAllergen((string[] ingredients, string[] allergens)[] input)
        {
            var possibleIngredientsPerAllergen = GetIngredientsWithPossibleAllergens(input);

            return GetIngredientsWithAllergen(possibleIngredientsPerAllergen);
        }

        private static Dictionary<string, string[]> GetIngredientsWithPossibleAllergens((string[] ingredients, string[] allergens)[] input)
        {
            var possibleIngredientsPerAllergen = new Dictionary<string, string[]>();
            foreach (var (ingredients, allergens) in input)
            {
                foreach (var allergen in allergens)
                {
                    string[] previousIngredients;
                    if (possibleIngredientsPerAllergen.TryGetValue(allergen, out previousIngredients))
                    {
                        var intersect = previousIngredients.Intersect(ingredients).ToArray();
                        if (previousIngredients.Length != intersect.Length)
                            possibleIngredientsPerAllergen[allergen] = intersect;
                    }
                    else
                        possibleIngredientsPerAllergen.Add(allergen, ingredients);
                }
            }
            return possibleIngredientsPerAllergen;
        }

        private static Dictionary<string, string> GetIngredientsWithAllergen(Dictionary<string, string[]> possibleIngredientsPerAllergen)
        {
            var tmp = new Dictionary<string, HashSet<string>>(
                possibleIngredientsPerAllergen.Select(kv => new KeyValuePair<string, HashSet<string>>(kv.Key, new HashSet<string>(kv.Value))));

            var ingredientWithAllergen = new Dictionary<string, string>();
            while (tmp.Count > 0)
            {
                foreach (var ingredient in tmp.Keys.ToArray())
                {
                    var allergens = tmp[ingredient];
                    if (allergens.Count == 1)
                    {
                        string allergen = allergens.First();
                        ingredientWithAllergen.Add(ingredient, allergen);
                        tmp.Remove(ingredient);
                        foreach (var kv in tmp)
                            kv.Value.Remove(allergen);
                    }
                }
            }
            return ingredientWithAllergen;
        }
    }
}