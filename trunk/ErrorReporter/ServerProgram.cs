using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServerTest
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.SetCompatibleTextRenderingDefault(false);
			Application.EnableVisualStyles();
			Application.Run(new ServerForm());
		}
	}
}
