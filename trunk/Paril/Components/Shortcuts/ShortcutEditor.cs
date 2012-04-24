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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MCSkin3D;

namespace Paril.Components.Shortcuts
{
	public partial class ShortcutEditor : Form
	{
		private readonly List<IShortcutImplementor> _shortcuts = new List<IShortcutImplementor>();

		public ShortcutEditor()
		{
			InitializeComponent();
		}

		public int ShortcutCount
		{
			get { return _shortcuts.Count; }
		}

		public IEnumerable<IShortcutImplementor> Shortcuts
		{
			get { return _shortcuts; }
		}

		private IShortcutImplementor SelectedShortcut
		{
			get { return (IShortcutImplementor) listBox1.SelectedItem; }
		}

		private IShortcutImplementor ShortcutInUse(Keys key)
		{
			if (key == 0)
				return null;

			foreach (IShortcutImplementor s in _shortcuts)
			{
				if (s.Keys != 0 && s.Keys == key)
					return s;
			}

			return null;
		}

		public event EventHandler<ShortcutExistsEventArgs> ShortcutExists;

		public void AddShortcut(IShortcutImplementor shortcut)
		{
			IShortcutImplementor already;
			if ((already = ShortcutInUse(shortcut.Keys)) != null)
			{
				if (ShortcutExists != null)
					ShortcutExists(this, new ShortcutExistsEventArgs(shortcut.Name, already.Name));
				shortcut.Keys = 0;
			}

			_shortcuts.Add(shortcut);
			Editor.MainForm.languageProvider1.SetPropertyNames(shortcut, "Name");
		}

		public IShortcutImplementor ShortcutAt(int index)
		{
			return _shortcuts[index];
		}

		protected override void OnLoad(EventArgs e)
		{
			KeyPreview = true;
			base.OnLoad(e);

			listBox1.Items.Clear();

			foreach (IShortcutImplementor cut in Shortcuts)
				listBox1.Items.Add(cut);

			if (listBox1.Items.Count > 0)
				listBox1.SelectedIndex = 0;

			labelControl1.Visible = false;
			pictureBox1.Visible = false;
		}

		private static string KeyToString(Keys keys)
		{
			switch (keys)
			{
				case Keys.Add:
					return "+";
				case Keys.Back:
					return "Backspace";
				case Keys.D0:
					return "0";
				case Keys.D1:
					return "1";
				case Keys.D2:
					return "2";
				case Keys.D3:
					return "3";
				case Keys.D4:
					return "4";
				case Keys.D5:
					return "5";
				case Keys.D6:
					return "6";
				case Keys.D7:
					return "7";
				case Keys.D8:
					return "8";
				case Keys.D9:
					return "9";
				case Keys.Divide:
					return "/";
				case Keys.Multiply:
					return "*";
				case Keys.Subtract:
					return "-";
				case Keys.Oem5:
					return "\\";
				case Keys.OemQuestion:
					return "/";
				case Keys.Oemcomma:
					return ",";
				case Keys.OemPeriod:
					return ".";
				case Keys.Oem1:
					return ";";
				case Keys.Oem7:
					return "\'";
				case Keys.OemOpenBrackets:
					return "[";
				case Keys.Oem6:
					return "]";
				case Keys.OemMinus:
					return "-";
				case Keys.Oemplus:
					return "=";
				case Keys.Oemtilde:
					return "`";
			}

			return keys.ToString();
		}

		public static string KeysToString(Keys k)
		{
			string s = "";

			if ((k & Keys.Control) != 0)
				s += "Ctrl+";
			if ((k & Keys.Shift) != 0)
				s += "Shift+";
			if ((k & Keys.Alt) != 0)
				s += "Alt+";

			s += KeyToString(k &= ~Keys.Modifiers);
			return s;
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null)
			{
				textBox1.ResetText();
				return;
			}

			textBox1.Text = KeysToString(SelectedShortcut.Keys);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			Keys key = keyData & ~Keys.Modifiers;

			if (key != 0)
			{
				if (key != Keys.ControlKey &&
				    key != Keys.ShiftKey &&
				    key != Keys.Menu)
				{
					IShortcutImplementor already;
					Keys oldCut = SelectedShortcut.Keys;
					SelectedShortcut.Keys = 0;

					if ((already = ShortcutInUse(key | ModifierKeys)) != null)
					{
						textBox1.Text = KeysToString(key | ModifierKeys);
						labelControl1.TextAlign = ContentAlignment.MiddleLeft;
						labelControl1.Text = Editor.GetLanguageString("C_SHORTCUTINUSE") + " \"" + already.Name + "\"";
						labelControl1.Visible = pictureBox1.Visible = true;
						SelectedShortcut.Keys = oldCut;
					}
					else
					{
						labelControl1.Visible = pictureBox1.Visible = false;
						SelectedShortcut.Keys = key | ModifierKeys;
						textBox1.Text = KeysToString(key | ModifierKeys);
						listBox1.Invalidate();
					}
				}
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void ShortcutEditor_Load(object sender, EventArgs e)
		{
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			label1.Text = Editor.GetLanguageString("M_SHORTCUTS");
		}

		private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			e.DrawFocusRectangle();

			if (e.Index == -1)
				return;

			var shortcut = (IShortcutImplementor) listBox1.Items[e.Index];

			TextRenderer.DrawText(e.Graphics, shortcut.ToString(), DefaultFont, e.Bounds, e.ForeColor,
			                      TextFormatFlags.VerticalCenter);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 17;
		}
	}

	public class ShortcutExistsEventArgs : EventArgs
	{
		public ShortcutExistsEventArgs(string shortcut, string other)
		{
			ShortcutName = shortcut;
			OtherName = other;
		}

		public string ShortcutName { get; set; }
		public string OtherName { get; set; }
	}
}