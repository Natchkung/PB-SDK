using System.IO;

namespace Skript47
{
    public class i3RegREAL32
    {
        public string name;
        public float vaule;

        public i3RegREAL32(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadBytes(4);
                vaule = br.ReadSingle();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", name, vaule);
        }
    }
}
