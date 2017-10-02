using System.IO;

namespace Skript47
{
    class i3WorldSectionTable
    {
        public byte[][] vaule;

        public i3WorldSectionTable(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                br.ReadBytes(4); // WST1
                vaule = new byte[br.ReadInt32()][];
                br.ReadInt64(); // 0
                br.ReadInt64(); // 0
                br.ReadInt64(); // 0
                br.ReadInt64(); // 0
                for (int i = 0; i < vaule.Length; i++)
                {
                    vaule[i] = br.ReadBytes(176);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("({0})", vaule.Length);
        }
    }
}
