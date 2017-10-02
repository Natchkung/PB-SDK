using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Skript47
{
    public class i3BoneMatrixListAttr
    {
        public int bonesCount;
        public List<Bone> _bone = new List<Bone>();

        public class Bone
        {
            public int ID;
            public int parent;
            public string name;
            public float[] vaule;

            public Bone()
            {

            }

            public Bone(string name, byte[] Hex)
            {
                this.name = name;
                this.ID = BitConverter.ToInt32(Hex, 0);
                this.parent = BitConverter.ToInt32(Hex, 80);

                this.vaule = new float[16];
                for (int i = 0; i < vaule.Length; i++)
                {
                    this.vaule[i] = BitConverter.ToSingle(Hex, 16 + i * 4);
                }
            }

            public override string ToString()
            {
                return string.Format("{0} {1} {2} | {3}", name, ID, parent, string.Join(", ", Array.ConvertAll(vaule, x => x.ToString("0.000000"))));
            }
        }

        public i3BoneMatrixListAttr(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                br.ReadInt32();
                bonesCount = br.ReadInt32();
                br.ReadBytes(40);
                for (int i = 0; i < bonesCount; i++)
                {
                    var ThisBone = new Bone();
                    ThisBone.name = Encoding.Default.GetString(br.ReadBytes(32)).Split('\0')[0];
                    ThisBone.ID = br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    ThisBone.vaule = new float[16];
                    for (int j = 0; j < ThisBone.vaule.Length; j++)
                    {
                        ThisBone.vaule[j] = br.ReadSingle();
                    }
                    ThisBone.parent = br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    _bone.Add(ThisBone);
                }
            }
        }

        public string ToSMD()
        {
            var SMD = new StringBuilder();

            SMD.AppendLine("version 1").AppendLine("nodes");
            for (int i = 0; i < _bone.Count; i++)
            {
                var v = "\"" + _bone[i].name + "\" ";
                var p = _bone[i].ID.ToString();
                SMD.AppendFormat("{0, 4} {1} {2}", i, v, p);
                SMD.AppendLine();
            }
            SMD.AppendLine("end");

            SMD.AppendLine("skeleton").AppendLine("time 0");
            for (int i = 0; i < _bone.Count; i++)
            {
                var px = (_bone[i].vaule[12] * 100).ToString("0.000000");
                var py = (_bone[i].vaule[13] * 100).ToString("0.000000");
                var pz = (_bone[i].vaule[14] * 100).ToString("0.000000");

                var rx = (_bone[i].vaule[5] * 1.570796).ToString("0.000000");
                var ry = (_bone[i].vaule[6] * 1.570796).ToString("0.000000");
                var rz = (_bone[i].vaule[7] * 1.570796).ToString("0.000000");

                var sx = _bone[i].vaule[2].ToString("0.000000"); // Масштаб по оси X
                var sy = _bone[i].vaule[4].ToString("0.000000"); // Масштаб по оси Y
                var sz = _bone[i].vaule[9].ToString("0.000000"); // Масштаб по оси Z

                SMD.AppendLine("  " + i.ToString("") + " " + px + " " + py + " " + pz + " " + 0 + " " + 0 + " " + 0);
            }

            SMD.AppendLine("end");
            return SMD.ToString();
        }

        public string ToText()
        {
            var text = new StringBuilder();
            for (int i = 0; i < _bone.Count; i++)
            {
                var val = string.Join(" ", Array.ConvertAll(_bone[i].vaule, x => x.ToString("0.000000")));
                var line = string.Format("{0} {1} {2} {3}", _bone[i].name, _bone[i].ID, _bone[i].parent, val);
                text.AppendLine(line);
            }
            return text.ToString();
        }

        public override string ToString()
        {
            return string.Format("Bones: {0}", bonesCount);
        }
    }
}
