//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using MCSkin3D.Properties;
using Paril.Extensions;
using Timer = System.Timers.Timer;

namespace MCSkin3D
{
	// Summary:
	//     Exposes a method that compares two objects.
	[ComVisible(true)]
	public class SkinNodeSorter : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			var l = (TreeNode) x;
			var r = (TreeNode) y;

			if (l is Skin && !(r is Skin))
				return 1;
			else if (!(l is Skin) && r is Skin)
				return -1;
			else if (l is Skin && r is Skin)
				return ((Skin) l).Name.CompareTo(((Skin) r).Name);

			return l.Text.CompareTo(r.Text);
		}

		#endregion
	}

	public class SkinTreeView : TreeView
	{
		private const int GWL_STYLE = (-16);
		private const int WS_HSCROLL = 0x100000;
		private const int WS_VSCROLL = 0x200000;
		private const int SB_HORZ = 0x0;
		private const int SB_VERT = 0x1;
		private static Image skinHeadImage;
		private readonly Bitmap _dragBitmap = new Bitmap(32, 32);
		private readonly Timer dragTimer = new Timer();
		private readonly Timer t = new Timer();
		private bool _canTryDragDrop;
		private TreeNode _dragNode;
		private TreeNode _hoverNode;
		private Point _hoverPoint;
		private int _newMaximum;
		private int _numVisible;
		private DragDropEffects _oldEffects = 0;
		private int _oldScrollValue;
		private TreeNode _overNode;
		private int _scrollX;
		private TreeNode dragDropNode;
		private int dragDropOverFolder;
		private TreeNode lastClick;
		private bool lastOpened;
		private bool mouseDown;
		private int mouseDownMargin = 5;
		private Point mouseDownPoint;
		private bool negativeTimer;
		private int prevValue;
		public int scrollMargin = 20;

		public SkinTreeView()
		{
			t.SynchronizingObject = this;
			t.Interval = 200;
			t.Elapsed += t_Elapsed;
			dragTimer.SynchronizingObject = this;
			dragTimer.Interval = 1000;
			dragTimer.Elapsed += dragTimer_Elapsed;
			SetStyle(
				ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
				ControlStyles.UserMouse, true);
			DoubleBuffered = true;
			skinHeadImage = new Bitmap(32, 32);

			DrawMode = TreeViewDrawMode.OwnerDrawAll;
			ItemHeight = 23;
			FullRowSelect = true;
			HotTracking = true;
			TreeViewNodeSorter = new SkinNodeSorter();
			AllowDrop = true;

			long style = GetWindowLong(Handle, GWL_STYLE);
			style |= 0x8000;

			SetWindowLong(Handle, GWL_STYLE, style);
		}

		public Point ScrollPosition
		{
			get
			{
				return new Point(
					_scrollX,
					GetScrollPos((int) Handle, SB_VERT));
			}

			set
			{
				_scrollX = value.X; // SetScrollPos((IntPtr)Handle, SB_HORZ, value.X, true);
				SetScrollPos(Handle, SB_VERT, value.Y, true);

				Invalidate();
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetScrollPos(int hWnd, int nBar);

		[DllImport("user32.dll")]
		private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

		[DllImport("user32.dll", ExactSpelling = false, CharSet = CharSet.Auto)]
		private static extern long GetWindowLong(IntPtr hwnd, int nIndex);

		[DllImport("user32.dll", ExactSpelling = false, CharSet = CharSet.Auto)]
		private static extern void SetWindowLong(IntPtr hwnd, int nIndex, long value);

		public void t_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (negativeTimer)
			{
				BeginUpdate();
				ScrollPosition = new Point(ScrollPosition.X, ScrollPosition.Y - 1);
				if (!(prevValue == ScrollPosition.Y))
					prevValue = ScrollPosition.Y;
				else
				{
					t.Stop();
					negativeTimer = false;
					prevValue = 0;
				}
				EndUpdate();
			}
			else
			{
				BeginUpdate();
				ScrollPosition = new Point(ScrollPosition.X, ScrollPosition.Y + 1);
				if (!(prevValue == ScrollPosition.Y))
					prevValue = ScrollPosition.Y;
				else
				{
					t.Stop();
					negativeTimer = false;
					prevValue = 0;
				}
				EndUpdate();
			}
		}

		public void dragTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_dragNode == null)
			{
				dragTimer.Stop();
				return;
			}

			dragDropOverFolder++;
			if (dragDropOverFolder == 1)
			{
				Point cp = PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
				TreeNode dragToItem = GetSelectedNodeAt(new Point(cp.X, cp.Y));
				if (!(dragToItem is Skin) && dragToItem != null)
				{
					if (dragToItem.Nodes.Count > 0)
						dragToItem.Expand();
				}

				dragTimer.Stop();
			}
		}

		private Point pointDifference(Point p1, Point p2)
		{
			int x = p1.X - p2.X;
			if (x < 0)
				x *= -1;

			int y = p1.Y - p2.Y;
			if (y < 0)
				y *= -1;

			return new Point(x, y);
		}

		private bool verticalScrollBarVisible()
		{
			long wndStyle = wndStyle = GetWindowLong(Handle, GWL_STYLE);
			return ((wndStyle & WS_VSCROLL) != 0);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			_numVisible = (int) Math.Ceiling(Height / (float) ItemHeight);
			base.OnSizeChanged(e);
		}

		private TreeNode GetSelectedNodeAt(int y, TreeNode node, ref int currentIndex)
		{
			if (currentIndex >= ScrollPosition.Y + _numVisible)
				return null;

			if (y <= node.Bounds.Y + ItemHeight)
				return node;

			currentIndex++;

			if (node.IsExpanded)
			{
				foreach (TreeNode child in node.Nodes)
				{
					TreeNode tryNode = GetSelectedNodeAt(y, child, ref currentIndex);

					if (tryNode != null)
						return tryNode;
				}
			}

			return null;
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			if (SelectedNode == lastClick && lastClick.IsExpanded == lastOpened)
				lastClick.Toggle();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_canTryDragDrop = (e.Button == MouseButtons.Left);
			mouseDownPoint = e.Location;
			mouseDown = true;
			base.OnMouseDown(e);
			TreeNode node = GetSelectedNodeAt(e.Location);
			SelectedNode = node;
			lastClick = SelectedNode;
			lastOpened = lastClick != null && lastClick.IsExpanded;

			if (verticalScrollBarVisible())
			{
				if (e.Y <= scrollMargin)
				{
					negativeTimer = true;
					t.Start();
				}
				else if (e.Y >= (Height - scrollMargin))
				{
					negativeTimer = false;
					t.Start();
				}
				else
				{
					t.Stop();
					negativeTimer = false;
					prevValue = 0;
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			_canTryDragDrop = false;
			dragDropOverFolder = 0;
			dragTimer.Stop();
			t.Stop();
			_dragNode = null;
			mouseDown = false;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_hoverPoint = e.Location;
			TreeNode hover = GetSelectedNodeAt(e.Location);
			if (_hoverNode == null || _hoverNode != hover)
			{
				_hoverNode = hover;
				Invalidate();
			}
			base.OnMouseMove(e);

			if ((MouseButtons & MouseButtons.Left) == 0 ||
			    SelectedNode == null ||
			    !_canTryDragDrop)
				return;

			if (mouseDown)
			{
				if (verticalScrollBarVisible())
				{
					if (e.Y <= scrollMargin)
					{
						negativeTimer = true;
						t.Start();
					}
					else if (e.Y >= (Height - scrollMargin))
					{
						negativeTimer = false;
						t.Start();
					}
					else
					{
						t.Stop();
						negativeTimer = false;
						prevValue = 0;
					}
				}
			}

			Point diff = pointDifference(e.Location, mouseDownPoint);

			if ((diff.X >= mouseDownMargin) || (diff.Y >= mouseDownMargin))
			{
				using (Graphics g = Graphics.FromImage(_dragBitmap))
				{
					g.Clear(Color.Magenta);

					g.InterpolationMode = InterpolationMode.NearestNeighbor;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					g.SmoothingMode = SmoothingMode.None;

					Image prevImage;

					if (SelectedNode is Skin)
						prevImage = (SelectedNode as Skin).Head;
					else
					{
						if (SelectedNode.IsExpanded)
							prevImage = Resources.FolderOpen_32x32_72;
						else
							prevImage = Resources.Folder_32x32;
					}

					g.DrawImage(prevImage, new Rectangle(0, 0, 32, 32), new Rectangle(0, 0, prevImage.Width, prevImage.Height),
					            GraphicsUnit.Pixel);
				}

				/*var kvps = new List<KeyValuePair<string, object>>();

				kvps.Add(new KeyValuePair<string, object>("MCSkin3D.Skin", SelectedNode));

				if (SelectedNode is Skin)
					kvps.Add(new KeyValuePair<string, object>(DataFormats.FileDrop, new string[] { ((Skin)SelectedNode).File.FullName }));
				
				DragSourceHelper.DoDragDrop(Editor.MainForm, _dragBitmap, new Point((_dragBitmap.Width / 2), _dragBitmap.Height), DragDropEffects.Move | DragDropEffects.Copy,
					kvps.ToArray());*/

				_dragNode = SelectedNode;
				var obj = new DataObject();

				obj.SetData("MCSkin3D.Skin", SelectedNode.FullPath);

				if (SelectedNode is Skin)
				{
					var strs = new StringCollection();
					strs.Add(((Skin) SelectedNode).File.FullName);
					obj.SetFileDropList(strs);
				}

				DoDragDrop(obj, DragDropEffects.Move | DragDropEffects.Copy);
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			t.Stop();
			mouseDown = false;
			negativeTimer = false;
			prevValue = 0;
			base.OnMouseLeave(e);
		}

		public void ZoomOut()
		{
			if (ItemHeight > 12)
				ItemHeight--;

			GlobalSettings.TreeViewHeight = ItemHeight;
		}

		public void ZoomIn()
		{
			ItemHeight++;

			GlobalSettings.TreeViewHeight = ItemHeight;
		}

		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			if (e.Bounds.Width == 0 || e.Bounds.Height == 0)
				return;

			int realX = e.Bounds.X + ((e.Node.Level + 1) * 20);

			Size textLen = TextRenderer.MeasureText(e.Node.Text, Font);
			if (realX + textLen.Width + ItemHeight + 15 > Width &&
			    _newMaximum < (realX + textLen.Width + ItemHeight + 15) - Width)
				_newMaximum = (realX + textLen.Width + ItemHeight + 15) - Width;

			realX -= _scrollX;

			e.Graphics.FillRectangle(new SolidBrush(BackColor), 0, e.Bounds.Y, Width, e.Bounds.Height);
			Skin skin = e.Node is Skin ? (Skin) e.Node : null;

			if (e.Node.IsSelected || e.Node == _overNode)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(127, SystemColors.Highlight)), realX, e.Bounds.Y, Width,
				                         e.Bounds.Height - 1);
			}
			else if (skin != null && skin.File.ToString() == GlobalSettings.LastSkin)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(127, Color.Yellow)), realX, e.Bounds.Y, Width,
				                         e.Bounds.Height - 1);
			}

			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			e.Graphics.SmoothingMode = SmoothingMode.None;

			if (skin == null)
			{
				if (e.Node.IsExpanded)
					e.Graphics.DrawImage(Resources.FolderOpen_32x32_72, realX, e.Bounds.Y, ItemHeight - 1, ItemHeight - 1);
				else
					e.Graphics.DrawImage(Resources.Folder_32x32, realX, e.Bounds.Y, ItemHeight - 1, ItemHeight - 1);

				if (e.Node.Level == 0 && !Editor.HasOneRoot)
				{
					e.Graphics.DrawImage(Resources.clone, realX + (ItemHeight - Resources.clone.Width) - 1,
					                     e.Bounds.Y + (ItemHeight - Resources.clone.Height) - 1, Resources.clone.Width,
					                     Resources.clone.Height);
				}
			}
			else
				e.Graphics.DrawImage(skin.Head, realX, e.Bounds.Y, ItemHeight - 1, ItemHeight - 1);

			if (skin == null && e.Node.Nodes.Count != 0)
			{
				if (e.Node.IsExpanded)
				{
					if ((e.State & TreeNodeStates.Hot) != 0)
					{
						e.Graphics.DrawImage(Resources.arrow_state_blue_expanded,
						                     new Rectangle(realX - 13, e.Bounds.Y + (ItemHeight / 2) - (16 / 2), 16, 16));
					}
					else
					{
						e.Graphics.DrawImage(Resources.arrow_state_grey_expanded,
						                     new Rectangle(realX - 13, e.Bounds.Y + (ItemHeight / 2) - (16 / 2), 16, 16));
					}
				}
				else
				{
					if ((e.State & TreeNodeStates.Hot) != 0)
					{
						e.Graphics.DrawImage(Resources.arrow_state_blue_right,
						                     new Rectangle(realX - 13, e.Bounds.Y + (ItemHeight / 2) - (16 / 2), 16, 16));
					}
					else
					{
						e.Graphics.DrawImage(Resources.arrow_state_grey_right,
						                     new Rectangle(realX - 13, e.Bounds.Y + (ItemHeight / 2) - (16 / 2), 16, 16));
					}
				}
			}

			string text = e.Node.ToString();

			TextRenderer.DrawText(e.Graphics, text, Font,
			                      new Rectangle(realX + ItemHeight + 1, e.Bounds.Y, Width, e.Bounds.Height),
			                      (e.Node.IsSelected || e.Node == _overNode) ? Color.White : Color.Black,
			                      TextFormatFlags.VerticalCenter);

			//TextRenderer.DrawText(e.Graphics, "64x32", new Font(Font.FontFamily, 7), new Rectangle(e.Bounds.X, e.Bounds.Y, Width - 20, e.Bounds.Height), (e.Node.IsSelected || e.Node == _overNode) ? Color.White : Color.Black, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
		}

		public TreeNode GetSelectedNodeAt(Point p)
		{
			int currentIndex = 0;

			TreeNode node = null;
			foreach (TreeNode child in Nodes)
			{
				node = GetSelectedNodeAt(p.Y, child, ref currentIndex);

				if (node != null)
					break;
			}

			return node;
		}

		private void RecursiveDrawCheck(PaintEventArgs args, TreeNode node, ref int currentIndex)
		{
			TreeNodeStates state = 0;

			if (_hoverNode == node)
				state |= TreeNodeStates.Hot;
			else if (node == SelectedNode)
				state |= TreeNodeStates.Selected;

			OnDrawNode(new DrawTreeNodeEventArgs(args.Graphics, node, new Rectangle(0, node.Bounds.Y, Width, ItemHeight), state));
			currentIndex++;

			if (node.IsExpanded)
			{
				foreach (TreeNode child in node.Nodes)
					RecursiveDrawCheck(args, child, ref currentIndex);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (!DesignMode)
			{
				_oldScrollValue = Editor.MainForm.hScrollBar1.Value;
				_newMaximum = 0;
			}

			int currentIndex = 0;
			foreach (TreeNode n in Nodes)
				RecursiveDrawCheck(e, n, ref currentIndex);

			if (!DesignMode)
			{
				Editor.MainForm.hScrollBar1.Maximum = _newMaximum;
				Editor.MainForm.hScrollBar1.Value = _oldScrollValue;

				Editor.MainForm.hScrollBar1.Visible = Editor.MainForm.hScrollBar1.Maximum != 0;
			}
		}

		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			//if (e.Button == System.Windows.Forms.MouseButtons.Left &&
			//	e.Item is object[])
			//	DoDragDrop(((object[])e.Item)[0], DragDropEffects.Move);
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			if (e.Data.GetDataPresent("MCSkin3D.Skin") && _dragNode != null)
			{
				TreeNode node = _dragNode;
				TreeNode selectedNode = GetSelectedNodeAt(PointToClient(Cursor.Position));

				if (!DropValid(node, selectedNode))
					e.Effect = DragDropEffects.None;
				else if ((ModifierKeys & Keys.Control) != 0)
					e.Effect = DragDropEffects.Copy;
				else
					e.Effect = DragDropEffects.Move;

				SetDragEnter(e.Effect, new Point(e.X, e.Y), e.Data);
			}
			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = e.AllowedEffect & DragDropEffects.Copy;
				//DropTargetHelper.DragEnter(this, e.Data, new Point(e.X, e.Y), e.Effect, Editor.GetLanguageString("C_IMPORTTO") + " %1", "MCSkin3D");
			}
		}

		private void SetDragEnter(DragDropEffects effect, Point p, IDataObject data)
		{
			if (_oldEffects == effect && _overNode == GetSelectedNodeAt(PointToClient(Cursor.Position)))
				return;

			TreeNode node = GetSelectedNodeAt(PointToClient(Cursor.Position));
			/*string nodeName;

			if (node == null)
				nodeName = Editor.HasOneRoot ? Editor.RootFolderString : "root";
			else
				nodeName = (node is Skin) ? ((node.Parent != null) ? node.Parent.Text : Editor.RootFolderString) : node.Text;

			if (effect == DragDropEffects.None)
				DropTargetHelper.DragEnter(this, data, p, effect, Editor.GetLanguageString("C_CANTMOVE") + " %1", nodeName);
			else if (effect == DragDropEffects.Copy)
				DropTargetHelper.DragEnter(this, data, p, effect, Editor.GetLanguageString("C_COPYTO") + " %1", nodeName);
			else if (effect == DragDropEffects.Move)
				DropTargetHelper.DragEnter(this, data, p, effect, Editor.GetLanguageString("C_MOVETO") + " %1", nodeName);*/

			_oldEffects = effect;
			_overNode = node;
			Invalidate();
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			if (e.Data.GetDataPresent("MCSkin3D.Skin") && _dragNode != null)
			{
				Point cp = PointToClient(new Point(e.X, e.Y));
				TreeNode dragToItem = GetSelectedNodeAt(new Point(cp.X, cp.Y));
				if (dragDropNode != dragToItem)
				{
					dragDropNode = dragToItem;
					dragDropOverFolder = 0;
				}
				dragTimer.Start();
				TreeNode node = _dragNode;
				TreeNode selectedNode = GetSelectedNodeAt(PointToClient(Cursor.Position));

				if (!DropValid(node, selectedNode))
					e.Effect = DragDropEffects.None;
				else if ((ModifierKeys & Keys.Control) != 0)
					e.Effect = DragDropEffects.Copy;
				else
					e.Effect = DragDropEffects.Move;
			}
			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = e.AllowedEffect & DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;

			SetDragEnter(e.Effect, new Point(e.X, e.Y), e.Data);
			//DropTargetHelper.DragOver(new Point(e.X, e.Y), e.Effect);
		}

		private bool DropValid(TreeNode node, TreeNode selectedNode)
		{
			if (selectedNode == null && !Editor.HasOneRoot)
				return false;
			else if (node is Skin && selectedNode is Skin)
			{
				if (node.GetParentCollection() == selectedNode.GetParentCollection())
					return false;
			}
			else if (node is Skin && selectedNode is FolderNode)
			{
				if (node.Parent == selectedNode)
					return false;
			}
			else if (node is FolderNode && selectedNode is Skin)
			{
				if (selectedNode.GetNodeChain().Contains(node))
					return false;
			}
			else if (node is FolderNode && selectedNode is FolderNode)
			{
				if (selectedNode.GetNodeChain().Contains(node))
					return false;
			}
			else if ((node is Skin || node is FolderNode) && selectedNode == null)
			{
				if (node.Parent == null)
					return false;
			}

			return true;
		}

		protected override void OnDragLeave(EventArgs e)
		{
			//DropTargetHelper.DragLeave(this);
			dragDropOverFolder = 0;
			dragTimer.Stop();
			//_dragNode = null;
			_overNode = null;
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			dragDropOverFolder = 0;
			dragTimer.Stop();

			if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
			    !e.Data.GetDataPresent("MCSkin3D.Skin"))
			{
				e.Effect = e.AllowedEffect & DragDropEffects.Copy;

				var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
				string folderLocation = Editor.GetFolderLocationForNode(_overNode);

				if (string.IsNullOrEmpty(folderLocation))
					return;

				foreach (string f in files)
				{
					string name = Path.GetFileNameWithoutExtension(f);

					while (File.Exists(folderLocation + name + ".png"))
						name += " (" + Editor.GetLanguageString("C_NEW") + ")";

					File.Copy(f, folderLocation + name + ".png");

					var skin = new Skin(folderLocation + name + ".png");

					if (_overNode != null)
					{
						if (!(_overNode is Skin))
							_overNode.Nodes.Add(skin);
						else
						{
							if (_overNode.Parent == null)
								_overNode.TreeView.Nodes.Add(skin);
							else
								_overNode.Parent.Nodes.Add(skin);
						}
					}
					else
						Nodes.Add(skin);

					skin.SetImages();
					SelectedNode = skin;
				}
			}
			else
			{
				if (!DropValid(SelectedNode, _overNode))
					e.Effect = DragDropEffects.None;
				else
				{
					TreeNode selNode = SelectedNode;
					if (e.Effect == DragDropEffects.Move)
						MoveNode(SelectedNode, _overNode);
					SelectedNode = selNode;
				}
			}

			//DropTargetHelper.Drop(e.Data, new Point(e.X, e.Y), e.Effect);

			_dragNode = null;
			_overNode = null;
		}

		private void MoveNode(TreeNode from, TreeNodeCollection to, TreeNode toNode)
		{
			string path = Editor.GetFolderForNode(toNode);

			if (string.IsNullOrEmpty(path))
				throw new InvalidOperationException();

			if (from is Skin)
			{
				string newPath = Path.GetFileNameWithoutExtension(((Skin) from).File.Name);
				((Skin) from).MoveTo(path + "\\" + newPath + ".png");
			}
			else
				((FolderNode) from).MoveTo(path + "\\" + ((FolderNode) from).Directory.Name);

			from.Remove();
			to.Add(from);
		}

		private void MoveNode(TreeNode from, TreeNode to)
		{
			if (from is Skin && to is Skin)
				MoveNode(from, to.GetParentCollection(), to);
			else if (from is Skin && to is FolderNode)
				MoveNode(from, to.Nodes, to);
			else if (from is FolderNode && to is Skin)
				MoveNode(from, to.GetParentCollection(), to);
			else if (from is FolderNode && to is FolderNode)
				MoveNode(from, to.Nodes, to);
			else if ((from is Skin || from is FolderNode) && to == null)
				MoveNode(from, Nodes, to);
		}

		/// <summary>
		/// A non-recursive function which will return a node based on a path.
		/// </summary>
		/// <param name="path">The full path to the node</param>
		/// <param name="returnClosest">Return the closest node found, if we didn't find the final node</param>
		/// <returns>The node that was found, or null if not found.</returns>
		public TreeNode NodeFromPath(string path, bool returnClosest = false)
		{
			string[] split = path.Split(new[] {PathSeparator}, StringSplitOptions.RemoveEmptyEntries);
			int splitIndex = 0;
			TreeNode closestNode = null;

			while (true)
			{
				TreeNode[] nodes = ((closestNode == null ? Nodes : closestNode.Nodes)).Find(split[splitIndex], false);

				if (nodes.Length == 0)
					return returnClosest ? closestNode : null;

				closestNode = nodes[0];
				splitIndex++;

				if (splitIndex == split.Length)
					return closestNode;
			}
		}
	}
}