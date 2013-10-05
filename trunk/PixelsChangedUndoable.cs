//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
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

using Paril.Compatibility;
using Paril.Components;
using Paril.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MCSkin3D
{
	public struct ColorAlpha
	{
		public Color Color;
		public float TotalAlpha;

		public ColorAlpha(Color c, float alpha) :
			this()
		{
			Color = c;
			TotalAlpha = alpha;
		}
	}

	public class PixelsChangedUndoable : IUndoable
	{
		public Dictionary<Point, Tuple<Color, ColorAlpha>> Points = new Dictionary<Point, Tuple<Color, ColorAlpha>>();

		public PixelsChangedUndoable(string action)
		{
			Action = action + " [" + DateTime.Now.ToString("h:mm:ss") + "]";
		}

		public PixelsChangedUndoable(string action, string tool) :
			this(action + " (" + tool + ")")
		{
		}

		#region IUndoable Members

		public string Action { get; private set; }

		public void Undo(object obj)
		{
			var skin = (Skin) obj;

			using (var grabber = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
			{
				grabber.Load();

				foreach (var kvp in Points)
				{
					Point p = kvp.Key;
					Tuple<Color, ColorAlpha> color = kvp.Value;
					grabber[p.X, p.Y] =
						new ColorPixel(color.Item1.R | (color.Item1.G << 8) | (color.Item1.B << 16) | (color.Item1.A << 24));

					if (!Editor.MainForm.PaintedPixels.ContainsKey(p))
						Editor.MainForm.PaintedPixels.Add(p, true);
				}

				grabber.Save();
			}

			Editor.MainForm.SetPartTransparencies();
		}

		public void Redo(object obj)
		{
			var skin = (Skin) obj;

			using (var grabber = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
			{
				grabber.Load();

				foreach (var kvp in Points)
				{
					Point p = kvp.Key;
					Tuple<Color, ColorAlpha> color = kvp.Value;
					grabber[p.X, p.Y] =
						new ColorPixel(color.Item2.Color.R | (color.Item2.Color.G << 8) | (color.Item2.Color.B << 16) |
									   (color.Item2.Color.A << 24));

					if (!Editor.MainForm.PaintedPixels.ContainsKey(p))
						Editor.MainForm.PaintedPixels.Add(p, true);
				}

				grabber.Save();
			}

			Editor.MainForm.SetPartTransparencies();
		}

		#endregion
	}
}