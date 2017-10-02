using System;
using System.Text;

namespace Skript47
{
    public class i3MaterialAttr
    {
        public float[] vaule;

        public i3MaterialAttr(byte[] data)
        {
            vaule = new float[data.Length / 4];
            for (int i = 0; i < vaule.Length; i++)
            {
                vaule[i] = BitConverter.ToSingle(data, i * 4);
            }
        }

        public override string ToString()
        {
            return string.Format("Vaule: {0}", string.Join(", ", Array.ConvertAll(vaule, x => x.ToString("0.00"))));
        }
    }
}
