using System;

namespace Skript47
{
    public class i3TextureEnableAttr
    {
        public bool vaule;

        public i3TextureEnableAttr(byte[] data)
        {
            vaule = BitConverter.ToBoolean(data, 0);
        }

        public override string ToString()
        {
            return vaule ? "+" : "-";
        }
    }
}
