using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Paril.Net
{
	public static class WebHelpers
	{
		public static byte[] DownloadFile(string url)
		{
			WebClient wc = new WebClient();
			return wc.DownloadData(url);
		}
	}
}
