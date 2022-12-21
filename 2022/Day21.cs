namespace AdventOfCode2022;

public class Day21 : IAdvent
{
    record Calc(string Result, object Left, char Op, object Right);

    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Replace(":", "").Split(' '))
            .Select(line => new Calc(line[0], line[1], line.Length > 2 ? line[2][0] : '=', line.Length > 3 ? line[3] : 0L))
            .ToArray();

        var todo = new Queue<Calc>(input);
        (long answer1, _) = Calculate(todo);
        Advent.AssertAnswer1(answer1, expected: 75147370123646, sampleExpected: 152);

        todo = new Queue<Calc>(input.Where(line => line.Result != "humn")); 
        (_, todo) = Calculate(todo);

        var nDict = todo.ToDictionary(c => c.Result);
        var r = nDict["root"];
        long answer2 = ReverseCalculate((string)r.Left, (long)r.Right);

        long ReverseCalculate(string some, long result)
        {
            if (some == "humn")
                return result;
            
            var calc = nDict[some];
            bool leftNum = calc.Left is long;
            long num = leftNum ? (long)calc.Left : (long)calc.Right;
            
            long newResult = (calc.Op, leftNum) switch
            {
                ('+', _) => result - num,
                ('-', true) =>  num - result,
                ('-', false) => result + num,
                ('*', _) => result / num,
                ('/', false) => result * num,
                _ => throw new ArgumentOutOfRangeException()
            };
            return ReverseCalculate(leftNum ? (string)calc.Right : (string)calc.Left, newResult);
        }
        Advent.AssertAnswer2(answer2, expected: 3423279932937, sampleExpected: 301);
    }

    private static (long rootValue, Queue<Calc> leftover) Calculate(Queue<Calc> todo)
    {
        var results = new Dictionary<string, long>();
        while (true)
        {
            int toGo = todo.Count;
            var todoNew = new Queue<Calc>();

            foreach (var calc in todo)
            {
                object left = GetLongIfNumber(calc.Left);
                object right = GetLongIfNumber(calc.Right);
                if (left is not long || right is not long)
                    todoNew.Enqueue(new Calc(calc.Result, left, calc.Op, right));
                else
                {
                    long newVal = calc.Op switch
                    {
                        '+' => (long)left + (long)right,
                        '-' => (long)left - (long)right,
                        '*' => (long)left * (long)right,
                        '/' => (long)left / (long)right,
                        '=' => (long)left,
                        _ => throw new ArgumentOutOfRangeException("line[1]")
                    };
                    results[calc.Result] = newVal;
                }
            }
            todo = todoNew;
            if (todoNew.Count == toGo)
                break;
        }

        results.TryGetValue("root", out long rootResult);
        return (rootResult, todo);

        object GetLongIfNumber(object val) => val is long l ? l : val is string str && results.TryGetValue(str, out l) ? l : char.IsDigit(((string)val)[0]) ? long.Parse((string)val) : val;
    }

    public static bool IsInt(string v)
        => long.TryParse(v, out long _);
}