#nullable enable
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AdventOfCode2019;

public class Day22 : IAdvent
{
    private static string GetInput() => File.ReadAllText(@"Input/input22.txt");

    [Fact]
    public void Example1()
    {
        string input = @"deal with increment 7
deal into new stack
deal into new stack";
        var commands = Parse(input);
        Assert.Equal(3, commands.Length);
        var stack = Enumerable.Range(0, 10).ToList();

        stack = DealWithIncrement(stack, 7);
    }

    [Fact]
    public void Example2()
    {
        string input = @"deal into new stack
cut -2
deal with increment 7
cut 8
cut -4
deal with increment 7
cut 3
deal with increment 9
deal with increment 3
cut -1
";
        var commands = Parse(input);
        var stack = Enumerable.Range(0, 10).ToList();

        foreach (var command in commands)
        {
            stack = command switch
            {
                (true, 0) => DealIntoNewStack(stack),
                (true, int num) => DealWithIncrement(stack, num),
                (false, int num) => CutStack(stack, num),
            };
        }
        stack.Should().BeEquivalentTo("9 2 5 8 1 4 7 0 3 6".Split(' ').Select(c => int.Parse(c)));
    }

    private List<int> CutStack(List<int> stack, int num)
    {
        if (num < 0)
            num = stack.Count + num;
        var newStack = new int[stack.Count];
        stack.CopyTo(0, newStack, stack.Count - num, num);
        stack.CopyTo(num, newStack, 0, stack.Count - num);

        return new List<int>(newStack);
    }

    private List<int> DealIntoNewStack(List<int> stack)
    {
        for (int i = 0; i < stack.Count / 2; i++)
        {
            int tmp = stack[i];
            stack[i] = stack[stack.Count - 1 - i];
            stack[stack.Count - 1 - i] = tmp;
        }
        return stack;
    }

    private static List<int> DealWithIncrement(List<int> stack, int increment)
    {
        var newStack = new List<int>(new int[stack.Count]);
        int length = stack.Count;
        for (int i = 0; i < length; i++)
            newStack[(i * increment) % length] = stack[i];
        return newStack;
    }

    void IAdvent.Run()
    {
        // deal into new stack
        // cut 9037
        // deal with increment 49
        // cut - 9932
        // deal with increment 5
        // cut 6434
        Line[] commands = Parse(GetInput());

        Assert.Equal(GetInput().SplitByNewLine().Length, commands.Length);

        var stack = Enumerable.Range(0, 10007).ToList();

        foreach (var command in commands)
        {
            stack = command switch
            {
                (true, 0) => DealIntoNewStack(stack),
                (true, int num) => DealWithIncrement(stack, num),
                (false, int num) => CutStack(stack, num),
            };
        }
        
        Advent.AssertAnswer1(stack.IndexOf(2019), 0);
    }

    private static Line[] Parse(string input)
    {
        var matches = Regex.Matches(input.ReplaceLineEndings("\n"), @"^(cut|deal) .*?([-\d]+)?$", RegexOptions.Multiline | RegexOptions.ECMAScript);
        var result = matches
            .Select(m => new Line(m.Groups[1].Value == "deal", m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : 0))
            .ToArray();
        Assert.Equal(input.SplitByNewLine().Length, result.Length);
        return result;
    }

    public record Line(bool deal, int number);
}
