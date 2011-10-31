using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace Paril.Components.Update
{
	public class Updater
	{
		public string URL { get; set; }
		public IVersion UpdateHandler { get; set; }
		public bool Checking { get { return _checking; } }
		public bool PrintOnEqual { get; set; }
		public string CurrentVersion { get; set; }
		public Thread Thread { get; set; }
		
		bool _checking = false;

		public Updater(string url, string currentVersion)
		{
			URL = url;
			CurrentVersion = currentVersion;
		}

		static void UpdaterThread(object parameter)
		{
			Updater updater = (Updater)parameter;

			HttpWebRequest request = (HttpWebRequest)FileWebRequest.Create(updater.URL);

			request.Timeout = 10000;

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			bool succeeded = false;

			if (response.StatusCode == HttpStatusCode.OK)
				using (var stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
						succeeded = updater.UpdateHandler.IsNewerVersion(updater.CurrentVersion, reader.ReadToEnd());
				}

			updater.Done(succeeded);
		}

		void Done(bool succeeded)
		{
			Thread = null;
			_checking = false;

			if (succeeded)
				NewVersionAvailable(this, EventArgs.Empty);
			else if (PrintOnEqual)
				SameVersion(this, EventArgs.Empty);
		}

		public void Abort()
		{
			if (Thread != null)
			{
				Thread.Abort();
				Done(false);
			}
		}

		public void CheckForUpdate()
		{
			if (_checking)
				throw new InvalidOperationException("Already checking for a new version!");

			_checking = true;

			Thread = new Thread(UpdaterThread);
			Thread.Start(this);
		}

		public event EventHandler NewVersionAvailable;
		public event EventHandler SameVersion;
	}

	public interface IVersion
	{
		bool IsNewerVersion(string myVersion, string siteVersion);
	}

	public class AssemblyVersion : IVersion
	{
		public bool IsNewerVersion(string myVersion, string siteVersion)
		{
			return Version.Parse(siteVersion) > Version.Parse(myVersion);
		}
	}
}
