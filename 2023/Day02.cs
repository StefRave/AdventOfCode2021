namespace AdventOfCode2023;

public class Day02 : IAdvent
{
    public void Run()
    {
        Rgb[][] input = Advent.ReadInput().SplitByNewLine()
            .Select(line => 
                line.Split(": ")[1].Split("; ")
                    .Select(g => {
                        var items = new int[3];
                        foreach(var item in g.Split(", "))
                        {
                            var sp = item.Split(' ');
                            items[(int)Enum.Parse<Color>(sp[1], ignoreCase: true)] = int.Parse(sp[0]);
                        }
                        return new Rgb(red: items[(int)Color.Red], green: items[(int)Color.Green], blue: items[(int)Color.Blue]);
                    }).ToArray()
            )
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


        long answer2 = input.Sum(game => {
            Rgb rgb = game.Aggregate((a, b) => a.Max(b));
            return rgb.Power;
        });

        Advent.AssertAnswer2(answer2, expected: 70950, sampleExpected: 2286);
    }

    record Rgb(int red, int green, int blue)
    {
        public bool IsPossible(Rgb grab)
             => grab.red <= red && grab.blue <= blue && grab.green <= green;

        public Rgb Max(Rgb other) => new Rgb(red: int.Max(red, other.red), green: int.Max(green, other.green), blue: int.Max(blue, other.blue));
        public long Power => red * green * blue;
    }
    
    enum Color { Green, Blue, Red }
}