using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace MCSkin3D.Swatches
{
	public class SwatchLoader
	{
		static void AddDirectory(string dir, List<ISwatch> swatches)
		{
			foreach (var swatchFile in Directory.GetFiles(dir, "*"))
			{
				var ext = Path.GetExtension(swatchFile);

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

		static void SwatchLoadThreadFunc()
		{
			List<ISwatch> swatches = new List<ISwatch>();
			AddDirectory("Swatches", swatches);

			Editor.MainForm.Invoke(() =>
				{
					Editor.SwatchContainer.AddSwatches(swatches);
					Editor.SwatchContainer.Enabled = true;
					Editor.MainForm.loadingSwatchLabel.Visible = false;
				});

			_thread = null;
		}

		static Thread _thread;
		public static void LoadSwatches()
		{
			Editor.SwatchContainer.Enabled = false;
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
