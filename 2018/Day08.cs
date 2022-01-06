using System.Text;

namespace AdventOfCode2018;

public class Day08 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().Trim().Split(' ').Select((num, i) => int.Parse(num)).ToArray();
        // Step X must be finished before step C can begin.

        var (root, _) = ParseNode(input);

        int metadataSum = SumMetadataEntries(root);
        Advent.AssertAnswer1(metadataSum, expected: 48443, sampleExpected: 138);

        int sum = root.GetSpecialSum();
        Advent.AssertAnswer2(sum, expected: 30063, sampleExpected: 66);

    }

    private (Node node, int index) ParseNode(Span<int> input)
    {
        int index = 0;
        int childCount = input[index++];
        int metadataEntries = input[index++];
        var children = new List<Node>();
        for (int i = 0; i < childCount; i++)
        {
            var (child, size) = ParseNode(input[index..]);
            index += size;
            children.Add(child);
        }

        var node = new Node(children.ToArray(), input[index..(index + metadataEntries)].ToArray());
        return (node, index + metadataEntries);
    }
    private int SumMetadataEntries(Node node) 
        => node.MetaData.Sum() + node.ChildNodes.Sum(cn => SumMetadataEntries(cn));


    public record Node(Node[] ChildNodes, int[] MetaData)
    {
        public int GetSpecialSum()
        {
            if (ChildNodes.Length == 0)
                return MetaData.Sum();
            int sum = 0;
            foreach (int index in MetaData)
                if (index > 0 && index <= ChildNodes.Length)
                    sum += ChildNodes[index - 1].GetSpecialSum();
            return sum;
        }
    }
}