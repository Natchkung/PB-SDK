using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Skript47
{
    class i3RefTex
    {
        public List<Block> _block = new List<Block>();

        public class Block
        {
            public string name;
            public int[] size = new int[2];
        }

        public i3RefTex(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                var vaule = new byte[br.ReadInt32()][];
                br.ReadInt64(); // 0
                br.ReadInt64(); // 0
                br.ReadInt64(); // 0
                br.ReadInt64(); // 0
                for (int i = 0; i < vaule.Length; i++)
                {
                    var ThisBlock = new Block();
                    ThisBlock.name = Encoding.Default.GetString(br.ReadBytes(256)).Split('\0')[0];
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    ThisBlock.size[0] = br.ReadInt32();
                    ThisBlock.size[1] = br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadBytes(32); // 0
                    _block.Add(ThisBlock);
                }
            }
        }

        public override string ToString()
        {
            var typeTemp = _block.Select(z => string.Format("{0} {1}x{2}", z.name, z.size[0], z.size[1])).ToArray();
            var result = string.Join(Environment.NewLine, typeTemp);
            if (result.Length > 0)
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
