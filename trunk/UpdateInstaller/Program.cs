using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace UpdateInstaller
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// Wait until MCSkin3D is closed.
			while (Process.GetProcessesByName("MCSkin3D").Length != 0)
				Thread.Sleep(100);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Installer());
		}
	}
}
