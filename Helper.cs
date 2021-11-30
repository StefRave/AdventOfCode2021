using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2021
{
    public class Helper
    {
        public static string[] ReadInputLines([CallerFilePath] string callerFilePath = null)
        {
            return File.ReadAllLines(GetInputName(callerFilePath));
        }

        public static string ReadInput([CallerFilePath] string callerFilePath = null)
        {
            return File.ReadAllText(GetInputName(callerFilePath));
        }

        private static string GetInputName(string callerFilePath)
        {
            var match = Regex.Match(callerFilePath, @"(\d+)\.cs");
            if (!match.Success)
                throw new Exception($"Number not found in test {callerFilePath}");
            string number = match.Groups[1].Value;

#if DEBUGSAMPLE
            return @$"input\input{number}sample.txt";
#else
            return @$"input\input{number}.txt";
#endif
        }

        private static string ReadResult(int answer1or2, string callerFilePath)
        {
            return File.ReadAllLines(GetInputName(callerFilePath).Replace(".txt", ".answer.txt"))[answer1or2 - 1];
        }

        public static void AssertAnswer1(object result, [CallerFilePath] string callerFilePath = null)
        {
            Assert.Equal(ReadResult(1, callerFilePath), Convert.ToString(result, CultureInfo.InvariantCulture));
        }

        public static void AssertAnswer2(object result, [CallerFilePath] string callerFilePath = null)
        {
            Assert.Equal(ReadResult(2, callerFilePath), Convert.ToString(result, CultureInfo.InvariantCulture));
        }
    }
}
