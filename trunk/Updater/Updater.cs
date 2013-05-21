using System;
using System.Net;
using Version = Paril.Components.Update.Version;

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