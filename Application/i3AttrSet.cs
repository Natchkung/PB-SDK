using System;
using System.IO;
using System.Linq;

namespace Skript47
{
    public class i3AttrSet
    {
        public string name;
        public int[] vauleA;
        public int[] vauleB;
        public float[] vauleC;

        public i3AttrSet(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadBytes(4); // INF2
                if (br.ReadInt32() == 0)
                {
                    vauleA = new int[br.ReadInt32()];
                    br.ReadInt32(); // 0
                    for (int i = 0; i < vauleA.Length; i++)
                    {
                        vauleA[i] = br.ReadInt32();
                    }

                    vauleB = new int[br.ReadInt16()];

                    if (vauleB.Length != 21569)
                    {
                        for (int i = 0; i < vauleB.Length; i++)
                        {
                            vauleB[i] = br.ReadInt32();
                        }
                    }
                    else
                    {
                        br.ReadInt16();
                        vauleB = new int[br.ReadInt32()];
                        br.ReadInt64();
                        for (int i = 0; i < vauleB.Length; i++)
                        {
                            vauleB[i] = br.ReadInt32();
                        }
                    }
                    vauleC = new float[6];
                }
                else
                {
                    vauleA = new int[br.ReadInt32()];
                    for (int i = 0; i < vauleA.Length; i++)
                    {
                        vauleA[i] = br.ReadInt32();
                    }
                    br.ReadInt32();
                    vauleC = new float[6];
                    for (int i = 0; i < vauleC.Length; i++)
                    {
                        vauleC[i] = br.ReadSingle();
                    }


                    vauleB = new int[0];
                }
            }
        }

        public override string ToString()
        {
            var a = string.Join(", ", Array.ConvertAll(vauleA, x => x.ToString()));
            var b = string.Join(", ", Array.ConvertAll(vauleB, x => x.ToString()));
            var c = string.Join(", ", Array.ConvertAll(vauleC, x => x.ToString("0.000000")));
            if (vauleC.All(x => x == 0))
            {
                c = "0";
            }
            return string.Format("{0} ({1}) ({2}) ({3})", name, a, b, c);
        }
    }
}
