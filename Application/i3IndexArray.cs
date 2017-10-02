using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Skript47
{
    public class i3IndexArray
    {
        public long edges { get; set; }
        public List<int[]> _faceV = new List<int[]>();
        public List<int[]> _faceN = new List<int[]>();
        public List<int[]> _faceT = new List<int[]>();

        public i3IndexArray(int faceCount)
        {
            for (int i = 0; i < faceCount; i++)
            {
                _faceV.Add(new[] { i * 3, i * 3 + 1, i * 3 + 2 });
            }
            _faceN = _faceV;
            _faceT = _faceV;
        }

        public i3IndexArray(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                int step = 2;
                if (Encoding.Default.GetString(data, 0, 4) == "IIA2")
                {
                    br.ReadInt32();
                    edges = br.ReadInt64();
                    if (br.ReadInt32() == 1)
                    {
                        step = 4;
                    }
                    br.ReadBytes(16);
                }
                else
                {
                    edges = br.ReadInt64();
                }
                if (step == 2)
                {
                    while (br.BaseStream.Position < data.Length)
                    {
                        _faceV.Add(new int[] { br.ReadInt16(), br.ReadInt16(), br.ReadInt16() });
                    }
                }
                if (step == 4)
                {
                    while (br.BaseStream.Position < data.Length)
                    {
                        _faceV.Add(new int[] { br.ReadInt32(), br.ReadInt32(), br.ReadInt32() });
                    }
                }
            }
            _faceN = _faceV;
            _faceT = _faceV;
        }

        public override string ToString()
        {
            return string.Format("Edges: {0}", edges);
        }

        public static byte[] CreateBlock(int[][] v)
        {
            bool x64 = v.SelectMany(x => x).Any(y => y > 32767); // Узнать есть ли числа int32
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(new byte[] { 0x49, 0x49, 0x41, 0x32 }); // IIA2
                    bw.Write((Int64)(v.Length * 3)); // Указать количество индексов
                    bw.Write(x64 ? 1 : 0); // Поставить паркер
                    bw.Write(new byte[16]); // Просто пропуск
                    for (int i = 0; i < v.Length; i++)
                    {
                        foreach (var element in v[i])
                        {
                            if (x64)
                            {
                                bw.Write(element);
                            }
                            else
                            {
                                bw.Write((Int16)element);
                            }
                        }
                    }
                    return ms.ToArray();
                }
            }
        }

        public byte[] ToBlock()
        {
            var body = new byte[8 + _faceV.Count * 6];
            Array.Copy(BitConverter.GetBytes(_faceV.Count), 0, body, 0, 4);
            for (int i = 0; i < _faceV.Count; i++)
            {
                Array.Copy(BitConverter.GetBytes(_faceV[i][0]), 0, body, i * 6 + 0 + 8, 2);
                Array.Copy(BitConverter.GetBytes(_faceV[i][1]), 0, body, i * 6 + 2 + 8, 2);
                Array.Copy(BitConverter.GetBytes(_faceV[i][2]), 0, body, i * 6 + 4 + 8, 2);
            }
            return body;
        }
    }
}
