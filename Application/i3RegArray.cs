using System;
using System.Text;
using System.Drawing;
using System.IO;

namespace Skript47
{
    public class i3RegArray
    {
        public string name;
        public int type;
        public int[] vauleA;
        public float[] vauleB;
        public Color[] vauleC;
        public string[] vauleD;
        public float[][] vauleE;

        public i3RegArray(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadBytes(4); // 9
                type = br.ReadInt32();
                if (type == 0)
                {
                    vauleA = new int[br.ReadInt32()];
                    for (int i = 0; i < vauleA.Length; i++)
                    {
                        vauleA[i] = br.ReadInt32();
                    }
                }
                if (type == 1)
                {
                    vauleB = new float[br.ReadInt32()];
                    for (int i = 0; i < vauleB.Length; i++)
                    {
                        vauleB[i] = br.ReadSingle();
                    }
                }
                if (type == 2)
                {
                    vauleD = new string[br.ReadInt32()];
                    for (int i = 0; i < vauleD.Length; i++)
                    {
                        var mark = Encoding.Default.GetString(br.ReadBytes(4));
                        if (mark == "RGS2")
                        {
                            int stringSize = br.ReadInt32();
                            vauleD[i] = Encoding.Default.GetString(br.ReadBytes(stringSize));
                        }
                        if (mark == "RGS3")
                        {
                            int stringSize = br.ReadInt32();
                            vauleD[i] = Encoding.Default.GetString(br.ReadBytes(stringSize * 2)).Replace("\0", string.Empty);
                        }
                    }
                }
                if (type == 3)
                {
                    vauleE = new float[br.ReadInt32()][];
                    for (int i = 0; i < vauleB.Length; i++)
                    {
                        vauleE[i] = new[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle() };
                    }
                }
                if (type == 4)
                {
                    vauleE = new float[br.ReadInt32()][];
                    for (int i = 0; i < vauleB.Length; i++)
                    {
                        vauleE[i] = new[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() };
                    }
                }
                if (type == 5)
                {
                    vauleE = new float[br.ReadInt32()][];
                    for (int i = 0; i < vauleB.Length; i++)
                    {
                        vauleE[i] = new[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() };
                    }
                }
                if (type == 6)
                {
                    vauleC = new Color[br.ReadInt32()];
                    for (int i = 0; i < vauleC.Length; i++)
                    {
                        vauleC[i] = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                    }
                }
            }
        }

        public string ToText()
        {
            if (vauleA != null)
            {
                return string.Format("{0}\r\n{1}", name, string.Join(Environment.NewLine, Array.ConvertAll(vauleA, x => x.ToString())));
            }
            else if (vauleB != null)
            {
                return string.Format("{0}\r\n{1}", name, string.Join(Environment.NewLine, Array.ConvertAll(vauleB, x => x.ToString("0.000000"))));
            }
            else if (vauleD != null)
            {
                return string.Format("{0}\r\n{1}", name, string.Join(Environment.NewLine, Array.ConvertAll(vauleD, x => x)));
            }
            else
            {
                return string.Empty;
            }
        }

        public override string ToString()
        {
            if (vauleA != null)
            {
                return string.Format("{0} {1}", name, string.Join(", ", Array.ConvertAll(vauleA, x => x)));
            }
            if (vauleB != null)
            {
                return string.Format("{0} {1}", name, string.Join(", ", Array.ConvertAll(vauleB, x => x)));
            }
            if (vauleD != null)
            {
                return string.Format("{0} {1}", name, string.Join(", ", Array.ConvertAll(vauleD, x => x)));
            }
            return string.Format("{0} '{1}'", name, type);
        }
    }
}
