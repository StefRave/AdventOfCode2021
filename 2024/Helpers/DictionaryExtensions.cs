namespace AdventOfCode2024.Helpers;

public static class DictionaryExtensions
{
    public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TValue> update)
    {
        if (dict.TryGetValue(key, out TValue prevValue))
            dict[key] = update(prevValue);
        else
            dict.Add(key, update(default));
    }
}
