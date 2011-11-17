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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace MCSkin3D
{
	static class Program
	{
		public static Editor MainForm { get; set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{

				String resourceName = "MCSkin3D.Resources." +

				   new AssemblyName(args.Name).Name + ".dll";

				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{

					if (stream == null)
						return null;

					Byte[] assemblyData = new Byte[stream.Length];

					stream.Read(assemblyData, 0, assemblyData.Length);

					return Assembly.Load(assemblyData);

				}

			};

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(MainForm = new Editor());
			}
			catch (Exception ex)
			{
				Paril.Windows.Dialogs.ExceptionDialog.Show(ex);
			}
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Paril.Windows.Dialogs.ExceptionDialog.Show(e.Exception);
		}
	}
}
