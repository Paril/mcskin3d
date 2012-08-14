using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Version = Paril.Components.Update.Version;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.Threading;
using MCSkin3D.Macros;
using Paril.Settings;

namespace UpdateInstaller
{
	public partial class Installer : Form
	{
		public Installer()
		{
			InitializeComponent();
		}

		const int MF_BYPOSITION = 0x400;

		[DllImport("user32")]

		private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);

		[DllImport("user32")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("user32")]
		private static extern int GetMenuItemCount(IntPtr hWnd);

		struct UpdateData : IComparable<UpdateData>
		{
			public string ZipPath;

			public string Name;
			public DateTime Date;
			public bool IsMajor;
			public bool IsProgramUpdate;
			public Version Version;
			public Guid GUID;

			public int CompareTo(UpdateData other)
			{
				if (IsProgramUpdate && other.IsProgramUpdate)
				{
					if (IsMajor && !other.IsMajor)
						return -1;
					else if (!IsMajor && other.IsMajor)
						return 1;
				}
				else if (IsProgramUpdate && !other.IsProgramUpdate)
					return -1;
				else if (!IsProgramUpdate && other.IsProgramUpdate)
					return 1;

				int cmp = Date.CompareTo(other.Date);

				if (cmp == 0)
					cmp = Version.CompareTo(other.Version);

				if (cmp == 0)
					cmp = Name.CompareTo(other.Name);

				return cmp;
			}
		};

		void UpdateProgress(string what, int value, int max)
		{
			if (InvokeRequired)
			{
				Invoke((Action<string, int, int>)UpdateProgress, what, value, max);
				return;
			}

			label3.Text = what;

			if (value == 0 && max == 0)
				progressBar1.Style = ProgressBarStyle.Marquee;
			else
			{
				progressBar1.Style = ProgressBarStyle.Continuous;
				progressBar1.Maximum = max;
				progressBar1.Value = value;
			}
		}

		static class GlobalSettings
		{
			[Savable]
			[DefaultValue("$(DataLocation)Skins\\")]
			[TypeSerializer(typeof(StringArraySerializer), true)]
			public static string[] SkinDirectories { get; set; }
		}

		void DoUpdates()
		{
			if (Environment.GetCommandLineArgs().Length != 2)
				MessageBox.Show("Due to some rather large breaking changes in MCSkin3D 1.4.1, the update system will no longer work with 1.4.0. Please head to the forum thread (via Help) and download 1.4.1 manually. Sorry for the inconvenience!");
			else
			{
				Settings globalSettings = new Settings();
				globalSettings.Structures.Add(typeof(GlobalSettings));
				globalSettings.Load("..\\settings.ini");

				MacroHandler.RegisterMacro("DataLocation", new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName);
				MacroHandler.RegisterMacro("DefaultSkinFolder", MacroHandler.ReplaceMacros(GlobalSettings.SkinDirectories[0]));

				List<UpdateData> _updates = new List<UpdateData>();

				UpdateProgress("Enumerating updates...", 0, 0);

				bool allDone = true;
				foreach (var datFile in Directory.GetFiles(".", "*.dat"))
				{
					try
					{
						var name = Path.GetFileNameWithoutExtension(datFile);

						UpdateData data = new UpdateData();
						data.ZipPath = name + ".zip";

						using (var reader = new BinaryReader(File.Open(datFile, FileMode.Open, FileAccess.Read)))
						{
							data.Name = reader.ReadString();

							int day = reader.ReadInt32();
							int month = reader.ReadInt32();
							int year = reader.ReadInt32();

							data.Date = new DateTime(year, month, day);

							data.IsMajor = reader.ReadBoolean();
							data.IsProgramUpdate = reader.ReadBoolean();

							int major = reader.ReadInt32();
							int minor = reader.ReadInt32();
							int build = reader.ReadInt32();
							int revision = reader.ReadInt32();

							data.Version = new Version(major, minor, revision, build);
							data.GUID = new Guid(reader.ReadBytes(16));

							_updates.Add(data);
						}
					}
					catch
					{
						allDone = false;
					}
				}

				_updates.Sort();

				UpdateProgress("Preparing to install...", 0, _updates.Count);
				var fz = new FastZip();

				using (var installed = new StreamWriter("../__installedupdates", true))
					foreach (var u in _updates)
					{
						try
						{
							UpdateProgress("Installing \"" + u.Name + "\"...", progressBar1.Value + 1, _updates.Count);

							var zip = new ZipFile(File.Open(u.ZipPath, FileMode.Open, FileAccess.Read));
							zip.IsStreamOwner = true;

							// find the cfg file
							ZipEntry configFile = null;

							foreach (ZipEntry entry in zip)
							{
								if (entry.IsFile &&
									entry.Name.EndsWith(".cfg"))
								{
									configFile = entry;
									break;
								}
							}

							if (configFile == null)
								throw new Exception();

							using (var reader = new StreamReader(zip.GetInputStream(configFile)))
							{
								while (!reader.EndOfStream)
								{
									var line = reader.ReadLine().Split('|');
									var entry = zip.FindEntry(line[0], true);

									if (entry == -1)
										throw new Exception();

									fz.ExtractZip(zip.GetInputStream(entry), MacroHandler.ReplaceMacros(line[1]), FastZip.Overwrite.Always, null, "", "", true, true);
								}
							}

							zip.Close();

							installed.WriteLine(u.GUID);
						}
						catch
						{
							allDone = false;
						}
					}

				Invoke((Action)delegate()
				{
					if (allDone)
						MessageBox.Show("All updates have installed successfully. MCSkin3D will now restart.", "Success!", MessageBoxButtons.OK);
					else
						MessageBox.Show("One or more updates failed to install. Please report this to the MCSkin3D team.", "Failure?", MessageBoxButtons.OK);
				});
			}

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.WorkingDirectory = Environment.CurrentDirectory + "..\\";
			psi.FileName = Environment.CurrentDirectory + "\\..\\" + "MCSkin3D.exe";
			Process.Start(psi);

			Invoke((Action)delegate()
			{
				Close();
			});
		}

		private void Installer_Load(object sender, EventArgs e)
		{
			IntPtr hMenu = GetSystemMenu(this.Handle, false);
			int menuItemCount = GetMenuItemCount(hMenu);
			RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);

			Thread updateThread = new Thread(DoUpdates);
			updateThread.Start();
		}
	}
}
