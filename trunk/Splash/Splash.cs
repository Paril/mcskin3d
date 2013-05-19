using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MCSkin3D.Languages;
using System.Globalization;
using MCSkin3D.Swatches;

namespace MCSkin3D.Forms
{
	public partial class Splash : Form
	{
		static Thread _loaderThread;

		public Splash()
		{
			InitializeComponent();
		}

		public void SetLoadingString(string s)
		{
			if (InvokeRequired)
			{
				Invoke((Action<string>)SetLoadingString, s);
				return;
			}

			label1.Text = s;
		}

		private void label1_Click(object sender, EventArgs e)
		{
		}

		Language LoadLanguages()
		{
			LanguageLoader.LoadLanguages(GlobalSettings.GetDataURI("Languages"));

			Language useLanguage = null;
			try
			{
				// stage 1 (prelim): if no language, see if our languages contain it
				if (string.IsNullOrEmpty(GlobalSettings.LanguageFile))
				{
					useLanguage =
						LanguageLoader.FindLanguage((CultureInfo.CurrentUICulture.IsNeutralCulture == false)
														? CultureInfo.CurrentUICulture.Parent.Name
														: CultureInfo.CurrentUICulture.Name);
				}

				// stage 2: load from last used language
				if (useLanguage == null)
					useLanguage = LanguageLoader.FindLanguage(GlobalSettings.LanguageFile);

				// stage 3: use English file, if it exists
				if (useLanguage == null)
					useLanguage = LanguageLoader.FindLanguage("English");
			}
			catch
			{
			}
			finally
			{
				// stage 4: fallback to built-in English file
				if (useLanguage == null)
				{
					Program.Context.SplashForm.Invoke((Action)(() => MessageBox.Show(this, "For some reason, the default language files were missing or failed to load (did you extract?) - we'll supply you with a base language of English just so you know what you're doing!")));
					useLanguage = LanguageLoader.LoadDefault();
				}
			}

			return useLanguage;
		}

		object _lockObj = new object();

		void PerformLoading()
		{
			try
			{
				SetLoadingString("Loading Languages...");

				var language = LoadLanguages();

				SetLoadingString("Initializing base forms...");

				Program.Context.SplashForm.Invoke((Action)(() =>
					{
						Editor.MainForm.FinishedLoadingLanguages();
						Editor.MainForm.Initialize(language);
					}));

				if (GlobalSettings.AutoUpdate)
				{
					SetLoadingString("Checking for updates...");

					if (Program.Context.Updater.CheckForUpdates())
					{
						Program.Context.SplashForm.Invoke((Action)(() =>
						{
							Editor.ShowUpdateDialog(Program.Context.SplashForm);
						}));
					}
				}

				SetLoadingString("Loading swatches...");

				SwatchLoader.LoadSwatches();
				Program.Context.SplashForm.Invoke((Action)SwatchLoader.FinishedLoadingSwatches);

				SetLoadingString("Loading models...");

				ModelLoader.LoadModels();
				Program.Context.SplashForm.Invoke((Action)Editor.MainForm.FinishedLoadingModels);

				SetLoadingString("Loading skins...");

				SkinLoader.LoadSkins();

				Program.Context.SplashForm.Invoke((Action)Program.Context.DoneLoadingSplash);
				Program.Context.Form.Invoke((Action)(() =>
					{
						Program.Context.SplashForm.Close();
						GC.Collect();
					}
					));
			}
			catch (Exception ex)
			{
				Program.RaiseException(new Exception("Failed to initialize program during \"" + label1.Text + "\"", ex));
				Application.Exit();
			}
		}

		public static void BeginLoaderThread()
		{
			_loaderThread = new Thread(Program.Context.SplashForm.PerformLoading);
			_loaderThread.Start();
		}

		private void Splash_Load(object sender, EventArgs e)
		{
			SetLoadingString("Doing nothing yet...");

			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 100;
			timer.Elapsed += (s, ev) => Splash.BeginLoaderThread();
			timer.AutoReset = false;
			timer.Start();
		}
	}
}
