using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Skript47
{
    public class OBJ
    {
        public Vertex _vertex = new Vertex();
        public List<Group> _group = new List<Group>();
        static readonly Regex regvp = new Regex(@"^v\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static readonly Regex regvn = new Regex(@"^vn\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static readonly Regex regvt = new Regex(@"^vt\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static readonly Regex regg = new Regex(@"^g\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static readonly Regex regf = new Regex(@"^f" + @"\s+(\d+)/(\d+)/(\d+)" + @"\s+(\d+)/(\d+)/(\d+)" + @"\s+(\d+)/(\d+)/(\d+)", RegexOptions.Compiled);
        static char[] space = new[] { ' ' };

        public class Vertex
        {
            public List<float[]> P = new List<float[]>();
            public List<float[]> N = new List<float[]>();
            public List<float[]> T = new List<float[]>();
        }

        public class Group
        {
            public string name;
            public string usemtl;
            public Face _face = new Face();

            public class Face
            {
                public List<int[]> V = new List<int[]>();
                public List<int[]> N = new List<int[]>();
                public List<int[]> T = new List<int[]>();
            }

            public Group(string g)
            {
                name = g;
            }

            public Group(string g, List<int[]> _faceV, List<int[]> _faceN, List<int[]> _faceT)
            {
                name = g;
                _face.V = _faceV;
                _face.N = _faceN;
                _face.T = _faceT;
            }
        }

        public OBJ(List<float[]> _vertexP, List<float[]> _vertexN, List<float[]> _vertexT, Group tempG)
        {
            _vertex.P = _vertexP;
            _vertex.N = _vertexN;
            _vertex.T = _vertexT;
            _group.Add(tempG);
        }

        public OBJ(string[] data)
        {
            var SSO_REE = StringSplitOptions.RemoveEmptyEntries;
            foreach (var line in data)
            {
                if (regvp.Match(line).Success) // Vertex
                {
                    var d = line.Split(space, SSO_REE);
                    _vertex.P.Add(new[] { float.Parse(d[1]), float.Parse(d[2]), float.Parse(d[3]) });
                }
                else if (regvn.Match(line).Success) // Normal
                {
                    var d = line.Split(space, SSO_REE);
                    _vertex.N.Add(new[] { float.Parse(d[1]), float.Parse(d[2]), float.Parse(d[3]) });
                }
                else if (regvt.Match(line).Success) // UV
                {
                    var d = line.Split(space, SSO_REE);
                    _vertex.T.Add(new[] { float.Parse(d[1]), float.Parse(d[2]) });
                }
                else if (line.StartsWith("mtllib")) // mtllib
                {
                    var d = line.Split(space, SSO_REE);
                }
                else if (regg.Match(line).Success) // Group
                {
                    var d = line.Split(space, SSO_REE);
                    _group.Add(new Group(d[1]));
                }
                else
                {
                    var m0 = regf.Match(line); // Face
                    if (m0.Success)
                    {
                        var d = line.Replace("/", " ").Split(space, SSO_REE);
                        _group[_group.Count - 1]._face.V.Add(new[] { int.Parse(d[1]) - 1, int.Parse(d[4]) - 1, int.Parse(d[7]) - 1 });
                        _group[_group.Count - 1]._face.T.Add(new[] { int.Parse(d[2]) - 1, int.Parse(d[5]) - 1, int.Parse(d[8]) - 1 });
                        _group[_group.Count - 1]._face.N.Add(new[] { int.Parse(d[3]) - 1, int.Parse(d[6]) - 1, int.Parse(d[9]) - 1 });
                    }
                }
            }
        }

        public float[][] BuildVertexArray(int g, bool flipUV, bool flipNormal)
        {
            var vertexList = new List<float[]>();
            for (int j = 0; j < _group[g]._face.V.Count; j++)
            {
                for (int k = 0; k < _group[g]._face.V[j].Length; k++)
                {
                    var px = (_vertex.P[_group[g]._face.V[j][k]][0]);
                    var py = (_vertex.P[_group[g]._face.V[j][k]][1]);
                    var pz = (_vertex.P[_group[g]._face.V[j][k]][2]);
                    var nx = (_vertex.N[_group[g]._face.N[j][k]][0]);
                    var ny = (_vertex.N[_group[g]._face.N[j][k]][1]);
                    var nz = (_vertex.N[_group[g]._face.N[j][k]][2]);
                    var ux = (_vertex.T[_group[g]._face.T[j][k]][0]);
                    var uy = (_vertex.T[_group[g]._face.T[j][k]][1]);
                    if (flipUV)
                    {
                        vertexList.Add(new[] { px, py, pz, nx, ny, nz, ux, uy * -1 });
                    }
                    else if (flipNormal)
                    {
                        vertexList.Add(new[] { px, py, pz, nx * -1, ny * -1, nz * -1, ux, uy });
                    }
                    else
                    {
                        vertexList.Add(new[] { px, py, pz, nx, ny, nz, ux, uy });
                    }
                }
            }
            return vertexList.ToArray();
        }

        public int[][] BuildIndexArray(int g)
        {
            var indexList = new List<int[]>();
            int size = _group[g]._face.V.Count;
            for (int i = 0; i < size; i++)
            {
                indexList.Add(new[] { i * 3, i * 3 + 1, i * 3 + 2 });
            }
            return indexList.ToArray();
        }

        public void SaveOBJ(string path)
        {
            var OBJ = new StringBuilder("#PB SKD - By Skript47").AppendLine();
            foreach (var i in _vertex.P)
            {
                OBJ.AppendFormat("v {0:0.000000} {1:0.000000} {2:0.000000}", i[0], i[1], i[2]).AppendLine();
            }
            OBJ.AppendFormat("# {0} vertices", _vertex.P.Count).AppendLine();
            foreach (var i in _vertex.N)
            {
                OBJ.AppendFormat("vn {0:0.000000} {1:0.000000} {2:0.000000}", i[0], i[1], i[2]).AppendLine();
            }
            OBJ.AppendFormat("# {0} vertex normals", _vertex.N.Count).AppendLine();
            foreach (var i in _vertex.T)
            {
                OBJ.AppendFormat("vt {0:0.000000} {1:0.000000}", i[0], i[1]).AppendLine();
            }
            OBJ.AppendFormat("# {0} texture coords", _vertex.T.Count).AppendLine();
            foreach (var g in _group)
            {
                OBJ.AppendLine("g " + g.name).AppendLine("s 1");
                for (int j = 0; j < g._face.V.Count; j++)
                {
                    OBJ.Append("f");
                    OBJ.AppendFormat(" {0}/{1}/{2}", g._face.V[j][0] + 1, g._face.T[j][0] + 1, g._face.N[j][0] + 1);
                    OBJ.AppendFormat(" {0}/{1}/{2}", g._face.V[j][1] + 1, g._face.T[j][1] + 1, g._face.N[j][1] + 1);
                    OBJ.AppendFormat(" {0}/{1}/{2}", g._face.V[j][2] + 1, g._face.T[j][2] + 1, g._face.N[j][2] + 1);
                    OBJ.AppendLine();
                }
                OBJ.AppendLine();
            }
            File.WriteAllText(path, OBJ.ToString());
        }
    }
}
