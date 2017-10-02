using System.IO;

namespace Skript47
{
    public class i3TransformSourceCombiner
    {
        public int[] vaule;

        public i3TransformSourceCombiner(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                br.ReadInt32();
                vaule = new int[br.ReadInt32()];
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", vaule.Length);
        }
    }
}
