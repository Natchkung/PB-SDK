using System;
using System.Text;
using System.IO;
using System.Linq;

namespace Skript47
{
    public class I3IxDDS
    {
        public string name;
        public string link;
        public string comment;
        public int x;
        public int y;
        public int mipMaps;
        public int mipMapsReal;
        public string type;
        public bool normal;
        public byte[] body;
        static byte[] I3ICode = { 0x49, 0x33, 0x49, 0x42, 0x05, 0x00, 0x00, 0x02, 0x00, 0x02, 0x80, 0x06, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static byte[] DDSCode = { 0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static string[] markNormal = { "_norm", "_normal", "_nomal" };

        public I3IxDDS(byte[] info, string path)
        {
            if (info.Length > 60)
            {
                if (Encoding.Default.GetString(info, 0, 4) == "DDS ")
                {
                    name = Path.GetFileName(path);
                    comment = "By_Skript47";
                    x = BitConverter.ToInt32(info, 16);
                    y = BitConverter.ToInt32(info, 12);
                    mipMaps = BitConverter.ToInt32(info, 28);

                    normal = markNormal.Any(s => name.Contains(s));

                    if (mipMaps == 0)
                    {
                        mipMaps = 1;
                    }
                    mipMapsReal = mipMaps;

                    type = "Unknown";
                    string Format = Encoding.Default.GetString(info, 84, 4);
                    if (Format.Substring(0, 3) == "DXT")
                    {
                        type = Format;
                    }
                    else
                    {
                        int TypeBit = BitConverter.ToInt32(info, 88);
                        if (TypeBit != 0)
                        {
                            if (info[94] == 0xFF & info[97] == 0xFF & info[100] == 0xFF)
                            {
                                if (TypeBit == 32)
                                {
                                    type = "X8R8G8B8";
                                    if (info[107] == 0xFF)
                                    {
                                        type = "A8B8G8R8";
                                    }
                                }
                                else if (TypeBit == 24)
                                {
                                    type = "R8G8B8";
                                }
                            }
                        }
                        if (info[113] != 0x00)
                        {
                            type = "Unknown";
                        }
                    }

                    int bodyLength = info.Length - 128;
                    if (mipMaps > 4)
                    {
                        bodyLength = FixBodySize(4);
                    }
                    body = new byte[bodyLength];
                    Array.Copy(info, 128, body, 0, body.Length);
                }
                if (Encoding.Default.GetString(info, 0, 4) == "I3IB")
                {
                    name = Path.GetFileName(path);
                    comment = Encoding.Default.GetString(info, 60, info[26]);
                    x = BitConverter.ToInt16(info, 6);
                    y = BitConverter.ToInt16(info, 8);
                    mipMaps = BitConverter.ToInt16(info, 24);
                    mipMapsReal = mipMaps;
                    normal = info[19] == 0x10;
                    if (!normal)
                    {
                        normal = markNormal.Any(s => name.ToLower().Contains(s));
                    }
                    type = "Unknown";
                    if (info[10] == 0x80 & info[13] == 0x80)
                    {
                        type = "DXT1";
                    }
                    else if (info[10] == 0x81 & info[13] == 0xA0)
                    {
                        type = "DXT1";
                    }
                    else if (info[10] == 0x02 & info[13] == 0xA0)
                    {
                        type = "DXT3";
                    }
                    else if (info[10] == 0x04 & info[13] == 0xA0)
                    {
                        type = "DXT5";
                    }
                    else if (info[10] == 0x02 & info[11] == 0x04 & info[13] == 0x00)
                    {
                        type = "X8R8G8B8";
                    }
                    else if (info[10] == 0x06 & info[13] == 0x20)
                    {
                        type = "A8R8G8B8";
                    }
                    else if (info[10] == 0x02 & info[11] == 0x03 & info[13] == 0x00)
                    {
                        type = "R8G8B8";
                    }
                    int bodyLength = info.Length - 60 - comment.Length;
                    body = new byte[bodyLength];
                    Array.Copy(info, 60 + comment.Length, body, 0, body.Length);
                }
            }
        }

        public byte[] ToI3I(string comment)
        {
            ClearAlpha();
            byte[] bodyFull = new byte[60 + comment.Length + body.Length];
            Array.Copy(I3ICode, 0, bodyFull, 0, I3ICode.Length);
            Array.Copy(body, 0, bodyFull, 60 + comment.Length, body.Length);
            Array.Copy(Encoding.Default.GetBytes(comment), 0, bodyFull, 60, comment.Length);
            Array.Copy(BitConverter.GetBytes(x), 0, bodyFull, 6, 2);
            Array.Copy(BitConverter.GetBytes(y), 0, bodyFull, 8, 2);
            Array.Copy(BitConverter.GetBytes(mipMaps), 0, bodyFull, 24, 2);
            Array.Copy(BitConverter.GetBytes(comment.Length), 0, bodyFull, 26, 2);
            if (type == "DXT1")
            {
                bodyFull[10] = 0x80;
                bodyFull[13] = 0x80;
            }
            else if (type == "DXT3")
            {
                bodyFull[10] = 0x02;
                bodyFull[13] = 0xA0;
            }
            else if (type == "DXT5")
            {
                bodyFull[10] = 0x04;
                bodyFull[13] = 0xA0;
            }
            else if (type == "A8B8G8R8")
            {
                bodyFull[10] = 0x06;
                bodyFull[11] = 0x04;
                bodyFull[13] = 0x20;
            }
            else if (type == "X8R8G8B8")
            {
                bodyFull[10] = 0x02;
                bodyFull[11] = 0x04;
                bodyFull[13] = 0x00;
            }
            else if (type == "R8G8B8")
            {
                bodyFull[10] = 0x02;
                bodyFull[11] = 0x03;
                bodyFull[13] = 0x00;
            }
            if (normal)
            {
                bodyFull[19] = 0x10;
            }
            return bodyFull;
        }

        public byte[] ToDDS()
        {
            ClearAlpha();
            byte[] bodyFull = new byte[128 + body.Length];
            Array.Copy(DDSCode, 0, bodyFull, 0, DDSCode.Length);
            Array.Copy(body, 0, bodyFull, 128, body.Length);
            Array.Copy(BitConverter.GetBytes(x), 0, bodyFull, 16, 4);
            Array.Copy(BitConverter.GetBytes(y), 0, bodyFull, 12, 4);
            Array.Copy(BitConverter.GetBytes(mipMaps), 0, bodyFull, 28, 4);
            if (type == "DXT1")
            {
                bodyFull[87] = 0x31;
            }
            else if (type == "DXT3")
            {
                bodyFull[87] = 0x33;
            }
            else if (type == "DXT5")
            {
                bodyFull[87] = 0x35;
            }
            else if (type == "X8R8G8B8")
            {
                bodyFull[10] = 0x02;
                bodyFull[80] = 0x40;
                bodyFull[84] = 0x00;
                bodyFull[85] = 0x00;
                bodyFull[86] = 0x00;
                bodyFull[87] = 0x00;
                bodyFull[88] = 0x20;
                bodyFull[94] = 0xFF;
                bodyFull[97] = 0xFF;
                bodyFull[100] = 0xFF;
                bodyFull[107] = 0x00;
                bodyFull[108] = 0x08;
                bodyFull[109] = 0x10;
                bodyFull[110] = 0x40;
            }
            else if (type == "A8R8G8B8")
            {
                bodyFull[80] = 0x41;
                bodyFull[84] = 0x00;
                bodyFull[85] = 0x00;
                bodyFull[86] = 0x00;
                bodyFull[87] = 0x00;
                bodyFull[88] = 0x20;
                bodyFull[94] = 0xFF;
                bodyFull[97] = 0xFF;
                bodyFull[100] = 0xFF;
                bodyFull[107] = 0xFF;
                bodyFull[108] = 0x02;
            }
            else if (type == "R8G8B8")
            {
                bodyFull[80] = 0x40;
                bodyFull[84] = 0x00;
                bodyFull[85] = 0x00;
                bodyFull[86] = 0x00;
                bodyFull[87] = 0x00;
                bodyFull[88] = 0x18;
                bodyFull[94] = 0xFF;
                bodyFull[97] = 0xFF;
                bodyFull[100] = 0xFF;
                bodyFull[107] = 0x00;
            }
            return bodyFull;
        }

        public string[] Item()
        {
            var Item = new string[6];
            Item[0] = name;
            Item[1] = type;
            Item[2] = string.Format("{0} x {1}", x, y);
            Item[3] = mipMapsReal.ToString();
            Item[4] = normal.ToString();
            return Item;
        }

        public int FixBodySize(int n)
        {
            if (mipMaps > n)
            {
                mipMaps = n;
            }
            int bodyLength = (x * y) / 1;
            for (int i = 1; i < n; i++)
            {
                bodyLength += (x * y) / (int)Math.Pow(4, i);
            }
            if (type == "DXT1")
            {
                bodyLength /= 2;
            }
            else if (type == "DXT3" | type == "DXT5")
            {
                bodyLength *= 1;
            }
            else if (type == "A8B8G8R8" | type == "X8R8G8B8")
            {
                bodyLength *= 4;
            }
            return bodyLength;
        }

        public void ClearAlpha()
        {
            if (type == "DXT5" | type == "DXT3")
            {
                bool removeAlpha = false;
                byte[] bodyFix = new byte[body.Length / 2];
                for (int j = 0; j < body.Length; j += 16)
                {
                    if (BitConverter.ToInt64(body, j) == 65535 | BitConverter.ToInt64(body, j) == -64256 | normal)
                    {
                        Array.Copy(body, j + 8, bodyFix, j / 2, 8);
                        removeAlpha = true;
                    }
                    else
                    {
                        removeAlpha = false;
                        break;
                    }
                }
                if (removeAlpha)
                {
                    body = bodyFix;
                    type = "DXT1";
                }
            }
        }
    }
}