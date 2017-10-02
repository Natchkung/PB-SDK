using System;

namespace Skript47
{
    public class i3FresnelAttr
    {
        public int vaule;

        public i3FresnelAttr(byte[] data)
        {
            vaule = BitConverter.ToInt32(data, 4);
        }

        public override string ToString()
        {
            return string.Format("?: {0}", vaule);
        }
    }
}