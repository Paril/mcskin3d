using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace DaveChambers.FolderBrowserDialogEx
{
	internal class Win32
	{
		// Constants for sending and receiving messages in BrowseCallBackProc
		public const int WM_USER = 0x400;
		public const int WM_GETFONT = 0x0031;

		public const int MAX_PATH = 260;

		public const int BFFM_INITIALIZED = 1;
		public const int BFFM_SELCHANGED = 2;
		public const int BFFM_VALIDATEFAILEDA = 3;
		public const int BFFM_VALIDATEFAILEDW = 4;
		public const int BFFM_IUNKNOWN = 5; // provides IUnknown to client. lParam: IUnknown*
		public const int BFFM_SETSTATUSTEXTA = WM_USER + 100;
		public const int BFFM_ENABLEOK = WM_USER + 101;
		public const int BFFM_SETSELECTIONA = WM_USER + 102;
		public const int BFFM_SETSELECTIONW = WM_USER + 103;
		public const int BFFM_SETSTATUSTEXTW = WM_USER + 104;
		public const int BFFM_SETOKTEXT = WM_USER + 105; // Unicode only
		public const int BFFM_SETEXPANDED = WM_USER + 106; // Unicode only

		// Browsing for directory.
		public const uint BIF_RETURNONLYFSDIRS = 0x0001;  // For finding a folder to start document searching
		public const uint BIF_DONTGOBELOWDOMAIN = 0x0002;  // For starting the Find Computer
		public const uint BIF_STATUSTEXT = 0x0004;  // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if
		// this flag is set.  Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the
		// rest of the text.  This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
		// all three lines of text.
		public const uint BIF_RETURNFSANCESTORS = 0x0008;
		public const uint BIF_EDITBOX = 0x0010;   // Add an editbox to the dialog
		public const uint BIF_VALIDATE = 0x0020;   // insist on valid result (or CANCEL)

		public const uint BIF_NEWDIALOGSTYLE = 0x0040;   // Use the new dialog layout with the ability to resize
		// Caller needs to call OleInitialize() before using this API
		public const uint BIF_USENEWUI = 0x0040 + 0x0010; //(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);

		public const uint BIF_BROWSEINCLUDEURLS = 0x0080;   // Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
		public const uint BIF_UAHINT = 0x0100;   // Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX
		public const uint BIF_NONEWFOLDERBUTTON = 0x0200;   // Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
		public const uint BIF_NOTRANSLATETARGETS = 0x0400;  // don't traverse target as shortcut

		public const uint BIF_BROWSEFORCOMPUTER = 0x1000;  // Browsing for Computers.
		public const uint BIF_BROWSEFORPRINTER = 0x2000;// Browsing for Printers
		public const uint BIF_BROWSEINCLUDEFILES = 0x4000; // Browsing for Everything
		public const uint BIF_SHAREABLE = 0x8000;  // sharable resources displayed (remote shares, requires BIF_USENEWUI)

		public delegate int BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lp, IntPtr wp);

		[StructLayout(LayoutKind.Sequential)]
		public struct BROWSEINFO
		{
			public IntPtr hwndOwner;
			public IntPtr pidlRoot;
			//public IntPtr pszDisplayName;
			public string pszDisplayName;
			//[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszTitle;
			public uint ulFlags;
			public BrowseCallbackProc lpfn;
			public IntPtr lParam;
			public int iImage;
		}

		[DllImport("shell32.dll")]
		public static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

		// Note that the BROWSEINFO object's pszDisplayName only gives you the name of the folder.
		// To get the actual path, you need to parse the returned PIDL
		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		public static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath);

		[DllImport("shell32.dll", SetLastError = true)]
		public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, ref IntPtr ppidl);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool SetWindowText(IntPtr hwnd, String lpString);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		public const int SW_HIDE = 0;
		public const int SW_SHOW = 5;
		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);//ShowWindowCommands nCmdShow);



		public const int SWP_ASYNCWINDOWPOS = 0x4000;
		public const int SWP_DEFERERASE = 0x2000;
		public const int SWP_DRAWFRAME = 0x0020;
		public const int SWP_FRAMECHANGED = 0x0020;
		public const int SWP_HIDEWINDOW = 0x0080;
		public const int SWP_NOACTIVATE = 0x0010;
		public const int SWP_NOCOPYBITS = 0x0100;
		public const int SWP_NOMOVE = 0x0002;
		public const int SWP_NOOWNERZORDER = 0x0200;
		public const int SWP_NOREDRAW = 0x0008;
		public const int SWP_NOREPOSITION = 0x0200;
		public const int SWP_NOSENDCHANGING = 0x0400;
		public const int SWP_NOSIZE = 0x0001;
		public const int SWP_NOZORDER = 0x0004;
		public const int SWP_SHOWWINDOW = 0x0040;

		public const int HWND_TOP = 0;
		public const int HWND_BOTTOM = 1;
		public const int HWND_TOPMOST = -1;
		public const int HWND_NOTOPMOST = -2;


		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);



		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
		public static void MoveWindow(IntPtr hWnd, RECT rect, bool bRepaint)
		{
			MoveWindow(hWnd, rect.Left, rect.Top, rect.Width, rect.Height, bRepaint);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public RECT(int left, int top, int width, int height)
			{
				this.Left = left;
				this.Top = top;
				this.Right = left + width;
				this.Bottom = top + height;
			}

			public int Height { get { return this.Bottom - this.Top; } }
			public int Width { get { return this.Right - this.Left; } }
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			public static implicit operator System.Drawing.Point(POINT p)
			{
				return new System.Drawing.Point(p.X, p.Y);
			}

			public static implicit operator POINT(System.Drawing.Point p)
			{
				return new POINT(p.X, p.Y);
			}
		}

		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

		public static bool ScreenToClient(IntPtr hWnd, ref RECT rc)
		{
			POINT pt1 = new POINT(rc.Left, rc.Top);
			if (!ScreenToClient(hWnd, ref pt1))
				return false;
			POINT pt2 = new POINT(rc.Right, rc.Bottom);
			if(!ScreenToClient(hWnd, ref pt2))
				return false;

			rc.Left = pt1.X;
			rc.Top = pt1.Y;
			rc.Right = pt2.X;
			rc.Bottom = pt2.Y;

			return true;
		}


		public static readonly int GWL_WNDPROC = (-4);
		public static readonly int GWL_HINSTANCE = (-6);
		public static readonly int GWL_HWNDPARENT = (-8);
		public static readonly int GWL_STYLE = (-16);
		public static readonly int GWL_EXSTYLE = (-20);
		public static readonly int GWL_USERDATA = (-21);
		public static readonly int GWL_ID = (-12);
		public static readonly int DS_CONTEXTHELP = 0x2000;
		public static readonly int WS_EX_CONTEXTHELP = 0x00000400;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr windowTitle);


		public static readonly uint SHGFI_PIDL = 0x000000008;			// pszPath is a pidl
		public static readonly uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes

		public static readonly uint SFGAO_LINK = 0x00010000;     // Shortcut (link)

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		//[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		//public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(IntPtr pidlPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

		[DllImport("user32.dll")]
		public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);

		[DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetParent(IntPtr hWnd);

		//[DllImport("user32.dll", SetLastError = true)]
		//public static extern IntPtr CreateWindowEx(
		//   WindowStylesEx dwExStyle,
		//   string lpClassName,
		//   string lpWindowName,
		//   WindowStyles dwStyle,
		//   int x,
		//   int y,
		//   int nWidth,
		//   int nHeight,
		//   IntPtr hWndParent,
		//   IntPtr hMenu,
		//   IntPtr hInstance,
		//   IntPtr lpParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CreateWindowEx(
		   uint dwExStyle,
		   string lpClassName,
		   string lpWindowName,
		   uint dwStyle,
		   int x,
		   int y,
		   int nWidth,
		   int nHeight,
		   IntPtr hWndParent,
		   IntPtr hMenu,
		   IntPtr hInstance,
		   IntPtr lpParam);
	}
}
