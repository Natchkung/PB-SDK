using System;
using System.IO;
using System.Collections.Generic;

namespace Skript47
{
    public class i3VertexArray
    {
        byte[] vertexHeader;
        int vertexCount;
        int vertexStep;
        public List<float[]> _vertexP = new List<float[]>();
        public List<float[]> _vertexN = new List<float[]>();
        public List<float[]> _vertexT = new List<float[]>();
        public float[][] vertexArray;

        public i3VertexArray()
        {

        }

        public i3VertexArray(List<float[]> _vertexP, List<float[]> _vertexN, List<float[]> _vertexT)
        {
            this._vertexP = _vertexP;
            this._vertexN = _vertexN;
            this._vertexT = _vertexT;
        }

        public i3VertexArray(byte[] info)
        {
            using (var br = new BinaryReader(new MemoryStream(info)))
            {
                vertexHeader = br.ReadBytes(40);
                vertexCount = BitConverter.ToInt32(info, 8);
                vertexStep = (info.Length - vertexHeader.Length) / vertexCount;
                vertexArray = new float[vertexCount][];
                for (int i = 0; i < vertexArray.Length; i++)
                {
                    var tempF = ByteToFloat(br.ReadBytes(vertexStep));
                    _vertexP.Add(new[] { tempF[0], tempF[1], tempF[2] });
                    _vertexN.Add(new[] { tempF[3], tempF[4], tempF[5] });
                    if (vertexStep > 28)
                    {
                        if (!float.IsNaN(tempF[6]))
                        {
                            _vertexT.Add(new[] { tempF[6], tempF[7] * -1 });
                        }
                        else
                        {
                            _vertexT.Add(new[] { tempF[7], tempF[8] * -1 });
                        }
                        vertexArray[i] = new[] { tempF[0], tempF[1], tempF[2], tempF[3], tempF[4], tempF[5], tempF[6], tempF[7] * -1 };
                    }
                    else
                    {
                        _vertexT.Add(new[] { tempF[3], tempF[4] });
                        vertexArray[i] = new[] { tempF[0], tempF[1], tempF[2], tempF[3], tempF[4], tempF[5], 0, 0 };
                    }
                }
            }
        }

        public static float[] ByteToFloat(byte[] info)
        {
            using (var br = new BinaryReader(new MemoryStream(info)))
            {
                var result = new float[info.Length / 4];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = br.ReadSingle();
                }
                return result;
            }
        }

        public static byte[] CreateBlock(float[][] v)
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(new byte[] { 0x56, 0x41, 0x33, 0x30 });
                    bw.Write(1155);
                    bw.Write(v.Length);
                    bw.Write(0);
                    bw.Write(new byte[] { 0x11, 0x81, 0x11, 0xC1, 0xCC, 0xCC, 0xCC, 0xCC });
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write((float)1);
                    for (int i = 0; i < v.Length; i++)
                    {
                        for (int j = 0; j < v[i].Length; j++)
                        {
                            bw.Write(v[i][j]);
                        }
                    }
                    return ms.ToArray();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Vertex: {0}, Step: {1}", vertexCount, vertexStep);
        }
    }
}
