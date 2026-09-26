using System.Collections.Generic;

namespace Projekat1
{
    static class Huffman
    {
        private class Node
        {
            public long Count;
            public byte Symbol;
            public bool IsLeaf;
            public Node? Left;
            public Node? Right;
        }

        public static int[] BuildLengths(long[] Ni)
        {
            var nodes = new List<Node>();
            for (int i = 0; i < 256; i++)
            {
                if (Ni[i] > 0)
                {
                    nodes.Add(new Node { Count = Ni[i], Symbol = (byte)i, IsLeaf = true });
                }
            }

            int[] lengths = new int[256];

            if (nodes.Count == 0)
            {
                return lengths;
            }

            if (nodes.Count == 1)
            {
                lengths[nodes[0].Symbol] = 1;
                return lengths;
            }

            while (nodes.Count > 1)
            {
                int i1 = IndexOfMin(nodes, -1);
                int i2 = IndexOfMin(nodes, i1);

                Node a = nodes[i1];
                Node b = nodes[i2];

                Node parent = new Node
                {
                    Count = a.Count + b.Count,
                    IsLeaf = false,
                    Left = a,
                    Right = b
                };

                if (i1 > i2)
                {
                    nodes.RemoveAt(i1);
                    nodes.RemoveAt(i2);
                }
                else
                {
                    nodes.RemoveAt(i2);
                    nodes.RemoveAt(i1);
                }

                nodes.Add(parent);
            }

            AssignLengths(nodes[0], 0, lengths);

            return lengths;
        }

        private static int IndexOfMin(List<Node> nodes, int exclude)
        {
            int best = -1;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == exclude)
                {
                    continue;
                }

                if (best == -1 || nodes[i].Count < nodes[best].Count)
                {
                    best = i;
                }
            }

            return best;
        }

        private static void AssignLengths(Node node, int depth, int[] lengths)
        {
            if (node.IsLeaf)
            {
                lengths[node.Symbol] = depth > 0 ? depth : 1;
                return;
            }

            AssignLengths(node.Left!, depth + 1, lengths);
            AssignLengths(node.Right!, depth + 1, lengths);
        }
    }
}