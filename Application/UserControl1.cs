using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Skript47
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public const int WM_USER = 0x400;
        public const int EM_GETSCROLLPOS = (WM_USER + 221);
        public const int EM_SETSCROLLPOS = (WM_USER + 222);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);

        unsafe POINT GetScrollPos(System.IntPtr myHandle)
        {
            POINT res = new POINT();
            IntPtr ptr = new IntPtr(&res);
            SendMessage(myHandle, EM_GETSCROLLPOS, 0, ptr);
            return res;
        }

        unsafe void SetScrollPos(POINT point, System.IntPtr myHandle)
        {
            IntPtr ptr = new IntPtr(&point);
            SendMessage(myHandle, EM_SETSCROLLPOS, 0, ptr);
        }

        void richTextBox1_VScroll(object sender, EventArgs e)
        {
            SetScrollPos(GetScrollPos(richTextBox1.Handle), richTextBox2.Handle);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            updateNumberLabel();
        }

        private void richTextBox1_Resize(object sender, EventArgs e)
        {
            SetScrollPos(GetScrollPos(richTextBox1.Handle), richTextBox2.Handle);
        }

        private void updateNumberLabel()
        {
            richTextBox2.Text = string.Join(Environment.NewLine, Enumerable.Range(1, richTextBox1.Lines.Length).ToArray());
            SetScrollPos(GetScrollPos(richTextBox1.Handle), richTextBox2.Handle);
        }
    }
}
