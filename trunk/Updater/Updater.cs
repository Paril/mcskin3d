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
using System.Diagnostics;
using System.Linq;

namespace MCSkin3D.UpdateSystem
{
	public partial class Updater : Form
	{
		private static readonly ImageList _updateImages = new ImageList();
		private static readonly object lockObj = new object();
		private Thread _updateThread;
		private List<UpdateItem> _updates;

		public Updater()
		{
			InitializeComponent();
		}

		public event EventHandler UpdatesAvailable;
		public bool checkedAlready = false;

		public Updater(string url) :
			this()
		{
			UpdateXMLURL = url;

			if (InstalledUpdates == null)
			{
				InstalledUpdates = new List<Guid>();

				if (File.Exists(GlobalSettings.GetDataURI("__installedUpdates")))
				{
					using (var sr = new StreamReader(GlobalSettings.GetDataURI("__installedUpdates")))
						while (!sr.EndOfStream)
						{
							try
							{
								InstalledUpdates.Add(new Guid(sr.ReadLine()));
							}
							catch { }
						}
				}
				else
				{
					var file = new FileInfo(GlobalSettings.GetDataURI("__installedUpdates"));
					file.Create().Dispose();
					file.Attributes |= FileAttributes.Hidden;
				}
			}

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
				var col = (OLVColumn)c;

				col.GroupKeyGetter = delegate(object row) { return ((UpdateItem)row).Group; };

				col.GroupKeyToTitleConverter = delegate(object key) { return (string)key; };
			}

			objectListView1.AllColumns[0].RendererDelegate = delegate(EventArgs args, Graphics g, Rectangle r, Object rowObject)
			{
				g.FillRectangle(new SolidBrush(objectListView1.BackColor),
								new Rectangle(r.X - 1, r.Y - 1,
											  objectListView1.Width, r.Height + 2));

				var realArgs = (DrawListViewSubItemEventArgs)args;

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
					realArgs.Item.ImageList.Images[((UpdateItem)rowObject).ImageIndex
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

		void GetUpdateDataVoid()
		{
			GetUpdateData();
		}

		public void CheckForUpdates()
		{
			checkedAlready = true;
			_updateThread = new Thread(GetUpdateDataVoid);
			_updateThread.Start();
		}

		public void TellCheckForUpdate()
		{
			checkedAlready = true;
		}

		public string UpdateXMLURL { get; set; }
		public List<Guid> InstalledUpdates { get; set; }

		// FIXME: RSS instead?
		private List<UpdateItem> LoadUpdates(XmlDocument document)
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

					if (item.Guid.Equals(Guid.Empty))
						continue; // bad update

					items.Add(item);
				}
			}

			return items;
		}

		List<UpdateItem> GetUpdates()
		{
			var client = new WebClient();
			client.Proxy = null;
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

			client.Dispose();

			return LoadUpdates(doc);
		}

		bool doneChecking = false;
		public bool GetUpdateData()
		{
			try
			{
				try
				{
					_updates = GetUpdates();
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

					using (var cl = new WebClient())
					{
						cl.Proxy = null;
						foreach (UpdateItem u in _updates)
						{
							string url = u.GroupImageURL;
							string fileName = Path.GetFileName(url);

							if (!Directory.Exists(GlobalSettings.GetDataURI("__updateFiles\\")))
							{
								var di = Directory.CreateDirectory(GlobalSettings.GetDataURI("__updateFiles\\"));
								di.Attributes |= FileAttributes.Hidden;
							}

							string fileDir = GlobalSettings.GetDataURI("__updateFiles\\") + fileName;

							if (!File.Exists(fileDir))
								cl.DownloadFile(url, fileDir);

							if (!fileNames.Contains(fileDir))
							{
								fileNames.Add(fileDir);
								_updateImages.Images.Add(Image.FromFile(fileDir));
							}

							u.ImageIndex = fileNames.IndexOf(fileDir);
						}
					}
				}
				catch
				{
				}

				doneChecking = true;

				if (Visible)
					Invoke((Action)UpdateFinished);
				else if (_updates.Any(item => item.IsProgramUpdate))
				{
					UpdatesAvailable(this, EventArgs.Empty);
					return true;
				}
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
					return false;

				Program.RaiseException(ex);
			}

			return false;
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
			if (e.UserState != null)
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
			if (e.UserState != null)
			Invoke((Action) delegate
								{
			                		((UpdateItem) e.UserState).Progress = e.ProgressPercentage;
			                		objectListView1.RedrawItems(0, _updates.Count - 1, false);
								});
		}

		public event EventHandler FormHidden;

		private void DownloadUpdateData()
		{
			try
			{
				var client = new WebClient();
				client.Proxy = null;

				client.DownloadProgressChanged += client_DownloadProgressChanged;
				client.DownloadFileCompleted += client_DownloadFileCompleted;

				if (!Directory.Exists(GlobalSettings.GetDataURI("__updateFiles\\")))
					Directory.CreateDirectory(GlobalSettings.GetDataURI("__updateFiles\\"));

				string url = "http://alteredsoftworks.com/mcskin3d/UpdateInstaller.exe";
				string fileName = Path.GetFileName(url);
				string fileDir = GlobalSettings.GetDataURI("__updateFiles\\") + fileName;

				client.DownloadFileAsync(new Uri(url), fileDir, null);

				lock (lockObj)
					Monitor.Wait(lockObj);

				foreach (UpdateItem u in _updates)
				{
					if (!u.IsChecked)
						continue;

					url = u.DownloadURL;
					fileName = Path.GetFileName(url);
					var withoutExt = Path.GetFileNameWithoutExtension(fileName);

					using (var dataFile = new BinaryWriter(File.Create(GlobalSettings.GetDataURI("__updateFiles\\") + withoutExt + ".dat")))
					{
						dataFile.Write(u.Name);

						dataFile.Write(u.RealDate.Day);
						dataFile.Write(u.RealDate.Month);
						dataFile.Write(u.RealDate.Year);

						dataFile.Write(u.IsMajor);
						dataFile.Write(u.IsProgramUpdate);

						dataFile.Write(u.Version.Major);
						dataFile.Write(u.Version.Minor);
						dataFile.Write(u.Version.Build);
						dataFile.Write(u.Version.Revision);

						dataFile.Write(u.Guid.ToByteArray());
					}

					fileDir = GlobalSettings.GetDataURI("__updateFiles\\") + fileName;

					client.DownloadFileAsync(new Uri(url), fileDir, u);

					lock (lockObj)
						Monitor.Wait(lockObj);
				}

				Invoke((Action)delegate
								{
									MessageBox.Show(Editor.GetLanguageString("M_UPDATESTARTING"));

									var psi = new ProcessStartInfo();
									psi.WorkingDirectory = GlobalSettings.GetDataURI("__updateFiles\\");
									psi.FileName = new FileInfo(GlobalSettings.GetDataURI("__updateFiles\\") + "UpdateInstaller.exe").FullName;
									psi.Arguments = "\"" + Environment.CurrentDirectory + "\"";
									Process.Start(psi);

									DialogResult = DialogResult.OK;

									if (FormHidden != null)
										FormHidden(this, EventArgs.Empty);

									Hide();
								});
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
					return;

				Program.RaiseException(ex);
			}
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
			if (!checkedAlready)
			{
				checkedAlready = true;
				_updateThread = new Thread(GetUpdateDataVoid);
				_updateThread.Start();
			}
			else if (_updates != null && doneChecking)
				UpdateFinished();
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

			if (FormHidden != null)
				FormHidden(this, EventArgs.Empty);

			Hide();
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

				if (cmp == 0)
					cmp = Name.CompareTo(item.Name);

				return cmp;
			}

			#endregion
		}

		#endregion

		private void Updater_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				DialogResult = DialogResult.Cancel;
				
				if (FormHidden != null)
					FormHidden(this, EventArgs.Empty);

				Hide();
			}
		}

		public void StopUpdates()
		{
			if (_updateThread != null && 
				_updateThread.ThreadState == System.Threading.ThreadState.Running)
				_updateThread.Abort();
		}
	}
}