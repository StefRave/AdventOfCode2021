using System;

namespace AdventOfCode2021
{
    public static class SplitStringExtensions
    {
        public static string[] SplitByNewLine(this string input)
            => input.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        public static string[] SplitByDoubleNewLine(this string input)
            => input.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
}
