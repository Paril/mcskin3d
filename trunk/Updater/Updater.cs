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
	public class UpdateChecker
	{
		string _file;

		public UpdateChecker(string file)
		{
			_file = file;
		}

		public bool CheckForUpdates()
		{
			try
			{
				WebClient wc = new WebClient();
				wc.Proxy = null;
				var version = new Version(wc.DownloadString(_file));

				return (version > Program.Version);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}