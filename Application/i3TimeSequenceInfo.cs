using System.IO;

namespace Skript47
{
    public class i3TimeSequenceInfo
    {
        public int A;

        public i3TimeSequenceInfo(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {

            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
