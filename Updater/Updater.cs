using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using BrightIdeasSoftware;
using System.Xml;
using System.IO;
using System.Threading;
using System.Net;

namespace MCSkin3D.UpdateSystem
{
	public partial class Updater : Form
	{
		public string UpdateXMLURL { get; set; }

		public Updater()
		{
			InitializeComponent();
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		class UpdateItem
		{
			public string Name { get; set; }
			public string Information { get; set; }
			public string Size { get; set; }
			public string Date { get; set; }
			public string Group { get; set; }
			public bool IsChecked { get; set; }
			public string DownloadURL { get; set; }
			public string GroupImageURL { get; set; }

			public int ImageIndex { get; set; }
			public Image Image { get { return _updateImages.Images[ImageIndex]; } }

			public UpdateItem() { }
		}

		// FIXME: RSS instead?
		static List<UpdateItem> LoadUpdates(XmlDocument document)
		{
			List<UpdateItem> items = new List<UpdateItem>();

			if (document.DocumentElement.Name.ToLower() == "updates")
			{
				foreach (XmlNode x in document.DocumentElement.ChildNodes)
				{
					if (x.Name.ToLower() != "update")
						continue;

					UpdateItem item = new UpdateItem();

					foreach (XmlNode subNode in x.ChildNodes)
					{
						if (subNode.Name.ToLower() == "name")
							item.Name = subNode.InnerText;
						else if (subNode.Name.ToLower() == "info")
							item.Information = subNode.InnerText;
						else if (subNode.Name.ToLower() == "size")
							item.Size = subNode.InnerText;
						else if (subNode.Name.ToLower() == "date")
							item.Date = subNode.InnerText;
						else if (subNode.Name.ToLower() == "group")
							item.Group = subNode.InnerText;
						else if (subNode.Name.ToLower() == "ismajor")
							item.IsChecked = bool.Parse(subNode.InnerText);
						else if (subNode.Name.ToLower() == "downloadurl")
							item.DownloadURL = subNode.InnerText;
						else if (subNode.Name.ToLower() == "groupimageurl")
							item.GroupImageURL = subNode.InnerText;
					}

					items.Add(item);
				}
			}

			return items;
		}

		List<UpdateItem> _updates = null;
		Thread _updateThread = null;
		static ImageList _updateImages = new ImageList();
		static readonly string _tempDirectory = Environment.ExpandEnvironmentVariables("%temp%");
		static readonly string _mcskin3dTemp = _tempDirectory + '\\' + "mcskin3d";

		void GetUpdateData()
		{
			WebClient client = new WebClient();
			XmlDocument doc = new XmlDocument();

			_updateImages.ColorDepth = ColorDepth.Depth32Bit;

			using (MemoryStream ms = new MemoryStream())
			{
				var data = client.DownloadData(UpdateXMLURL);
				ms.Write(data, 0, data.Length);

				ms.Position = 0;

				using (var sr = new StreamReader(ms, Encoding.Unicode))
					doc.LoadXml(sr.ReadToEnd());
			}

			_updates = LoadUpdates(doc);

			List<string> fileNames = new List<string>();

			foreach (var u in _updates)
			{
				string url = u.GroupImageURL;
				string fileName = Path.GetFileName(url);

				if (!Directory.Exists(_mcskin3dTemp))
					Directory.CreateDirectory(_mcskin3dTemp);

				var fileDir = _mcskin3dTemp + '\\' + fileName;

				if (!File.Exists(fileDir))
					client.DownloadFile(url, fileDir);

				if (!fileNames.Contains(fileDir))
				{
					fileNames.Add(fileDir);
					_updateImages.Images.Add(Image.FromFile(fileDir));
				}

				u.ImageIndex = fileNames.IndexOf(fileDir);
			}

			Invoke((Action)delegate() { UpdateFinished(); });
		}

		void UpdateFinished()
		{
			listView1.SmallImageList = listView1.LargeImageList = _updateImages;

			listView1.BeginUpdate();
			listView1.SetObjects(_updates);
			listView1.Columns[1].Width = listView1.Width - (listView1.Columns[0].Width + listView1.Columns[2].Width + listView1.Columns[3].Width) - 4;
			listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			listView1.EndUpdate();

			listView1.Enabled = true;
			button1.Enabled = true;
			panel2.Visible = false;
		}

		private void Updater_Load(object sender, EventArgs e)
		{
			Thread thread = new Thread(GetUpdateData);
			thread.Start();

			listView1.BeginUpdate();
			listView1.View = View.Details;
			listView1.CheckBoxes = true;
			listView1.GridLines = true;
			listView1.AllowColumnReorder = false;
			listView1.ShowItemCountOnGroups = true;

			foreach (var c in listView1.Columns)
			{
				OLVColumn col = (OLVColumn)c;

				col.GroupKeyGetter = delegate(object row)
				{
					return ((UpdateItem)row).Group;
				};

				col.GroupKeyToTitleConverter = delegate(object key)
				{
					return (string)key;
				};
				col.ImageGetter = delegate(object key)
				{
					return ((UpdateItem)key).Image;
				};
			}

			listView1.CheckedAspectName = "IsChecked";
			listView1.EndUpdate();
		}
	}
}
