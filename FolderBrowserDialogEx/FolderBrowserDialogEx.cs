using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace DaveChambers.FolderBrowserDialogEx
{
	public class FolderBrowserDialogEx
	{
		#region Fields that mimic the same-named fields in FolderBrowserDialog
		public Environment.SpecialFolder RootFolder { get; set; }
		public string SelectedPath { get; set; }
		public bool ShowNewFolderButton { get; set; }
		public FormStartPosition StartPosition { get; set; }
		#endregion

		// Fields specific to CustomFolderBrowserDialog
		public string Title { get; set; }
		public bool ShowEditbox { get; set; }

		// These are the control IDs used in the dialog
		private struct CtlIds
		{
			public const int PATH_EDIT = 0x3744;
			//public const int PATH_EDIT_LABEL = 0x3748;	// Only when BIF_NEWDIALOGSTYLE
			public const int TITLE = 0x3742;
			public const int TREEVIEW = 0x3741;
			public const int NEW_FOLDER_BUTTON = 0x3746;
			public const int IDOK = 1;
			public const int IDCANCEL = 2;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct InitData
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]	// Titles shouldn't too long, should they?
			public string Title;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
			public string InitialPath;

			public bool ShowEditbox;
			public bool ShowNewFolderButton;
			public FormStartPosition StartPosition;
			public IntPtr hParent;

			public InitData(FolderBrowserDialogEx dlg, IntPtr hParent)
			{
				// We need to make copies of these values from the dialog.
				// I tried passing the dlg obj itself in this struct, but Windows will barf after repeated invocations.
				this.Title = dlg.Title;
				this.InitialPath = dlg.SelectedPath;
				this.ShowNewFolderButton = dlg.ShowNewFolderButton;
				this.ShowEditbox = dlg.ShowEditbox;
				this.StartPosition = dlg.StartPosition;
				this.hParent = hParent;
			}
		}

		public FolderBrowserDialogEx()
		{
			Title = "Browse For Folder";	// Default to same caption as std dialog
			RootFolder = Environment.SpecialFolder.Desktop;
			SelectedPath = @"c:\";
			ShowEditbox = false;
			ShowNewFolderButton = false;
			StartPosition = FormStartPosition.WindowsDefaultLocation;
		}

		public DialogResult ShowDialog(IWin32Window owner)
		{
			InitData initdata = new InitData(this, owner.Handle);

			Win32.BROWSEINFO bi = new Win32.BROWSEINFO();
			bi.iImage = 0;
			bi.hwndOwner = owner.Handle;
			if (0 != Win32.SHGetSpecialFolderLocation(owner.Handle, (int)this.RootFolder, ref bi.pidlRoot))
				bi.pidlRoot = IntPtr.Zero;
			bi.lpszTitle = "";
			bi.ulFlags = Win32.BIF_RETURNONLYFSDIRS;	// do NOT use BIF_NEWDIALOGSTYLE or BIF_STATUSTEXT
			if (this.ShowEditbox)
				bi.ulFlags |= Win32.BIF_EDITBOX;
			if (!this.ShowNewFolderButton)
				bi.ulFlags |= Win32.BIF_NONEWFOLDERBUTTON;
			bi.lpfn = new Win32.BrowseCallbackProc(_browseCallbackHandler);
			// Initialization data, used in _browseCallbackHandler
			IntPtr hInit = Marshal.AllocHGlobal(Marshal.SizeOf(initdata));
			Marshal.StructureToPtr(initdata, hInit, true);
			bi.lParam = hInit;

			IntPtr pidlSelectedPath = IntPtr.Zero;
			try
			{
				pidlSelectedPath = Win32.SHBrowseForFolder(ref bi);
				StringBuilder sb = new StringBuilder(256);
				if (Win32.SHGetPathFromIDList(pidlSelectedPath, sb))
				{
					SelectedPath = sb.ToString();
					return DialogResult.OK;
				}
			}
			finally
			{
				// Caller is responsible for freeing this memory.
				Marshal.FreeCoTaskMem(pidlSelectedPath);
			}

			return DialogResult.Cancel;
		}

		private int _browseCallbackHandler(IntPtr hDlg, int msg, IntPtr lParam, IntPtr lpData)
		{
			switch (msg)
			{
				case Win32.BFFM_INITIALIZED:
					// remove context help button from dialog caption
					int lStyle = Win32.GetWindowLong(hDlg, Win32.GWL_STYLE);
					lStyle &= ~Win32.DS_CONTEXTHELP;
					Win32.SetWindowLong(hDlg, Win32.GWL_STYLE, lStyle);
					lStyle = Win32.GetWindowLong(hDlg, Win32.GWL_EXSTYLE);
					lStyle &= ~Win32.WS_EX_CONTEXTHELP;
					Win32.SetWindowLong(hDlg, Win32.GWL_EXSTYLE, lStyle);

					_adjustUi(hDlg, lpData);
					break;
				case Win32.BFFM_SELCHANGED:
					{
						bool ok = false;
						StringBuilder sb = new StringBuilder(Win32.MAX_PATH);
						if (Win32.SHGetPathFromIDList(lParam, sb))
						{
							ok = true;
							string dir = sb.ToString();
							IntPtr hEdit = Win32.GetDlgItem(hDlg, CtlIds.PATH_EDIT);
							Win32.SetWindowText(hEdit, dir);
#if UsingStatusText
							// We're not using status text, but if we were, this is how you'd set it
							Win32.SendMessage(hDlg, Win32.BFFM_SETSTATUSTEXTW, 0, dir);
#endif

#if SHBrowseForFolder_lists_links
							// This check doesn't seem to be necessary - the SHBrowseForFolder dirtree doesn't seem to list links
							Win32.SHFILEINFO sfi = new Win32.SHFILEINFO();
							Win32.SHGetFileInfo(lParam, 0, ref sfi, Marshal.SizeOf(sfi), Win32.SHGFI_PIDL | Win32.SHGFI_ATTRIBUTES);

							// fail if pidl is a link
							if ((sfi.dwAttributes & Win32.SFGAO_LINK) == Win32.SFGAO_LINK)
								ok = false;
#endif
						}

						// if invalid selection, disable the OK button
						if (!ok)
							Win32.EnableWindow(Win32.GetDlgItem(hDlg, CtlIds.IDOK), false);

						break;
					}
			}

			return 0;
		}

		private void _adjustUi(IntPtr hDlg, IntPtr lpData)
		{
			// Only do the adjustments if InitData was supplied
			if (lpData == IntPtr.Zero)
				return;
			object obj = Marshal.PtrToStructure(lpData, typeof(InitData));
			if (obj == null)
				return;
			InitData initdata = (InitData)obj;

			// Only do the adjustments if we can find the dirtree control
			IntPtr hTree = Win32.GetDlgItem(hDlg, CtlIds.TREEVIEW);
			if (hTree == IntPtr.Zero)
			{
				hTree = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "SysTreeView32", IntPtr.Zero);
				if (hTree == IntPtr.Zero)
				{
					// This usually means that BIF_NEWDIALOGSTYLE is enabled.
					hTree = Win32.FindWindowEx(hDlg, IntPtr.Zero, "SHBrowseForFolder ShellNameSpace Control", IntPtr.Zero);
				}
			}
			if (hTree == IntPtr.Zero)
				return;

			// Prep the basic UI
			Win32.SendMessage(hDlg, Win32.BFFM_SETSELECTIONW, 1, initdata.InitialPath);
			Win32.SetWindowText(hDlg, initdata.Title);

			if (initdata.StartPosition == FormStartPosition.CenterParent)
			{
				_centerTo(hDlg, initdata.hParent);
			}
			else if (initdata.StartPosition == FormStartPosition.CenterScreen)
			{
				_centerTo(hDlg, Win32.GetDesktopWindow());
			}
			// else we do nothing

			// Prep the edit box
			Win32.RECT rcEdit = new Win32.RECT();
			IntPtr hEdit = Win32.GetDlgItem(hDlg, CtlIds.PATH_EDIT);
			if (hEdit != IntPtr.Zero)
			{
				if (initdata.ShowEditbox)
				{
					Win32.GetWindowRect(hEdit, out rcEdit);
					Win32.ScreenToClient(hEdit, ref rcEdit);
				}
				else
				{
					Win32.ShowWindow(hEdit, Win32.SW_HIDE);
				}
			}

			// make the dialog larger
			Win32.RECT rcDlg;
			Win32.GetWindowRect(hDlg, out rcDlg);
			rcDlg.Right += 40;
			rcDlg.Bottom += 30;
			if (hEdit != IntPtr.Zero)
				rcDlg.Bottom += (rcEdit.Height + 5);
			Win32.MoveWindow(hDlg, rcDlg, true);
			Win32.GetClientRect(hDlg, out rcDlg);

			int vMargin = 10;
			// Accomodate the resizing handle's width
			int hMargin = 10;// SystemInformation.VerticalScrollBarWidth;

			// Move the Cancel button
			Win32.RECT rcCancel = new Win32.RECT();
			IntPtr hCancel = Win32.GetDlgItem(hDlg, CtlIds.IDCANCEL);
			if (hCancel != IntPtr.Zero)
			{
				Win32.GetWindowRect(hCancel, out rcCancel);
				Win32.ScreenToClient(hDlg, ref rcCancel);

				rcCancel = new Win32.RECT(rcDlg.Right-(rcCancel.Width + hMargin),
											rcDlg.Bottom - (rcCancel.Height + vMargin), 
											rcCancel.Width, 
											rcCancel.Height);

				Win32.MoveWindow(hCancel, rcCancel, false);
			}

			// Move the OK button
			Win32.RECT rcOK = new Win32.RECT();
			IntPtr hOK = Win32.GetDlgItem(hDlg, CtlIds.IDOK);
			if (hOK != IntPtr.Zero)
			{
				Win32.GetWindowRect(hOK, out rcOK);
				Win32.ScreenToClient(hDlg, ref rcOK);

				rcOK = new Win32.RECT(rcCancel.Left - (rcCancel.Width + hMargin), 
										rcCancel.Top, 
										rcOK.Width,
										rcOK.Height);

				Win32.MoveWindow(hOK, rcOK, false);
			}

			// Manage the "Make New Folder" button
			IntPtr hBtn = Win32.GetDlgItem(hDlg, CtlIds.NEW_FOLDER_BUTTON);
			if (!initdata.ShowNewFolderButton)
			{
				// Make sure this button is not visible
				Win32.ShowWindow(hBtn, Win32.SW_HIDE);
			}
			else if (hBtn == IntPtr.Zero)
			{
				// Create a button - button is only auto-created under BIF_NEWDIALOGSTYLE
				// This is failing, and I don't know why!
				hBtn = Win32.CreateWindowEx(0x50010000,
											"button",
											"&Make New Folder",
											0x00000004,
											hMargin,
											rcOK.Top,
											105,
											rcOK.Height,
											hDlg,
											new IntPtr(CtlIds.NEW_FOLDER_BUTTON),
											Process.GetCurrentProcess().Handle,
											IntPtr.Zero);
			}

			// Position the path editbox and it's label
			// We'll repurpose the Title (static) control as the editbox label
			int treeTop = vMargin;
			if (hEdit != IntPtr.Zero)
			{
				int xEdit = hMargin;
				int cxEdit = rcDlg.Width - (2 * hMargin);
				IntPtr hLabel = Win32.GetDlgItem(hDlg, CtlIds.TITLE);
				if (hLabel != IntPtr.Zero)
				{
					string labelText = "Folder: ";
					Win32.SetWindowText(hLabel, labelText);

					// This code obtains the required size of the static control that serves as the label for the editbox.
					// All this GDI code is a bit excessive, but I figured "what the hell".
					IntPtr hdc = Win32.GetDC(hLabel);
					IntPtr hFont = Win32.SendMessage(hLabel, Win32.WM_GETFONT, IntPtr.Zero, IntPtr.Zero);
					IntPtr oldfnt = Win32.SelectObject(hdc, hFont);
					Size szLabel = Size.Empty;
					Win32.GetTextExtentPoint32(hdc, labelText, labelText.Length, out szLabel);
					Win32.SelectObject(hdc, oldfnt);
					Win32.ReleaseDC(hLabel, hdc);

					Win32.RECT rcLabel = new Win32.RECT(hMargin, 
														vMargin + ((rcEdit.Height - szLabel.Height) / 2), 
														szLabel.Width,
														szLabel.Height);
					Win32.MoveWindow(hLabel, rcLabel, false);

					xEdit += rcLabel.Width;
					cxEdit -= rcLabel.Width;
				}

				// Expand the folder tree to fill the dialog
				rcEdit = new Win32.RECT(xEdit,
										vMargin,
										cxEdit,
										rcEdit.Height);

				Win32.MoveWindow(hEdit, rcEdit, false);
				treeTop = rcEdit.Bottom + 5;
			}

			Win32.RECT rcTree = new Win32.RECT(hMargin,
				treeTop,
				rcDlg.Width - (2 * hMargin),
				rcDlg.Bottom - (treeTop + (2*vMargin) + rcOK.Height));

			Win32.MoveWindow(hTree, rcTree, false);
		}

		private void _centerTo(IntPtr hDlg, IntPtr hRef)
		{
			Win32.RECT rcDlg;
			Win32.GetWindowRect(hDlg, out rcDlg);

			Win32.RECT rcRef;
			Win32.GetWindowRect(hRef, out rcRef);

			int cx = (rcRef.Width - rcDlg.Width) / 2;
			int cy = (rcRef.Height - rcDlg.Height) / 2;
			Win32.RECT rcNew = new Win32.RECT(rcRef.Left + cx,
												rcRef.Top + cy,
												rcDlg.Width,
												rcDlg.Height);
			Win32.MoveWindow(hDlg, rcNew, true);
		}

	}

}
