using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D
{
	public partial class ShortcutEditor : Form
	{
		public ShortcutEditor()
		{
			InitializeComponent();
		}

		List<IShortcutImplementor> _shortcuts = new List<IShortcutImplementor>();

		[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		public IList<IShortcutImplementor> Shortcuts
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
				textBox1.Clear();
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
					SelectedShortcut.Keys = key | ModifierKeys;
					textBox1.Text = KeysToString(key | ModifierKeys);
					listBox1.Invalidate();
				}
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void ShortcutEditor_Load(object sender, EventArgs e)
		{

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

	class KeyReceivedEventArgs : EventArgs
	{
		public Keys Key
		{
			get;
			set;
		}

		public KeyReceivedEventArgs(Keys key)
		{
			Key = key;
		}
	}

	class TextBoxTest : TextBox
	{
	}

	public interface IShortcutImplementor
	{
		string Name { get; }
		Keys Keys { get; set; }
		Action Pressed { get; set; }
	}

	public class ShortcutBase : IShortcutImplementor
	{
		string _name;
		public string Name
		{
			get { return _name; }
		}

		public Keys Keys { get; set; }
		public Action Pressed { get; set; }

		public ShortcutBase(string name, Keys keys)
		{
			_name = name;
			Keys = keys;
		}

		public override string ToString()
		{
			return _name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
		}
	}
}
