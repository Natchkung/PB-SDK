using System;

namespace Skript47
{
    public class i3Node
    {
        public int[] vaule;

        public i3Node(byte[] data)
        {
            int N = BitConverter.ToInt32(data, data[0] + 9);
            int start = data[0] + 17;

            vaule = new int[N];
            for (int i = 0; i < N; i++)
            {
                vaule[i] = BitConverter.ToInt32(data, start + (i * 4));
            }
        }

        public override string ToString()
        {
            return string.Format("({0})", string.Join(", ", Array.ConvertAll(vaule, x => x.ToString())));
        }
    }
}
