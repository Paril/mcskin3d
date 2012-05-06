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
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using MCSkin3D.ExceptionHandler;
using SVN;
using Version = Paril.Components.Update.Version;

namespace MCSkin3D
{
	internal static class Program
	{
		public static Version Version;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			                                           {
			                                           	String resourceName = "MCSkin3D.Resources." +
			                                           	                      new AssemblyName(args.Name).Name + ".dll";

			                                           	using (
			                                           		Stream stream =
			                                           			Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
			                                           		)
			                                           	{
			                                           		if (stream == null)
			                                           			return null;

			                                           		var assemblyData = new Byte[stream.Length];
			                                           		stream.Read(assemblyData, 0, assemblyData.Length);
			                                           		return Assembly.Load(assemblyData);
			                                           	}
			                                           };

			MainCore();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void MainCore()
		{
#if !DEBUG
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += Application_ThreadException;
#endif

			Version = new Version(Application.ProductVersion);
			Version.Revision = Repository.Revision;

#if !DEBUG
			try
			{
#endif
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Editor());

#if !DEBUG
			}
			catch (Exception ex)
			{
				RaiseException(ex);
			}
#endif
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			RaiseException(e.Exception);
		}

		public static void RaiseException(Exception ex)
		{
			if (Editor.MainForm.InvokeRequired)
			{
				Editor.MainForm.Invoke((Action)delegate() { RaiseException(ex); });
				return;
			}

			var form = new ExceptionForm();
			form.Exception = ex;
			form.languageProvider1.LanguageChanged(Editor.CurrentLanguage);
			SystemSounds.Asterisk.Play();

			if (Editor.MainForm.Visible)
				form.ShowDialog(Editor.MainForm);
			else
				form.ShowDialog();
		}
	}
}