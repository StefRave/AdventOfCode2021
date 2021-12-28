using System.Linq;

namespace AdventOfCode2019.Helpers
{
    public static class FlexArrayExtensions
    {
        public static string AsString(this FlexArray<char> self)
        {
            return new string(self.Items.ToArray());
        }
        public static string AsString(this FlexArray2D<char> self)
        {
            return string.Join('\n', self.GetFixedArray().Select(a => new string(a)));
        }
    }
}
