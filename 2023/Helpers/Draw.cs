namespace AdventOfCode2023.Helpers
{
    public static class Draw
    {
        public static void DrawYx(IEnumerable<(int y, int x, char c)> input, int? step = null, string info = null)
        {
            if (step.HasValue)
                Console.WriteLine($"Step {step}");
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine(info);
            var ar = input.ToArray();

            int xs = ar.Select(p => p.x).Min();
            int xe = ar.Select(p => p.x).Max() + 1;
            int ys = ar.Select(p => p.y).Min();
            int ye = ar.Select(p => p.y).Max() + 1;
            var field = Enumerable.Repeat(0, ye - ys).Select(_ => "".PadRight(xe - xs, '.').ToArray()).ToArray();
            foreach (var item in ar)
                field[item.y - ys][item.x - xs] = item.c;

            foreach (var line in field)
                Console.WriteLine(new string(line));
            Console.WriteLine("");
        }
    }
}
