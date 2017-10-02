using System;
using System.Text;

namespace Skript47
{
    public class i3Geometry
    {
        public string name;
        public int link;

        public i3Geometry(byte[] data)
        {
            if (data.Length > 24)
            {
                name = Encoding.Default.GetString(data, 1, data[0]);

                if (data.Length - (data[0] + 1) == 24)
                {
                    link = BitConverter.ToInt32(data, data[0] + 1 + 20);
                }
                if (data.Length - (data[0] + 1) == 48)
                {
                    link = BitConverter.ToInt32(data, data[0] + 1 + 44);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", name, link);
        }
    }
}
