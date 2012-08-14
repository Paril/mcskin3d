using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCSkin3D.Forms;
using System.IO;
using System.Reflection;
using MCSkin3D.UpdateSystem;
using System.Threading;

namespace MCSkin3D
{
	class MCSkin3DAppContext : ApplicationContext
	{
		public Form Form;
		public Splash SplashForm;
		public Updater Updater;

		public MCSkin3DAppContext()
		{
			Program.Context = this;
			Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			GlobalSettings.Load();

			try
			{
				if (Directory.Exists(GlobalSettings.GetDataURI("__updateFiles")))
					Directory.Delete(GlobalSettings.GetDataURI("__updateFiles"), true);
			}
			catch
			{
			}

			SplashForm = new Splash();

			Updater = new UpdateSystem.Updater(
#if BETA
				"http://alteredsoftworks.com/mcskin3d/updates_beta.xml"
#else
				"http://alteredsoftworks.com/mcskin3d/updates.xml"
#endif
);
			Updater.FormHidden += SplashForm.Updater_FormClosed;
			Updater.UpdatesAvailable += SplashForm.Updater_UpdatesAvailable;

			Form = new Editor();
			Form.FormClosing += (sender, e) => GlobalSettings.Save();
			Form.FormClosed += (sender, e) => ExitThread();
			
			SplashForm.Show();
		}

		public void DoneLoadingSplash()
		{
			Updater.FormHidden -= SplashForm.Updater_FormClosed;
			Updater.UpdatesAvailable -= SplashForm.Updater_UpdatesAvailable;

			Form.Show();
		}
	}
}
