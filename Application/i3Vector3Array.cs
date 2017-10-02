using System;
using System.IO;

namespace Skript47
{
    public class i3Vector3Array
    {
        public float[][] vaule;

        public i3Vector3Array(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                vaule = new float[br.ReadInt32()][];
                for (int i = 0; i < vaule.Length; i++)
                {
                    vaule[i] = new[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle() };
                }
            }
        }

        public string ToText()
        {
            return string.Join(Environment.NewLine, Array.ConvertAll(vaule, x => string.Join(" ", x)));
        }

        public override string ToString()
        {
            return string.Format("Vectors: {0}", vaule.Length);
        }
    }
}
