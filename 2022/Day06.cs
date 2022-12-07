namespace AdventOfCode2022;

public class Day06 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()[0];

        int answer1 = FindEndOfStartOfMessageMarker(input, 4);
        Advent.AssertAnswer1(answer1, expected: 1760, sampleExpected: 7);


        int answer2 = FindEndOfStartOfMessageMarker(input, 14);
        Advent.AssertAnswer2(answer2, expected: 2974, sampleExpected: 19);
    }

    private static int FindEndOfStartOfMessageMarker(string input, int markerLength)
    {
        for (int startIndex = 0; startIndex < input.Length - markerLength; startIndex++)
        {
            string startOfPacket = input.Substring(startIndex, markerLength);
            if (startOfPacket.Distinct().Count() == markerLength)
                return startIndex + markerLength;
        }
        throw new Exception("Not found");
    }
}