using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MCSkin3DLanguageEditor.Jonas
{
    class fixPanel : Panel
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
    }
}
