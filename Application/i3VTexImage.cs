using System;
using System.IO;
using System.Text;

namespace Skript47
{
    public class i3VTexImage
    {
        public uint x;
        public uint y;
        public byte[][] block;
        public string comment;

        public i3VTexImage(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                var un1 = br.ReadInt32();
                if (un1 == 1212765270)
                {
                    var un2 = br.ReadInt32();
                    if (data.Length > 2048)
                    {
                        x = br.ReadUInt32();
                        y = br.ReadUInt32();
                        br.ReadInt32();
                        var commentB = new byte[br.ReadInt32()];
                        Array.Copy(data, 128, commentB, 0, commentB.Length);
                        comment = Encoding.Default.GetString(commentB);
                        br.BaseStream.Position = 2048;
                        var u = x * y / 128;
                        block = new byte[u][];
                        for (int i = 0; i < block.Length; i++)
                        {
                            block[i] = br.ReadBytes(512);
                        }
                    }
                }
            }
        }

        int[] ReverseID()
        {
            var result = new int[block.Length];
            int r = 0;
            int n = (int)(y / 128);
            for (int g = 0; g < n; g++)
            {
                for (int j = 0; j < 128; j++)
                {
                    for (int i = g * n; i < g * n + n; i++)
                    {
                        result[r++] = i * 128 + j;
                    }
                }
            }
            return result;
        }

        public byte[] Build(bool Reverse)
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(1212765270);
                    bw.Write(1);
                    bw.Write((Int32)x);
                    bw.Write((Int32)y);
                    bw.Write(0);
                    bw.Write(comment.Length);
                    bw.Write(new byte[104]);
                    bw.Write(Encoding.Default.GetBytes(comment));
                    bw.Write(new byte[2048 - bw.BaseStream.Position]);
                    var r = ReverseID();
                    if (Reverse)
                    {
                        for (int i = 0; i < block.Length; i++)
                        {
                            bw.Write(block[r[i]]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < block.Length; i++)
                        {
                            bw.Write(block[i]);
                        }
                    }
                }
                return ms.ToArray();
            }
        }

        public void GetTGA(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                var un1 = br.ReadInt32();
                if (un1 == 131072)
                {
                    if (data.Length > 18)
                    {
                        br.ReadBytes(8);
                        x = br.ReadUInt16();
                        y = br.ReadUInt16();
                        br.ReadInt16();
                        var u = x * y / 128;
                        block = new byte[u][];
                        var r = ReverseID();
                        for (int i = 0; i < block.Length; i++)
                        {
                            block[r[i]] = br.ReadBytes(512);
                        }
                    }
                }
            }
        }

        public byte[] ToTGA()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(131072);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write((Int16)(x));
                    bw.Write((Int16)(y));
                    bw.Write(new byte[] { 0x20, 0x08 });
                    var r = ReverseID();
                    for (int i = 0; i < block.Length; i++)
                    {
                        bw.Write(block[r[i]]);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}