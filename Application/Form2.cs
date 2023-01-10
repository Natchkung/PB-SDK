using System;
using System.IO;
using System.Windows.Forms;

namespace Skript47
{
    public partial class Form2 : Form
    {
        Form1 main;

        public Form2()
        {
            InitializeComponent();
        }

        void Form2_Load(object sender, EventArgs e)
        {
            main = this.Owner as Form1;
        }

        void listView1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var element in files)
            {
                if (Directory.Exists(element))
                {
                    Directory.GetFiles(element, "*.i3vteximage");
                }
                else if (File.Exists(element))
                {
                    if (Path.GetExtension(element).ToLower() == ".i3vteximage")
                    {

                    }
                }
            }

        }

        void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }
    }
}
