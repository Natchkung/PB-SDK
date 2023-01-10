using System;
using System.Collections.Generic;
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
        public string link;
        static byte[] DDSCode = { 0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public i3VTexImage(byte[] data, string link)
        {
            this.link = link;
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                var un1 = br.ReadInt32();
                if (un1 == 1212765270) // i3VTexImage
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
                if (un1 == 131072) // TGA
                {
                    if (data.Length > 18)
                    {
                        br.ReadBytes(8);
                        y = br.ReadUInt16();
                        x = br.ReadUInt16();
                        var b = br.ReadByte();
                        br.ReadByte();
                        if (b == 32)
                        {
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
                if (un1 == 542327876) // DDS
                {
                    if (data.Length > 128)
                    {
                        br.ReadBytes(8);
                        y = br.ReadUInt32();
                        x = br.ReadUInt32();
                        br.ReadBytes(108);
                        var u = x * y / 128;
                        block = new byte[u][];
                        var r = ReverseID();
                        for (int i = 0; i < block.Length; i++)
                        {
                            block[r[i]] = br.ReadBytes(512);
                        }
                    }
                }
                if (un1 == 1112093513) // I3I
                {
                    br.ReadBytes(2);
                    x = br.ReadUInt16();
                    y = br.ReadUInt16();
                    var t = br.ReadInt32();
                    if (t == 536871942)
                    {
                        br.ReadBytes(12);
                        var commentB = new byte[br.ReadInt16()];
                        Array.Copy(data, 128, commentB, 0, commentB.Length);
                        comment = Encoding.Default.GetString(commentB);
                        br.BaseStream.Position = 60 + comment.Length;
                        var u = x * y / 128;
                        block = new byte[u][];
                        var r = ReverseID();
                        for (int i = 0; i < block.Length; i++)
                        {
                            block[r[i]] = br.ReadBytes(512);
                        }
                    }

                }
                if (string.IsNullOrEmpty(comment))
                {
                    comment = GetAutoComment();
                }
            }
        }

        string GetAutoComment()
        {
            var result = Path.GetFileName(link);
            if (result.StartsWith("weapon_select"))
            {
                result = "Image/WeaponShape/" + result;
            }
            else if (result.StartsWith("item_"))
            {
                result = "Image/Item/" + result;
            }
            else if (result.StartsWith("UI_"))
            {
                result = "Image/Interface/" + result;
            }
            else if (result.StartsWith("Weapon_Acc_"))
            {
                result = "Image/Weapon_Acc" + result;
            }
            else
            {
                result = "Image/Main/" + result;
            }
            return result;
        }

        int[] ReverseID()
        {
            var result = new int[block.Length];
            int r = 0;
            int n = (int)(y / 128);
            if (y != 256)
            {
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
            }
            else
            {
                for (int g = 0; g < n; g++)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        for (int i = g * n; i < g * n + n; i++)
                        {
                            result[r++] = i * 128 + j;
                        }
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

        public byte[] ToDDS()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(542327876);
                    bw.Write(124);
                    bw.Write(4111);
                    bw.Write((Int32)(x));
                    bw.Write((Int32)(y));
                    bw.Write(1024);
                    bw.Write(new byte[52]);
                    bw.Write(32);
                    bw.Write(65);
                    bw.Write(0);
                    bw.Write(32);
                    bw.Write(16711680);
                    bw.Write(65280);
                    bw.Write(255);
                    bw.Write(4278190080);
                    bw.Write(4096);
                    bw.Write(new byte[16]);
                    var r = ReverseID();
                    for (int i = 0; i < block.Length; i++)
                    {
                        bw.Write(block[r[i]]);
                    }
                }
                return ms.ToArray();
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
                    bw.Write((Int16)(y));
                    bw.Write((Int16)(x));
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

        public byte[] ToI3I()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(1112093513);
                    bw.Write((Int16)5);
                    bw.Write((Int16)(y));
                    bw.Write((Int16)(x));
                    bw.Write(536871942);
                    bw.Write(new byte[10]);
                    bw.Write((Int16)1);
                    bw.Write(comment.Length);
                    bw.Write(new byte[30]);
                    bw.Write(Encoding.Default.GetBytes(comment));
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

    public class TGA
    {
        public uint x = 0;
        public uint y = 0;
        public int[][] p;

        public TGA(uint x, uint y)
        {
            this.x = x;
            this.y = y;
            p = new int[y][];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new int[x];
                for (int j = 0; j < p[i].Length; j++)
                {
                    p[i][j] = 0;
                }
            }
        }

        public TGA(byte[] data, bool head)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                if (head)
                {
                    if (data.Length > 12)
                    {
                        br.ReadBytes(12);
                        x = br.ReadUInt16();
                        y = br.ReadUInt16();
                        br.ReadInt16();
                        p = new int[y][];
                        for (int i = 0; i < p.Length; i++)
                        {
                            p[i] = new int[x];
                            for (int j = 0; j < p[i].Length; j++)
                            {
                                p[i][j] = br.ReadInt32();
                            }
                        }
                    }
                }
                else
                {
                    var h = Math.Sqrt(data.Length * 0.25);
                    x = (uint)h;
                    y = (uint)h;
                    p = new int[y][];
                    for (int i = 0; i < p.Length; i++)
                    {
                        p[i] = new int[x];
                        for (int j = 0; j < p[i].Length; j++)
                        {
                            p[i][j] = br.ReadInt32();
                        }
                    }
                }
            }
        }

        public void FlipY()
        {
            Array.Reverse(p);
        }

        public byte[] Build(uint width, uint height, uint u, uint v)
        {
            if (width == 0 || height == 0)
            {
                width = x;
                height = y;
            }
            v = (uint)p.Length - v - height;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(131072);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write((Int16)width);
                    bw.Write((Int16)height);
                    bw.Write(new byte[] { 0x20, 0x08 });
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            bw.Write(p[i + v][j + u]);
                        }
                    }
                }
                return ms.ToArray();
            }
        }
    }


}