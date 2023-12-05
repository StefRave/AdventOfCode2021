namespace AdventOfCode2023;

public class Day05 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        long[] seeds = input[0].Split(':')[1].GetLongs();

        M[][] maps = input[1..]
            .Select(mapInput => mapInput.SplitByNewLine()[1..].Select(line =>
            {
                var l = line.GetLongs();
                return new M(l[0], l[1], l[2]);
            }).ToArray())
            .ToArray();

        var answer1 = maps.Aggregate(seeds, (i, m) => M.Map(m, i));
        Advent.AssertAnswer1(answer1.Min(), expected: 218513636, sampleExpected: 35);

        var seedsRange = seeds.Chunk(2).Select(a => new R(a.First(), a.Last()));
        var answer2 = maps.Aggregate(seedsRange, (i, m) => M.MapRange(m, i));
        Advent.AssertAnswer2(answer2.Min(r => r.Start), expected: 81956384, sampleExpected: 46);
    }

    record R(long Start, long Length)
    {
        public long End => Start + Length;
    }

    record M(long Dest, long Src, long Length)
    {

        public static long[] Map(M[] mapping, long[] input)
        {
            var r = new List<long>();
            foreach (var i in input)
            {
                bool found = false;
                foreach (var m in mapping)
                {
                    long delta = i - m.Src;
                    if (delta >= 0 && delta < m.Length)
                    {
                        r.Add(m.Dest + delta);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    r.Add(i);
            }
            return r.ToArray();
        }

        public static IEnumerable<R> MapRange(M[] mapping, IEnumerable<R> range)
        {
            var currentRanges = range.ToList();
            foreach (var m in mapping)
            {
                var notYetMapped = new List<R>();
                foreach (R cr in currentRanges)
                {
                    foreach (R sr in m.Split(cr))
                    {
                        if (sr.Start >= m.Src && sr.End <= m.Src + m.Length)
                            yield return new R(sr.Start + m.Dest - m.Src, sr.Length);
                        else
                            notYetMapped.Add(sr);
                    }
                }
                currentRanges = notYetMapped.ToList();
            }
            foreach (var r in currentRanges)
                yield return r;
        }

        public IEnumerable<R> Split(R r)
        {
            foreach (var sr in SplitAt(this.Src, r))
            {
                foreach (var er in SplitAt(this.Src + this.Length, sr))
                    yield return er;
            }
        }

        public static IEnumerable<R> SplitAt(long pos, R range)
        {
            if (pos < range.Start || pos >= range.End)
                yield return range;
            else
            {
                yield return new R(range.Start, pos - range.Start);
                yield return new R(pos, range.End - pos);
            }
        }
    }

}
