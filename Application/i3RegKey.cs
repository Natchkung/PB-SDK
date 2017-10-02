using System;
using System.IO;

namespace Skript47
{
    public class i3RegKey
    {
        public string name;
        public Int64[] vauleA;

        public i3RegKey(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadBytes(4); //TRN3
                br.ReadBytes(8);
                br.ReadInt32(); // ? 1
                br.ReadBytes(60);
                br.ReadInt32(); // ? A9 03 00 00
                br.ReadBytes(4); //RGK1

                vauleA = new Int64[br.ReadInt32()];
                br.ReadInt64(); // 8 нулей
                for (int i = 0; i < vauleA.Length; i++)
                {
              //      vauleA[i] = br.ReadInt64();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: ({1})", name, string.Join(", ", Array.ConvertAll(vauleA, x => x.ToString())));
        }
    }
}
