using MCSkin3D.Forms;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MCSkin3D
{
	class MCSkin3DAppContext : ApplicationContext
	{
		public Form Form;
		public Splash SplashForm;
		public UpdateSystem.UpdateChecker Updater;

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
			Updater = new UpdateSystem.UpdateChecker("http://alteredsoftworks.com/mcskin3d/update");

			Form = new Editor();
			Form.FormClosing += (sender, e) => GlobalSettings.Save();
			Form.FormClosed += (sender, e) => ExitThread();
			
			SplashForm.Show();
		}

		public void DoneLoadingSplash()
		{
			Form.Show();
		}
	}
}
