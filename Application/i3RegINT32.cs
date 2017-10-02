using System.IO;

namespace Skript47
{
    public class i3RegINT32
    {
        public string name;
        public int vaule;

        public i3RegINT32(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadBytes(4);
                vaule = br.ReadInt32();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", name, vaule);
        }
    }
}
