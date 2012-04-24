using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MCSkin3D.Swatches
{
	public class SwatchLoader
	{
		private static Thread _thread;

		private static void AddDirectory(string dir, List<ISwatch> swatches)
		{
			foreach (string swatchFile in Directory.GetFiles(dir, "*"))
			{
				string ext = Path.GetExtension(swatchFile);

				ISwatch swatch = null;

				if (ext.ToLower() == ".swtch")
					swatch = new MCSwatch(swatchFile);
				else if (ext.ToLower() == ".gpl" || ext.ToLower() == ".gimp")
					swatch = new GIMPSwatch(swatchFile);
				else if (ext.ToLower() == ".act")
					swatch = new ACTSwatch(swatchFile);
				else if (ext.ToLower() == ".aco")
					swatch = new ACOSwatch(swatchFile);

				if (swatch != null)
				{
					swatches.Add(swatch);
					swatch.Load();
				}
			}
		}

		private static void SwatchLoadThreadFunc()
		{
			var swatches = new List<ISwatch>();
			AddDirectory("Swatches", swatches);

			Editor.MainForm.Invoke(() =>
			                       {
			                       	Editor.MainForm.ColorPanel.SwatchContainer.AddSwatches(swatches);
			                       	Editor.MainForm.ColorPanel.SwatchContainer.Enabled = true;
			                       	Editor.MainForm.ColorPanel.SetLoading(false);
			                       });

			_thread = null;
		}

		public static void LoadSwatches()
		{
			Editor.MainForm.ColorPanel.SwatchContainer.Enabled = false;
			_thread = new Thread(SwatchLoadThreadFunc);
			_thread.Start();
		}

		public static void CancelLoadSwatches()
		{
			if (_thread != null)
			{
				_thread.Abort();
				_thread = null;
			}
		}
	}
}