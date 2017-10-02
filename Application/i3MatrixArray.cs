using System;

namespace Skript47
{
    public class i3MatrixArray
    {
        public int vaule;

        public i3MatrixArray(byte[] data)
        {
            vaule = BitConverter.ToInt32(data, 0);
        }

        public override string ToString()
        {
            return string.Format("Matrixs: {0}", vaule);
        }
    }
}
