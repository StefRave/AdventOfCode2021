using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Helpers
{
    public class FlexArray<T>
    {
        public int Min { get; private set; }
        public int Max => Items.Count + Min;

        public List<T> Items { get; } = new List<T>();
        public T DefaultValue { get; set; } = default;
        public Func<T> DefaultFunc { get; set; }
        public T GetDefault() => DefaultFunc != null ? DefaultFunc() : DefaultValue;
        public IEnumerable<T> GetDefaults(int count) => Enumerable.Range(0, count).Select(_ => GetDefault());

        public T this[int i]
        {
            get
            {
                if (i < Min || i - Min >= Items.Count)
                {
                    T val = GetDefault();
                    this[i] = val;
                    return val;
                }
                return Items[i - Min];
            }
            set
            {
                if (i < Min)
                {
                    Items.InsertRange(0, new T[] { value }.Concat(GetDefaults(Min - i - 1)));
                    Min = i;
                }
                else if (i < Items.Count + Min)
                    Items[i - Min] = value;
                else if (i == Items.Count - Min)
                    Items.Add(value);
                else
                    Items.AddRange(GetDefaults(i - Min - Items.Count).Concat(new T[] { value }));
            }
        }
    }
}
