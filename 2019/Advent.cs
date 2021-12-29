using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CommandLine;
using FluentAssertions;

namespace AdventOfCode2019;

public class Options
{
    [Option('d', "day", Required = true, HelpText = "The day to run")]
    public string Day { get; set; }

    [Option('s', "sample", Required = false, HelpText = "Run using the sample data")]
    public bool Sample { get; set; }

}

public class Advent
{
    public static string Day;
    public static bool UseSampleData;

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .MapResult(o =>
            {
                Day = o.Day.PadLeft(2, '0'); // ensure leading 0
                UseSampleData = o.Sample;

                Type type = GetAdventSoltion();
                if (type == null)
                {
                    Console.WriteLine($"Day {o.Day} not found");
                    return 10;
                }
                Console.WriteLine($"Running {type.Name}");
                var adv = (IAdvent)Activator.CreateInstance(type);
                var stopWatch = Stopwatch.StartNew();
                try
                {
                    adv.Run();
                }
                catch (FailedToAnswerCorrectlyExcaption) { }
                stopWatch.Stop();
                Console.WriteLine("Elapsed: " + TimeToString(stopWatch.Elapsed));
                return 0;
            }, errs =>
            {
                return -1;
            });
    }

    private static string TimeToString(TimeSpan timeSpan)
    {
        return timeSpan.Ticks switch
        {
            < TimeSpan.TicksPerMinute => timeSpan.ToString(@"s\.fff"),
            _ => timeSpan.ToString(@"hh\:mm\:ss"),
        };
    }
    private static Type GetAdventSoltion()
    {
        IEnumerable<Type> enumerable = Assembly.GetExecutingAssembly().GetTypes().ThatImplement<IAdvent>();
        var type = enumerable.FirstOrDefault(t => Regex.Match(t.Name, @"\d+[a-z]*$", RegexOptions.IgnoreCase).Value.TrimStart('0').Equals(Day.TrimStart('0'), StringComparison.OrdinalIgnoreCase));
        return type;
    }

    public static string[] ReadInputLines()
    {
        return File.ReadAllLines(GetInputName());
    }

    public static string ReadInput()
    {
        return File.ReadAllText(GetInputName());
    }

    private static string GetInputName()
    {
        if (UseSampleData)
            return @$"input\input{Day[0..2]} sample.txt";
        else
            return @$"input\input{Day[0..2]}.txt";
    }

    private static void AssertAnswer(object result, int partNumber, object expected)
    {
        string actualString = Convert.ToString(result, CultureInfo.InvariantCulture);
        string expectedString = expected != null
            ? Convert.ToString(expected, CultureInfo.InvariantCulture)
            : File.ReadAllLines(GetInputName().Replace(".txt", ".answer.txt"))[partNumber - 1];
        bool correct = actualString == expectedString;

        Console.Write("");
        var fg = Console.ForegroundColor;
        if (correct)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Answer day {Day} part {partNumber}: {actualString}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Answer day {Day} part {partNumber}: {actualString}  correct answer: {expectedString}");
        }
        Console.ForegroundColor = fg;
        if (!correct)
            throw new FailedToAnswerCorrectlyExcaption();
    }

    public static void AssertAnswer1(object result, object expected = null)
        => AssertAnswer(result, 1, expected);

    public static void AssertAnswer2(object result, object expected = null)
        => AssertAnswer(result, 2, expected);
}

public interface IAdvent
{
    void Run();
}
public class FailedToAnswerCorrectlyExcaption : Exception
{

}