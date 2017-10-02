using System;

namespace Skript47
{
    public class i3TextureBindAttr
    {
        public int Texture;

        public i3TextureBindAttr(byte[] data)
        {
            Texture = BitConverter.ToInt16(data, 0);
        }
        public override string ToString()
        {
            return string.Format("i3Texture: {0}", Texture);
        }
    }
}
