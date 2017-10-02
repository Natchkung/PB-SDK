using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Skript47
{
    public class I3S
    {
        public string link;
        public string name;
        public string extension;
        public string parentLink;
        public byte[] parent = new byte[0];
        public byte[] backUp = new byte[0];
        public byte[] header = new byte[0];
        public byte[] text = new byte[0];
        public byte[] info = new byte[0];
        public string[] textLines;
        public Dictionary<int, I3S.Block> _block = new Dictionary<int, I3S.Block>();
        public Dictionary<int, I3S.Block> _blockD = new Dictionary<int, I3S.Block>();
        public List<I3S.Geometry> _geometry = new List<I3S.Geometry>();
        public List<I3S.Pack> _pack = new List<I3S.Pack>();
        public List<I3S.LOD> _lod = new List<I3S.LOD>();
        public List<I3S.Material> _mat = new List<I3S.Material>();

        public static string[] i3TypeList = new string[] {
            "i3ShapeSetContainer", "i3Object", "i3LevelLight", "i3LevelWorld", "i3LevelObject", "i3LevelRespawn", "i3LevelCameraObject", "i3LevelLayer", "i3LevelScene", "i3Particle", "i3GICDiagram", "i3Camera", "i3Transform2", "i3Node",
            "i3ColliderSet", "i3UICheckBox", "i3UIImageBoxEx", "i3UIUserControl", "i3UIRadio", "i3GuiEditBox", "i3GuiButton", "i3GuiRoot", "i3GuiWindow", "i3GuiImage", "i3GuiTemplateElement", "i3GuiStatic", "i3UIButtonImageSet", "i3UIProgressBar",
            "i3UITab", "i3UIButton", "i3UIEditBox", "i3UIComboBox", "i3UIButtonComposed3", "i3UIImageBox", "i3UIScene", "i3UIFrameWnd", "i3UIStaticText", "i3UIListView_Item", "i3UIListView_Header", "i3UIListView_Box", "i3TransformSequence",
            "i3LightObject", "CObjectState", "CObjectTemplateType", "i3TimeSequence", "CChara", "CLOD", "GICShapeAI", "i3GICDiagram", "AI", "i3Font", "i3Shader", "i3Body", "i3ActionBreakObject", "i3TimeEventInfo", "i3TimeEventGen", "i3World",
            "i3GamePath", "i3Framework", "i3StageInstanceInfo", "i3StageInfo", "i3SgLayer", "i3PhysixShapeSet", "i3Transform", "i3AI", "i3AIContext", "i3AIState", "i3SoundNode", "i3Chara",
            "i3UITemplate_ImageBox","i3UILTreeNode_Template","i3UILTreeNode_Filter","i3UITemplate_Button","i3UITemplate_VScrollBar","i3UITemplate_HScrollBar","i3UITemplate_FrameWnd","i3UITemplate_ButtonComposed3",
            "i3UITemplate_EditBox","i3UITemplate_StaticText","i3UITemplate_LVBox","i3UITemplate_ListBox","i3UITemplate_ComboBox","i3UITemplate_LVSep","i3UITemplate_LVHeader","i3UITemplate_LVCell","i3UITemplate_LVItem","i3UITemplate_Tab","i3UITemplate_ProgressBar",
            "i3UITemplate_CheckBox","i3UITemplate_Radio","i3UITemplate_ToolTip","i3UILibrary"
        };
        public static string[] markNormal = { "_norm", "_normal", "_nomal" };

        public int[] ListKeysOfSelectedType(string targetType)
        {
            var n = _block.Where(a => a.Value.type == targetType).Select((orden, index) => orden.Key).ToArray();
            return n;
        }

        public int[] BlockListIndexsOfSelectedType(string targetType)
        {
            var n = _block.Select((s, i) => new { Str = s, Index = i }).
                Where(x => x.Str.Value.type == targetType & x.Str.Value.data.Length > 0).
                Select(x => x.Index).ToArray();
            return n;
        }

        public int[] PackListIndexsOfSelectedType(string targetType)
        {
            var n = _pack.Select((s, i) => new { Str = s, Index = i }).
                Where(x => Path.GetExtension(x.Str.name).ToLower() == targetType).
                Select(x => x.Index).ToArray();
            return n;
        }

        public void Optimize()
        {
            int change = 0;
            if (extension.ToLower() == ".i3s")
            {
                foreach (var element in ListKeysOfSelectedType("i3Texture"))
                {
                    if (_block[element].data.Length > 60)
                    {
                        int before = _block[element].data.Length;
                        var _tex = new I3IxDDS(_block[element].data, "");
                        _block[element].data = new I3IxDDS(_block[element].data, _tex.comment).ToI3I("");
                        int after = _block[element].data.Length;
                        change += before - after;
                    }
                }
                foreach (var element in ListKeysOfSelectedType("i3NormalMapBindAttr"))
                {
                    var tempData = _block[element].data;
                    var TextureBindAttrID = new i3TextureBindAttr(tempData).Texture;
                    if (_block[TextureBindAttrID].data.Length > 60)
                    {
                        if (_block[TextureBindAttrID].data[19] != 0x10)
                        {
                            _block[TextureBindAttrID].data[19] = 0x10;
                        }
                    }
                }
            }
            if (extension.ToLower() == ".i3pack")
            {
                var n = _pack.Select((v, i) => new { v, i }).Where(a => Path.GetExtension(a.v.name).ToLower() == ".i3i").Select(x => x.i).ToArray();
                int count = 0;
                foreach (var element in n)
                {
                    if (_pack[element].data.Length > 60)
                    {
                        int before = _pack[element].data.Length;
                        var _tex = new I3IxDDS(_pack[element].data, _pack[element].name);
                        if (_tex.type != "Unknown")
                        {
                            _pack[element].data = new I3IxDDS(_pack[element].data, _pack[element].name).ToI3I("");
                            count++;
                        }
                        int after = _pack[element].data.Length;
                        change += before - after;
                    }
                }
            }
            MessageBox.Show(string.Format("Bytes removed: {0}", change));
        }

        public void CopyListOfTypes()
        {
            var typeTemp = _block.Select(z => z.Value.type).Distinct().ToArray();
            var result = string.Join(Environment.NewLine, typeTemp);
            if (result.Length > 0)
            {
                Clipboard.SetText(result);
                MessageBox.Show(result);
            }
        }

        public void CopyListOfFiles()
        {
            var typeTemp = _pack.Select(z => z.name).ToArray();
            var result = string.Join(Environment.NewLine, typeTemp);
            if (result.Length > 0)
            {
                Clipboard.SetText(result);
                MessageBox.Show(result);
            }
        }

        public void ImportOBJ(int g, string[] data)
        {
            var gInfo = new int[4];
            gInfo[0] = _geometry.ElementAt(g).triangles;
            gInfo[1] = _geometry.ElementAt(g).geometryAttr;
            gInfo[2] = _geometry.ElementAt(g).vertexArray;
            gInfo[3] = _geometry.ElementAt(g).indexArray;

            var Custom = new OBJ(data);

            // i3GeometryAttr

            gInfo[0] = Custom._group[0]._face.N.Count;
            _geometry.ElementAt(g).triangles = gInfo[0];
            _block[gInfo[1]].data = new i3GeometryAttr(gInfo[0], gInfo[2], gInfo[3]).ToBlock();

            // i3VertexArray

            _block[gInfo[2]].data = i3VertexArray.CreateBlock(Custom.BuildVertexArray(0, true, false));

            // i3IndexArray

            if (_block.ContainsKey(gInfo[3]))
            {
                _block[gInfo[3]].data = i3IndexArray.CreateBlock(Custom.BuildIndexArray(0));
            }
            _geometry.ElementAt(g).edit = true;
        }

        public void ImportPBG(int g, byte[] data)
        {
            var gInfo = new int[4];
            gInfo[0] = _geometry.ElementAt(g).triangles;
            gInfo[1] = _geometry.ElementAt(g).geometryAttr;
            gInfo[2] = _geometry.ElementAt(g).vertexArray;
            gInfo[3] = _geometry.ElementAt(g).indexArray;

            var PBG = new byte[2][];
            PBG[0] = new byte[BitConverter.ToInt32(data, 4)];
            PBG[1] = new byte[BitConverter.ToInt32(data, 8)];
            Array.Copy(data, 12, PBG[0], 0, PBG[0].Length);
            Array.Copy(data, 12 + PBG[0].Length, PBG[1], 0, PBG[1].Length);

            // i3GeometryAttr

            gInfo[0] = BitConverter.ToInt32(data, 0);
            _geometry.ElementAt(g).triangles = gInfo[0];
            _block[gInfo[1]].data = new i3GeometryAttr(gInfo[0], gInfo[2], gInfo[3]).ToBlock();

            // i3VertexArray

            _block[gInfo[2]].data = PBG[0];

            // i3IndexArray

            if (_block.ContainsKey(gInfo[3]))
            {
                _block[gInfo[3]].data = PBG[1];
            }
            _geometry.ElementAt(g).edit = true;
        }

        public void CleanupMeshByList(int[] n)
        {
            foreach (var index in n)
            {
                _geometry.ElementAt(index).CleanupMesh(_block);
            }
        }

        public void SavePackByList(string dir, int[] n, bool toDDS)
        {
            if (n.Length > 0)
            {
                var path = Path.Combine(dir, name);
                Directory.CreateDirectory(path);
                foreach (var index in n)
                {
                    _pack.ElementAt(index).SaveAsFile(path, toDDS, false);
                }
            }
        }

        public void SaveMeshByList(string dir, int[] n, bool sOBJ, bool sPBG, bool optimize)
        {
            if (n.Length > 0)
            {
                var path = Path.Combine(dir, name);
                Directory.CreateDirectory(path);
                foreach (var index in n)
                {
                    if (sOBJ)
                    {
                        _geometry.ElementAt(index).SaveAsOBJ(_block, path, optimize);
                    }
                    if (sPBG)
                    {
                        _geometry.ElementAt(index).SaveAsPBG(_block, path);
                    }
                }
            }
        }

        public void SaveAllBones(string dir)
        {
            var n = BlockListIndexsOfSelectedType("i3BoneMatrixListAttr");
            foreach (var index in n)
            {
                string path = Path.Combine(dir, name);
                Directory.CreateDirectory(path);

                var temp = new i3BoneMatrixListAttr(_block.ElementAt(index).Value.data);
                var path_1 = Path.Combine(path, (_block.ElementAt(index).Key).ToString() + ".txt");
                File.WriteAllText(path_1, temp.ToText());

                var path_2 = Path.Combine(path, (_block.ElementAt(index).Key).ToString() + ".smd");
                File.WriteAllText(path_2, temp.ToSMD());
            }
        }

        public void SaveBlockByList(string dir, int[] n, bool toDDS)
        {
            if (n.Length > 0)
            {
                string path = Path.Combine(dir, name);
                Directory.CreateDirectory(path);
                foreach (var index in n)
                {
                    _block.ElementAt(index).Value.SaveAsFile(path, toDDS);
                }
            }
        }

        public void SetBlockTargetByList(int[] n, int t)
        {
            foreach (var index in n)
            {
                _block.ElementAt(index).Value.target = t;
            }
        }

        public class LOD
        {

        }

        public class Material
        {
            public Dictionary<int, string> _texure = new Dictionary<int, string>();

            public Material()
            {

            }
        }

        public class Geometry
        {
            public int ID;
            public string name;
            public int LOD;
            public int triangles;
            public int geometryAttr;
            public int vertexArray;
            public int indexArray;
            public bool edit = false;

            public Geometry(int ID, string name, int LOD, int triangles, int geometryAttr, int vertexArray, int indexArray)
            {
                this.ID = ID;
                this.name = name;
                this.LOD = LOD;
                this.triangles = triangles;
                this.geometryAttr = geometryAttr;
                this.vertexArray = vertexArray;
                this.indexArray = indexArray;
            }

            public void CleanupMesh(Dictionary<int, I3S.Block> _block)
            {
                triangles = 0;
                edit = true;
                _block[geometryAttr].data = new i3GeometryAttr(0, vertexArray, indexArray).ToBlock();
            }

            public void SaveAsPBG(Dictionary<int, I3S.Block> _block, string path)
            {
                //i3VertexArray vTemp;
                i3IndexArray fTemp;
                byte[] fBody;

                path += string.Format(@"/{0}_{1}.pbg", ID, name);

                if (_block.ContainsKey(vertexArray))
                {
                    if (_block.ContainsKey(indexArray))
                    {
                        fTemp = new i3IndexArray(_block[indexArray].data);
                        fBody = _block[indexArray].data;
                    }
                    else
                    {
                        fTemp = new i3IndexArray(triangles);
                        fBody = fTemp.ToBlock();
                    }
                    using (var ms = new MemoryStream())
                    {
                        using (var bw = new BinaryWriter(ms))
                        {
                            bw.Write(triangles); // Кол полигонов
                            bw.Write(_block[vertexArray].data.Length); // Длина блока вершин
                            bw.Write(fBody.Length); // Длина блока индексов
                            bw.Write(_block[vertexArray].data);
                            bw.Write(fBody);
                            File.WriteAllBytes(path, ms.ToArray());
                        }
                    }
                }
            }

            public void SaveAsOBJ(Dictionary<int, I3S.Block> _block, string path, bool optimize)
            {
                i3VertexArray vTemp;
                i3IndexArray fTemp;

                path += string.Format(@"/{0}_{1}.obj", ID, name);

                if (_block.ContainsKey(vertexArray))
                {
                    vTemp = new i3VertexArray(_block[vertexArray].data);
                    if (_block.ContainsKey(indexArray))
                    {
                        fTemp = new i3IndexArray(_block[indexArray].data);
                    }
                    else
                    {
                        fTemp = new i3IndexArray(triangles);
                    }

                    var _tempFace = fTemp._faceV;
                    if (optimize)
                    {
                        var _vertexToText = vTemp.vertexArray.Select((item, index) => new { Value = string.Join("", item), Index = index });
                        var _clones = _vertexToText.GroupBy(x => x.Value).Select(x => x.ToArray()).Where(x => x.Length > 1);
                        _tempFace = fTemp._faceV;
                        foreach (var element in _clones)
                        {
                            for (int i = 1; i < element.Length; i++)
                            {
                                for (int j = 0; j < _tempFace.Count; j++)
                                {
                                    for (int k = 0; k < _tempFace[j].Length; k++)
                                    {
                                        if (_tempFace[j][k] == element[i].Index)
                                        {
                                            _tempFace[j][k] = element[0].Index;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var TempOBJ = new OBJ(vTemp._vertexP, vTemp._vertexN, vTemp._vertexT, new OBJ.Group(ID.ToString(), _tempFace, _tempFace, _tempFace));
                    TempOBJ.SaveOBJ(path);
                }
            }

            public string[] Item()
            {
                var Item = new string[7];
                Item[1] = ID.ToString();
                Item[0] = name;
                Item[2] = LOD.ToString();
                Item[3] = triangles.ToString();
                Item[4] = geometryAttr.ToString();
                Item[5] = vertexArray.ToString();
                Item[6] = indexArray.ToString();
                return Item;
            }
        }

        public class Block
        {
            public I3S root;
            public int ID;
            public int line;
            public int target;
            public string type;
            public int start;
            public byte[] data;
            public byte[] collision;

            public Block(int ID, int line, int target, byte[] data, I3S root)
            {
                this.ID = ID;
                this.line = line;
                this.target = target;
                this.type = root.textLines[line];
                this.data = data;
                this.root = root;
            }

            public Block()
            {

            }

            public void SaveAsFile(string dir, bool toDDS)
            {
                string path = Path.Combine(dir, ID.ToString() + "." + type);
                if (type == "i3Texture" & data.Length > 0)
                {
                    if (toDDS)
                    {
                        File.WriteAllBytes(Path.ChangeExtension(path, "dds"), new I3IxDDS(data, "").ToDDS());
                    }
                    else
                    {
                        File.WriteAllBytes(Path.ChangeExtension(path, "i3i"), data);
                    }
                }
                else
                {
                    File.WriteAllBytes(path, data);
                }
            }

            public void CopyHEX()
            {
                if (data.Length > 0)
                {
                    Clipboard.SetText(BitConverter.ToString(data).Replace("-", " "));
                    if (data.Length > 600)
                    {
                        MessageBox.Show(BitConverter.ToString(data).Replace("-", " ").Remove(600));
                    }
                    else
                    {
                        MessageBox.Show(BitConverter.ToString(data).Replace("-", " ").Replace("-", " "));
                    }
                }
            }

            public void CopyType()
            {
                if (type.Length > 0)
                {
                    Clipboard.SetText(type);
                    MessageBox.Show(type);
                }
            }

            public void CopyStart()
            {
                var result = start.ToString();
                if (result.Length > 0)
                {
                    Clipboard.SetText(result);
                    MessageBox.Show(result);
                }
            }

            public void CopySize()
            {
                var result = data.Length.ToString();
                if (result.Length > 0)
                {
                    Clipboard.SetText(result);
                    MessageBox.Show(result);
                }
            }

            public void CopyComment()
            {
                if (Comment().Length > 0)
                {
                    Clipboard.SetText(Comment());
                    MessageBox.Show(Comment());
                }
            }

            public string Comment()
            {
                string comment = "";
                try
                {
                    if (data.Length > 0)
                    {
                        if (type == "i3Animation")
                        {
                            comment = new i3Animation(data).ToString();
                        }
                        if (type == "i3AttrSet")
                        {
                            comment = new i3AttrSet(data).ToString();
                        }
                        else if (type == "i3TransformSourceCombiner")
                        {
                            comment = new i3TransformSourceCombiner(data).ToString();
                        }
                        else if (type == "i3SceneGraphInfo")
                        {
                            comment = new i3SceneGraphInfo(data).ToString();
                        }
                        else if (type == "i3RegKey")
                        {
                            comment = new i3RegKey(data).ToString();
                        }
                        else if (type == "i3RegINT32")
                        {
                            comment = new i3RegINT32(data).ToString();
                        }
                        else if (type == "i3RegREAL32")
                        {
                            comment = new i3RegREAL32(data).ToString();
                        }
                        else if (type == "i3RegString")
                        {
                            comment = new i3RegString(data).ToString();
                        }
                        else if (type == "i3RegArray")
                        {
                            comment = new i3RegArray(data).ToString();
                        }
                        else if (type == "i3Node")
                        {
                            comment = new i3Node(data).ToString();
                        }
                        else if (type == "i3PackNode")
                        {
                            comment = new i3PackNode(data).ToString();
                        }
                        else if (type == "i3Skeleton")
                        {
                            comment = new i3Skeleton(data).ToString();
                        }
                        else if (type == "i3Texture")
                        {
                            comment = new i3Texture(data).ToString();
                        }
                        else if (type == "i3EmissiveMapEnableAttr" | type == "i3ZWriteEnableAttr" | type == "i3AlphaFuncAttr" | type == "i3BlendEnableAttr" | type == "i3AlphaTestEnableAttr" | type == "i3FaceCullModeAttr" | type == "i3TextureEnableAttr" | type == "i3NormalMapEnableAttr" | type == "i3SpecularMapEnableAttr" | type == "i3ReflectMapEnableAttr" | type == "i3ReflectMaskMapEnableAttr" | type == "i3LightingEnableAttr" | type == "i3LuxMapEnableAttr")
                        {
                            comment = new i3TextureEnableAttr(data).ToString();
                        }
                        else if (type == "i3EmissiveMapBindAttr" | type == "i3TextureBindAttr" | type == "i3NormalMapBindAttr" | type == "i3SpecularMapBindAttr" | type == "i3ReflectMaskMapBindAttr" | type == "i3LuxMapBindAttr")
                        {
                            comment = new i3TextureBindAttr(data).ToString();
                        }
                        else if (type == "i3LOD")
                        {
                            comment = new i3LOD(data).ToString();
                        }
                        else if (type == "i3Geometry")
                        {
                            comment = new i3Geometry(data).ToString();
                        }
                        else if (type == "i3GeometryAttr")
                        {
                            comment = new i3GeometryAttr(data).ToString();
                        }
                        else if (type == "i3Vector3Array")
                        {
                            comment = new i3Vector3Array(data).ToString();
                        }
                        else if (type == "i3VertexArray")
                        {
                            comment = new i3VertexArray(data).ToString();
                        }
                        else if (type == "i3IndexArray")
                        {
                            comment = new i3IndexArray(data).ToString();
                        }
                        else if (type == "i3MatrixObject")
                        {
                            comment = new i3MatrixObject(data).ToString();
                        }
                        else if (type == "i3MatrixArray")
                        {
                            comment = new i3MatrixArray(data).ToString();
                        }
                        else if (type == "i3MaterialAttr")
                        {
                            comment = new i3MaterialAttr(data).ToString();
                        }
                        else if (type == "i3BoneRef")
                        {
                            comment = new i3BoneRef(data).ToString();
                        }
                        else if (type == "i3BoneMatrixListAttr")
                        {
                            comment = new i3BoneMatrixListAttr(data).ToString();
                        }
                        else if (type == "i3UITemplate_User")
                        {
                            comment = new i3UITemplate_User(data).ToString();
                        }
                        else if (type == "i3UITemplate_ButtonImageSet")
                        {
                            comment = new i3UITemplate_User(data).ToString();
                        }
                        else if (type == "i3WorldSectionTable")
                        {
                            comment = new i3WorldSectionTable(data).ToString();
                        }
                    }
                    if (target < root.textLines.Length & target > 0)
                    {
                        comment += "→ " + root.textLines[target];
                    }
                    else if (target != 0)
                    {
                        comment += "→ " + target.ToString("X4");
                    }
                    if (i3TypeList.Contains(type) & data.Length > 1)
                    {
                        comment = Encoding.Default.GetString(data, 1, data[0]);
                    }
                }
                catch { }
                return comment;
            }

            public string[] Item()
            {
                string[] Item = new string[6];
                Item[0] = type;
                Item[1] = ID.ToString();
                Item[2] = Comment();
                Item[3] = start.ToString();
                Item[4] = data.Length.ToString();
                return Item;
            }
        }

        public class Pack
        {
            public int ID;
            public int start;
            public string folder;
            public string name;
            public byte[] data;

            public Pack(int ID, int start, string folder, string name, byte[] data)
            {
                this.ID = ID;
                this.start = start;
                this.folder = folder;
                this.name = name;
                this.data = data;
            }

            public void CopyName()
            {
                if (name.Length > 0)
                {
                    Clipboard.SetText(name);
                    MessageBox.Show(name);
                }
            }

            public void SaveAsFile(string dir, bool toDDS, bool open)
            {
                var path = Path.Combine(dir, name);
                if (Path.GetExtension(name).ToLower() == ".i3i" & toDDS)
                {
                    path = Path.ChangeExtension(path, ".dds");
                    File.WriteAllBytes(path, new I3IxDDS(data, name).ToDDS());
                }
                else
                {
                    File.WriteAllBytes(path, data);
                }
                if (open)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    catch
                    {

                    }
                }
            }

            public string[] Item()
            {
                return new string[] { name, folder, start.ToString("X4"), data.Length.ToString("X4") };
            }
        }

        public void SaveAllTextures(string dir, bool toDDS)
        {
            var path = dir;
            Directory.CreateDirectory(path);
            if (extension.ToLower() == ".i3pack")
            {
                var i3iList = PackListIndexsOfSelectedType(".i3i");
                SavePackByList(path, i3iList, toDDS);
                foreach (var element in _pack)
                {
                    if (Path.GetExtension(element.name).ToLower() == ".i3s")
                    {
                        new I3S(element.data, element.name).SaveAllTextures(Path.Combine(path, name), toDDS);
                    }
                }
            }
            if (extension.ToLower() == ".i3s")
            {
                var i3iList = BlockListIndexsOfSelectedType("i3Texture");
                SaveBlockByList(path, i3iList, toDDS);
            }
        }

        public void SaveAllMeshes(string dir, bool optimize)
        {
            var path = dir;
            Directory.CreateDirectory(path);

            if (extension.ToLower() == ".i3pack")
            {
                foreach (var element in _pack)
                {
                    if (Path.GetExtension(element.name).ToLower() == ".i3s")
                    {
                        new I3S(element.data, element.name).SaveAllMeshes(Path.Combine(path, name), optimize);
                    }
                }
            }
            if (extension.ToLower() == ".i3s")
            {
                SaveMeshByList(path, Enumerable.Range(0, _geometry.Count).ToArray(), true, true, optimize);
                SaveAllBones(dir);
            }
        }

        public void SaveAllBloks(string dir)
        {
            var path = dir;
            Directory.CreateDirectory(path);

            if (extension.ToLower() == ".i3pack")
            {
                foreach (var element in _pack)
                {
                    if (Path.GetExtension(element.name).ToLower() == ".i3s")
                    {
                        new I3S(element.data, element.name).SaveAllBloks(Path.Combine(path, name));
                    }
                }
            }
            SaveBlockByList(path, Enumerable.Range(0, _block.Count).ToArray(), false);
        }

        public void SaveAllContent(string dir, bool toDDS, bool withBlocks, bool withMeshes, bool optimize)
        {
            SaveAllTextures(dir, toDDS);
            SaveAllMeshes(dir, optimize);
            SaveAllBloks(dir);
        }

        public byte[] CreateI3S()
        {
            var fileBodyFull = new byte[0];



            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(Encoding.Default.GetBytes("I3R2"));
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(textLines.Length);
                    bw.Write((Int64)184);
                    bw.Write((Int64)text.Length);
                    bw.Write(_block.Count);
                    bw.Write((Int64)(text.Length + 184));
                    bw.Write((Int64)(_block.Count * 28));

                    bw.Write(text);

                    int startA = 184 + text.Length + _block.Count * 28;
                    for (int i = 0; i < _block.Count; i++)
                    {
                        bw.Write(_block.ElementAt(i).Value.line);
                        bw.Write(_block.ElementAt(i).Value.ID);
                        bw.Write(_block.ElementAt(i).Value.line);
                        bw.Write(_block.ElementAt(i).Value.data.Length);
                    }

                }
            }

            var infoPart = new byte[_block.Count * 28];

            fileBodyFull = PackByte.AttachBytes(fileBodyFull, header);
            fileBodyFull = PackByte.AttachBytes(fileBodyFull, text);
            fileBodyFull = PackByte.AttachBytes(fileBodyFull, infoPart);

            int start = header.Length + text.Length + infoPart.Length;
            int offset = header.Length + text.Length;

            for (int i = 0; i < _block.Count; i++)
            {
                var HEX_line = BitConverter.GetBytes(_block.ElementAt(i).Value.line);
                var HEX_ID = BitConverter.GetBytes((UInt16)_block.ElementAt(i).Value.ID);
                var HEX_start = BitConverter.GetBytes(start);
                var HEX_size = BitConverter.GetBytes(_block.ElementAt(i).Value.data.Length);
                Array.Copy(HEX_line, 0, fileBodyFull, 0 + i * 28 + offset, 4); // Указать номер линии
                Array.Copy(HEX_ID, 0, fileBodyFull, 4 + i * 28 + offset, 2);  // Указать номер ID
                Array.Copy(HEX_start, 0, fileBodyFull, 12 + i * 28 + offset, 4); // Указать место начала
                Array.Copy(HEX_size, 0, fileBodyFull, 20 + i * 28 + offset, 4); // Указать размер

                int target = _block.ElementAt(i).Value.target;
                if (target != 0)
                {
                    Array.Copy(BitConverter.GetBytes(target + 32768), 0, fileBodyFull, 6 + i * 28 + offset, 2); // Указать размер
                }

                fileBodyFull = PackByte.AttachBytes(fileBodyFull, _block.ElementAt(i).Value.data);

                if (_block.ElementAt(i).Value.type == "i3PhysixShapeSet")
                {
                    fileBodyFull = PackByte.AttachBytes(fileBodyFull, _block.ElementAt(i).Value.collision);
                    start += _block.ElementAt(i).Value.collision.Length;
                }
                if (_block.ElementAt(i).Value.type == "i3PackNode")
                {
                    var result = _pack.Where(x => x.ID == _block.ElementAt(i).Value.ID);
                    foreach (var element in result)
                    {
                        fileBodyFull = PackByte.AttachBytes(fileBodyFull, element.data);
                        start += element.data.Length;
                    }
                }

                start += _block.ElementAt(i).Value.data.Length;
            }

            Array.Copy(BitConverter.GetBytes(184), 0, fileBodyFull, 16, 4); // Указать размер
            Array.Copy(BitConverter.GetBytes(textLines.Length), 0, fileBodyFull, 12, 4); // Указать новое кол строк
            Array.Copy(BitConverter.GetBytes(text.Length), 0, fileBodyFull, 24, 4); // Указать новую длину текста
            Array.Copy(BitConverter.GetBytes(_block.Count), 0, fileBodyFull, 32, 4); // Указать новое кол блоков
            Array.Copy(BitConverter.GetBytes(text.Length + 184), 0, fileBodyFull, 36, 4); // Указать старт блока ссылок
            Array.Copy(BitConverter.GetBytes(infoPart.Length), 0, fileBodyFull, 44, 4); // Указать старт блока ссылок

            return fileBodyFull;
        }

        public static void DecryptA(byte[] buffer, byte key)
        {
            int length = buffer.Length;
            byte result = (byte)length;
            if (buffer.Length > 0)
            {
                int index = length - 1;
                int b;
                for (byte i = buffer[length - 1]; (index & 0x80000000) == 0; buffer[index + 1] = result)
                {
                    if (index <= 0)
                    {
                        b = i;
                    }
                    else
                    {
                        b = buffer[index - 1];
                    }
                    result = (byte)((b << 8 - key) | (buffer[index--] >> key));
                }
            }
        }

        public static byte[] DecryptB(byte[] data, int shift)
        {
            byte num = data[data.Length - 1];
            for (int index = data.Length - 1; index > 0; --index)
            {
                data[index] = (byte)(((int)data[index - 1] & (int)byte.MaxValue) << 8 - shift | ((int)data[index] & (int)byte.MaxValue) >> shift);
            }
            data[0] = (byte)((int)num << 8 - shift | ((int)data[0] & (int)byte.MaxValue) >> shift);
            return data;
        }

        public I3S(byte[] data, string path)
        {
            if (Encoding.Default.GetString(data, 0, 4) != "I3R2")
            {
                var dec = DecryptB(data, 3);
                MessageBox.Show("Decrypt 3");
                File.WriteAllBytes(Path.ChangeExtension(path, ".PefDec"), dec);
            }
            if (Encoding.Default.GetString(data, 1, 3) == "3R2")
            {
                backUp = data;

                int blockCount = 0;

                name = Path.GetFileNameWithoutExtension(path);
                extension = Path.GetExtension(path);
                link = path;

                using (var br = new BinaryReader(new MemoryStream(data)))
                {
                    br.ReadInt32(); // I3R2
                    br.ReadInt32(); // 0
                    br.ReadInt16(); // 1
                    br.ReadInt16(); // 8
                    br.ReadInt32(); // Количество строк

                    long start = 0;
                    header = new byte[184];
                    Array.Copy(data, start, header, 0, header.Length);

                    start = br.ReadInt64();
                    text = new byte[br.ReadInt64()];
                    Array.Copy(data, start, text, 0, text.Length);

                    blockCount = br.ReadInt32(); // Количество блоков

                    start = br.ReadInt64(); // Старт блока ссылок
                    info = new byte[br.ReadInt64()];
                    Array.Copy(data, start, info, 0, info.Length);
                }

                var textPartDec = Encoding.Default.GetString(text, 0, text.Length - 2);
                textLines = textPartDec.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                using (var br = new BinaryReader(new MemoryStream(info)))
                {
                    for (int i = 0; i < blockCount; i++)
                    {
                        var block = new I3S.Block();
                        block.root = this;
                        block.line = br.ReadInt32();
                        block.ID = br.ReadUInt16();
                        block.target = br.ReadUInt16();
                        if (block.target != 0)
                        {
                            block.target -= 32768;
                        }
                        int what = br.ReadInt32();
                        if (what != 0)
                        {
                            MessageBox.Show("yes");
                        }
                        block.start = (int)br.ReadInt64();
                        block.data = new byte[br.ReadUInt64()];
                        Array.Copy(data, block.start, block.data, 0, block.data.Length);

                        if (block.line < textLines.Length)
                        {
                            block.type = textLines[block.line];
                        }
                        else
                        {
                            block.type = string.Format("Error ({0})", block.line);
                        }
                        if (block.type == "i3PhysixShapeSet")
                        {
                            int offset = block.start + block.data.Length;
                            var col = new byte[BitConverter.ToInt32(data, offset + 12) + 68];
                            Array.Copy(data, offset, col, 0, col.Length);
                            block.collision = col;
                        }
                        if (!_block.ContainsKey(block.ID))
                        {
                            _block.Add(block.ID, block);
                        }
                        else
                        {
                            _blockD.Add(block.ID, block);
                        }
                    }
                    if (_blockD.Count > 0)
                    {
                        var l = string.Join(", ", Array.ConvertAll(_blockD.Keys.ToArray(), x => x.ToString()));
                        MessageBox.Show(string.Format("Blocks with ID: {0} is already contained in list!", l, name, MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    }
                }

                foreach (var element in _block)
                {
                    if (element.Value.type == "i3Geometry")
                    {
                        var geometry = new i3Geometry(element.Value.data);
                        if (geometry.link != 0)
                        {
                            var geometryAttr = new i3GeometryAttr(_block[geometry.link].data);
                            var geometryName = new i3Geometry(element.Value.data).name;
                            _geometry.Add(new I3S.Geometry(element.Value.ID, geometryName, 1, geometryAttr.trianglesCount, geometry.link, geometryAttr.vertexArray, geometryAttr.indexArray));
                        }
                    }
                    if (element.Value.type == "i3PackNode")
                    {
                        var pack = new i3PackNode(element.Value.data, backUp, element.Value.ID);
                        _pack.AddRange(pack._pack.ToArray());
                    }

                    if (element.Value.type == "i3LOD")
                    {
                        var lod = new i3LOD(element.Value.data);
                        var boneRef = new i3BoneRef(_block[lod.boneRef].data);
                        if (_block[boneRef.vaule[0]].type == "i3BoneRef")
                        {

                        }
                    }
                    if (element.Value.type == "i3AttrSet")
                    {


                    }
                }
            }
        }
    }
}
