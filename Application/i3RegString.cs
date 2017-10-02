using System.IO;
using System.Text;

namespace Skript47
{
    public class i3RegString
    {
        public string name;
        public string vaule;

        public i3RegString(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadBytes(4); // 2 это тип
                var mark = Encoding.Default.GetString(br.ReadBytes(4));
                if (mark == "RGS2")
                {
                    int stringSize = br.ReadInt32();
                    vaule = Encoding.Default.GetString(br.ReadBytes(stringSize));
                }
                if (mark == "RGS3")
                {
                    int stringSize = br.ReadInt32();
                    vaule = Encoding.Default.GetString(br.ReadBytes(stringSize * 2)).Replace("\0", string.Empty);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", name, vaule);
        }
    }
}
