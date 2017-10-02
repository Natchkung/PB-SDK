using System.IO;
using System.Text;

namespace Skript47
{
    public class i3Texture
    {
        public string comment;

        public i3Texture(byte[] data)
        {
            if (data.Length > 60)
            {
                comment = Encoding.Default.GetString(data, 60, data[26]);
            }
        }

        public override string ToString()
        {
            return Path.GetFileName(comment);
        }
    }
}
