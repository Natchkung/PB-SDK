using System;
using System.Text;

namespace Skript47
{
    public class i3GeometryAttr
    {
        public int trianglesCount;
        public int vertexArray;
        public int indexArray;

        public i3GeometryAttr(int trianglesCount, int vertexArray, int indexArray)
        {
            this.trianglesCount = trianglesCount;
            this.vertexArray = vertexArray;
            this.indexArray = indexArray;
        }

        public i3GeometryAttr (byte[] data)
        {
            if (Encoding.Default.GetString(data, 0, 4) == "GEO2")
            {
                trianglesCount = BitConverter.ToInt32(data, 5);
                vertexArray = BitConverter.ToInt32(data, 13);
                indexArray = BitConverter.ToInt32(data, 17);
            }
            else
            {
                trianglesCount = BitConverter.ToInt32(data, 1);
                vertexArray = BitConverter.ToInt32(data, 9);
                indexArray = BitConverter.ToInt32(data, 13);
            }
        }

        public override string ToString()
        {
            return string.Format("Triangles: {0}, V: {1} I: {2}", trianglesCount, vertexArray, indexArray);
        }

        public byte[] ToBlock()
        {
            byte[] body = new byte[21];
            body[0] = 0x04;
            Array.Copy(BitConverter.GetBytes(trianglesCount), 0, body, 1, 4); // Кол триугольников
            Array.Copy(BitConverter.GetBytes(vertexArray), 0, body, 9, 4); // i3VertexArray
            Array.Copy(BitConverter.GetBytes(indexArray), 0, body, 13, 4); // i3IndexArray
            return body;
        }
    }
}
