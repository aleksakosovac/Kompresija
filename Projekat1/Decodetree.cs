namespace Projekat1
{
    class DecodeTreeNode
    {
        public DecodeTreeNode? Zero;
        public DecodeTreeNode? One;
        public bool IsLeaf;
        public byte Symbol;
    }

    static class DecodeTree
    {
        public static DecodeTreeNode Build((uint code, int length)[] codes)
        {
            var root = new DecodeTreeNode();

            for (int sym = 0; sym < 256; sym++)
            {
                int length = codes[sym].length;
                if (length == 0)
                {
                    continue;
                }

                uint code = codes[sym].code;
                DecodeTreeNode node = root;

                for (int i = length - 1; i >= 0; i--)
                {
                    int bit = (int)((code >> i) & 1);
                    if (bit == 0)
                    {
                        node.Zero ??= new DecodeTreeNode();
                        node = node.Zero;
                    }
                    else
                    {
                        node.One ??= new DecodeTreeNode();
                        node = node.One;
                    }
                }

                node.IsLeaf = true;
                node.Symbol = (byte)sym;
            }

            return root;
        }
    }
}