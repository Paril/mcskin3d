//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Paril.Components.Update
{
	public class Updater
	{
		private static readonly object waitObject = new object();
		private bool _checking;

		public Updater(string url, string currentVersion)
		{
			URL = url;
			CurrentVersion = currentVersion;
		}

		public string URL { get; set; }
		public IVersion UpdateHandler { get; set; }

		public bool Checking
		{
			get { return _checking; }
		}

		public bool PrintOnEqual { get; set; }
		public string CurrentVersion { get; set; }
		public Thread Thread { get; set; }

		private static void CallbackAsync(IAsyncResult ar)
		{
			var objs = ar.AsyncState as object[];
			bool succeeded = false;
			var updater = (Updater) objs[1];
			try
			{
				var request = objs[0] as HttpWebRequest;
				var response = (HttpWebResponse) request.GetResponse();

				if (response.StatusCode == HttpStatusCode.OK)
				{
					using (Stream stream = response.GetResponseStream())
					using (var reader = new StreamReader(stream))
						succeeded = updater.UpdateHandler.IsNewerVersion(updater.CurrentVersion, reader.ReadToEnd());
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				updater.Done(succeeded);
			}

			lock (waitObject)
				Monitor.Pulse(waitObject);
		}

		private static void UpdaterThread(object parameter)
		{
			Updater updater = null;
			HttpWebRequest request = null;
			try
			{
				updater = (Updater) parameter;
				request = (HttpWebRequest) WebRequest.Create(updater.URL);

				request.Timeout = 10000;

				IAsyncResult response = request.BeginGetResponse(CallbackAsync, new object[] {request, updater});

				lock (waitObject)
					Monitor.Wait(waitObject);

				request.Abort();
			}
			catch
			{
			}
			finally
			{
				if (request != null)
					request.Abort();
				if (updater != null)
					updater.Done(false);
			}
		}

		private void Done(bool succeeded)
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
				lock (waitObject)
					Monitor.Pulse(waitObject);
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
		#region IVersion Members

		public bool IsNewerVersion(string myVersion, string siteVersion)
		{
			return new Version(siteVersion) > new Version(myVersion);
		}

		#endregion
	}
}