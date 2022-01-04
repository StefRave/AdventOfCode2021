#nullable enable
using System.Text.RegularExpressions;

namespace AdventOfCode2019;

public class Day25 : IAdvent
{
    private static List<long> GetInput() => File.ReadAllText(@"Input/input25.txt").Split(",").Select(long.Parse).ToList();
    private static Dictionary<char, (int y, int x)> nsew = new Dictionary<char, (int y, int x)> 
        { {'n', (-1, 0) },  {'s', (1, 0) },  {'w', (0, -1) },  {'e', (0, 1) },  };
    void IAdvent.Run()
    {
        var initial = new Queue<string>(new string[] { "north", "take mutex", "east", "east", "east", "take whirled peas", "west", "west", "west", "west", "east", "south", "south", "take cake", "south", "north", "north", "west", "take space law space brochure", "south", "take hologram", "west", "take manifold", "south", "north", "north", "south", "east", "south", "north", "west", "north", "south", "east", "north", "east", "south", "west", "south", "take easter egg", "south", "inv", "trydropping" });
        var programm = new IntCode(GetInput());

        var p = (y: 0, x: 0);
        var newPos = p;
        var array = new FlexArray2D<char>() { Default2D = '.' };
        var history = new List<string>();
        string doors = "";
        string[] itemsHere = new string[0];
        while (true)
        {
            programm.RunUntilInputNeeded();

            var output = programm.OutputString.SplitByDoubleNewLine().ToList();
            programm.Output.Clear();
            if (output[0].IndexOf("==") >= 0)
                Clear();
            if (output[0].IndexOf("You can't go that way.") >= 0)
                WriteLine("no");
            else
            {
                array[p.y][p.x] = '+';
                p = newPos;
                array[p.y][p.x] = 'x';

                itemsHere = new string[0];
                if (output.Count > 2 && output[2].IndexOf("Items here") >= 0)
                {
                    itemsHere = output[2].SplitByNewLine().Skip(1).Select(line => line[2..]).ToArray();
                    output.RemoveAt(2);
                }
                if (output.Count > 1 && output[1].IndexOf("Doors here lead") >= 0)
                {
                    doors = string.Join(", ", output[1].SplitByNewLine().Skip(1).Select(line => line[2..3]));
                    output.RemoveAt(1);
                }
                if (output[^1].IndexOf("Command") >= 0)
                    output.RemoveAt(output.Count - 1);
                else
                    WriteLine("dead?");
            }
            WriteLine($"Pos {p}");
            
            foreach (var item in output)
                WriteLine(item + "\n");
            WriteLine($"Doors: {doors}");
            if (itemsHere.Length > 0)
            {
                string itemsString = string.Join(", ", itemsHere);
                WriteLine($"Items!!!!!!!!!!!!!!!!!: {itemsString}");
            }
            foreach (var (dir, d) in nsew)
                if (!"+xo".Contains(array[p.y + d.y][p.x + d.x]))
                    array[p.y + d.y][p.x + d.x] = doors.Contains(dir) ? 'o' : '#';
            WriteLine(array.AsString());


            string line = initial.Count > 0 ? initial.Dequeue() : ReadLine();
            if (line == "history")
            {
                WriteLine('"' + string.Join("\",\"", history) + '"');
                line = "inv";
            }
            if (line == "trydropping")
                break;
            line = line switch
            {
                "n" => "north",
                "s" => "south",
                "e" => "east",
                "w" => "west",
                "t" => $"take {itemsHere.FirstOrDefault()}",
                _ => line ?? ""
            };
            newPos = line switch
            {
                "north" => (y: p.y - 1, x: p.x),
                "south" => (y: p.y + 1, x: p.x),
                "east" =>  (y: p.y    , x: p.x + 1),
                "west" =>  (y: p.y    , x: p.x - 1),
                _ => p
            };
            history.Add(line);

            programm.AddInput(line + '\n');
        }
        programm.AddInput("inv\n");
        programm.RunUntilInputNeeded();
        
        var inventory = programm.OutputString.SplitByNewLine().Where(line => line.StartsWith("- ")).Select(line => line[2..]).ToArray();
        programm.Output.Clear();
        int dropped = 0;
        for (int i = 0; i < 255; i++)
        {
            int bit = 0;
            string command = "";
            while (dropped != i)
            {
                if ((i & (1 << bit)) == 0 && (dropped & (1 << bit)) != 0)
                {
                    command += $"take {inventory[bit]}\n";
                    dropped &= ~(1 << bit);
                }
                if ((i & (1 << bit)) != 0 && (dropped & (1 << bit)) == 0)
                {
                    command += $"drop {inventory[bit]}\n";
                    dropped |= (1 << bit);
                }
                bit++;
            }
            command += "south\n";
            programm.AddInput(command);
            programm.RunUntilInputNeeded();
            WriteLine(programm.OutputString);
            if (programm.OutputString.Contains("Analysis complete"))
                break;
        }

        Advent.AssertAnswer1(Regex.Match(programm.OutputString, @"\d\d+").Value, "262848");
    }
}
