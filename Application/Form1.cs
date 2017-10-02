using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Skript47
{
    public partial class Form1 : Form
    {
        string loginName = "None";
        string loginDate = "31.12.2015";
        string YriyRublevsky = "Yuri Rublevsky";
        public I3S tempPack;
        public I3S tempFile;
        public string[] fileType = { "i3Pack", "i3s", "pef", "i3a", "I3CHR", "i3CharaEditor", "i3wrd", "i3Light", "i3Obj", "i3ObjectEditor", "i3Evt", "i3Path", "i3Game", "i3GL", "i3font", "env", "i3UIs", "gui", "guiNode", "i3LevelDesign", "i3AI", "i3UILib" };
        public string[] fileTag = { "", "Weapon", "Chara", "Equip", "Object", "Sound", "Script", "String", "UI", "F_", "M_", "G_", "O_" };
        public string directory;
        OpenFileDialog ofd1 = new OpenFileDialog();
        SaveFileDialog sfd1 = new SaveFileDialog();
        OpenFileDialog ofd2 = new OpenFileDialog();
        SaveFileDialog sfd2 = new SaveFileDialog();
        OpenFileDialog ofd3 = new OpenFileDialog();
        FolderBrowserDialog fbd1 = new FolderBrowserDialog();
        Form2 f2 = new Form2();
        Form3 f3 = new Form3();

        public Form1()
        {
            InitializeComponent();
            lv1.Columns.Add("Type", 210, HorizontalAlignment.Left);
            lv1.Columns.Add("ID", 60, HorizontalAlignment.Left);
            lv1.Columns.Add("Comment", 350, HorizontalAlignment.Left);
            lv1.Columns.Add("Start", 80, HorizontalAlignment.Left);
            lv1.Columns.Add("Size", 80, HorizontalAlignment.Left);
            lv2.Columns.Add("Name", 260, HorizontalAlignment.Left);
            lv2.Columns.Add("ID", 60, HorizontalAlignment.Left);
            lv2.Columns.Add("LOD", 60, HorizontalAlignment.Left);
            lv2.Columns.Add("Triangles", 80, HorizontalAlignment.Left);
            lv2.Columns.Add("G", 60, HorizontalAlignment.Left);
            lv2.Columns.Add("V", 60, HorizontalAlignment.Left);
            lv2.Columns.Add("I", 60, HorizontalAlignment.Left);
            lv3.Columns.Add("File", 320, HorizontalAlignment.Left);
            lv3.Columns.Add("Folder", 220, HorizontalAlignment.Left);
            lv3.Columns.Add("Start", 80, HorizontalAlignment.Left);
            lv3.Columns.Add("Size", 80, HorizontalAlignment.Left);
            lv1.Font = new Font(FontFamily.GenericMonospace, lv1.Font.Size);
            lv2.Font = new Font(FontFamily.GenericMonospace, lv2.Font.Size);
            lv3.Font = new Font(FontFamily.GenericMonospace, lv3.Font.Size);
            rtb1.Font = new Font(FontFamily.GenericMonospace, rtb1.Font.Size);
            ofd3.Multiselect = true;
            ofd3.Filter = "i3VTexImage (*.i3VTexImage)|*.i3VTexImage;";
            //lv1.Enabled = false;
            //alllContentToolStripMenuItem.Enabled = false;
            //importStripMenuItem.Enabled = false;
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            ofd1.Filter = "All Types (*.*)|*.*|I3 Engine Files (*.*)|" + string.Join(";", Array.ConvertAll(fileType, a => string.Format("*.{0}", a)));
            ofd1.Filter += "|" + string.Join("|", Array.ConvertAll(fileType, a => string.Format("{0} (*.{0})|*.{0}", a)));
            ReadCommandLineArgs(Environment.GetCommandLineArgs());
            ReadDirectoryList(false);
            ClearTempFiles();
        }

        public void ReadDirectoryList(bool update)
        {
            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(myDocuments, "PB SDK - By Skript47");
            var file = @"\Directory.ini";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(path + file))
            {
                File.WriteAllText(path + file, string.Empty);
            }
            directory = File.ReadAllLines(path + file).Where(a => Directory.Exists(a)).FirstOrDefault();
            if (directory == null || update)
            {
                var fbd1 = new FolderBrowserDialog() { Description = "Please select the 'pack' folder path" };
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    directory = fbd1.SelectedPath;
                    File.WriteAllText(path + file, directory);
                }
            }
        }

        void ClearTempFiles()
        {
            var path = Path.Combine(Path.GetTempPath(), "i3pack");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        void ReadCommandLineArgs(string[] args)
        {
            if (args.Length > 1)
            {
                tempFile = new I3S(File.ReadAllBytes(args[1]), args[1]);
                ShowI3S();
            }
        }

        void b1_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                tempFile = new I3S(File.ReadAllBytes(ofd1.FileName), ofd1.FileName);
                ShowI3S();
            }
        }

        public void ShowI3S()
        {
            lv1.BeginUpdate();
            lv2.BeginUpdate();
            lv3.BeginUpdate();
            try
            {
                tssl1.Text = tempFile.name + tempFile.extension;
                tssl2.Text = "Size: " + String.Format(new FileSizeFormatProvider(), "{0:fs}", tempFile.backUp.Length);
                tssl3.Text = "Header: " + tempFile.text.Length.ToString() + "/" + tempFile.textLines.Length.ToString();
                tssl4.Text = "Content: " + tempFile._block.Count.ToString();
                rtb1.Clear();
                rtb1.Text = Encoding.Default.GetString(tempFile.text);

                userControl1.richTextBox1.Text = Encoding.Default.GetString(tempFile.text);
                lv1.Items.Clear();
                lv2.Items.Clear();
                lv3.Items.Clear();
                foreach (var element in tempFile._block)
                {
                    var item = new ListViewItem(element.Value.Item()) { Checked = false };
                    lv1.Items.Add(item);
                }
                foreach (var element in tempFile._geometry)
                {
                    var item = new ListViewItem(element.Item()) { Checked = false };
                    if (element.edit)
                    {
                        item.ForeColor = Color.Red;
                    }
                    lv2.Items.Add(item);
                }
                foreach (var element in tempFile._pack)
                {
                    var item = new ListViewItem(element.Item()) { Checked = false };
                    if (element.name.ToLower().EndsWith(".i3i"))
                    {
                        var temp = new I3IxDDS(element.data, element.name);
                        item.ToolTipText = string.Format("Type:{0} Size:{1} x {2} MipMaps:{3} Normal:{4}", temp.type, temp.x, temp.y, temp.mipMaps, temp.normal);
                    }
                    lv3.Items.Add(item);
                    lv3.ShowItemToolTips = true;
                }
                tabControl1.SelectedTab = tp2;
                if (tempFile._pack.Count > 0)
                {
                    tabControl1.SelectedTab = tp4;
                }
                if (tempFile._geometry.Count > 0)
                {
                    tabControl1.SelectedTab = tp3;
                }
            }
            catch (NullReferenceException)
            {

            }
            lv1.EndUpdate();
            lv2.EndUpdate();
            lv3.EndUpdate();
        }

        void UpdateI3S()
        {
            tempFile.textLines = rtb1.Lines;
            tempFile.text = Encoding.Default.GetBytes((rtb1.Text + "\0\0").Replace("\n", "\r\n"));
        }

        void b2_Click(object sender, EventArgs e)
        {
            if (tempFile != null)
            {
                if (tempFile.link.Length > 0)
                {
                    sfd1.InitialDirectory = Path.GetDirectoryName(tempFile.link);
                }
                sfd1.FileName = Path.GetFileNameWithoutExtension(tempFile.link);
                sfd1.Filter = tempFile.extension + "|*" + tempFile.extension;
                if (sfd1.ShowDialog() == DialogResult.OK)
                {
                    UpdateI3S();
                    File.WriteAllBytes(sfd1.FileName, tempFile.CreateI3S());
                }
            }
        }

        void b6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PB SDK v3.0. Copyright © 2015 " + YriyRublevsky + "\nUser: " + loginName + ". License expiration date: " + loginDate, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void b7_Click(object sender, EventArgs e)
        {
            byte[] code = { 0x68, 0x74, 0x74, 0x70, 0x73, 0x3A, 0x2F, 0x2F, 0x6C, 0x69, 0x76, 0x65, 0x74, 0x6F, 0x77, 0x69, 0x6E, 0x33, 0x34, 0x2E, 0x62, 0x6C, 0x6F, 0x67, 0x73, 0x70, 0x6F, 0x74, 0x2E, 0x63, 0x6F, 0x6D };
            System.Diagnostics.Process.Start(Encoding.UTF8.GetString(code));
        }

        void openMenuItem_Click(object sender, EventArgs e)
        {
            if (lv3.SelectedIndices.Count > 0)
            {
                //try
                {
                    var path = Path.Combine(Path.GetTempPath(), "i3pack");
                    Directory.CreateDirectory(path);

                    tempPack = tempFile;

                    var newData = tempFile._pack.ElementAt(lv3.SelectedIndices[0]).data;
                    var newName = tempFile._pack.ElementAt(lv3.SelectedIndices[0]).name;

                    var pData = tempFile.backUp;
                    var pName = tempFile.link;

                    if (fileType.Any(i => ("." + i).ToLower() == Path.GetExtension(newName).ToLower()))
                    {
                        tempFile = new I3S(newData, newName);
                        tempFile.parent = pData;
                        tempFile.parentLink = pName;
                        ShowI3S();
                    }
                    else
                    {
                        tempFile._pack.ElementAt(lv3.SelectedIndices[0]).SaveAsFile(path, true, true);
                    }
                }
                //catch
                {

                }
            }
        }

        void toolItem2_Click(object sender, EventArgs e)
        {
            var n = LvGetCheckedIndexs(lv3);
            if (n.Length > 0)
            {
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    tempFile.SavePackByList(fbd1.SelectedPath, n, checkBox1.Checked);
                }
            }
        }

        void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var n = LvGetCheckedIndexs(lv1);
            if (n.Length > 0)
            {
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    tempFile.SaveBlockByList(fbd1.SelectedPath, n, checkBox1.Checked);
                }
            }
        }

        void toolStripMenuItem3_Click(object sender, EventArgs e) // Импорт файла в I3pack
        {
            if (lv3.SelectedItems.Count > 0)
            {
                if (ofd2.ShowDialog() == DialogResult.OK)
                {
                    var newBlock = File.ReadAllBytes(ofd2.FileName);
                    if (Path.GetExtension(ofd2.FileName).ToLower() == ".dds" && Path.GetExtension(tempFile._pack[lv3.SelectedIndices[0]].name).ToLower() == ".i3i")
                    {
                        newBlock = new I3IxDDS(newBlock, ofd2.FileName).ToI3I("");
                    }
                    if (Path.GetExtension(ofd2.FileName).ToLower() == ".i3i")
                    {
                        newBlock = new I3IxDDS(newBlock, ofd2.FileName).ToI3I("");
                    }

                    if (tempFile._pack[lv3.SelectedIndices[0]].data.Length >= newBlock.Length)
                    {
                        tempFile._pack[lv3.SelectedIndices[0]].data = newBlock;
                        lv3.SelectedItems[0].ForeColor = Color.Red;
                    }
                    else
                    {
                        MessageBox.Show(tempFile._pack[lv3.SelectedIndices[0]].data.Length.ToString());
                        MessageBox.Show(newBlock.Length.ToString());
                        tempFile._pack[lv3.SelectedIndices[0]].data = newBlock;
                    }
                }
            }
        }

        void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                var tempType = tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.type;
                ofd2.Filter = tempType + "|*." + tempType;
                if (tempType == "i3Texture")
                {
                    ofd2.Filter = "Image Files (*.i3i, *.dds)|*.i3i;*.dds";
                    ofd2.Filter += "|i3i (*.i3i)|*.i3i";
                    ofd2.Filter += "|dds (*.dds)|*.dds";
                }
                if (ofd2.ShowDialog() == DialogResult.OK)
                {
                    var newBlock = File.ReadAllBytes(ofd2.FileName);
                    if (Path.GetExtension(ofd2.FileName).ToLower() == ".dds")
                    {
                        newBlock = new I3IxDDS(newBlock, ofd2.FileName).ToI3I(Path.GetFileNameWithoutExtension(ofd2.FileName));
                    }
                    tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.data = newBlock;
                    tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.target = 0;
                    lv1.SelectedItems[0].ForeColor = Color.Red;
                }
            }
        }

        void exportMenuItem_Click(object sender, EventArgs e)
        {
            var n = LvGetCheckedIndexs(lv2);
            if (n.Length > 0)
            {
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    tempFile.SaveMeshByList(fbd1.SelectedPath, n, true, true, checkBox2.Checked);
                }
            }
        }

        void importMenuItem_Click(object sender, EventArgs e)
        {
            if (lv2.SelectedItems.Count > 0)
            {
                ofd2.Filter = "Mesh Files (*.obj, *.pbg)|*.obj;*.pbg";
                ofd2.Filter += "|obj (*.obj)|*.obj";
                ofd2.Filter += "|pbg (*.pbg)|*.pbg";
                if (ofd2.ShowDialog() == DialogResult.OK)
                {
                    int gID = tempFile._geometry.ElementAt(lv2.SelectedIndices[0]).ID;
                    int gV = tempFile._geometry.ElementAt(lv2.SelectedIndices[0]).vertexArray;
                    int gF = tempFile._geometry.ElementAt(lv2.SelectedIndices[0]).indexArray;

                    if (tempFile._geometry.Where(x => x.vertexArray == gV).ToArray().Length > 1)
                    {
                        var dialogResult = MessageBox.Show(string.Format("i3VertexArray {0} is not unique, do you want create new i3VertexArray?", gV), "", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            var nLine = tempFile._block.FirstOrDefault(a => a.Value.type == "i3VertexArray").Value.line;
                            CreateBlock(1, nLine);
                            tempFile._geometry.ElementAt(lv2.SelectedIndices[0]).vertexArray = tempFile._block.Count;
                        }
                    }

                    if (Path.GetExtension(ofd2.FileName).ToLower() == ".obj")
                    {
                        tempFile.ImportOBJ(lv2.SelectedIndices[0], File.ReadAllLines(ofd2.FileName));
                    }

                    if (Path.GetExtension(ofd2.FileName).ToLower() == ".pbg")
                    {
                        tempFile.ImportPBG(lv2.SelectedIndices[0], File.ReadAllBytes(ofd2.FileName));
                    }
                }
            }
            ShowI3S();
        }

        void clearMeshMenuItem_Click(object sender, EventArgs e)
        {
            if (lv2.SelectedItems.Count > 0)
            {
                var n = LvGetCheckedIndexs(lv2);
                tempFile.CleanupMeshByList(n);
            }
            ShowI3S();
        }

        void flipNormalsMenuItem_Click(object sender, EventArgs e)
        {
            if (lv2.SelectedItems.Count > 0)
            {
                var n = LvGetCheckedIndexs(lv2);
                tempFile.CleanupMeshByList(n);
            }
            ShowI3S();
        }

        void blockHEXMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.CopyHEX();
            }
        }

        void typeMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.CopyType();
            }
        }

        void commentMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.CopyComment();
            }
        }

        void nameMenuItem_Click(object sender, EventArgs e)
        {
            if (lv3.SelectedItems.Count > 0)
            {
                tempFile._pack[lv3.SelectedIndices[0]].CopyName();
            }
        }

        void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            CreateBlock(1, 0);
        }

        void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            CreateBlock(2, 0);
        }

        void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            CreateBlock(5, 0);
        }

        void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            CreateBlock(10, 0);
        }

        void CreateBlock(int number, int l)
        {
            for (int i = 0; i < number; i++)
            {
                tempFile._block.Add(tempFile._block.Count + 1, new I3S.Block(tempFile._block.Count + 1, l, 0, new byte[0], tempFile));
                var item = new ListViewItem(tempFile._block.ElementAt(lv1.Items.Count).Value.Item());
                item.Checked = false;
                lv1.Items.Add(item);
            }
        }

        void removeMenuItem_Click(object sender, EventArgs e)
        {
            lv1.BeginUpdate();
            foreach (ListViewItem item in lv1.CheckedItems)
            {
                tempFile._block.Remove(tempFile._block.ElementAt(item.Index).Key);
                lv1.Items.Remove(item);
            }
            lv1.EndUpdate();
        }

        void tssl1_Click(object sender, EventArgs e)
        {
            StatusCopy(tssl1.Text);
        }

        void tssl2_Click(object sender, EventArgs e)
        {
            StatusCopy(tssl2.Text);
        }

        void tssl3_Click(object sender, EventArgs e)
        {
            StatusCopy(tssl3.Text);
        }

        void tssl4_Click(object sender, EventArgs e)
        {
            StatusCopy(tssl4.Text);
        }

        void tssl5_Click(object sender, EventArgs e)
        {
            StatusCopy(tssl5.Text);
        }

        void StatusCopy(string text)
        {
            if (text.Length > 0)
            {
                MessageBox.Show(text);
                Clipboard.SetText(text);
            }
        }

        protected string GetMD5HashFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                    }
                }
            }
            else
            {
                return "-";
            }
        }

        void b3_Click(object sender, EventArgs e)
        {
            if (f2 == null || f2.IsDisposed)
            {
                f2 = new Form2();
            }
            f2.Owner = this;
            f2.Show();
            f2.Focus();
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.O && e.Control)
            {
                b2_Click(null, null);
            }
            if (e.KeyCode == Keys.S && e.Control)
            {
                b2_Click(null, null);
            }
            if (e.KeyCode == Keys.R && e.Control)
            {
                tempFile = new I3S(tempFile.backUp, tempFile.link);
                ShowI3S();
            }
            if (e.KeyCode == Keys.E && e.Control)
            {
                if (tempFile.parent.Length > 0)
                {
                    tempFile = new I3S(tempFile.parent, tempFile.parentLink);
                    ShowI3S();
                }
            }
        }

        void b5_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)
            {
                f3 = new Form3();
            }
            f3.Owner = this;
            f3.Show();
            f3.Focus();
        }

        void Form1_Activated(object sender, EventArgs e)
        {
            statusStrip1.Refresh();
        }

        void lv1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            LvCheckAllItems(lv1, true);
        }

        void deselectAllMenuItem_Click(object sender, EventArgs e)
        {
            LvCheckAllItems(lv1, false);
        }

        void selectAllMenuItem2_Click(object sender, EventArgs e)
        {
            LvCheckAllItems(lv2, true);
        }

        void deselectAllMenuItem2_Click(object sender, EventArgs e)
        {
            LvCheckAllItems(lv2, false);
        }

        void selectAllMenuItem1_Click(object sender, EventArgs e)
        {
            LvCheckAllItems(lv3, true);
        }

        void deselectAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LvCheckAllItems(lv3, false);
        }

        int[] LvGetCheckedIndexs(ListView lvw)
        {
            return lvw.CheckedIndices.Cast<int>().ToArray();
        }

        void LvCheckAllItems(ListView lvw, bool check)
        {
            lvw.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = check);
        }

        void typesListMenuItem_Click(object sender, EventArgs e)
        {
            tempFile.CopyListOfTypes();
        }

        void namesLisMenuItem_Click(object sender, EventArgs e)
        {
            tempFile.CopyListOfFiles();
        }

        void sameTypeMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                var type = tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.type;
                foreach (ListViewItem item in lv1.Items)
                {
                    item.Checked = item.Text == type;
                }
            }
        }

        void lv1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                var type = tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.type;
                var data = tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.data;
                if (type == "i3RegArray")
                {
                    f2.textBox1.Text = new i3RegArray(data).ToText();
                }
                else
                {
                    f2.textBox1.Text = string.Empty;
                }
            }
        }

        void selectTexturesMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lv3.Items)
            {
                item.Checked = Path.GetExtension(item.SubItems[0].Text).ToLower() == ".i3i";
            }
        }

        void selectOthersMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lv3.Items)
            {
                item.Checked = Path.GetExtension(item.SubItems[0].Text).ToLower() != ".i3i";
            }
        }

        void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (tempFile != null)
            {
                tempFile.Optimize();
            }
        }

        void filesMenuItem_Click(object sender, EventArgs e)
        {
            if (tempFile != null)
            {
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    tempFile.SavePackByList(fbd1.SelectedPath, Enumerable.Range(0, tempFile._pack.Count).ToArray(), checkBox1.Checked);
                }
            }
        }

        void texturesMenuItem_Click(object sender, EventArgs e)
        {
            if (tempFile != null)
            {
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    tempFile.SaveAllTextures(fbd1.SelectedPath, checkBox1.Checked);
                }
            }
        }

        void geometryMenuItem_Click(object sender, EventArgs e)
        {
            if (tempFile != null)
            {
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    tempFile.SaveAllMeshes(fbd1.SelectedPath, checkBox2.Checked);
                }
            }
        }

        void alllContentMenuItem_Click(object sender, EventArgs e)
        {
            if (tempFile != null)
            {
                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    tempFile.SaveAllBloks(fbd1.SelectedPath);
                }
            }
        }

        void blackItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                var MyDialog = new ColorDialog();
                MyDialog.AllowFullOpen = false;
                MyDialog.ShowHelp = true;
                if (MyDialog.ShowDialog() == DialogResult.OK)
                {
                    var newTexture = new I3IxDDS(new byte[10], "Color");
                    newTexture.comment = "Color";
                    newTexture.body = ColorToHex(MyDialog.Color);
                    newTexture.x = 1;
                    newTexture.y = 1;
                    newTexture.mipMaps = 1;
                    newTexture.type = "X8R8G8B8";
                    tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.data = newTexture.ToI3I(MyDialog.Color.ToString());
                    tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.target = 0;
                    lv1.SelectedItems[0].ForeColor = Color.Red;
                }
            }
        }

        static byte[] ColorToHex(Color c)
        {
            return new byte[] { (byte)c.B, (byte)c.G, (byte)c.R, (byte)c.A };
        }

        void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.CopyStart();
            }
        }

        void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                tempFile._block.ElementAt(lv1.SelectedIndices[0]).Value.CopySize();
            }
        }

        void i3VTexImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (f2 == null || f2.IsDisposed)
            {
                f2 = new Form2();
            }
            f2.Owner = this;
            f2.Show();
            f2.Focus();

            try
            {
                if (ofd3.ShowDialog() == DialogResult.OK)
                {
                    var filesCount = 0;
                    foreach (var element in ofd3.FileNames)
                    {
                        var x = new i3VTexImage(File.ReadAllBytes(element));
                        var name = Path.ChangeExtension(element, ".tga");
                        File.WriteAllBytes(name, x.ToTGA());
                        if (ofd3.FileNames.Length == 1)
                        {
                            System.Diagnostics.Process.Start(name);
                        }
                        filesCount++;
                    }
                    if (ofd3.FileNames.Length > 1)
                    {
                        MessageBox.Show(filesCount.ToString() + " files successfully converted!", "PB SDK - By Skript47");
                    }
                }
            }
            catch
            {

            }
        }
    }
}