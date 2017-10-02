using System;
using System.Text;

namespace Skript47
{
    public class i3MemoryBuffer
    {
        public i3MemoryBuffer()
        {

        }
        
        public static string GetComment(byte[] data)
        {
            string v = "";
            for (int j = 4; j < data.Length; j += 4)
            {
                if (BitConverter.ToInt32(data, j) < 100000000)
                {
                    v += BitConverter.ToInt32(data, j).ToString() + " ";
                }
                else
                {
                    byte[] blockName = new byte[BitConverter.ToInt32(data, 0)];
                    Array.Copy(data, 4, blockName, 0, blockName.Length);
                    v = Encoding.Default.GetString(blockName);
                    break;
                }
            }
            return v;
        }
    }
}
