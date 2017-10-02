using System;

namespace Skript47
{
    public class i3GuiImage
    {
        public static string GetComment(byte[] data)
        {
            string v = "(" + BitConverter.ToInt16(data, data[0] + 1 + 4).ToString() + ")";
            return v;
        }
    }
}
