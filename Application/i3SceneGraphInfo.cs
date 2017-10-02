using System;
using System.IO;

namespace Skript47
{
    public class i3SceneGraphInfo
    {
        public string name;
        public int[] vaule;
        
        public i3SceneGraphInfo(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadBytes(4);

                vaule = new int[9];
                for (int i = 0; i < vaule.Length; i++)
                {
                    vaule[i] = br.ReadInt32();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", name, string.Join(", ", Array.ConvertAll(vaule, x => x.ToString())));
        }
    }
}
