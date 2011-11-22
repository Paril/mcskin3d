using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace MCSkin3DLanguageEditor.Jonas
{
    class fixLinkLabel : LinkLabel
    {
        private int WM_SETCURSOR = 32;
        private int IDC_HAND = 32649;

        [DllImport("user32.dll")]
        public static extern int LoadCursor(int hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        public static extern int SetCursor(int hCusor);

        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == WM_SETCURSOR)
            {
                SetCursor(LoadCursor(0, IDC_HAND));
                msg.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref msg);
        }

        public fixLinkLabel()
        {
            this.ActiveLinkColor = Color.FromArgb(0, 14, 151);
            this.LinkColor = Color.FromArgb(0, 112, 238);
            this.VisitedLinkColor = Color.FromArgb(0, 112, 238);
        }
    }
}
