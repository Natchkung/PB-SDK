using System;
using System.Text;

namespace Skript47
{
    class i3UITemplate_User
    {
        public int combo;
        public string name;

        public i3UITemplate_User(byte[] data)
        {
            combo = BitConverter.ToInt32(data, 4);
            name = Encoding.Default.GetString(data, 141, data[140]);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", name, combo);
        }
    }
}
