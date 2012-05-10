using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCSkin3D.Forms;
using System.IO;
using System.Reflection;
using MCSkin3D.UpdateSystem;

namespace MCSkin3D
{
	class MCSkin3DAppContext : ApplicationContext
	{
		public Form Form;
		public Splash SplashForm;
		public Updater Updater;

		public MCSkin3DAppContext()
		{
			try
			{
				if (Directory.Exists("__updateFiles"))
					Directory.Delete("__updateFiles", true);
			}
			catch
			{
			}

			Program.Context = this;
			Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			GlobalSettings.Load();

			SplashForm = new Splash();

			Updater = new UpdateSystem.Updater("http://alteredsoftworks.com/mcskin3d/updates.xml");
			Updater.FormHidden += SplashForm.Updater_FormClosed;
			Updater.UpdatesAvailable += SplashForm.Updater_UpdatesAvailable;

			Form = new Editor();
			Form.FormClosing += (sender, e) => GlobalSettings.Save();
			Form.FormClosed += (sender, e) => ExitThread();
			
			SplashForm.Show();

			Splash.BeginLoaderThread();
		}

		public void DoneLoadingSplash()
		{
			Updater.FormHidden -= SplashForm.Updater_FormClosed;
			Updater.UpdatesAvailable -= SplashForm.Updater_UpdatesAvailable;

			SplashForm.Close();
			Form.Show();
		}
	}
}
