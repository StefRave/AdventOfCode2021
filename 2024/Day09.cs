namespace AdventOfCode2024;

public class Day09 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput();
        
        List<int> disk = ParseInput(input);
        Defrag1(disk);
        long answer1 = Calc(disk);
        Advent.AssertAnswer1(answer1, expected: 6201130364722, sampleExpected: 1928);

        disk = ParseInput(input);
        Defrag2(disk);
        long answer2 = Calc(disk);
        Advent.AssertAnswer2(answer2, expected: 6221662795602, sampleExpected: 2858);
    }

    static long Calc(List<int> disk)
    {
        long answer = 0;
        for (int i = 0; i < disk.Count; i++)
        {
            var num = disk[i];
            if (num > 0)
                answer += i * num;
        }
        return answer;
    }
    
    static void Defrag1(List<int> disk)
    {
        int i = 0;
        int end = disk.Count;
        while (i < --end)
        {
            if (disk[end] == -1)
                continue;
            
            while (disk[i] != -1)
                i++;

            disk[i++] = disk[end];
            disk[end] = -1;
        }
    }
    
    private static void Defrag2(List<int> disk)
    {
        int i = 0;
        int end = disk.Count;
        while (i < --end)
        {
            if (disk[end] == -1)
                continue;
            int spaceNeeded = 1;
            while (disk[end] == disk[end - 1])
            {
                spaceNeeded++;
                end--;
            }
            while (disk[i] != -1)
                i++;
            if (i > end)
                break;
            int j = i;
            int space = 0;
            while (j < end)
            {
                space = 1;
                while (disk[j + space] == -1)
                    space++;
                if (space >= spaceNeeded)
                    break;
                j += space;
                while (disk[j] != -1)
                    j++;
            }
            if (space >= spaceNeeded)
            {
                for (int k = 0; k < spaceNeeded; k++)
                {
                    disk[j + k] = disk[end + k];
                    disk[end + k] = -1;
                }
            }
        }
    }

    private static List<int> ParseInput(string input)
    {
        var disk = new List<int>();
        bool isSpace = false;
        int num = 0;
        foreach (var len in input)
        {
            for (int i = '0'; i < len; i++)
                disk.Add(isSpace ? -1 : num);
            isSpace = !isSpace;
            if (!isSpace)
                num++;
        }
        return disk;
    }
}








