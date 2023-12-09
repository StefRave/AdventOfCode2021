namespace AdventOfCode2023;

public class Day02 : IAdvent
{
    public void Run()
    {
        Rgb[][] input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.Split(": ")[1].Split("; ").Select(Rgb.FromString).ToArray())
            .ToArray();

        var maxGame1Bag = new Rgb(red: 12, green: 13, blue: 14);
        int answer1 = 0;
        for (int i = 0; i < input.Length; i++)
        {
            bool possible = input[i].All(maxGame1Bag.IsPossible);
            if (possible)
                answer1 += i + 1;
        }
        Advent.AssertAnswer1(answer1, expected: 2076, sampleExpected: 8);


        long answer2 = input.Sum(game =>
        {
            Rgb rgb = game.Aggregate((a, b) => a.Max(b));
            return rgb.Power;
        });

        Advent.AssertAnswer2(answer2, expected: 70950, sampleExpected: 2286);
    }

    public record Rgb(int red, int green, int blue)
    {
        public static Rgb FromString(string colorCounts)
        {
            // 3 blue, 4 red
            var dict = colorCounts
                .Split(", ")
                .Select(cv => cv.Split(" "))
                .ToDictionary(cv => cv[1], cv => int.Parse(cv[0]));
            return new Rgb(dict.GetValueOrDefault("red"), dict.GetValueOrDefault("green"), dict.GetValueOrDefault("blue"));
        }

        public bool IsPossible(Rgb grab)
             => grab.red <= red && grab.blue <= blue && grab.green <= green;

        public Rgb Max(Rgb other) => new Rgb(red: int.Max(red, other.red), green: int.Max(green, other.green), blue: int.Max(blue, other.blue));
        public long Power => red * green * blue;
    }
}