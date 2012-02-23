using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientTest
{
	public struct ErrorReport
	{
		public Exception Exception { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string ExtraInfo { get; set; }
	}
}
