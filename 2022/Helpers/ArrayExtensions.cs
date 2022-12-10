namespace AdventOfCode2022.Helpers
{
    public static class Init
    {
        public static T[] Array<T>(T value, int size)
        {
            var result = new T[size];
            for (int i = 0; i < size; i++)
                result[i] = value;
            return result;
        }
        public static T[] Array<T>(Func<T> value, int size)
        {
            var result = new T[size];
            for (int i = 0; i < size; i++)
                result[i] = value();
            return result;
        }
    }
}
