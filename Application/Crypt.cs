using System;

namespace Skript47
{
    class Crypt
    {
        public static void Decrypt(byte[] data, int shift)
        {
            if (data.Length > 0) {
            byte lastElement = data[data.Length - 1];
            for (int i = data.Length - 1; i > 0; i--)
            {
                data[i] = (byte)((data[i - 1] << (8 - shift)) | (data[i] >> shift));
            }
            data[0] = (byte)((lastElement << (8 - shift)) | (data[0] >> shift));
            }
        }

        public static void Encrypt(byte[] data, byte shift)
        {
            byte lastElement = data[data.Length - 1];
            for (int i = data.Length - 1; i > 0; i--)
            {
                data[i] = (byte)((data[i - 1] << (8 - shift)) | (data[i] >> shift));
            }
            data[0] = (byte)((lastElement << (8 - shift)) | (data[0] >> shift));

            //первый элемент в полученом массиве переносим в конец массива
            byte[] datatemp = new byte[data.Length];
            for (int i = 0; i < data.Length - 1; i++)
            {
                datatemp[i] = data[i + 1];
            }
            datatemp[data.Length - 1] = data[0];
            datatemp.CopyTo(data, 0);
        }
    }
}
