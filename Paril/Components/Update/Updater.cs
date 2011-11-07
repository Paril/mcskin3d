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

		static void CallbackAsync(IAsyncResult ar)
		{
			var objs = ar.AsyncState as object[];
			bool succeeded = false;
			HttpWebRequest request = objs[0] as HttpWebRequest;
			var response = (HttpWebResponse)request.GetResponse();
			var updater = (Updater)objs[1];

			if (response.StatusCode == HttpStatusCode.OK)
				using (var stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
						succeeded = updater.UpdateHandler.IsNewerVersion(updater.CurrentVersion, reader.ReadToEnd());
				}

			updater.Done(succeeded);
		}

		static void UpdaterThread(object parameter)
		{
			Updater updater = (Updater)parameter;

			try
			{
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(updater.URL);

				request.Timeout = -1;

				IAsyncResult response = (IAsyncResult)request.BeginGetResponse(CallbackAsync, new object[] { request, updater });
				Thread.Sleep(20000);

				request.Abort();
				throw new TimeoutException();
			}
			catch
			{
				updater.Done(false);
			}
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
			int myMajor = int.Parse(myVersion[0].ToString());
			int myMinor = int.Parse(myVersion[2].ToString());
			int siteMajor = int.Parse(siteVersion[0].ToString());
			int siteMinor = int.Parse(siteVersion[2].ToString());

			return (siteMajor > myMajor || (siteMajor == myMajor && siteMinor > myMinor));
			//return Version.Parse(siteVersion) > Version.Parse(myVersion);
		}
	}
}
