using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Skript47
{
    public partial class Form3 : Form
    {
        Form1 main;
        FolderBrowserDialog fbd1 = new FolderBrowserDialog();

        public Form3()
        {
            InitializeComponent();
            lv1.Columns.Add("Name", 250, HorizontalAlignment.Left);
        }

        void Form3_Load(object sender, EventArgs e)
        {
            main = this.Owner as Form1;
            toolStripStatusLabel1.Text = main.directory;
            cb2.Items.AddRange(main.fileTag);
            cb2.SelectedItem = cb2.Items[0];
        }

        void cb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadList();
        }

        void cb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadList();
        }

        void LoadList()
        {
            toolStripStatusLabel1.Text = main.directory;
            if (!string.IsNullOrEmpty(toolStripStatusLabel1.Text) & cb2.SelectedIndex != -1)
            {
                var filter = textBox1.Text;
                if (Directory.Exists(toolStripStatusLabel1.Text))
                {
                    lv1.BeginUpdate();
                    lv1.Items.Clear();
                    var A = toolStripStatusLabel1.Text;
                    var B = cb2.SelectedItem.ToString();
                    var _fileList = Directory.GetFiles(A, B + "*.i3Pack", SearchOption.TopDirectoryOnly);
                    _fileList = _fileList.Where(a => (Path.GetFileNameWithoutExtension(a).ToLower()).Contains(filter.ToLower())).ToArray();
                    if (checkBox1.Checked)
                    {
                        var _itemList = Array.ConvertAll(_fileList, f => new ListViewItem(Path.GetFileNameWithoutExtension(f)) { ToolTipText = f });
                        lv1.Items.AddRange(_itemList);
                    }
                    else
                    {
                        foreach (var element in _fileList)
                        {
                            var props = new FileInfo(element);
                            if (props.Length > 313)
                            {
                                lv1.Items.Add(new ListViewItem(Path.GetFileNameWithoutExtension(element)) { ToolTipText = element });
                            }
                        }
                    }
                    lv1.EndUpdate();
                }
            }
        }

        void lv1_DoubleClick(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                if (File.Exists(lv1.SelectedItems[0].ToolTipText))
                {
                    main.tempFile = new I3S(File.ReadAllBytes(lv1.SelectedItems[0].ToolTipText), lv1.SelectedItems[0].ToolTipText);
                    main.ShowI3S();
                }
                else
                {
                    LoadList();
                }
            }
        }

        void extract_Click(object sender, EventArgs e)
        {
            if (fbd1.ShowDialog() == DialogResult.OK)
            {
                bool toDDS = main.checkBox1.Checked;
                foreach (ListViewItem item in lv1.SelectedItems)
                {
                    var Temp = new I3S(File.ReadAllBytes(item.ToolTipText), item.ToolTipText);
                    Temp.SavePackByList(fbd1.SelectedPath, Enumerable.Range(0, Temp._pack.Count).ToArray(), toDDS);
                }
            }
        }

        void extractGeometry_Click(object sender, EventArgs e)
        {
            if (fbd1.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem item in lv1.SelectedItems)
                {
                    var Temp = new I3S(File.ReadAllBytes(item.ToolTipText), item.ToolTipText);
                    Temp.SaveAllMeshes(fbd1.SelectedPath, main.checkBox2.Checked);
                }
            }
        }

        void extractBloks_Click(object sender, EventArgs e)
        {
            if (fbd1.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem item in lv1.SelectedItems)
                {
                    var Temp = new I3S(File.ReadAllBytes(item.ToolTipText), item.ToolTipText);
                    Temp.SaveAllBloks(fbd1.SelectedPath);
                }
            }
        }

        void extractTextures_Click(object sender, EventArgs e)
        {
            if (fbd1.ShowDialog() == DialogResult.OK)
            {
                bool toDDS = main.checkBox1.Checked;
                foreach (ListViewItem item in lv1.SelectedItems)
                {
                    var Temp = new I3S(File.ReadAllBytes(item.ToolTipText), item.ToolTipText);
                    Temp.SaveAllTextures(fbd1.SelectedPath, toDDS);
                }
            }
        }

        void lv1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        void lv1_DragDrop(object sender, DragEventArgs e)
        {
            int exs = 0;
            int total = ((string[])e.Data.GetData(DataFormats.FileDrop)).Length;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (var element in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    var patchMain = Path.Combine(toolStripStatusLabel1.Text, Path.GetFileName(element));
                    if (File.Exists(patchMain))
                    {
                        exs++;
                    }
                    File.Copy(element, patchMain, true);
                }
                MessageBox.Show(string.Format("Added: {0}. Replaced: {1}", total - exs, exs), "Done!");
            }
        }

        void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                if (File.Exists(lv1.SelectedItems[0].ToolTipText))
                {
                    Process.Start("explorer.exe", "/select, " + lv1.SelectedItems[0].ToolTipText);
                }
                else
                {
                    LoadList();
                }
            }
        }

        void selectAnotherFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            main.ReadDirectoryList(true);
            LoadList();
        }

        void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                LoadList();
            }
        }

        void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                var paths = new StringCollection();
                foreach (ListViewItem element in lv1.SelectedItems)
                {
                    paths.Add(element.ToolTipText);
                }
                Clipboard.SetFileDropList(paths);
            }
        }

        void optimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                foreach (ListViewItem element in lv1.SelectedItems)
                {

                }
            }
        }

        void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadList();
        }

        void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                var temp = lv1.SelectedItems.Cast<ListViewItem>().Select(a => a.Text).ToArray();
                Clipboard.SetText(string.Join(Environment.NewLine, temp));
            }
        }

        void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            main.ReadDirectoryList(true);
            LoadList();
        }

        void textBox1_Enter(object sender, EventArgs e)
        {
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                if (lang.Culture.Name == "en-US")
                {
                    InputLanguage.CurrentInputLanguage = lang;
                }
            }
        }

        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            LoadList();
        }
    }
}
