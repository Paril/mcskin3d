using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCSkin3D;

namespace Paril.Components.Shortcuts
{
	public partial class ShortcutEditor : Form
	{
		public ShortcutEditor()
		{
			InitializeComponent();
		}

		List<IShortcutImplementor> _shortcuts = new List<IShortcutImplementor>();

		IShortcutImplementor ShortcutInUse(Keys key)
		{
			if (key == 0)
				return null;

			foreach (var s in _shortcuts)
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
			Program.MainForm.languageProvider1.SetPropertyNames(shortcut, "Name");
		}

		public int ShortcutCount
		{
			get { return _shortcuts.Count; }
		}

		public IShortcutImplementor ShortcutAt(int index)
		{
			return _shortcuts[index];
		}

		public IEnumerable<IShortcutImplementor> Shortcuts
		{
			get { return _shortcuts; }
		}

		protected override void OnLoad(EventArgs e)
		{
			KeyPreview = true;
			base.OnLoad(e);

			listBox1.Items.Clear();

			foreach (var cut in Shortcuts)
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

		IShortcutImplementor SelectedShortcut
		{
			get { return (IShortcutImplementor)listBox1.SelectedItem; }
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
					var oldCut = SelectedShortcut.Keys;
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

			IShortcutImplementor shortcut = (IShortcutImplementor)listBox1.Items[e.Index];

			TextRenderer.DrawText(e.Graphics, shortcut.ToString(), Font, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}
	}

	public class ShortcutExistsEventArgs : EventArgs
	{
		public string ShortcutName { get; set; }
		public string OtherName { get; set; }

		public ShortcutExistsEventArgs(string shortcut, string other)
		{
			ShortcutName = shortcut;
			OtherName = other;
		}
	}
}
