using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using BrightIdeasSoftware;
using Version = Paril.Components.Update.Version;

namespace MCSkin3D.UpdateSystem
{
	public partial class Updater : Form
	{
		private static readonly ImageList _updateImages = new ImageList();
		private static readonly string _tempDirectory = Environment.ExpandEnvironmentVariables("%temp%");
		private static readonly string _mcskin3dTemp = _tempDirectory + '\\' + "mcskin3d";
		private static readonly object lockObj = new object();
		private Thread _updateThread;
		private List<UpdateItem> _updates;

		public Updater()
		{
			InitializeComponent();
		}

		public Updater(string url, string installedUpdatesFiles) :
			this()
		{
			UpdateXMLURL = url;

			InstalledUpdates = new List<Guid>();
			if (installedUpdatesFiles != null && File.Exists(installedUpdatesFiles))
			{
				using (var sr = new StreamReader(installedUpdatesFiles))
					while (!sr.EndOfStream)
						InstalledUpdates.Add(new Guid(sr.ReadLine()));
			}
			else
			{
				var file = new FileInfo(installedUpdatesFiles);
				file.Create().Dispose();
				file.Attributes |= FileAttributes.Hidden;
			}
		}

		public string UpdateXMLURL { get; set; }

		public List<Guid> InstalledUpdates { get; set; }

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		// FIXME: RSS instead?
		private static List<UpdateItem> LoadUpdates(XmlDocument document)
		{
			var items = new List<UpdateItem>();

			if (document.DocumentElement.Name.ToLower() == "updates")
			{
				foreach (XmlNode x in document.DocumentElement.ChildNodes)
				{
					if (x.Name.ToLower() != "update")
						continue;

					var item = new UpdateItem();

					foreach (XmlNode subNode in x.ChildNodes)
					{
						var subName = subNode.Name.ToLower();

						if (subName == "name")
							item.Name = subNode.InnerText;
						else if (subName == "info")
							item.Information = subNode.InnerText;
						else if (subName == "size")
							item.Size = subNode.InnerText;
						else if (subName == "date")
						{
							item.Date = subNode.InnerText;

							string[] data = item.Date.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
							int month = int.Parse(data[0]);
							int day = int.Parse(data[1]);
							int year = int.Parse(data[2]);

							item.RealDate = new DateTime(year, month, day);
						}
						else if (subName == "group")
							item.Group = subNode.InnerText;
						else if (subName == "ismajor")
							item.IsMajor = item.IsChecked = bool.Parse(subNode.InnerText);
						else if (subName == "isprogramupdate")
							item.IsProgramUpdate = bool.Parse(subNode.InnerText);
						else if (subName == "downloadurl")
							item.DownloadURL = subNode.InnerText;
						else if (subName == "groupimageurl")
							item.GroupImageURL = subNode.InnerText;
						else if (subName == "guid")
							item.Guid = new Guid(subNode.InnerText);
						else if (subName == "version")
							item.Version = Version.Parse(subNode.InnerText);
						else if (subName == "minversion")
						{
							item.MinVersion = Version.Parse(subNode.InnerText);
							item.HasMinVersion = true;
						}
						else if (subName == "maxversion")
						{
							item.MaxVersion = Version.Parse(subNode.InnerText);
							item.HasMaxVersion = true;
						}
					}

					items.Add(item);
				}
			}

			return items;
		}

		private void GetUpdateData()
		{
			var client = new WebClient();
			var doc = new XmlDocument();

			_updateImages.ColorDepth = ColorDepth.Depth32Bit;

			using (var ms = new MemoryStream())
			{
				byte[] data = client.DownloadData(UpdateXMLURL);
				ms.Write(data, 0, data.Length);

				ms.Position = 0;

				using (var sr = new StreamReader(ms, Encoding.Unicode))
					doc.LoadXml(sr.ReadToEnd());
			}

			_updates = LoadUpdates(doc);
			_updates.RemoveAll(
				item =>
				{
					if (InstalledUpdates.Contains(item.Guid))
						return true;

					// this update isn't compatible with our version
					if (item.HasMinVersion && Program.Version < item.MinVersion)
						return true;
					if (item.HasMaxVersion && Program.Version > item.MaxVersion)
						return true;

					return false;
				}
			);

			_updates.Sort();

			var fileNames = new List<string>();

			foreach (UpdateItem u in _updates)
			{
				string url = u.GroupImageURL;
				string fileName = Path.GetFileName(url);

				if (!Directory.Exists(_mcskin3dTemp))
					Directory.CreateDirectory(_mcskin3dTemp);

				string fileDir = _mcskin3dTemp + '\\' + fileName;

				if (!File.Exists(fileDir))
					client.DownloadFile(url, fileDir);

				if (!fileNames.Contains(fileDir))
				{
					fileNames.Add(fileDir);
					_updateImages.Images.Add(Image.FromFile(fileDir));
				}

				u.ImageIndex = fileNames.IndexOf(fileDir);
			}

			Invoke((Action) UpdateFinished);
		}

		private void UpdateFinished()
		{
			objectListView1.SmallImageList = objectListView1.LargeImageList = _updateImages;

			objectListView1.BeginUpdate();
			objectListView1.SetObjects(_updates);
			objectListView1.Columns[1].Width = objectListView1.Width -
			                                   (objectListView1.Columns[0].Width + objectListView1.Columns[2].Width +
			                                    objectListView1.Columns[3].Width) - 4;
			objectListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			objectListView1.EndUpdate();
			objectListView1.Sort(3);

			objectListView1.Enabled = true;
			button1.Enabled = true;
			panel2.Visible = false;
		}

		private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			Invoke((Action)delegate
			{
				((UpdateItem)e.UserState).Progress = 100;
				objectListView1.RedrawItems(0, _updates.Count - 1, false);
			});

			lock (lockObj)
				Monitor.Pulse(lockObj);
		}

		private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			Invoke((Action) delegate
			                {
			                	((UpdateItem) e.UserState).Progress = e.ProgressPercentage;
			                	objectListView1.RedrawItems(0, _updates.Count - 1, false);
			                });
		}

		private void DownloadUpdateData()
		{
			var client = new WebClient();

			client.DownloadProgressChanged += client_DownloadProgressChanged;
			client.DownloadFileCompleted += client_DownloadFileCompleted;

			foreach (UpdateItem u in _updates)
			{
				if (!u.IsChecked)
					continue;

				string url = u.DownloadURL;

				string fileName = Path.GetFileName(url);

				if (!Directory.Exists(_mcskin3dTemp))
					Directory.CreateDirectory(_mcskin3dTemp);

				string fileDir = _mcskin3dTemp + '\\' + fileName;

				client.DownloadFileAsync(new Uri(url), fileDir, u);

				lock (lockObj)
					Monitor.Wait(lockObj);
			}

			Invoke((Action) delegate
			                {
								MessageBox.Show(Editor.GetLanguageString("M_UPDATESTARTING"));

			                	DialogResult = DialogResult.OK;
			                	Close();
			                });
		}

		private void button1_Click(object sender, EventArgs e)
		{
			objectListView1.BeginUpdate();
			olvColumn2.IsVisible = false;
			olvColumn5.IsVisible = true;
			objectListView1.RebuildColumns();
			objectListView1.EndUpdate();

			_updateThread = new Thread(DownloadUpdateData);
			_updateThread.Start();
		}

		private void Updater_Load(object sender, EventArgs e)
		{
			_updateThread = new Thread(GetUpdateData);
			_updateThread.Start();

			objectListView1.BeginUpdate();
			objectListView1.View = View.Details;
			objectListView1.CheckBoxes = true;
			objectListView1.GridLines = false;
			objectListView1.AllowColumnReorder = false;
			objectListView1.FullRowSelect = true;
			objectListView1.ShowItemCountOnGroups = true;
			objectListView1.OwnerDraw = true;

			olvColumn5.Renderer = new BarRenderer(0, 100);

			foreach (object c in objectListView1.Columns)
			{
				var col = (OLVColumn) c;

				col.GroupKeyGetter = delegate(object row) { return ((UpdateItem) row).Group; };

				col.GroupKeyToTitleConverter = delegate(object key) { return (string) key; };
			}

			objectListView1.AllColumns[0].RendererDelegate = delegate(EventArgs args, Graphics g, Rectangle r, Object rowObject)
			                                                 {
			                                                 	g.FillRectangle(new SolidBrush(objectListView1.BackColor),
			                                                 	                new Rectangle(r.X - 1, r.Y - 1,
			                                                 	                              objectListView1.Width, r.Height + 2));

			                                                 	var realArgs = (DrawListViewSubItemEventArgs) args;

			                                                 	bool isHot = objectListView1.HotRowIndex == realArgs.ItemIndex &&
			                                                 	             objectListView1.PointToClient(Cursor.Position).X <
			                                                 	             r.X + 5 + 17;

			                                                 	CheckBoxRenderer.DrawCheckBox(g, new Point(r.X + 5, r.Y + 2),
			                                                 	                              realArgs.Item.Checked
			                                                 	                              	? (isHot
			                                                 	                              	   	? CheckBoxState.CheckedHot
			                                                 	                              	   	: CheckBoxState.CheckedNormal)
			                                                 	                              	: (isHot
			                                                 	                              	   	? CheckBoxState.UncheckedHot
			                                                 	                              	   	: CheckBoxState.UncheckedNormal));

			                                                 	g.DrawImage(
			                                                 		realArgs.Item.ImageList.Images[((UpdateItem) rowObject).ImageIndex
			                                                 			], new Point(r.X + 24, r.Y));

			                                                 	TextRenderer.DrawText(g, realArgs.Item.Text, Font,
			                                                 	                      new Rectangle(r.X + 24 + 16, r.Y,
			                                                 	                                    r.Width - (24 + 16), r.Height),
			                                                 	                      SystemColors.ControlText,
			                                                 	                      TextFormatFlags.Left |
			                                                 	                      TextFormatFlags.VerticalCenter);

			                                                 	return true;
			                                                 };

			objectListView1.CheckedAspectName = "IsChecked";
			objectListView1.EndUpdate();
		}


		private void objectListView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
		}

		private void objectListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		#region Nested type: UpdateItem

		private class UpdateItem : IComparable<UpdateItem>
		{
			public string Name { get; set; }
			public string Information { get; set; }
			public string Size { get; set; }
			public string Date { get; set; }
			public string Group { get; set; }
			public string DownloadURL { get; set; }
			public string GroupImageURL { get; set; }
			public bool IsMajor { get; set; }
			public bool IsProgramUpdate { get; set; }
			public int Progress { get; set; }
			public DateTime RealDate { get; set; }
			public Guid Guid { get; set; }
			public Version Version { get; set; }

			public bool HasMinVersion { get; set; }
			public Version MinVersion { get; set; }

			public bool HasMaxVersion { get; set; }
			public Version MaxVersion { get; set; }

			public int ImageIndex { get; set; }
			public bool IsChecked { get; set; }

			public UpdateItem()
			{
				HasMinVersion = HasMaxVersion = false;
			}

			#region IComparable<UpdateItem> Members

			public int CompareTo(UpdateItem item)
			{
				int cmp = RealDate.CompareTo(item.RealDate);

				if (cmp == 0)
					cmp = Version.CompareTo(item.Version);

				return cmp;
			}

			#endregion
		}

		#endregion
	}
}