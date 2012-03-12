using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MCSkin3D.Forms
{
	public partial class GUIDPicker : Form
	{
		class GUIDIndex
		{
			public string Name { get; set; }
			public DateTime Date { get; set; }
			public Guid Guid { get; set; }

			public override string ToString()
			{
				return Name + " @ " + Date.ToString() + " " + Guid.ToString();
			}
		}

		public GUIDPicker()
		{
			InitializeComponent();
		}

		string _fileName;
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; LoadValues(); }
		}

		void LoadValues()
		{
			if (File.Exists(_fileName))
			using (StreamReader sr = new StreamReader(_fileName, Encoding.Unicode))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine();
					var split = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

					GUIDIndex index = new GUIDIndex();
					index.Name = split[0];
					index.Date = DateTime.Parse(split[1]);
					index.Guid = new Guid(split[2]);

					comboBox1.Items.Add(index);
				}
			}
		}

		public GUIDPicker(string fileName) :
			this()
		{
			FileName = fileName;
		}

		private void GUIDPicker_Load(object sender, EventArgs e)
		{

		}

		Guid GenGuid()
		{
			while (true)
			{
				bool matched = false;
				Guid guid = Guid.NewGuid();

				foreach (GUIDIndex x in comboBox1.Items)
				{
					if (x.Guid.CompareTo(guid) == 0)
					{
						matched = true;
						break;
					}
				}

				if (!matched)
					return guid;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			GUIDIndex index = new GUIDIndex();
			index.Name = textBox1.Text;
			index.Date = DateTime.Now;
			index.Guid = GenGuid();

			comboBox1.Items.Add(index);
			comboBox1.SelectedItem = index;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			GUIDIndex index = (GUIDIndex)comboBox1.SelectedItem;

			textBox1.Text = index.Name;
			textBox2.Text = index.Date.ToString();
			textBox3.Text = index.Guid.ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			using (StreamWriter sr = new StreamWriter(_fileName, false, Encoding.Unicode))
			{
				foreach (GUIDIndex x in comboBox1.Items)
					sr.WriteLine(x.Name + "|" + x.Date.ToString() + "|" + x.Guid.ToString());
			}

			Close();
		}
	}
}
