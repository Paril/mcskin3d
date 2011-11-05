using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

class McSkinCursor
{

    public enum CursorModes { copy, move, no }

    public struct IconInfo
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

    [DllImport("user32.dll")]
    public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

    public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
    {
        IntPtr ptr = bmp.GetHicon();
        IconInfo tmp = new IconInfo();
        GetIconInfo(ptr, ref tmp);
        tmp.xHotspot = xHotSpot;
        tmp.yHotspot = yHotSpot;
        tmp.fIcon = false;
        ptr = CreateIconIndirect(ref tmp);
        return new Cursor(ptr);
    }

    public static Cursor createHeadCursor(CursorModes cursorMode, Image skinHead)
    {
        Cursor drawToCursor;
        switch (cursorMode)
        {
            case CursorModes.copy:
                drawToCursor = new Cursor(new MemoryStream(global::MCSkin3D.Properties.Resources.copy));
                break;
            case CursorModes.move:
                drawToCursor = new Cursor(new MemoryStream(global::MCSkin3D.Properties.Resources.move));
                break;
            default:
                drawToCursor = new Cursor(new MemoryStream(global::MCSkin3D.Properties.Resources.no));
                break;
        }
        Bitmap bmp = new Bitmap(64, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawImage(skinHead, new Rectangle(0, 0, 32, 32));
            drawToCursor.Draw(g, new Rectangle(35, 0, drawToCursor.Size.Width, drawToCursor.Size.Height));
        }
        Point hotSpot = new Point(35, 0);
        return CreateCursor(bmp, hotSpot.X, hotSpot.Y);
    }
}