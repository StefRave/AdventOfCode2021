using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Helpers
{
    public class FlexArray2D<T> : FlexArray<FlexArray<T>>
    {
        public T Default2D { get; set; } = default;
        public FlexArray2D()
        {
            DefaultFunc = () => new FlexArray<T> { DefaultValue = Default2D };
        }

        public IEnumerable<T> GroupSelect() => Items.SelectMany(item => item.Items);

        public T[][] GetFixedArray()
        {
            int min = 0;
            int max = 0;
            foreach (var item in Items)
            {
                min = Math.Min(min, item.Min);
                max = Math.Max(max, item.Max);
            }
            var rows = new List<T[]>();
            foreach (var item in Items)
            {
                rows.Add(item.GetDefaults(item.Min - min).Concat(item.Items).Concat(item.GetDefaults(max - item.Max)).ToArray());
            }
            return rows.ToArray();
        }
    }
}
