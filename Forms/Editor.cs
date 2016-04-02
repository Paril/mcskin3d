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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MCSkin3D.Controls;
using MCSkin3D.Forms;
using MCSkin3D.Languages;
using MCSkin3D.Macros;
using MCSkin3D.Properties;
using Microsoft.VisualBasic.FileIO;
using MultiPainter;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Paril.Components;
using Paril.Components.Shortcuts;
using Paril.Controls;
using Paril.Drawing;
using Paril.Extensions;
using Paril.Imaging;
using Paril.Net;
using Paril.OpenGL;
using PopupControl;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace MCSkin3D
{
	public partial class Editor : Form
	{
		#region Variables

		private static readonly ShortcutEditor _shortcutEditor = new ShortcutEditor();
		private List<BackgroundImage> _backgrounds = new List<BackgroundImage>();
		private Dictionary<Size, Texture> _charPaintSizes = new Dictionary<Size, Texture>();
		private UndoRedoPanel _redoListBox;
		private List<ToolIndex> _tools = new List<ToolIndex>();
		private UndoRedoPanel _undoListBox;

		private float _2DCamOffsetX;
		private float _2DCamOffsetY;
		private float _2DZoom = 8;
		private Vector3 _3DOffset = Vector3.Zero;
		static float _3DRotationX = 180, _3DRotationY;
		private float _3DZoom = -80;
		private Texture _alphaTex;
		private UndoBuffer _currentUndoBuffer;
		private ViewMode _currentViewMode = ViewMode.Perspective;
		private Texture _font;
		private Texture _grassTop;
		private Skin _lastSkin;
		private bool _mouseIsDown;
		private Point _mousePoint;
		private Texture _previewPaint;
		private int _selectedBackground;
		private ToolIndex _selectedTool;
		static Texture _cubeSides;

		#endregion

		// ===============================================
		// Private/Static variables
		// ===============================================

		private static bool _screenshotMode;

		public static Stopwatch _renderTimer = new Stopwatch();
		public static Stopwatch _sortTimer = new Stopwatch();
		public static Stopwatch _batchTimer = new Stopwatch();
		public static Stopwatch _compileTimer = new Stopwatch();
		private static bool _firstCalc;
		private static ToolStripMenuItem[] _antialiasOpts;
		private readonly ImportSite _importFromSite = new ImportSite();
		private Rectangle _currentViewport;
		private BackgroundImage _dynamicOverlay;
		private bool _isValidPick;
		private bool _mouseIn3D;
		private string[] _newSkinDirs;
		private ModelToolStripMenuItem _oldModel;
		private bool _opening = true;
		private Point _pickPosition = new Point(-1, -1);
		private TreeNode _rightClickedNode;
		private bool _waitingForRestart;
		private Form _popoutForm;
		private Skin _uploadedSkin;

		public ToolIndex SelectedTool
		{
			get { return _selectedTool; }
		}

		public Renderer MeshRenderer { get; private set; }

		public DodgeBurnOptions DodgeBurnOptions { get; private set; }
		public DarkenLightenOptions DarkenLightenOptions { get; private set; }
		public PencilOptions PencilOptions { get; private set; }
		public FloodFillOptions FloodFillOptions { get; private set; }
		public NoiseOptions NoiseOptions { get; private set; }
		public EraserOptions EraserOptions { get; private set; }
		public StampOptions StampOptions { get; private set; }

		public static Editor MainForm { get; private set; }

		// ===============================================
		// Constructor
		// ===============================================

		public GLControl Renderer
		{
			get;
			private set;
		}

		public MouseButtons CameraRotate
		{
			get
			{
				if (_selectedTool == _tools[(int)Tools.Camera])
					return MouseButtons.Left;
				else
					return MouseButtons.Right;
			}
		}

		public MouseButtons CameraZoom
		{
			get
			{
				if (_selectedTool == _tools[(int)Tools.Camera])
					return MouseButtons.Right;
				else
					return MouseButtons.Middle;
			}
		}

		public ColorPanel ColorPanel
		{
			get { return colorPanel; }
		}

		public static Model CurrentModel
		{
			get { return MainForm._lastSkin == null ? null : MainForm._lastSkin.Model; }
		}

		public float ToolScale
		{
			get { return 200.0f / Renderer.Size.Width; }
		}

		public static Language CurrentLanguage
		{
			get { return LanguageHandler.Language; }
			set
			{
				if (LanguageHandler.Language != null &&
					LanguageHandler.Language.Item != null)
					LanguageHandler.Language.Item.Checked = false;

				LanguageHandler.Language = value;

				GlobalSettings.LanguageFile = LanguageHandler.Language.Culture.Name;

				if (MainForm._selectedTool != null)
					MainForm.toolStripStatusLabel1.Text = MainForm._selectedTool.Tool.GetStatusLabelText();

				if (LanguageHandler.Language.Item != null)
					LanguageHandler.Language.Item.Checked = true;
			}
		}

		public static Vector3 CameraPosition { get; private set; }

		public static float GrassY { get; private set; }

		public static string RootFolderString
		{
			get
			{
				if (HasOneRoot)
					return new DirectoryInfo(MacroHandler.ReplaceMacros(GlobalSettings.SkinDirectories[0])).FullName;

				throw new InvalidOperationException();
			}
		}

		public static bool HasOneRoot
		{
			get { return GlobalSettings.SkinDirectories.Length == 1; }
		}

		public void PerformResetCamera()
		{
			_2DCamOffsetX = 0;
			_2DCamOffsetY = 0;
			_2DZoom = 8;
			_3DZoom = -80;
			_3DRotationX = 180;
			_3DRotationY = 0;
			_3DOffset = Vector3.Zero;

			CalculateMatrices();
			Renderer.Invalidate();
		}

		byte[] _charWidths = new byte[128];

		private void InitGL()
		{
			GL.ClearColor(GlobalSettings.BackgroundColor);

			GL.Enable(EnableCap.Texture2D);
			GL.ShadeModel(ShadingModel.Smooth); // Enable Smooth Shading
			GL.Enable(EnableCap.DepthTest); // Enables Depth Testing
			GL.DepthFunc(DepthFunction.Lequal); // The Type Of Depth Testing To Do
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest); // Really Nice Perspective Calculations
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
			_cubeSides = new TextureGL(Resources.cube_sides);
			_cubeSides.SetMipmapping(true);
			_cubeSides.SetRepeat(true);

			var tinyFont = Resources.tinyfont;

			_font = new TextureGL(tinyFont);
			_font.SetMipmapping(false);
			_font.SetRepeat(false);

			for (var c = 0; c < 128; ++c)
			{
				var x = (c % 16) * 8;
				var y = (c / 16) * 8;

				for (var px = x + 7; px >= x; --px)
				{
					var empty = true;

					for (var py = y; py < y + 8; ++py)
					{
						var pixl = tinyFont.GetPixel(px, py);

						if (pixl.A != 0)
						{
							empty = false;
							break;
						}
					}

					if (!empty)
					{
						_charWidths[c] = (byte)(px - x + 1);
						break;
					}
				}
			}

			_charWidths[(byte)' ' - 32] = 4;

			_grassTop = new TextureGL(GlobalSettings.GetDataURI("grass.png"));
			_grassTop.SetMipmapping(false);
			_grassTop.SetRepeat(true);

			_dynamicOverlay = new BackgroundImage("Dynamic", "Dynamic", null);
			_dynamicOverlay.Item = mDYNAMICOVERLAYToolStripMenuItem;
			_backgrounds.Add(_dynamicOverlay);

			foreach (string file in Directory.EnumerateFiles(GlobalSettings.GetDataURI("Overlays"), "*.png"))
			{
				try
				{
					var image = new TextureGL(file);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

					_backgrounds.Add(new BackgroundImage(file, Path.GetFileNameWithoutExtension(file), image));
				}
				catch
				{
					MessageBox.Show(this, string.Format(GetLanguageString("B_MSG_OVERLAYERROR"), file));
				}
			}

			int index = 0;
			foreach (BackgroundImage b in _backgrounds)
			{
				ToolStripMenuItem item = b.Item ?? new ToolStripMenuItem(b.Name);
				b.Item = item;

				if (b.Path == GlobalSettings.LastBackground)
				{
					item.Checked = true;
					_selectedBackground = index;
				}

				item.Click += item_Clicked;
				item.Tag = index++;

				if (!backgroundsToolStripMenuItem.DropDownItems.Contains(item))
					backgroundsToolStripMenuItem.DropDownItems.Add(item);
			}

			_previewPaint = new TextureGL();
			GlobalDirtiness.CurrentSkin = new TextureGL();

			_previewPaint.SetMipmapping(false);
			_previewPaint.SetRepeat(false);

			GlobalDirtiness.CurrentSkin.SetMipmapping(false);
			GlobalDirtiness.CurrentSkin.SetRepeat(false);

			_alphaTex = new TextureGL();
			_alphaTex.Upload(new byte[]
			{
				127, 127, 127, 255,
				80, 80, 80, 255,
				80, 80, 80, 255,
				127, 127, 127, 255
			}, 2, 2);
			_alphaTex.SetMipmapping(false);
			_alphaTex.SetRepeat(true);

			bool supportsArrays = GL.GetString(StringName.Extensions).Contains("GL_EXT_vertex_array");

			if (supportsArrays)
				MeshRenderer = new ClientArrayRenderer();
			else
				MeshRenderer = new ImmediateRenderer();
		}

		private void item_Clicked(object sender, EventArgs e)
		{
			var item = (ToolStripMenuItem)sender;
			_backgrounds[_selectedBackground].Item.Checked = false;
			_selectedBackground = (int)item.Tag;
			GlobalSettings.LastBackground = _backgrounds[_selectedBackground].Path;
			item.Checked = true;
		}

		private Texture GetPaintTexture(int width, int height)
		{
			if (!_charPaintSizes.ContainsKey(new Size(width, height)))
			{
				var tex = new TextureGL();

				var arra = new int[width * height];
				unsafe
				{
					fixed (int* texData = arra)
					{
						int* d = texData;

						for (int y = 0; y < height; ++y)
						{
							for (int x = 0; x < width; ++x)
							{
								*d = ((y * width) + x) | (255 << 24);
								d++;
							}
						}
					}
				}

				tex.Upload(arra, width, height);
				tex.SetMipmapping(false);
				tex.SetRepeat(false);

				_charPaintSizes.Add(new Size(width, height), tex);

				return tex;
			}

			return _charPaintSizes[new Size(width, height)];
		}

		private void DrawSkinnedRectangle
			(float x, float y, float z, float width, float length, float height,
			 int topSkinX, int topSkinY, int topSkinW, int topSkinH,
			 Texture texture, int skinW = 64, int skinH = 32)
		{
			texture.Bind();

			GL.Begin(PrimitiveType.Quads);

			width /= 2;
			length /= 2;
			height /= 2;

			float tsX = (float)topSkinX / skinW;
			float tsY = (float)topSkinY / skinH;
			float tsW = (float)topSkinW / skinW;
			float tsH = (float)topSkinH / skinH;

			GL.TexCoord2(tsX, tsY + tsH - 0.00005);
			GL.Vertex3(x - width, y + length, z + height); // Bottom Right Of The Quad (Top)
			GL.TexCoord2(tsX + tsW - 0.00005, tsY + tsH - 0.00005);
			GL.Vertex3(x + width, y + length, z + height); // Bottom Left Of The Quad (Top)
			GL.TexCoord2(tsX + tsW - 0.00005, tsY);
			GL.Vertex3(x + width, y + length, z - height); // Top Left Of The Quad (Top)
			GL.TexCoord2(tsX, tsY);
			GL.Vertex3(x - width, y + length, z - height); // Top Right Of The Quad (Top)

			GL.End();
		}

		private void DrawCharacter2D(Texture font, byte c, float xOfs, float yOfs, float width, float height)
		{
			font.Bind();

			float tx = (((c - 32) % 16) * 8) / 128.0f;
			float ty = (((c - 32) / 16) * 8) / 64.0f;
			const float txw = 8.0f / 128.0f;
			const float txh = 8.0f / 64.0f;

			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(tx, ty);
			GL.Vertex2(xOfs, yOfs);
			GL.TexCoord2(tx + txw, ty);
			GL.Vertex2(xOfs + width, yOfs);
			GL.TexCoord2(tx + txw, ty + txh);
			GL.Vertex2(xOfs + width, height + yOfs);
			GL.TexCoord2(tx, ty + txh);
			GL.Vertex2(xOfs, height + yOfs);
			GL.End();
		}

		private void DrawStringWithinRectangle(Texture font, RectangleF rect, string s, float spacing, float size)
		{
			float start = rect.X + (2 * GlobalSettings.DynamicOverlayTextSize) / _2DZoom, x = start;
			float y = rect.Y;

			foreach (char c in s)
			{
				DrawCharacter2D(font, (byte)c, x, y, size, size);
				x += ((_charWidths[(byte)c - 32] + 1) * GlobalSettings.DynamicOverlayTextSize) / _2DZoom;

				if ((x + spacing) > rect.X + rect.Width)
				{
					x = start;
					y += spacing;
				}

				if ((y + spacing) > rect.Y + rect.Height)
					break;
			}

			TextureGL.Unbind();
		}

		private void DrawPlayer2D(Texture tex, Skin skin)
		{
			if (GlobalSettings.AlphaCheckerboard)
			{
				_alphaTex.Bind();

				GL.Begin(PrimitiveType.Quads);
				GL.TexCoord2(0, 0);
				GL.Vertex2(0, 0);
				GL.TexCoord2(_currentViewport.Width / 16.0f, 0);
				GL.Vertex2(_currentViewport.Width, 0);
				GL.TexCoord2(_currentViewport.Width / 16.0f, _currentViewport.Height / 16.0f);
				GL.Vertex2(_currentViewport.Width, _currentViewport.Height);
				GL.TexCoord2(0, _currentViewport.Height / 16.0f);
				GL.Vertex2(0, _currentViewport.Height);
				GL.End();
			}

			if (skin != null)
				tex.Bind();

			GL.PushMatrix();

			GL.Translate((_2DCamOffsetX), (_2DCamOffsetY), 0);
			GL.Translate((_currentViewport.Width / 2) + -_2DCamOffsetX, (_currentViewport.Height / 2) + -_2DCamOffsetY, 0);
			GL.Scale(_2DZoom, _2DZoom, 1);

			GL.Enable(EnableCap.Blend);

			GL.Translate((_2DCamOffsetX), (_2DCamOffsetY), 0);
			if (skin != null)
			{
				float w = skin.Width;
				float h = skin.Height;
				GL.Begin(PrimitiveType.Quads);
				GL.TexCoord2(0, 0);
				GL.Vertex2(-(CurrentModel.DefaultWidth / 2), -(CurrentModel.DefaultHeight / 2));
				GL.TexCoord2(1, 0);
				GL.Vertex2((CurrentModel.DefaultWidth / 2), -(CurrentModel.DefaultHeight / 2));
				GL.TexCoord2(1, 1);
				GL.Vertex2((CurrentModel.DefaultWidth / 2), (CurrentModel.DefaultHeight / 2));
				GL.TexCoord2(0, 1);
				GL.Vertex2(-(CurrentModel.DefaultWidth / 2), (CurrentModel.DefaultHeight / 2));
				GL.End();
			}

			TextureGL.Unbind();

			if (GlobalSettings.GridEnabled && GlobalSettings.DynamicOverlayGridColor.A > 0)
			{
				GL.Color4(GlobalSettings.DynamicOverlayGridColor);
				GL.PushMatrix();
				GL.Translate(-(CurrentModel.DefaultWidth / 2), -(CurrentModel.DefaultHeight / 2), 0);
				GL.Begin(PrimitiveType.Lines);

				float wX = skin.Width / CurrentModel.DefaultWidth;
				float wY = skin.Height / CurrentModel.DefaultHeight;

				for (int i = 0; i <= skin.Width; ++i)
				{
					GL.Vertex2(i / wX, 0);
					GL.Vertex2(i / wX, skin.Height / wY);
				}

				for (int i = 0; i <= skin.Height; ++i)
				{
					GL.Vertex2(0, i / wY);
					GL.Vertex2(skin.Width / wX, i / wY);
				}

				GL.End();
				GL.PopMatrix();
			}

			if (GlobalSettings.TextureOverlay && skin != null)
			{
				if (_backgrounds[_selectedBackground] == _dynamicOverlay)
				{
					GL.PushMatrix();
					GL.Translate(-(CurrentModel.DefaultWidth / 2), -(CurrentModel.DefaultHeight / 2), 0);

					float stub = (GlobalSettings.DynamicOverlayLineSize / _2DZoom);
					float one = (1.0f / _2DZoom);

					var done = new List<RectangleF>();
					foreach (Mesh mesh in CurrentModel.Meshes)
					{
						foreach (Face face in mesh.Faces)
						{
							RectangleF toint = face.TexCoordsToFloat((int)CurrentModel.DefaultWidth, (int)CurrentModel.DefaultHeight);

							if (toint.Width == 0 ||
								toint.Height == 0)
								continue;
							if (done.Contains(toint))
								continue;

							done.Add(toint);

							GL.Color4(GlobalSettings.DynamicOverlayLineColor);
							GL.Begin(PrimitiveType.Quads);
							GL.Vertex2(toint.X, toint.Y);
							GL.Vertex2(toint.X + toint.Width, toint.Y);
							GL.Vertex2(toint.X + toint.Width, toint.Y + stub);
							GL.Vertex2(toint.X, toint.Y + stub);

							GL.Vertex2(toint.X, toint.Y);
							GL.Vertex2(toint.X + stub, toint.Y);
							GL.Vertex2(toint.X + stub, toint.Y + toint.Height);
							GL.Vertex2(toint.X, toint.Y + toint.Height);

							GL.Vertex2(toint.X + toint.Width + one, toint.Y);
							GL.Vertex2(toint.X + toint.Width + one, toint.Y + toint.Height);
							GL.Vertex2(toint.X + toint.Width + one - stub, toint.Y + toint.Height);
							GL.Vertex2(toint.X + toint.Width + one - stub, toint.Y);

							GL.Vertex2(toint.X, toint.Y + toint.Height + one);
							GL.Vertex2(toint.X, toint.Y + toint.Height + one - stub);
							GL.Vertex2(toint.X + toint.Width, toint.Y + toint.Height + one - stub);
							GL.Vertex2(toint.X + toint.Width, toint.Y + toint.Height + one);
							GL.End();
							GL.Color4(Color.White);

							GL.Color4(Color.Black);
							var shadow = toint;
							shadow.Offset((1.0f * GlobalSettings.DynamicOverlayTextSize) / _2DZoom, (1.0f * GlobalSettings.DynamicOverlayTextSize) / _2DZoom);

							DrawStringWithinRectangle(_font, shadow, mesh.Name + " " + Model.SideFromNormal(face.Normal),
														(6 * GlobalSettings.DynamicOverlayTextSize) / _2DZoom,
														(8.0f * GlobalSettings.DynamicOverlayTextSize) / _2DZoom);
							GL.Color4(GlobalSettings.DynamicOverlayTextColor);
							DrawStringWithinRectangle(_font, toint, mesh.Name + " " + Model.SideFromNormal(face.Normal),
														(6 * GlobalSettings.DynamicOverlayTextSize) / _2DZoom,
														(8.0f * GlobalSettings.DynamicOverlayTextSize) / _2DZoom);
							GL.Color4(Color.White);
						}
					}

					GL.PopMatrix();
				}
				else
				{
					_backgrounds[_selectedBackground].GLImage.Bind();

					GL.Begin(PrimitiveType.Quads);
					GL.TexCoord2(0, 0);
					GL.Vertex2(-(CurrentModel.DefaultWidth / 2), -(CurrentModel.DefaultHeight / 2));
					GL.TexCoord2(1, 0);
					GL.Vertex2((CurrentModel.DefaultWidth / 2), -(CurrentModel.DefaultHeight / 2));
					GL.TexCoord2(1, 1);
					GL.Vertex2((CurrentModel.DefaultWidth / 2), (CurrentModel.DefaultHeight / 2));
					GL.TexCoord2(0, 1);
					GL.Vertex2(-(CurrentModel.DefaultWidth / 2), (CurrentModel.DefaultHeight / 2));
					GL.End();
				}
			}

			GL.PopMatrix();

			GL.Disable(EnableCap.Blend);
		}

		private void DrawPlayer(Texture tex, Skin skin)
		{
			TextureGL.Unbind();

			Point clPt = Renderer.PointToClient(Cursor.Position);
			int x = clPt.X - (_currentViewport.Width / 2);
			int y = clPt.Y - (_currentViewport.Height / 2);

			if (GlobalSettings.Transparency == TransparencyMode.All)
				GL.Enable(EnableCap.Blend);
			else
				GL.Disable(EnableCap.Blend);

			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Front);

			if (grassToolStripMenuItem.Checked)
				DrawSkinnedRectangle(0, GrassY + 0.25f, 0, 1024, 0, 1024, 0, 0, 1024, 1024, _grassTop, 16, 16);

			GL.Disable(EnableCap.CullFace);

			// add meshes
			if (GlobalSettings.RenderBenchmark && IsRendering)
				_compileTimer.Start();

			if (CurrentModel != null)
			{
				int meshIndex = -1;
				foreach (Mesh mesh in CurrentModel.Meshes)
				{
					meshIndex++;
					bool meshVisible = CurrentModel.PartsEnabled[meshIndex];

					if (meshVisible == false && !GlobalSettings.Ghost)
						continue;

					if (!mesh.IsSolid)
						mesh.HasTransparency = _lastSkin.TransparentParts[meshIndex];

					mesh.Texture = tex;

					// Lazy Man Update!
					mesh.LastDrawTransparent = mesh.DrawTransparent;
					mesh.DrawTransparent = (meshVisible == false && GlobalSettings.Ghost);

					if (mesh.LastDrawTransparent != mesh.DrawTransparent)
						MeshRenderer.UpdateUserData(mesh);

					MeshRenderer.AddMesh(mesh);
				}
			}

			if (GlobalSettings.RenderBenchmark && IsRendering)
				_compileTimer.Stop();

			MeshRenderer.Render();
		}

		private void SetPreview()
		{
			if (_lastSkin == null)
			{
				using (var preview = new ColorGrabber(_previewPaint, 64, 32))
					preview.Save();
			}
			else
			{
				Skin skin = _lastSkin;

				using (var currentSkin = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
				{
					bool pick = GetPick(_mousePoint.X, _mousePoint.Y, out _pickPosition);

					currentSkin.Load();
					if (_selectedTool.Tool.RequestPreview(currentSkin, skin, _pickPosition.X, _pickPosition.Y))
					{
						currentSkin.Texture = _previewPaint;
						currentSkin.Save();
					}
					else
					{
						currentSkin.Texture = _previewPaint;
						currentSkin.Save();
					}
				}
			}
		}

		static bool prettyDarnCloseToZero(float v, float ep = float.Epsilon)
		{
			return (v > -ep && v < ep);
		}

		// adapted from http://java3d.sourcearchive.com/documentation/0.0.cvs.20090202.dfsg/Intersect_8java-source.html#l01099
		public static bool segmentAndPoly(Vector3[] coordinates,
									  Vector3 start, Vector3 end,
									  out float dist)
		{
			dist = 0;

			Vector3 vec0 = new Vector3(); // Edge vector from point 0 to point 1;
			Vector3 vec1 = new Vector3(); // Edge vector from point 0 to point 2 or 3;
			Vector3 pNrm = new Vector3();
			float absNrmX, absNrmY, absNrmZ, pD = 0.0f;
			Vector3 tempV3d = new Vector3();
			Vector3 direction = new Vector3();
			float pNrmDotrDir = 0.0f;
			int axis, nc, sh, nsh;

			Vector3 iPnt = new Vector3(); // Point of intersection.

			float[] uCoor = new float[4]; // Only need to support up to quad.
			float[] vCoor = new float[4];
			float tempD;

			int i, j;

			// Compute plane normal.
			for (i = 0; i < coordinates.Length - 1;)
			{
				vec0.X = coordinates[i + 1].X - coordinates[i].X;
				vec0.Y = coordinates[i + 1].Y - coordinates[i].Y;
				vec0.Z = coordinates[i + 1].Z - coordinates[i++].Z;
				if (vec0.Length > 0.0f)
					break;
			}

			for (j = i; j < coordinates.Length - 1; j++)
			{
				vec1.X = coordinates[j + 1].X - coordinates[j].X;
				vec1.Y = coordinates[j + 1].Y - coordinates[j].Y;
				vec1.Z = coordinates[j + 1].Z - coordinates[j].Z;
				if (vec1.Length > 0.0f)
					break;
			}

			if (j == (coordinates.Length - 1))
				return false;  // Degenerated polygon.

			Vector3.Cross(ref vec0, ref vec1, out pNrm);

			if (pNrm.Length == 0.0f)
				return false;  // Degenerated polygon.

			// Compute plane D.
			tempV3d = coordinates[0];
			Vector3.Dot(ref pNrm, ref tempV3d, out pD);

			direction.X = end.X - start.X;
			direction.Y = end.Y - start.Y;
			direction.Z = end.Z - start.Z;

			Vector3.Dot(ref pNrm, ref direction, out pNrmDotrDir);

			// Segment is parallel to plane. 
			if (pNrmDotrDir == 0.0f)
				return false;

			tempV3d = start;

			dist = (pD - Vector3.Dot(pNrm, tempV3d)) / pNrmDotrDir;

			// Segment intersects the plane behind the segment's start.
			// or exceed the segment's length.
			if ((dist < 0.0f) || (dist > 1.0f))
				return false;

			// Now, one thing for sure the segment intersect the plane.
			// Find the intersection point.
			iPnt.X = start.X + direction.X * dist;
			iPnt.Y = start.Y + direction.Y * dist;
			iPnt.Z = start.Z + direction.Z * dist;

			// System.out.println("dist " + dist[0] + " iPnt : " + iPnt);

			// Project 3d points onto 2d plane and apply Jordan curve theorem. 
			// Note : Area of polygon is not preserve in this projection, but
			// it doesn't matter here. 

			// Find the axis of projection.
			absNrmX = Math.Abs(pNrm.X);
			absNrmY = Math.Abs(pNrm.Y);
			absNrmZ = Math.Abs(pNrm.Z);

			if (absNrmX > absNrmY)
				axis = 0;
			else
				axis = 1;

			if (axis == 0)
			{
				if (absNrmX < absNrmZ)
					axis = 2;
			}
			else if (axis == 1)
			{
				if (absNrmY < absNrmZ)
					axis = 2;
			}

			for (i = 0; i < coordinates.Length; i++)
			{
				switch (axis)
				{
					case 0:
						uCoor[i] = coordinates[i].Y - iPnt.Y;
						vCoor[i] = coordinates[i].Z - iPnt.Z;
						break;
					case 1:
						uCoor[i] = coordinates[i].X - iPnt.X;
						vCoor[i] = coordinates[i].Z - iPnt.Z;
						break;
					case 2:
						uCoor[i] = coordinates[i].X - iPnt.X;
						vCoor[i] = coordinates[i].Y - iPnt.Y;
						break;
				}
			}

			// initialize number of crossing, nc.
			nc = 0;

			if (vCoor[0] < 0.0f)
				sh = -1;
			else
				sh = 1;

			for (i = 0; i < coordinates.Length; i++)
			{
				j = i + 1;
				if (j == coordinates.Length)
					j = 0;

				if (vCoor[j] < 0.0)
					nsh = -1;
				else
					nsh = 1;

				if (sh != nsh)
				{
					if ((uCoor[i] > 0.0f) && (uCoor[j] > 0.0f))
					{
						// This line must cross U+.
						nc++;
					}
					else if ((uCoor[i] > 0.0f) || (uCoor[j] > 0.0f))
					{
						// This line might cross U+. We need to compute intersection on U azis.
						tempD = uCoor[i] - vCoor[i] * (uCoor[j] - uCoor[i]) / (vCoor[j] - vCoor[i]);
						if (tempD > 0)
							// This line cross U+.
							nc++;
					}
					sh = nsh;
				}
			}

			if ((nc % 2) == 1)
			{
				// calculate the distance
				dist *= direction.Length;
				return true;
			}

			return false;
		}

		static bool rayTriangleIntersect(
			Vector3 orig, Vector3 dir,
			Vector3 v0, Vector3 v1, Vector3 v2,
			ref float t, ref float u, ref float v)
		{
			Vector3 v0v1 = v1 - v0;
			Vector3 v0v2 = v2 - v0;
			Vector3 pvec = Vector3.Cross(dir, v0v2);
			float det = Vector3.Dot(v0v1, pvec);
#if CULLING
			// if the determinant is negative the triangle is backfacing
			// if the determinant is close to 0, the ray misses the triangle
			if (det < 1e-8f) return false;
#else
			// ray and triangle are parallel if det is close to 0
			if (prettyDarnCloseToZero(det, 1e-8f)) return false;
#endif

			float invDet = 1 / det;

			Vector3 tvec = orig - v0;
			u = Vector3.Dot(tvec, pvec) * invDet;
			if (u < 0 || u > 1) return false;

			Vector3 qvec = Vector3.Cross(tvec, v0v1);
			v = Vector3.Dot(dir, qvec) * invDet;
			if (v < 0 || u + v > 1) return false;

			t = Vector3.Dot(v0v2, qvec) * invDet;

			return true;
		}

		private static bool pointAndPoly(Vector3[] coordinates, Vector3 point)
		{
			Vector3 vec0 = new Vector3(); // Edge vector from point 0 to point 1;
			Vector3 vec1 = new Vector3(); // Edge vector from point 0 to point 2 or 3;
			Vector3 pNrm = new Vector3();
			float absNrmX, absNrmY, absNrmZ, pD = 0.0f;
			Vector3 tempV3d = new Vector3();
			float pNrmDotrDir = 0.0f;

			float tempD;

			int i, j;

			// Compute plane normal.
			for (i = 0; i < coordinates.Length - 1;)
			{
				vec0.X = coordinates[i + 1].X - coordinates[i].X;
				vec0.Y = coordinates[i + 1].Y - coordinates[i].Y;
				vec0.Z = coordinates[i + 1].Z - coordinates[i++].Z;
				if (vec0.Length > 0.0)
					break;
			}

			for (j = i; j < coordinates.Length - 1; j++)
			{
				vec1.X = coordinates[j + 1].X - coordinates[j].X;
				vec1.Y = coordinates[j + 1].Y - coordinates[j].Y;
				vec1.Z = coordinates[j + 1].Z - coordinates[j].Z;
				if (vec1.Length > 0.0)
					break;
			}

			if (j == (coordinates.Length - 1))
			{
				// System.out.println("(1) Degenerated polygon.");
				return false;  // Degenerated polygon.
			}

			/* 
			   System.out.println("Ray orgin : " + ray.origin + " dir " + ray.direction);
			   System.out.println("Triangle/Quad :");
			   for(i=0; i<coordinates.length; i++) 
			   System.out.println("P" + i + " " + coordinates[i]);
			   */

			pNrm = Vector3.Cross(vec0, vec1);

			if (pNrm.Length == 0.0)
			{
				// System.out.println("(2) Degenerated polygon.");
				return false;  // Degenerated polygon.
			}
			// Compute plane D.
			tempV3d = coordinates[0];
			pD = Vector3.Dot(pNrm, tempV3d);

			if (prettyDarnCloseToZero(pD - Vector3.Dot(pNrm, point), 0.001f))
				return true;

			return false;

		}

		public bool Unproject(ref Matrix4 matrix, Rectangle viewport, Vector2 inPoint, out Vector3 outPoint, float z, float minDepth, float maxDepth)
		{
			if (viewport.X != 0)
				inPoint.X -= viewport.X;
			if (viewport.Y != 0)
				inPoint.Y -= viewport.Y;

			var mp = new Vector3(((inPoint.X - 0) / viewport.Width * 2.0f) - 1.0f,
									-(((inPoint.Y - 0) / viewport.Height * 2.0f) - 1.0f),
									(z - minDepth) / (maxDepth - minDepth));

			var invert = Matrix4.Invert(matrix);
			Vector3.Transform(ref mp, ref invert, out outPoint);

			float a = (((mp.X * invert.M14) + (mp.Y * invert.M24)) + (mp.Z * invert.M34)) + invert.M44;

			if (a <= float.Epsilon)
				return false;

			outPoint /= a;
			return true;
		}

		static float distValue(float min, float max, float v)
		{
			if (v < min || v > max)
				throw new Exception();

			return (v - min) / (max - min);
		}

		public bool GetPick(int x, int y, out Point hitPixel)
		{
			hitPixel = new Point(-1, -1);

			if (x < 0 || y < 0 || x >= Renderer.Width || y >= Renderer.Height)
				return false;

			var vp2d = GetViewport2D();
			var vp3d = GetViewport3D();
			var mousePos = new Vector2(x, y);

			if (_currentViewMode == ViewMode.Orthographic || (_currentViewMode == ViewMode.Hybrid && vp2d.Contains(x, y)))
			{
				Vector3 output;

				if (Unproject(ref _orthoCameraMatrix, vp2d, mousePos, out output, 0, -1, 1))
				{
					if (output.X < 0 || output.Y < 0 ||
						output.X >= CurrentModel.DefaultWidth ||
						output.Y >= CurrentModel.DefaultHeight)
						return false;

					hitPixel = new Point((int)Math.Floor(output.X), (int)Math.Floor(output.Y));
					return true;
				}

				return false;
			}
			else if (_currentViewMode == ViewMode.Perspective || (_currentViewMode == ViewMode.Hybrid && vp3d.Contains(x, y)))
			{
				Vector3 outputNear, outputFar;

				if (Unproject(ref _projectionMatrix, vp3d, mousePos, out outputNear, 8, 8, 512) &&
					Unproject(ref _projectionMatrix, vp3d, mousePos, out outputFar, 512, 8, 512))
				{
					float nearest = float.MaxValue;
					Face? hitFace = null;
					Vector3[] hitVerts = null;

					foreach (var mesh in CurrentModel.Meshes)
					{
						if (!CurrentModel.PartsEnabled[CurrentModel.Meshes.IndexOf(mesh)])
							continue;

						foreach (var face in mesh.Faces)
						{
							var verts = (Vector3[])face.Vertices.Clone();
							float dist;

							for (var i = 0; i < face.Vertices.Length; ++i)
								Vector3.Transform(ref verts[i], ref mesh.Matrix, out verts[i]);

							if (segmentAndPoly(verts, CameraPosition, outputFar, out dist))
							{
								if (hitFace == null || dist < nearest)
								{
									nearest = dist;
									hitFace = face;

									var dir = Vector3.Normalize(outputFar - CameraPosition);
									outputFar = CameraPosition + (dir * dist);
									hitVerts = verts;
								}
							}
						}
					}

					if (hitFace.HasValue)
					{
						var face = hitFace.Value;
						/*var tl = hitVerts[1];
						var br = hitVerts[3];

						var inside = outputFar - tl;
						br -= tl;

						var axis = 0;
						Vector2 texcoord;

						for (var i = 0; i < 3; ++i)
						{
							if (Math.Abs(br[i]) <= 0.001f)
								axis = i;
							else
								inside[i] /= br[i];
						}

						switch (axis)
						{
							case 2:
							default:
								texcoord = new Vector2(inside[0], inside[1]);
								break;
							case 0:
								texcoord = new Vector2(inside[2], inside[1]);
								break;
							case 1:
								texcoord = new Vector2(inside[0], inside[2]);
								break;
						}
							
						hitPixel = new Point(
							(int)Math.Floor((face.TexCoords[1].X + ((face.TexCoords[3].X - face.TexCoords[1].X) * texcoord.X)) * CurrentModel.DefaultWidth),
							(int)Math.Floor((face.TexCoords[1].Y + ((face.TexCoords[3].Y - face.TexCoords[1].Y) * texcoord.Y)) * CurrentModel.DefaultHeight));*/

						var tri0 = new int[] { face.Indices[0], face.Indices[1], face.Indices[2] };
						var tri1 = new int[] { face.Indices[0], face.Indices[2], face.Indices[3] };

						var orig = CameraPosition;
						var dir = outputFar - CameraPosition;

						float t = 0, u = 0, v = 0;
						int[] indicesHit;

						if (rayTriangleIntersect(orig, dir, hitVerts[tri0[0]], hitVerts[tri0[1]], hitVerts[tri0[2]], ref t, ref u, ref v))
						{
							Text = "tri0";
							indicesHit = tri0;
						}
						else if (rayTriangleIntersect(orig, dir, hitVerts[tri1[0]], hitVerts[tri1[1]], hitVerts[tri1[2]], ref t, ref u, ref v))
						{
							Text = "tri1";
							indicesHit = tri1;
						}
						else
							// how?
							return false;

						var st0 = face.TexCoords[indicesHit[0]];
						var st1 = face.TexCoords[indicesHit[1]];
						var st2 = face.TexCoords[indicesHit[2]];

						//var coord = u * tc0 + v * tc1 + (1 - u - v) * tc2;
						var coord = (1 - u - v) * st0 + u * st1 + v * st2;
						hitPixel = new Point((int)Math.Floor(coord.X * CurrentModel.DefaultWidth), (int)Math.Floor(coord.Y * CurrentModel.DefaultHeight));

						return true;
					}

					return false;
				}

				return false;
			}

			return false;
		}

		private void rendererControl_MouseWheel(object sender, MouseEventArgs e)
		{
			CheckMouse(e.Y);

			if (_currentViewMode == ViewMode.Perspective || (_currentViewMode == ViewMode.Hybrid && _mouseIn3D))
				_3DZoom += e.Delta / 50;
			else
				_2DZoom += e.Delta / 50;

			if (_2DZoom < 0.25f)
				_2DZoom = 0.25f;

			CalculateMatrices();
			Renderer.Invalidate();
		}

		public Rectangle GetViewport3D()
		{
			if (_currentViewMode == ViewMode.Perspective)
				return new Rectangle(0, 0, Renderer.Width, Renderer.Height);
			else
			{
				var halfHeight = (int)Math.Ceiling(Renderer.Height / 2.0f);
				return new Rectangle(0, halfHeight, Renderer.Width, halfHeight);
			}
		}

		public Rectangle GetViewport2D()
		{
			if (_currentViewMode == ViewMode.Orthographic)
				return new Rectangle(0, 0, Renderer.Width, Renderer.Height);
			else
			{
				var halfHeight = (int)Math.Ceiling(Renderer.Height / 2.0f);
				return new Rectangle(0, 0, Renderer.Width, halfHeight);
			}
		}

		public static bool IsRendering { get; private set; }

		//static uint frameCount = 0;
		static TimeSpan _compileSpan = TimeSpan.MinValue,
						_batchSpan = TimeSpan.MinValue,
						_sortSpan = TimeSpan.MinValue,
						_renderSpan = TimeSpan.MinValue;

		static void DrawCube(float width, float height, float depth, bool textured)
		{
			if (textured)
			{
				_cubeSides.Bind();
				GL.Color4((byte)255, (byte)255, (byte)255, (byte)(255 - 88));
			}
			else
				GL.Color4((byte)88, (byte)88, (byte)88, (byte)(255 - 88));

			GL.Begin(PrimitiveType.Quads);

			float xSep = 64.0f / 256.0f;
			float ySep = 64.0f / 128.0f;

			// front
			GL.TexCoord2(0, ySep); GL.Vertex3(-(width / 2), -(height / 2), -(depth / 2));
			GL.TexCoord2(xSep, ySep); GL.Vertex3((width / 2), -(height / 2), -(depth / 2));
			GL.TexCoord2(xSep, 0); GL.Vertex3((width / 2), (height / 2), -(depth / 2));
			GL.TexCoord2(0, 0); GL.Vertex3(-(width / 2), (height / 2), -(depth / 2));

			// back
			GL.TexCoord2(xSep, ySep); GL.Vertex3((width / 2), -(height / 2), (depth / 2));
			GL.TexCoord2(xSep * 2, ySep); GL.Vertex3(-(width / 2), -(height / 2), (depth / 2));
			GL.TexCoord2(xSep * 2, 0); GL.Vertex3(-(width / 2), (height / 2), (depth / 2));
			GL.TexCoord2(xSep, 0); GL.Vertex3((width / 2), (height / 2), (depth / 2));

			bool invertTopBottom = !(Math.Cos(MathHelper.DegreesToRadians(_3DRotationY)) < 0);

			// top
			if (invertTopBottom)
			{
				GL.TexCoord2(xSep * 2, ySep); GL.Vertex3(-(width / 2), (height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 3, ySep); GL.Vertex3((width / 2), (height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 3, 0); GL.Vertex3((width / 2), (height / 2), (depth / 2));
				GL.TexCoord2(xSep * 2, 0); GL.Vertex3(-(width / 2), (height / 2), (depth / 2));
			}
			else
			{
				GL.TexCoord2(xSep * 3, 0); GL.Vertex3(-(width / 2), (height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 2, 0); GL.Vertex3((width / 2), (height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 2, ySep); GL.Vertex3((width / 2), (height / 2), (depth / 2));
				GL.TexCoord2(xSep * 3, ySep); GL.Vertex3(-(width / 2), (height / 2), (depth / 2));
			}

			// bottom
			if (invertTopBottom)
			{
				GL.TexCoord2(xSep * 4, 0); GL.Vertex3((width / 2), -(height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 3, 0); GL.Vertex3(-(width / 2), -(height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 3, ySep); GL.Vertex3(-(width / 2), -(height / 2), (depth / 2));
				GL.TexCoord2(xSep * 4, ySep); GL.Vertex3((width / 2), -(height / 2), (depth / 2));
			}
			else
			{
				GL.TexCoord2(xSep * 3, ySep); GL.Vertex3((width / 2), -(height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 4, ySep); GL.Vertex3(-(width / 2), -(height / 2), -(depth / 2));
				GL.TexCoord2(xSep * 4, 0); GL.Vertex3(-(width / 2), -(height / 2), (depth / 2));
				GL.TexCoord2(xSep * 3, 0); GL.Vertex3((width / 2), -(height / 2), (depth / 2));
			}

			// left
			GL.TexCoord2(0, 1); GL.Vertex3(-(width / 2), -(height / 2), (depth / 2));
			GL.TexCoord2(xSep, 1); GL.Vertex3(-(width / 2), -(height / 2), -(depth / 2));
			GL.TexCoord2(xSep, ySep); GL.Vertex3(-(width / 2), (height / 2), -(depth / 2));
			GL.TexCoord2(0, ySep); GL.Vertex3(-(width / 2), (height / 2), (depth / 2));

			// right
			GL.TexCoord2(xSep * 2, ySep); GL.Vertex3((width / 2), (height / 2), (depth / 2));
			GL.TexCoord2(xSep, ySep); GL.Vertex3((width / 2), (height / 2), -(depth / 2));
			GL.TexCoord2(xSep, 1); GL.Vertex3((width / 2), -(height / 2), -(depth / 2));
			GL.TexCoord2(xSep * 2, 1); GL.Vertex3((width / 2), -(height / 2), (depth / 2));

			GL.End();

			TextureGL.Unbind();
		}

		private void rendererControl_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);

				IsRendering = true;
				if (GlobalSettings.RenderBenchmark)
					_renderTimer.Start();

				Renderer.MakeCurrent();

				_mousePoint = Renderer.PointToClient(MousePosition);

				SetPreview();

				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				Skin skin = _lastSkin;

				GL.PushMatrix();
				if (_currentViewMode == ViewMode.Perspective)
				{
					Setup3D(new Rectangle(0, 0, Renderer.Width, Renderer.Height));
					DrawPlayer(_previewPaint, skin);

					int sizeOfMiniport = 120;
					float sizeOfCube = sizeOfMiniport;

					GL.Clear(ClearBufferMask.DepthBufferBit);

					Setup2D(new Rectangle(Renderer.Width - sizeOfMiniport, Renderer.Height - sizeOfMiniport, sizeOfMiniport, sizeOfMiniport));
					GL.Enable(EnableCap.DepthTest);
					GL.Enable(EnableCap.CullFace);
					GL.Enable(EnableCap.Blend);
					GL.CullFace(CullFaceMode.Back);

					TextureGL.Unbind();

					GL.Translate(sizeOfCube / 2, sizeOfCube / 2, -8);
					GL.Rotate(-_3DRotationX, 1, 0, 0);
					GL.Rotate(-_3DRotationY, 0, 1, 0);
					DrawCube(sizeOfCube / 2, sizeOfCube / 2, sizeOfCube / 2, true);

					GL.Disable(EnableCap.DepthTest);
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
					DrawCube(sizeOfCube / 2, sizeOfCube / 2, sizeOfCube / 2, false);
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

					GL.Disable(EnableCap.Blend);
					GL.Disable(EnableCap.CullFace);
					GL.CullFace(CullFaceMode.Front);

				}
				else if (_currentViewMode == ViewMode.Orthographic)
				{
					Setup2D(new Rectangle(0, 0, Renderer.Width, Renderer.Height));
					DrawPlayer2D(_previewPaint, skin);
				}
				else
				{
					var halfHeight = (int)Math.Ceiling(Renderer.Height / 2.0f);

					Setup3D(new Rectangle(0, 0, Renderer.Width, halfHeight));
					DrawPlayer(_previewPaint, skin);

					Setup2D(new Rectangle(0, halfHeight, Renderer.Width, halfHeight));
					DrawPlayer2D(_previewPaint, skin);
				}

				GL.PopMatrix();
				_renderTimer.Stop();

				if (!_screenshotMode)
					Renderer.SwapBuffers();
				IsRendering = false;
			}
			catch (Exception ex)
			{
				Program.RaiseException(ex);
			}
		}

		Matrix4 _orthoMatrix, _orthoCameraMatrix, _projectionMatrix, _viewMatrix3d;

		private void CalculateMatrices()
		{
			Rectangle viewport = GetViewport3D();
			_projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), viewport.Width / (float)viewport.Height, 8, 512);

			viewport = GetViewport2D();
			_orthoMatrix = Matrix4.CreateOrthographicOffCenter(viewport.Left, viewport.Right, viewport.Bottom, viewport.Top, -1, 1);

			Bounds3 vec = Bounds3.EmptyBounds;
			Bounds3 allBounds = Bounds3.EmptyBounds;
			int count = 0;

			if (CurrentModel != null)
			{
				var viewMatrix =
					Matrix4.CreateTranslation(-(CurrentModel.DefaultWidth / 2), -(CurrentModel.DefaultHeight / 2), 0) *
					Matrix4.CreateTranslation((_2DCamOffsetX), (_2DCamOffsetY), 0) *
					Matrix4.CreateScale(_2DZoom, _2DZoom, 1) *
					Matrix4.CreateTranslation((viewport.Width / 2), (viewport.Height / 2), 0);

				_orthoCameraMatrix = viewMatrix * _orthoMatrix;

				int meshIndex = 0;
				foreach (Mesh mesh in CurrentModel.Meshes)
				{
					allBounds += mesh.Bounds;

					if (CurrentModel.PartsEnabled[meshIndex])
					{
						vec += mesh.Bounds;
						count++;
					}

					meshIndex++;
				}
			}

			if (count == 0)
				vec.Mins = vec.Maxs = allBounds.Mins = allBounds.Maxs = Vector3.Zero;

			GrassY = allBounds.Maxs.Y;
			Vector3 center = vec.Center;

			_viewMatrix3d =
				Matrix4.CreateTranslation(-center.X + _3DOffset.X, -center.Y + _3DOffset.Y, -center.Z + _3DOffset.Z) *
				Matrix4.CreateFromAxisAngle(new Vector3(0, -1, 0), MathHelper.DegreesToRadians(_3DRotationY)) *
				Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(_3DRotationX)) *
				Matrix4.CreateTranslation(0, 0, _3DZoom);

			_projectionMatrix = _viewMatrix3d * _projectionMatrix;

			var cameraMatrix = _viewMatrix3d;
			cameraMatrix.Invert();

			CameraPosition = Vector3.TransformPosition(Vector3.Zero, cameraMatrix);
		}

		private void Setup3D(Rectangle viewport)
		{
			_currentViewport = viewport;

			if (!_firstCalc)
			{
				CalculateMatrices();
				_firstCalc = true;
			}

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.Viewport(viewport);
			GL.MultMatrix(ref _projectionMatrix);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			//GL.LoadMatrix(ref _viewMatrix3d);

			GL.Enable(EnableCap.DepthTest);
		}

		private void Setup2D(Rectangle viewport)
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.Viewport(viewport);
			GL.Ortho(0, viewport.Width, viewport.Height, 0, -99, 99);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			_currentViewport = viewport;

			GL.Disable(EnableCap.DepthTest);
		}

		private void rendererControl_Resize(object sender, EventArgs e)
		{
			Renderer.MakeCurrent();

			CalculateMatrices();
			Renderer.Invalidate();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			_opening = !_opening;

			if (_opening)
			{
				splitContainer4.SplitterDistance = 74;
				toolStripButton1.Image = Resources.arrow_Up_16xLG;
			}
			else
			{
				splitContainer4.SplitterDistance = splitContainer4.Panel1MinSize;
				toolStripButton1.Image = Resources.arrow_Down_16xLG;
			}
		}

		public void RotateView(Point delta, float factor)
		{
			if (_currentViewMode == ViewMode.Perspective || (_currentViewMode == ViewMode.Hybrid && _mouseIn3D))
			{
				_3DRotationY += (delta.X * ToolScale) * factor;
				_3DRotationX += (delta.Y * ToolScale) * factor;
			}
			else
			{
				_2DCamOffsetX += delta.X / _2DZoom;
				_2DCamOffsetY += delta.Y / _2DZoom;
			}

			CalculateMatrices();
			Renderer.Invalidate();
		}

		public void ScaleView(Point delta, float factor)
		{
			if (_currentViewMode == ViewMode.Perspective || (_currentViewMode == ViewMode.Hybrid && _mouseIn3D))
			{
				_3DZoom += (-delta.Y * ToolScale) * factor;
			}
			else
			{
				_2DZoom += -delta.Y / 25.0f;

				if (_2DZoom < 0.25f)
					_2DZoom = 0.25f;
			}

			CalculateMatrices();
			Renderer.Invalidate();
		}

		private void CheckMouse(int y)
		{
			if (y > (Renderer.Height / 2))
				_mouseIn3D = true;
			else
				_mouseIn3D = false;
		}

		public void SetPartTransparencies()
		{
			using (var grabber = new ColorGrabber(GlobalDirtiness.CurrentSkin, _lastSkin.Width, _lastSkin.Height))
			{
				grabber.Load();

				var paintedParts = new Dictionary<int, bool>();

				foreach (var p in _paintedPixels)
				{
					List<int> parts = CurrentModel.GetIntersectingParts(p.Key, _lastSkin);

					foreach (int part in parts)
					{
						if (!paintedParts.ContainsKey(part))
							paintedParts.Add(part, true);
					}
				}

				foreach (var p in paintedParts)
					_lastSkin.CheckTransparentPart(grabber, p.Key);
			}

			_paintedPixels.Clear();
		}

		private void rendererControl_MouseDown(object sender, MouseEventArgs e)
		{
			Skin skin = _lastSkin;

			if (skin == null)
				return;

			CheckMouse(e.Y);

			_mousePoint = e.Location;
			_mouseIsDown = true;

			//_isValidPick = GetPick(e.X, e.Y, ref _pickPosition);

			using (var backup = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
			{
				backup.Load();

				try
				{
					if (e.Button == MouseButtons.Left)
					{
						if (_isValidPick)
							_selectedTool.Tool.BeginClick(_lastSkin, _pickPosition, e);
						else
							_selectedTool.Tool.BeginClick(_lastSkin, new Point(-1, -1), e);
						UseToolOnViewport(e.X, e.Y);
					}
					else
						_tools[(int)Tools.Camera].Tool.BeginClick(_lastSkin, Point.Empty, e);
				}
				catch
				{
					backup.Save();
					saveAllToolStripMenuItem_Click(null, null);
					throw;
				}
			}
		}

		private void rendererControl_MouseMove(object sender, MouseEventArgs e)
		{
			Skin skin = _lastSkin;

			if (skin == null)
				return;

			_isValidPick = GetPick(e.X, e.Y, out _pickPosition);

			using (var backup = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
			{
				backup.Load();

				try
				{
					if (_mouseIsDown)
					{
						if (e.Button == MouseButtons.Left)
						{
							_selectedTool.Tool.MouseMove(_lastSkin, e);
							UseToolOnViewport(e.X, e.Y);
						}
						else
							_tools[(int)Tools.Camera].Tool.MouseMove(_lastSkin, e);
					}

					_mousePoint = e.Location;
					Renderer.Invalidate();
				}
				catch
				{
					backup.Save();
					saveAllToolStripMenuItem_Click(null, null);
					throw;
				}
			}
		}

		private void rendererControl_MouseUp(object sender, MouseEventArgs e)
		{
			Skin skin = _lastSkin;

			if (skin == null)
				return;

			using (var backup = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
			{
				backup.Load();

				try
				{
					if (_mouseIsDown)
					{
						var currentSkin = new ColorGrabber();

						if (e.Button == MouseButtons.Left)
						{
							currentSkin = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height);
							currentSkin.Load();

							if (_selectedTool.Tool.EndClick(currentSkin, skin, e))
							{
								SetCanSave(true);
								skin.Dirty = true;
								treeView1.Invalidate();
								currentSkin.Save();
							}

							SetPartTransparencies();
						}
						else
							_tools[(int)Tools.Camera].Tool.EndClick(currentSkin, _lastSkin, e);
					}
				}
				catch
				{
					backup.Save();
					saveAllToolStripMenuItem_Click(null, null);
					throw;
				}
			}

			_mouseIsDown = false;
			treeView1.Invalidate();
		}

		public void CheckUndo()
		{
			if (_currentUndoBuffer != null)
			{
				undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = _currentUndoBuffer.CanUndo;
				redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = _currentUndoBuffer.CanRedo;
			}
			else
			{
				undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = false;
				redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = false;
			}
		}

		private void rendererControl_MouseLeave(object sender, EventArgs e)
		{
			_mousePoint = new Point(-1, -1);
		}

		private void VerifySelectionButtons()
		{
			changeNameToolStripMenuItem.Enabled =
				deleteToolStripMenuItem.Enabled = cloneToolStripMenuItem.Enabled = toolStrip2.CloneToolStripButton.Enabled = true;
			mDECRESToolStripMenuItem.Enabled = mINCRESToolStripMenuItem.Enabled = true;
			bROWSEIDToolStripMenuItem.Enabled = treeView1.SelectedNode != null;

			string folderLocation = GetFolderLocationForNode(treeView1.SelectedNode);
			bool canDoOperation = string.IsNullOrEmpty(folderLocation);

			toolStrip2.ImportToolStripButton.Enabled = !canDoOperation;
			toolStrip2.NewSkinToolStripButton.Enabled = !canDoOperation;
			toolStrip2.NewFolderToolStripButton.Enabled = !canDoOperation;
			toolStrip2.FetchToolStripButton.Enabled = !canDoOperation;

			importHereToolStripMenuItem.Enabled = !canDoOperation;
			newSkinToolStripMenuItem.Enabled = !canDoOperation;
			newFolderToolStripMenuItem.Enabled = !canDoOperation;
			mFETCHNAMEToolStripMenuItem.Enabled = !canDoOperation;

			bool itemSelected = treeView1.SelectedNode == null || (!HasOneRoot && treeView1.SelectedNode.Parent == null);

			toolStrip2.RenameToolStripButton.Enabled = !itemSelected;
			toolStrip2.DeleteToolStripButton.Enabled = !itemSelected;
			toolStrip2.DecResToolStripButton.Enabled = !itemSelected;
			toolStrip2.IncResToolStripButton.Enabled = !itemSelected;

			changeNameToolStripMenuItem.Enabled = !itemSelected;
			deleteToolStripMenuItem.Enabled = !itemSelected;
			mDECRESToolStripMenuItem.Enabled = !itemSelected;
			mINCRESToolStripMenuItem.Enabled = !itemSelected;

			if (treeView1.SelectedNode == null ||
				!(treeView1.SelectedNode is Skin))
			{
				cloneToolStripMenuItem.Enabled =
				toolStrip2.CloneToolStripButton.Enabled = false;
			}
			else if (treeView1.SelectedNode is Skin)
			{
				var skin = treeView1.SelectedNode as Skin;

				if (skin.Width <= 1 || skin.Height <= 1)
					mDECRESToolStripMenuItem.Enabled = false;
				//else if (skin.Width == 256 || skin.Height == 128)
				//	mINCRESToolStripMenuItem.Enabled = false;
			}
		}

		private Bitmap GenerateCheckBoxBitmap(CheckBoxState state)
		{
			var uncheckedImage = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			using (Graphics g = Graphics.FromImage(uncheckedImage))
			{
				Size glyphSize = CheckBoxRenderer.GetGlyphSize(g, state);
				CheckBoxRenderer.DrawCheckBox(g, new Point((16 / 2) - (glyphSize.Width / 2), (16 / 2) - (glyphSize.Height / 2)),
											  state);
			}

			return uncheckedImage;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeView1.SelectedNode == _lastSkin ||
				!(e.Node is Skin))
				return;

			Renderer.MakeCurrent();

			if (_lastSkin != null && treeView1.SelectedNode != _lastSkin)
			{
				// Copy over the current changes to the tex stored in the skin.
				// This allows us to pick up where we left off later, without undoing any work.
				_lastSkin.CommitChanges(GlobalDirtiness.CurrentSkin, false);

				// if we aren't dirty, unload
				if (!_lastSkin.Dirty)
					_lastSkin.Unload();
			}

			//if (_lastSkin != null)
			//	_lastSkin.Undo.Clear();

			var skin = (Skin)treeView1.SelectedNode;
			SetCanSave(skin.Dirty);

			if (skin.GLImage == null)
				skin.Create();

			if (skin == null)
			{
				_currentUndoBuffer = null;
				TextureGL.Unbind();

				using (var currentSkin = new ColorGrabber(GlobalDirtiness.CurrentSkin, 64, 32))
					currentSkin.Save();

				undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = false;
				redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = false;
			}
			else
			{
				using (var glImage = new ColorGrabber(skin.GLImage, skin.Width, skin.Height))
				{
					glImage.Load();

					glImage.Texture = GlobalDirtiness.CurrentSkin;
					glImage.Save();
					glImage.Texture = _previewPaint;
					glImage.Save();
				}

				_currentUndoBuffer = skin.Undo;
				CheckUndo();
			}

			_lastSkin = (Skin)treeView1.SelectedNode;

			SetModel(skin.Model);
			Renderer.Invalidate();

			VerifySelectionButtons();
			FillPartList();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void grassToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleGrass();
		}

		private void treeView1_MouseUp(object sender, MouseEventArgs e)
		{
			treeView1_MouseDown(sender, e);
			if (e.Button == MouseButtons.Right)
				contextMenuStrip1.Show(Cursor.Position);
		}

		private void treeView1_MouseDown(object sender, MouseEventArgs e)
		{
			_rightClickedNode = treeView1.GetSelectedNodeAt(e.Location);
			VerifySelectionButtons();
		}

		private void undoToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			PerformUndo();
		}

		private void perspectiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Perspective);
		}

		private void textureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Orthographic);
		}

		private void perspectiveToolStripButton_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Perspective);
		}

		private void orthographicToolStripButton_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Orthographic);
		}

		private void hybridToolStripButton_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Hybrid);
		}

		private void hybridViewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Hybrid);
		}

		private void offToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencyMode.Off);
		}

		private void helmetOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencyMode.Helmet);
		}

		private void allToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencyMode.All);
		}

		private void headToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.Head);
		}

		private void helmetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.Helmet);
		}

		private void chestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.Chest);
		}

		private void leftArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftArm);
		}

		private void rightArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightArm);
		}

		private void leftLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftLeg);
		}

		private void rightLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightLeg);
		}

		private void chestArmorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.ChestArmor);
		}

		private void leftArmArmorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftArmArmor);
		}

		private void rightArmArmorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightArmArmor);
		}

		private void leftLegArmorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftLegArmor);
		}

		private void rightLegArmorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightLegArmor);
		}

		private void toggleHeadToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.Head);
		}

		private void toggleHelmetToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.Helmet);
		}

		private void toggleChestToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.Chest);
		}

		private void toggleLeftArmToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftArm);
		}

		private void toggleRightArmToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightArm);
		}

		private void toggleLeftLegToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftLeg);
		}

		private void toggleRightLegToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightLeg);
		}

		private void toggleChestArmorToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.ChestArmor);
		}

		private void toggleLeftArmArmorToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftArmArmor);
		}

		private void toggleRightArmArmorToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightArmArmor);
		}

		private void toggleLeftLegArmorToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.LeftLegArmor);
		}

		private void toggleRightLegArmorToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ModelPart.RightLegArmor);
		}

		private void alphaCheckerboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleAlphaCheckerboard();
		}

		private void textureOverlayToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleOverlay();
		}

		public static void ShowUpdateDialog(IWin32Window owner)
		{
			if (MessageBox.Show(owner, "There is a new update available. Would you like to head to a download location?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
				Process.Start("http://www.planetminecraft.com/mod/mcskin3d/");
		}

		static void CheckForUpdates(IWin32Window owner)
		{
			if (Program.Context.Updater.CheckForUpdates())
				ShowUpdateDialog(owner);
			else
				MessageBox.Show(owner, "You have the latest version.");
		}

		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckForUpdates(this);
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformUndo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformRedo();
		}

		private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_shortcutEditor.ShowDialog();
		}

		private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var picker = new ColorPicker())
			{
				picker.CurrentColor = GlobalSettings.BackgroundColor;

				if (picker.ShowDialog() == DialogResult.OK)
				{
					GlobalSettings.BackgroundColor = picker.CurrentColor;

					Renderer.Invalidate();
				}
			}
		}

		private void screenshotToolStripButton_Click(object sender, EventArgs e)
		{
			if ((ModifierKeys & Keys.Shift) != 0)
				SaveScreenshot();
			else
				TakeScreenshot();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformSaveAs();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformSave();
		}

		private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformSaveAll();
		}

		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			PerformSave();
		}

		private void saveAlltoolStripButton_Click(object sender, EventArgs e)
		{
			PerformSaveAll();
		}

		private void changeNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformNameChange();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformDeleteSkin();
		}

		private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformCloneSkin();
		}

		private void automaticallyCheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GlobalSettings.AutoUpdate =
				automaticallyCheckForUpdatesToolStripMenuItem.Checked = !automaticallyCheckForUpdatesToolStripMenuItem.Checked;
		}

		private void labelEditTextBox_Leave(object sender, EventArgs e)
		{
			DoneEditingNode(labelEditTextBox.Text, _currentlyEditing);
		}

		private void labelEditTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
			{
				treeView1.Focus();
				e.Handled = true;
			}
		}

		private void labelEditTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' || e.KeyChar == '\n')
				e.Handled = true;
		}

		private void labelEditTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
				e.Handled = true;
		}

		private void importHereToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformImportSkin();
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			PerformNewFolder();
		}

		private void ghostHiddenPartsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleGhosting();
		}

		private void SetSampleMenuItem(int samples)
		{
			if (_antialiasOpts == null)
			{
				_antialiasOpts = new[]
								 {
									 xToolStripMenuItem4, xToolStripMenuItem, xToolStripMenuItem1, xToolStripMenuItem2,
									 xToolStripMenuItem3
								 };
			}

			int index = 0;

			switch (samples)
			{
				case 0:
				default:
					index = 0;
					break;
				case 1:
					index = 1;
					break;
				case 2:
					index = 2;
					break;
				case 4:
					index = 3;
					break;
				case 8:
					index = 4;
					break;
			}

			foreach (ToolStripMenuItem item in _antialiasOpts)
				item.Checked = false;

			GlobalSettings.Multisamples = samples;
			_antialiasOpts[index].Checked = true;
		}

		private void xToolStripMenuItem4_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(0);
			MessageBox.Show(this, GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(1);
			MessageBox.Show(this, GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(2);
			MessageBox.Show(this, GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(4);
			MessageBox.Show(this, GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem3_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(8);
			MessageBox.Show(this, GetLanguageString("B_MSG_ANTIALIAS"));
		}

		public static string GetLanguageString(string id)
		{
			if (!CurrentLanguage.StringTable.ContainsKey(id))
			{
#if BETA
				MessageBox.Show("Stringtable string not found: " + id);
#endif

				return id;
			}

			return CurrentLanguage.StringTable[id];
		}

		static void CreateModelDropdownItems(ToolStripItemCollection mainCollection, bool main, Action<Model> callback)
		{
			foreach (var x in ModelLoader.Models)
			{
				ToolStripItemCollection collection = mainCollection;
				string path = Path.GetDirectoryName(x.Value.File.ToString()).Substring(Environment.CurrentDirectory.Length + 7);

				while (!string.IsNullOrEmpty(path))
				{
					string sub = path.Substring(1, path.IndexOf('\\', 1) == -1 ? (path.Length - 1) : (path.IndexOf('\\', 1) - 1));

					ToolStripItem[] subMenu = collection.Find(sub, false);

					if (subMenu.Length == 0)
					{
						var item = ((ToolStripMenuItem)collection.Add(sub));
						item.Name = item.Text;
						collection = item.DropDownItems;
					}
					else
						collection = ((ToolStripMenuItem)subMenu[0]).DropDownItems;

					path = path.Remove(0, sub.Length + 1);
				}

				collection.Add(new ModelToolStripMenuItem(x.Value, main, callback));
			}
		}

		public void FinishedLoadingModels()
		{
			CreateModelDropdownItems(toolStripDropDownButton1.DropDownItems, true, (model) =>
			{
				SetModel(model);
			});
			CreateModelDropdownItems(toolStrip2.NewSkinToolStripButton.DropDownItems, false, (model) =>
			{
				PerformNewSkin(model);
			});

			toolStripDropDownButton1.Enabled = true;
			toolStripDropDownButton1.Text = "None";

			Renderer.MakeCurrent();

			// Compile model userdata
			foreach (var model in ModelLoader.Models)
			{
				foreach (var m in model.Value.Meshes)
				{
					m.UserData = MeshRenderer.CreateUserData(m);
					MeshRenderer.UpdateUserData(m);
				}
			}
		}

		public void BeginFinishedLoadingSkins(List<TreeNode> rootNodes)
		{
			treeView1.BeginUpdate();

			foreach (var root in rootNodes)
				treeView1.Nodes.Add(root);
		}

		public void FinishedLoadingSkins(List<Skin> skins, TreeNode selected)
		{
			treeView1.EndUpdate();
			treeView1.SelectedNode = selected;

			toolStrip2.Enabled = true;
			treeView1.Enabled = true;

			foreach (var s in skins)
			{
				if (s.File.ToString() == GlobalSettings.LastSkin)
				{
					s.IsLastSkin = true;
					_uploadedSkin = s;
					break;
				}
			}
		}

		private void languageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CurrentLanguage = (Language)((ToolStripMenuItem)sender).Tag;
		}

		private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			treeView1.ScrollPosition = new Point(hScrollBar1.Value, treeView1.ScrollPosition.Y);
		}

		private bool ShowDontAskAgain()
		{
			bool againValue = GlobalSettings.ResChangeDontShowAgain;
			bool ret = DontAskAgain.Show(CurrentLanguage, "M_IRREVERSIBLE", ref againValue);
			GlobalSettings.ResChangeDontShowAgain = againValue;

			return ret;
		}

		public void PerformDecreaseResolution()
		{
			if (!treeView1.Enabled)
				return;

			if (_lastSkin == null)
				return;
			if (_lastSkin.Width <= 1 || _lastSkin.Height <= 1)
				return;
			if (!ShowDontAskAgain())
				return;

			_lastSkin.Resize(_lastSkin.Width / 2, _lastSkin.Height / 2);

			using (var grabber = new ColorGrabber(_lastSkin.GLImage, _lastSkin.Width, _lastSkin.Height))
			{
				grabber.Load();
				grabber.Texture = GlobalDirtiness.CurrentSkin;
				grabber.Save();
				grabber.Texture = _previewPaint;
				grabber.Save();
			}
		}

		public void PerformIncreaseResolution()
		{
			if (!treeView1.Enabled)
				return;

			if (_lastSkin == null)
				return;
			if (!ShowDontAskAgain())
				return;

			_lastSkin.Resize(_lastSkin.Width * 2, _lastSkin.Height * 2);

			using (var grabber = new ColorGrabber(_lastSkin.GLImage, _lastSkin.Width, _lastSkin.Height))
			{
				grabber.Load();
				grabber.Texture = GlobalDirtiness.CurrentSkin;
				grabber.Save();
				grabber.Texture = _previewPaint;
				grabber.Save();
			}
		}

		public void PerformImportFromSite()
		{
			if (!treeView1.Enabled)
				return;

			string accountName = _importFromSite.Show();

			if (string.IsNullOrEmpty(accountName))
				return;

			string url = "http://s3.amazonaws.com/MinecraftSkins/" + accountName + ".png";

			string folderLocation;
			TreeNodeCollection collection;

			if (_rightClickedNode == null)
				_rightClickedNode = treeView1.SelectedNode;

			GetFolderLocationAndCollectionForNode(treeView1, _rightClickedNode, out folderLocation, out collection);

			string newSkinName = accountName;

			while (File.Exists(folderLocation + newSkinName + ".png"))
				newSkinName += " - " + GetLanguageString("C_NEW");

			try
			{
				byte[] pngData = WebHelpers.DownloadFile(url);

				using (FileStream file = File.Create(folderLocation + newSkinName + ".png"))
					file.Write(pngData, 0, pngData.Length);

				var skin = new Skin(folderLocation + newSkinName + ".png");
				collection.Add(skin);
				skin.SetImages();

				treeView1.Invalidate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, GetLanguageString("M_SKINERROR") + "\r\n" + ex);
				return;
			}
		}

		private void mDECRESToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformDecreaseResolution();
		}

		private void mINCRESToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformIncreaseResolution();
		}

		private void mFETCHNAMEToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformImportFromSite();
		}

		private void toolStripMenuItem4_Click(object sender, EventArgs e)
		{
			PerformNewSkin();
		}

		private void mSKINDIRSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_waitingForRestart)
			{
				MessageBox.Show(GetLanguageString("C_RESTART"), GetLanguageString("C_RESTART_CAPTION"), MessageBoxButtons.OK);
				return;
			}

			using (var dl = new DirectoryList())
			{
				dl.StartPosition = FormStartPosition.CenterParent;
				foreach (string dir in GlobalSettings.SkinDirectories.OrderBy(x => x))
					dl.Directories.Add(dir);

				if (dl.ShowDialog() == DialogResult.OK)
				{
					if (RecursiveNodeIsDirty(treeView1.Nodes))
					{
						DialogResult mb = MessageBox.Show(GetLanguageString("D_UNSAVED"), GetLanguageString("D_UNSAVED_CAPTION"),
														  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						switch (mb)
						{
							case DialogResult.No:
								_waitingForRestart = true;
								_newSkinDirs = dl.Directories.ToArray();
								return;
							case DialogResult.Cancel:
								return;
						}
					}

					GlobalSettings.SkinDirectories = dl.Directories.ToArray();
				}
			}
		}

		private void mLINESIZEToolStripMenuItem_NumericBox_ValueChanged(object sender, EventArgs e)
		{
			GlobalSettings.DynamicOverlayLineSize = (int)mLINESIZEToolStripMenuItem.NumericBox.Value;

			CalculateMatrices();
			Renderer.Invalidate();
		}

		private void mOVERLAYTEXTSIZEToolStripMenuItem_NumericBox_ValueChanged(object sender, EventArgs e)
		{
			GlobalSettings.DynamicOverlayTextSize = (int)mOVERLAYTEXTSIZEToolStripMenuItem.NumericBox.Value;

			CalculateMatrices();
			Renderer.Invalidate();
		}

		private void mGRIDOPACITYToolStripMenuItem_NumericBox_ValueChanged(object sender, EventArgs e)
		{
			GlobalSettings.DynamicOverlayGridColor = Color.FromArgb((int)mGRIDOPACITYToolStripMenuItem.NumericBox.Value,
																	GlobalSettings.DynamicOverlayGridColor);

			Renderer.Invalidate();
		}

		public void SetModel(Model Model)
		{
			if (_lastSkin == null)
				return;

			if (_oldModel != null &&
				_oldModel.Model == Model)
				return;

			if (_lastSkin.Model != Model)
			{
				var oldAspect = (float)_lastSkin.Width / (float)_lastSkin.Height;
				var newAspect = (float)Model.DefaultWidth / (float)Model.DefaultHeight;

				if (Math.Abs(oldAspect - newAspect) > 0.01f)
				{
					ResizeType resizeType;
					if ((resizeType = SkinSizeMismatch.Show(GetLanguageString("M_SKINSIZEMISMATCH"))) == ResizeType.None)
						return;

					_lastSkin.Model = Model;
					_lastSkin.Resize((int)Model.DefaultWidth, (int)Model.DefaultHeight, resizeType);

					using (var grabber = new ColorGrabber(_lastSkin.GLImage, _lastSkin.Width, _lastSkin.Height))
					{
						grabber.Load();
						grabber.Texture = GlobalDirtiness.CurrentSkin;
						grabber.Save();
						grabber.Texture = _previewPaint;
						grabber.Save();
					}
				}
				else
					_lastSkin.Model = Model;

				_lastSkin.Dirty = true;
				SetCanSave(true);
				CheckUndo();
			}

			if (_oldModel != null)
			{
				_oldModel.Checked = false;

				for (ToolStripItem parent = _oldModel.OwnerItem; parent != null; parent = parent.OwnerItem)
					parent.Image = null;
			}

			toolStripDropDownButton1.Text = _lastSkin.Model.Name;
			_oldModel = _lastSkin.Model.DropDownItem;
			_oldModel.Checked = true;

			_lastSkin.TransparentParts.Clear();
			_lastSkin.SetTransparentParts();
			FillPartList();

			for (ToolStripItem parent = _oldModel.OwnerItem; parent != null; parent = parent.OwnerItem)
				parent.Image = Resources.right_arrow_next;

			treeView1.Invalidate();

			CalculateMatrices();
			Renderer.Invalidate();
		}

		private void resetCameraToolStripButton_Click(object sender, EventArgs e)
		{
			PerformResetCamera();
		}

		private void mLINECOLORToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var picker = new ColorPicker())
			{
				picker.CurrentColor = GlobalSettings.DynamicOverlayLineColor;

				if (picker.ShowDialog() == DialogResult.OK)
				{
					GlobalSettings.DynamicOverlayLineColor = picker.CurrentColor;

					mLINECOLORToolStripMenuItem.BackColor = GlobalSettings.DynamicOverlayLineColor;

					CalculateMatrices();
					Renderer.Invalidate();
				}
			}
		}

		private void mTEXTCOLORToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var picker = new ColorPicker())
			{
				picker.CurrentColor = GlobalSettings.DynamicOverlayTextColor;

				if (picker.ShowDialog() == DialogResult.OK)
				{
					GlobalSettings.DynamicOverlayTextColor = picker.CurrentColor;

					mTEXTCOLORToolStripMenuItem.BackColor = GlobalSettings.DynamicOverlayTextColor;

					CalculateMatrices();
					Renderer.Invalidate();
				}
			}
		}

		private void mGRIDCOLORToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var picker = new ColorPicker())
			{
				picker.CurrentColor = GlobalSettings.DynamicOverlayGridColor;

				if (picker.ShowDialog() == DialogResult.OK)
				{
					GlobalSettings.DynamicOverlayGridColor = picker.CurrentColor;
					mGRIDCOLORToolStripMenuItem.BackColor = Color.FromArgb(255, GlobalSettings.DynamicOverlayGridColor);

					Renderer.Invalidate();
				}
			}
		}

		private void mINFINITEMOUSEToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mINFINITEMOUSEToolStripMenuItem.Checked = !mINFINITEMOUSEToolStripMenuItem.Checked;
			GlobalSettings.InfiniteMouse = mINFINITEMOUSEToolStripMenuItem.Checked;
		}

		private void PerformBrowseTo()
		{
			if (treeView1.SelectedNode == null)
				return;

			if (treeView1.SelectedNode is Skin)
				Process.Start("explorer.exe", "/select,\"" + ((Skin)treeView1.SelectedNode).File.FullName + "\"");
			else
				Process.Start("explorer.exe", ((FolderNode)treeView1.SelectedNode).Directory.FullName);
		}

		private void bROWSEIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformBrowseTo();
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			bROWSEIDToolStripMenuItem.Text = string.Format(GetLanguageString("M_BROWSE"),
														   (treeView1.SelectedNode is Skin)
															   ? GetLanguageString("M_SKIN")
															   : GetLanguageString("M_FOLDER"));
		}

		private void mRENDERSTATSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mRENDERSTATSToolStripMenuItem.Checked = !mRENDERSTATSToolStripMenuItem.Checked;
			GlobalSettings.RenderBenchmark = mRENDERSTATSToolStripMenuItem.Checked;
		}

		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;

			if (_popoutForm == null)
			{
				int oldWidth = Width;
				Width = helpToolStripMenuItem.Bounds.X + helpToolStripMenuItem.Bounds.Width + 18;

				// Move the items to a new form
				_popoutForm = new Form();
				_popoutForm.Height = Height;
				_popoutForm.Width = (oldWidth - Width) + 4;
				_popoutForm.Text = "Render Window";
				_popoutForm.Icon = Icon;
				_popoutForm.ControlBox = false;
				_popoutForm.Show();
				_popoutForm.Location = new Point(Location.X + Width, Location.Y);
				_popoutForm.KeyDown += popoutForm_KeyDown;
				_popoutForm.KeyPreview = true;
				_popoutForm.FormClosing += new FormClosingEventHandler(_popoutForm_FormClosing);

				splitContainer1.Panel2.Controls.Remove(panel4);
				_popoutForm.Controls.Add(panel4);
			}
			else
			{
				_popoutForm.Controls.Remove(panel4);
				splitContainer1.Panel2.Controls.Add(panel4);

				Width += _popoutForm.Width - 4;

				_popoutForm.Close();
				_popoutForm.Dispose();
				_popoutForm = null;
			}
		}

		void _popoutForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		#region Nested type: PartTreeNode

		private class PartTreeNode : TreeNode
		{
			public PartTreeNode(Model model, Mesh mesh, int index)
			{
				Name = Text = mesh.Name;
				Model = model;
				Mesh = mesh;
				PartIndex = index;

				if (PartIndex != -1)
					PartEnabled = model.PartsEnabled[index];
			}

			public PartTreeNode(Model model, Mesh mesh, int index, string name) :
				this(model, mesh, index)
			{
				Text = name;

				if (name.StartsWith("@"))
				{
					IsRadio = true;
					Text = name.Substring(1);
				}

				Name = name;
			}

			private Model Model { get; set; }
			private Mesh Mesh { get; set; }
			private int PartIndex { get; set; }
			public bool IsRadio { get; set; }

			public bool PartEnabled
			{
				get
				{
					if (PartIndex == -1)
					{
						bool hasEnabled = false, hasDisabled = false;

						foreach (PartTreeNode node in Nodes)
							RecursiveGroupImageCheck(node, ref hasEnabled, ref hasDisabled);

						return hasEnabled;
					}

					return Model.PartsEnabled[PartIndex];
				}

				set
				{
					Model.PartsEnabled[PartIndex] = value;
					SelectedImageIndex = ImageIndex = (value) ? IsRadio ? 10 : 3 : IsRadio ? 9 : 0;

					for (TreeNode node = Parent; node != null; node = node.Parent)
						((PartTreeNode)node).CheckGroupImage();
				}
			}

			private void RecursiveGroupImageCheck(PartTreeNode node, ref bool hasEnabled, ref bool hasDisabled)
			{
				if (hasEnabled && hasDisabled)
					return;

				if (node.Nodes.Count == 0)
				{
					if (node.PartEnabled)
						hasEnabled = true;
					else
						hasDisabled = true;
				}
				else
				{
					foreach (PartTreeNode n in node.Nodes)
						RecursiveGroupImageCheck(n, ref hasEnabled, ref hasDisabled);
				}
			}

			public PartTreeNode IsOtherRadioButtonEnabled()
			{
				TreeNodeCollection parentCollection;

				if (Parent != null)
					parentCollection = Parent.Nodes;
				else
					parentCollection = TreeView.Nodes;

				foreach (PartTreeNode node in parentCollection)
				{
					if (node == this)
						continue;
					if (node.IsRadio && node.PartEnabled)
						return node;
				}

				return null;
			}

			public void CheckGroupImage()
			{
				bool hasEnabled = false, hasDisabled = false;

				foreach (PartTreeNode node in Nodes)
					RecursiveGroupImageCheck(node, ref hasEnabled, ref hasDisabled);

				if (hasEnabled && hasDisabled)
					SelectedImageIndex = ImageIndex = 6;
				else if (hasEnabled)
					SelectedImageIndex = ImageIndex = IsRadio ? 10 : 3;
				else if (hasDisabled)
					SelectedImageIndex = ImageIndex = IsRadio ? 9 : 0;
			}

			public static void RecursiveAssign(PartTreeNode node, bool setAll, bool setTo = true)
			{
				foreach (PartTreeNode subNode in node.Nodes)
				{
					if (subNode.Nodes.Count != 0)
						RecursiveAssign(subNode, setAll, setTo);
					else
					{
						if (setAll)
							subNode.PartEnabled = setTo;
						else
							subNode.PartEnabled = !subNode.PartEnabled;
					}
				}

				node.CheckGroupImage();
			}

			public void TogglePart()
			{
				if (Nodes.Count != 0)
				{
					if (ImageIndex == 9)
					{
						RecursiveAssign(this, true);

						var other = IsOtherRadioButtonEnabled();

						if (other != null)
						{
							RecursiveAssign(other, true, false);
							other.CheckGroupImage();
						}

						CheckGroupImage();
					}
					else
					{
						bool setAll = ImageIndex == 6;
						RecursiveAssign(this, setAll);

						CheckGroupImage();
					}
				}
				else
					PartEnabled = !PartEnabled;
			}
		}

		#endregion

		private void CreatePartList()
		{
			_partItems = new[] { null, headToolStripMenuItem, helmetToolStripMenuItem, chestToolStripMenuItem, leftArmToolStripMenuItem, rightArmToolStripMenuItem, leftLegToolStripMenuItem, rightLegToolStripMenuItem, chestArmorToolStripMenuItem, leftArmArmorToolStripMenuItem, rightArmArmorToolStripMenuItem, leftLegArmorToolStripMenuItem, rightLegArmorToolStripMenuItem };
			_partButtons = new[] { null, toggleHeadToolStripButton, toggleHelmetToolStripButton, toggleChestToolStripButton, toggleLeftArmToolStripButton, toggleRightArmToolStripButton, toggleLeftLegToolStripButton, toggleRightLegToolStripButton, toggleChestArmorToolStripButton, toggleLeftArmArmorToolStripButton, toggleRightArmArmorToolStripButton, toggleLeftLegArmorToolStripButton, toggleRightLegArmorToolStripButton };

			var list = new ImageList();
			list.ColorDepth = ColorDepth.Depth32Bit;
			list.ImageSize = new Size(16, 16);
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.UncheckedNormal));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.UncheckedHot));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.UncheckedPressed));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.CheckedNormal));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.CheckedHot));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.CheckedPressed));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.MixedNormal));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.MixedHot));
			list.Images.Add(GenerateCheckBoxBitmap(CheckBoxState.MixedPressed));
			list.Images.Add(Resources.radio_unchecked);
			list.Images.Add(Resources.radio_checked);

			treeView2.ImageList = list;
		}

		private PartTreeNode CreateNodePath(TreeView treeView, Model m, Mesh part, string[] path, List<PartTreeNode> owners, List<PartTreeNode> radios)
		{
			PartTreeNode node = null;

			for (int i = 0; i < path.Length - 1; ++i)
			{
				if (node == null)
				{
					TreeNode[] nodes = treeView.Nodes.Find(path[i], false);

					if (nodes.Length == 0)
					{
						treeView.Nodes.Add(node = new PartTreeNode(CurrentModel, part, -1, path[i]));
						owners.Add(node);

						if (node.IsRadio)
							radios.Add(node);
					}
					else
						node = (PartTreeNode)nodes[0];
				}
				else
				{
					TreeNode[] nodes = node.Nodes.Find(path[i], false);

					if (nodes.Length == 0)
					{
						PartTreeNode old = node;
						old.Nodes.Add(node = new PartTreeNode(CurrentModel, part, -1, path[i]));
						owners.Add(node);

						if (node.IsRadio)
							radios.Add(node);
					}
					else
						node = (PartTreeNode)nodes[0];
				}
			}

			return node;
		}

		private void FillPartList()
		{
			var owners = new List<PartTreeNode>();
			var radios = new List<PartTreeNode>();

			treeView2.Nodes.Clear();

			int meshIndex = 0;
			foreach (Mesh part in CurrentModel.Meshes)
			{
				string name = part.Name;
				PartTreeNode node;

				if (name.Contains('.'))
				{
					string[] args = name.Split('.');
					PartTreeNode owner = CreateNodePath(treeView2, CurrentModel, part, args, owners, radios);

					owner.Nodes.Add(node = new PartTreeNode(CurrentModel, part, meshIndex, args[args.Length - 1]));
				}
				else
					treeView2.Nodes.Add(node = new PartTreeNode(CurrentModel, part, meshIndex));

				if (node.IsRadio)
					radios.Add(node);

				meshIndex++;
			}

			treeView2.Sort();

			radios.Reverse();

			foreach (PartTreeNode n in owners)
				n.CheckGroupImage();

			foreach (var r in radios)
			{
				var other = r.IsOtherRadioButtonEnabled();

				if (other != null)
				{
					PartTreeNode.RecursiveAssign(r, true, false);
					r.CheckGroupImage();
				}
			}

			for (int i = 1; i <= (int)ModelPart.RightLegArmor; ++i)
				CheckQuickPartState((ModelPart)i);
		}

		void CheckQuickPartState(ModelPart part)
		{
			var meshes = new List<Mesh>();
			var item = _partItems[(int)part];
			var button = _partButtons[(int)part];

			foreach (var m in CurrentModel.Meshes)
				if (m.Part == part)
					meshes.Add(m);

			if (meshes.Count == 0)
			{
				item.Enabled = button.Enabled = false;
				return;
			}

			item.Enabled = button.Enabled = true;
			item.Checked = button.Checked = true;

			foreach (var m in meshes)
			{
				if (CurrentModel.PartsEnabled[CurrentModel.Meshes.IndexOf(m)])
					continue;

				item.Checked = button.Checked = false;
				break;
			}
		}

		private void treeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			var nodeBounds = e.Node.Bounds;
			var pos = e.Location;
			var node = (PartTreeNode)e.Node;

			if (pos.X > nodeBounds.X - 18 && pos.X < nodeBounds.X - 4)
			{
				node.TogglePart();

				for (int i = 1; i <= (int)ModelPart.RightLegArmor; ++i)
					CheckQuickPartState((ModelPart)i);

				CalculateMatrices();
				Renderer.Invalidate();
			}
		}

		#region Update

		public void Invoke(Action action)
		{
			Invoke((Delegate)action);
		}

		#endregion

		#region Shortcuts

		private string CompileShortcutKeys()
		{
			string c = "";

			for (int i = 0; i < _shortcutEditor.ShortcutCount; ++i)
			{
				IShortcutImplementor shortcut = _shortcutEditor.ShortcutAt(i);

				if (i != 0)
					c += "|";

				Keys key = shortcut.Keys & ~Keys.Modifiers;
				var modifiers = (Keys)((int)shortcut.Keys - (int)key);

				if (modifiers != 0)
					c += shortcut.SaveName + "=" + key + "+" + modifiers;
				else
					c += shortcut.SaveName + "=" + key;
			}

			return c;
		}

		private IShortcutImplementor FindShortcut(string name)
		{
			foreach (IShortcutImplementor s in _shortcutEditor.Shortcuts)
			{
				if (s.SaveName == name)
					return s;
			}

			return null;
		}

		private void LoadShortcutKeys(string s)
		{
			if (string.IsNullOrEmpty(s))
				return; // leave defaults

			string[] shortcuts = s.Split('|');

			foreach (string shortcut in shortcuts)
			{
				string[] args = shortcut.Split('=');

				string name = args[0];
				string key;
				string modifiers = "0";

				if (args[1].Contains('+'))
				{
					string[] mods = args[1].Split('+');

					key = mods[0];
					modifiers = mods[1];
				}
				else
					key = args[1];

				IShortcutImplementor sh = FindShortcut(name);

				if (sh == null)
					continue;

				sh.Keys = (Keys)Enum.Parse(typeof(Keys), key) | (Keys)Enum.Parse(typeof(Keys), modifiers);
			}
		}

		private void InitMenuShortcut(ToolStripMenuItem item, Action callback)
		{
			var shortcut = new MenuStripShortcut(item);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		private void InitMenuShortcut(ToolStripMenuItem item, Keys keys, Action callback)
		{
			var shortcut = new MenuStripShortcut(item, keys);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		private void InitUnlinkedShortcut(string name, Keys defaultKeys, Action callback)
		{
			var shortcut = new ShortcutBase(name, defaultKeys);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		private void InitControlShortcut(string name, Control control, Keys defaultKeys, Action callback)
		{
			var shortcut = new ControlShortcut(name, defaultKeys, control);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		private void InitShortcuts()
		{
			// shortcut menus
			InitMenuShortcut(undoToolStripMenuItem, PerformUndo);
			InitMenuShortcut(redoToolStripMenuItem, PerformRedo);
			InitMenuShortcut(perspectiveToolStripMenuItem, () => SetViewMode(ViewMode.Perspective));
			InitMenuShortcut(textureToolStripMenuItem, () => SetViewMode(ViewMode.Orthographic));
			InitMenuShortcut(hybridViewToolStripMenuItem, () => SetViewMode(ViewMode.Hybrid));
			InitMenuShortcut(grassToolStripMenuItem, ToggleGrass);
			InitMenuShortcut(ghostHiddenPartsToolStripMenuItem, ToggleGhosting);
			InitMenuShortcut(alphaCheckerboardToolStripMenuItem, ToggleAlphaCheckerboard);
			InitMenuShortcut(textureOverlayToolStripMenuItem, ToggleOverlay);
			InitMenuShortcut(offToolStripMenuItem, () => SetTransparencyMode(TransparencyMode.Off));
			InitMenuShortcut(helmetOnlyToolStripMenuItem, () => SetTransparencyMode(TransparencyMode.Helmet));
			InitMenuShortcut(allToolStripMenuItem, () => SetTransparencyMode(TransparencyMode.All));
			InitMenuShortcut(headToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.Head));
			InitMenuShortcut(helmetToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.Helmet));
			InitMenuShortcut(chestToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.Chest));
			InitMenuShortcut(leftArmToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.LeftArm));
			InitMenuShortcut(rightArmToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.RightArm));
			InitMenuShortcut(leftLegToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.LeftLeg));
			InitMenuShortcut(rightLegToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.RightLeg));
			InitMenuShortcut(chestArmorToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.ChestArmor));
			InitMenuShortcut(leftArmArmorToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.LeftArmArmor));
			InitMenuShortcut(rightArmArmorToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.RightArmArmor));
			InitMenuShortcut(leftLegArmorToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.LeftLegArmor));
			InitMenuShortcut(rightLegArmorToolStripMenuItem,
							 () => ToggleVisiblePart(ModelPart.RightLegArmor));
			InitMenuShortcut(saveToolStripMenuItem, PerformSave);
			InitMenuShortcut(saveAsToolStripMenuItem, PerformSaveAs);
			InitMenuShortcut(saveAllToolStripMenuItem, PerformSaveAll);

			foreach (ToolIndex item in _tools)
				InitMenuShortcut(item.MenuItem, item.DefaultKeys, item.SetMeAsTool);

			// not in the menu
			InitUnlinkedShortcut("S_TOGGLETRANS", Keys.Shift | Keys.U, ToggleTransparencyMode);
			InitUnlinkedShortcut("S_TOGGLEVIEW", Keys.Control | Keys.V, ToggleViewMode);
			InitUnlinkedShortcut("S_SCREENSHOT_CLIP", Keys.Control | Keys.B, TakeScreenshot);
			InitUnlinkedShortcut("S_SCREENSHOT_SAVE", Keys.Control | Keys.Shift | Keys.B, SaveScreenshot);
			InitUnlinkedShortcut("S_DELETE", Keys.Delete, PerformDeleteSkin);
			InitUnlinkedShortcut("S_CLONE", Keys.Control | Keys.C, PerformCloneSkin);
			InitUnlinkedShortcut("S_RENAME", Keys.Control | Keys.R, PerformNameChange);
			InitUnlinkedShortcut("T_TREE_IMPORTHERE", Keys.Control | Keys.I, PerformImportSkin);
			InitUnlinkedShortcut("T_TREE_NEWFOLDER", Keys.Control | Keys.Shift | Keys.N, PerformNewFolder);
			InitUnlinkedShortcut("M_NEWSKIN_HERE", Keys.Control | Keys.Shift | Keys.M, () => PerformNewSkin(null));
			InitUnlinkedShortcut("S_COLORSWAP", Keys.S, PerformSwitchColor);
			InitControlShortcut("S_SWATCH_ZOOMIN", ColorPanel.SwatchContainer.SwatchDisplayer, Keys.Oemplus, PerformSwatchZoomIn);
			InitControlShortcut("S_SWATCH_ZOOMOUT", ColorPanel.SwatchContainer.SwatchDisplayer, Keys.OemMinus,
								PerformSwatchZoomOut);
			InitControlShortcut("S_TREEVIEW_ZOOMIN", treeView1, Keys.Control | Keys.Oemplus, PerformTreeViewZoomIn);
			InitControlShortcut("S_TREEVIEW_ZOOMOUT", treeView1, Keys.Control | Keys.OemMinus, PerformTreeViewZoomOut);
			InitUnlinkedShortcut("T_DECRES", Keys.Control | Keys.Shift | Keys.D, PerformDecreaseResolution);
			InitUnlinkedShortcut("T_INCRES", Keys.Control | Keys.Shift | Keys.I, PerformIncreaseResolution);
			InitUnlinkedShortcut("T_RESETCAMERA", Keys.Control | Keys.Shift | Keys.R, PerformResetCamera);

			InitUnlinkedShortcut("T_SWATCHEDIT", Keys.Shift | Keys.S, ColorPanel.SwatchContainer.ToggleEditMode);
			InitUnlinkedShortcut("T_ADDSWATCH", Keys.Shift | Keys.A, ColorPanel.SwatchContainer.PerformAddSwatch);
			InitUnlinkedShortcut("T_DELETESWATCH", Keys.Shift | Keys.R, ColorPanel.SwatchContainer.PerformRemoveSwatch);
		}

		private void PerformSwitchColor()
		{
			colorPanel.SwitchColors();
		}

		public void SetSelectedTool(ToolIndex index)
		{
			if (_selectedTool != null)
				_selectedTool.MenuItem.Checked = _selectedTool.Button.Checked = false;

			ToolIndex oldTool = _selectedTool;
			_selectedTool = index;
			index.MenuItem.Checked = index.Button.Checked = true;

			splitContainer4.Panel1.Controls.Clear();

			if (oldTool != null && oldTool.OptionsPanel != null)
				oldTool.OptionsPanel.BoxHidden();
			if (_selectedTool.OptionsPanel != null)
				_selectedTool.OptionsPanel.BoxShown();

			if (_selectedTool.OptionsPanel != null)
			{
				_selectedTool.OptionsPanel.Dock = DockStyle.Fill;
				splitContainer4.Panel1.Controls.Add(_selectedTool.OptionsPanel);
			}

			toolStripStatusLabel1.Text = index.Tool.GetStatusLabelText();
		}

		private void ToolMenuItemClicked(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			SetSelectedTool((ToolIndex)item.Tag);
		}

		public void PerformTreeViewZoomIn()
		{
			if (!treeView1.Enabled)
				return;

			treeView1.ZoomIn();
		}

		public void PerformTreeViewZoomOut()
		{
			if (!treeView1.Enabled)
				return;

			treeView1.ZoomOut();
		}

		private void PerformSwatchZoomOut()
		{
			colorPanel.SwatchContainer.ZoomOut();
		}

		private void PerformSwatchZoomIn()
		{
			colorPanel.SwatchContainer.ZoomIn();
		}

		public static bool PerformShortcut(Keys key, Keys modifiers)
		{
			foreach (IShortcutImplementor shortcut in _shortcutEditor.Shortcuts)
			{
				if (shortcut.CanEvaluate() && (shortcut.Keys & ~Keys.Modifiers) == key &&
					(shortcut.Keys & ~(shortcut.Keys & ~Keys.Modifiers)) == modifiers)
				{
					shortcut.Pressed();
					return true;
				}
			}

			return false;
		}

		#endregion

		#region Overrides

		private bool CheckKeyShortcut(KeyEventArgs e)
		{
			if (!colorPanel.HexTextBox.ContainsFocus &&
				!labelEditTextBox.ContainsFocus &&
				!colorPanel.SwatchContainer.SwatchRenameTextBoxHasFocus)
			{
				if (PerformShortcut(e.KeyCode & ~Keys.Modifiers, e.Modifiers))
					return true;
			}

			return false;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((e.Handled = CheckKeyShortcut(e)))
				return;

			base.OnKeyDown(e);
		}

		private void popoutForm_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = CheckKeyShortcut(e);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (RecursiveNodeIsDirty(treeView1.Nodes))
			{
				if (
					MessageBox.Show(this, GetLanguageString("C_UNSAVED"), GetLanguageString("C_UNSAVED_CAPTION"),
									MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}
			}

			GlobalSettings.ShortcutKeys = CompileShortcutKeys();

			colorPanel.SwatchContainer.SaveSwatches();

			if (_newSkinDirs != null)
				GlobalSettings.SkinDirectories = _newSkinDirs;

			base.OnFormClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			SetTransparencyMode(GlobalSettings.Transparency);
			SetViewMode(_currentViewMode);

			Renderer.MakeCurrent();

			toolToolStripMenuItem.DropDown.Closing += DontCloseMe;
			modeToolStripMenuItem.DropDown.Closing += DontCloseMe;
			threeDToolStripMenuItem.DropDown.Closing += DontCloseMe;
			twoDToolStripMenuItem.DropDown.Closing += DontCloseMe;
			transparencyModeToolStripMenuItem.DropDown.Closing += DontCloseMe;
			visiblePartsToolStripMenuItem.DropDown.Closing += DontCloseMe;
			mSHAREDToolStripMenuItem.DropDown.Closing += DontCloseMe;

			if (!Directory.Exists(GlobalSettings.GetDataURI("Models")) ||
				Environment.CurrentDirectory.StartsWith(Environment.ExpandEnvironmentVariables("%temp%")))
			{
				MessageBox.Show(GetLanguageString("M_TEMP"));
				Application.Exit();
			}
		}

		private void DontCloseMe(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked ||
				e.CloseReason == ToolStripDropDownCloseReason.Keyboard)
				e.Cancel = true;
		}

		#endregion

		#region Private Functions

		private readonly Dictionary<Point, bool> _paintedPixels = new Dictionary<Point, bool>();

		public Dictionary<Point, bool> PaintedPixels
		{
			get { return _paintedPixels; }
		}

		private void PixelWritten(Point p, ColorPixel c)
		{
			if (!_paintedPixels.ContainsKey(p))
				_paintedPixels.Add(p, true);
		}

		private void UseToolOnViewport(int x, int y, bool begin = false)
		{
			if (_lastSkin == null)
				return;

			if (_isValidPick)
			{
				Skin skin = _lastSkin;

				using (var currentSkin = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
				{
					currentSkin.Load();
					currentSkin.OnWrite = PixelWritten;

					if (_selectedTool.Tool.MouseMoveOnSkin(currentSkin, skin, _pickPosition.X, _pickPosition.Y))
					{
						SetCanSave(true);
						skin.Dirty = true;
						currentSkin.Save();
					}
				}
			}

			Renderer.Invalidate();
		}

		private void ToggleGrass()
		{
			grassToolStripMenuItem.Checked = !grassToolStripMenuItem.Checked;
			GlobalSettings.Grass = grassToolStripMenuItem.Checked;

			Renderer.Invalidate();
		}

		private void ToggleGhosting()
		{
			ghostHiddenPartsToolStripMenuItem.Checked = !ghostHiddenPartsToolStripMenuItem.Checked;
			GlobalSettings.Ghost = ghostHiddenPartsToolStripMenuItem.Checked;

			Renderer.Invalidate();
		}

		private void DoneEditingNode(string newName, TreeNode _currentlyEditing)
		{
			labelEditTextBox.Hide();

			if (_currentlyEditing is Skin)
			{
				var skin = (Skin)_currentlyEditing;

				if (skin.Name == newName)
					return;

				if (skin.ChangeName(newName) == false)
					SystemSounds.Beep.Play();
			}
			else
			{
				var folder = (FolderNode)_currentlyEditing;

				folder.MoveTo(GetFolderForNode(_currentlyEditing.Parent) + newName);
			}
		}

		private void BeginUndo()
		{
			Renderer.MakeCurrent();
		}

		private void DoUndo()
		{
			if (!_currentUndoBuffer.CanUndo)
				throw new Exception();

			_currentUndoBuffer.Undo();
		}

		private void EndUndo()
		{
			undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = _currentUndoBuffer.CanUndo;
			redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = _currentUndoBuffer.CanRedo;

			SetCanSave(_lastSkin.Dirty = true);

			Renderer.Invalidate();
		}

		private void PerformUndo()
		{
			if (!_currentUndoBuffer.CanUndo)
				return;

			BeginUndo();
			DoUndo();
			EndUndo();
		}

		private void UndoListBox_MouseClick(object sender, MouseEventArgs e)
		{
			undoToolStripButton.DropDown.Close();

			if (!_currentUndoBuffer.CanUndo)
				return;

			BeginUndo();
			for (int i = 0; i <= _undoListBox.ListBox.HighestItemSelected; ++i)
				DoUndo();
			EndUndo();
		}

		private void undoToolStripButton_DropDownOpening(object sender, EventArgs e)
		{
			_undoListBox.ListBox.Items.Clear();

			foreach (IUndoable x in _currentUndoBuffer.UndoList)
				_undoListBox.ListBox.Items.Insert(0, x.Action);
		}

		private void BeginRedo()
		{
			Renderer.MakeCurrent();
		}

		private void DoRedo()
		{
			if (!_currentUndoBuffer.CanRedo)
				throw new Exception();

			_currentUndoBuffer.Redo();
		}

		private void EndRedo()
		{
			SetCanSave(_lastSkin.Dirty = true);

			Renderer.Invalidate();
		}

		private void PerformRedo()
		{
			if (!_currentUndoBuffer.CanRedo)
				return;

			BeginRedo();
			DoRedo();
			EndRedo();
		}

		private void RedoListBox_MouseClick(object sender, MouseEventArgs e)
		{
			redoToolStripButton.DropDown.Close();

			BeginRedo();
			for (int i = 0; i <= _redoListBox.ListBox.HighestItemSelected; ++i)
				DoRedo();
			EndRedo();
		}

		private void redoToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			PerformRedo();
		}

		private void redoToolStripButton_DropDownOpening(object sender, EventArgs e)
		{
			_redoListBox.ListBox.Items.Clear();

			foreach (IUndoable x in _currentUndoBuffer.RedoList)
				_redoListBox.ListBox.Items.Insert(0, x.Action);
		}

		private void SetViewMode(ViewMode newMode)
		{
			perspectiveToolStripButton.Checked = orthographicToolStripButton.Checked = hybridToolStripButton.Checked = false;
			perspectiveToolStripMenuItem.Checked = textureToolStripMenuItem.Checked = hybridViewToolStripMenuItem.Checked = false;
			_currentViewMode = newMode;

			switch (_currentViewMode)
			{
				case ViewMode.Orthographic:
					orthographicToolStripButton.Checked = true;
					textureToolStripMenuItem.Checked = true;
					break;
				case ViewMode.Perspective:
					perspectiveToolStripButton.Checked = true;
					perspectiveToolStripMenuItem.Checked = true;
					break;
				case ViewMode.Hybrid:
					hybridToolStripButton.Checked = true;
					hybridViewToolStripMenuItem.Checked = true;
					break;
			}

			CalculateMatrices();
			Renderer.Invalidate();
		}

		private void SetTransparencyMode(TransparencyMode trans)
		{
			offToolStripMenuItem.Checked = helmetOnlyToolStripMenuItem.Checked = allToolStripMenuItem.Checked = false;
			GlobalSettings.Transparency = trans;

			switch (GlobalSettings.Transparency)
			{
				case TransparencyMode.Off:
					offToolStripMenuItem.Checked = true;
					break;
				case TransparencyMode.Helmet:
					helmetOnlyToolStripMenuItem.Checked = true;
					break;
				case TransparencyMode.All:
					allToolStripMenuItem.Checked = true;
					break;
			}

			Renderer.Invalidate();
		}

		private void ToggleVisiblePart(ModelPart part)
		{
			var meshes = new List<Mesh>();
			var item = _partItems[(int)part];
			var button = _partButtons[(int)part];

			foreach (var m in CurrentModel.Meshes)
				if (m.Part == part)
					meshes.Add(m);

			if (meshes.Count != 0)
			{
				item.Checked = button.Checked = !item.Checked;

				foreach (var m in meshes)
					CurrentModel.PartsEnabled[CurrentModel.Meshes.IndexOf(m)] = item.Checked;
			}

			CalculateMatrices();
			Renderer.Invalidate();
		}

		private void ToggleAlphaCheckerboard()
		{
			GlobalSettings.AlphaCheckerboard = !GlobalSettings.AlphaCheckerboard;
			alphaCheckerboardToolStripMenuItem.Checked = GlobalSettings.AlphaCheckerboard;
			Renderer.Invalidate();
		}

		private void ToggleOverlay()
		{
			GlobalSettings.TextureOverlay = !GlobalSettings.TextureOverlay;
			textureOverlayToolStripMenuItem.Checked = GlobalSettings.TextureOverlay;
			Renderer.Invalidate();
		}

		private void ToggleTransparencyMode()
		{
			switch (GlobalSettings.Transparency)
			{
				case TransparencyMode.Off:
					SetTransparencyMode(TransparencyMode.Helmet);
					break;
				case TransparencyMode.Helmet:
					SetTransparencyMode(TransparencyMode.All);
					break;
				case TransparencyMode.All:
					SetTransparencyMode(TransparencyMode.Off);
					break;
			}
		}

		private void ToggleViewMode()
		{
			switch (_currentViewMode)
			{
				case ViewMode.Orthographic:
					SetViewMode(ViewMode.Perspective);
					break;
				case ViewMode.Perspective:
					SetViewMode(ViewMode.Hybrid);
					break;
				case ViewMode.Hybrid:
					SetViewMode(ViewMode.Orthographic);
					break;
			}
		}

		#region Screenshots

		private Bitmap CopyScreenToBitmap()
		{
			Renderer.MakeCurrent();
			_screenshotMode = true;
			rendererControl_Paint(null, null);
			_screenshotMode = false;

			var b = new Bitmap(Renderer.Width, Renderer.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			var pixels = new int[Renderer.Width * Renderer.Height];
			GL.ReadPixels(0, 0, Renderer.Width, Renderer.Height, PixelFormat.Rgba,
						  PixelType.UnsignedByte, pixels);

			BitmapData locked = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite,
										   System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			unsafe
			{
				fixed (void* inPixels = pixels)
				{
					void* outPixels = locked.Scan0.ToPointer();

					var inInt = (int*)inPixels;
					var outInt = (int*)outPixels;

					for (int y = 0; y < b.Height; ++y)
					{
						for (int x = 0; x < b.Width; ++x)
						{
							Color color = Color.FromArgb((*inInt >> 24) & 0xFF, (*inInt >> 0) & 0xFF, (*inInt >> 8) & 0xFF,
														 (*inInt >> 16) & 0xFF);
							*outInt = color.ToArgb();

							inInt++;
							outInt++;
						}
					}
				}
			}

			b.UnlockBits(locked);
			b.RotateFlip(RotateFlipType.RotateNoneFlipY);

			return b;
		}

		private void TakeScreenshot()
		{
			Clipboard.SetImage(CopyScreenToBitmap());
		}

		private void SaveScreenshot()
		{
			using (var sfd = new SaveFileDialog())
			{
				sfd.Filter = "PNG Image|*.png";
				sfd.RestoreDirectory = true;

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					using (Bitmap bmp = CopyScreenToBitmap())
						bmp.Save(sfd.FileName);
				}
			}
		}

		#endregion

		#region Saving

		private void SetCanSave(bool value)
		{
			saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = value;
			CheckUndo();

			//treeView1.Invalidate();
		}

		private void PerformSaveAs()
		{
			Skin skin = _lastSkin;

			using (var grabber = new ColorGrabber(GlobalDirtiness.CurrentSkin, skin.Width, skin.Height))
			{
				grabber.Load();

				var b = new Bitmap(skin.Width, skin.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				BitmapData locked = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite,
											   System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				unsafe
				{
					void* inPixels = grabber.Array;
					void* outPixels = locked.Scan0.ToPointer();

					var inInt = (int*)inPixels;
					var outInt = (int*)outPixels;

					for (int y = 0; y < b.Height; ++y)
					{
						for (int x = 0; x < b.Width; ++x)
						{
							Color color = Color.FromArgb((*inInt >> 24) & 0xFF, (*inInt >> 0) & 0xFF, (*inInt >> 8) & 0xFF,
															(*inInt >> 16) & 0xFF);
							*outInt = color.ToArgb();

							inInt++;
							outInt++;
						}
					}
				}

				b.UnlockBits(locked);

				using (var sfd = new SaveFileDialog())
				{
					sfd.Filter = "Skin Image|*.png";
					sfd.RestoreDirectory = true;

					if (sfd.ShowDialog() == DialogResult.OK)
						b.Save(sfd.FileName);
				}

				b.Dispose();
			}
		}

		private void PerformSaveSkin(Skin s)
		{
			Renderer.MakeCurrent();

			s.CommitChanges((s == _lastSkin) ? GlobalDirtiness.CurrentSkin : s.GLImage, true);
		}

		private bool RecursiveNodeIsDirty(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (node is Skin)
				{
					var skin = (Skin)node;

					if (skin.Dirty)
						return true;
				}
				else if (RecursiveNodeIsDirty(node.Nodes))
					return true;
			}

			return false;
		}

		private void RecursiveNodeSave(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (node is Skin)
				{
					var skin = (Skin)node;

					if (skin.Dirty)
						PerformSaveSkin(skin);
				}
				else
					RecursiveNodeSave(node.Nodes);
			}
		}

		private void PerformSaveAll()
		{
			RecursiveNodeSave(treeView1.Nodes);
			treeView1.Invalidate();
		}

		private void PerformSave()
		{
			Skin skin = _lastSkin;

			if (skin == null || !skin.Dirty)
				return;

			SetCanSave(false);
			PerformSaveSkin(skin);
			treeView1.Invalidate();
		}

		#endregion

		#region Skin Management

		private TreeNode _currentlyEditing;

		private void ImportSkin(string fileName, string folderLocation, TreeNode parentNode)
		{
			string name = Path.GetFileNameWithoutExtension(fileName);

			while (File.Exists(folderLocation + name + ".png"))
				name += " - " + GetLanguageString("C_NEW");

			File.Copy(fileName, folderLocation + name + ".png");

			var skin = new Skin(folderLocation + name + ".png");

			if (parentNode != null)
			{
				if (!(parentNode is Skin))
					parentNode.Nodes.Add(skin);
				else
					parentNode.GetParentCollection().Add(skin);
			}
			else
				treeView1.Nodes.Add(skin);

			skin.SetImages();
			treeView1.SelectedNode = skin;
		}

		public static string GetFolderForNode(TreeNode node)
		{
			if (node == null)
				return RootFolderString;
			else if (node is Skin)
			{
				if (node.Parent == null)
				{
					if (HasOneRoot)
						return RootFolderString;

					throw new Exception();
				}
				else
					return GetFolderForNode(node.Parent);
			}
			else if (node is FolderNode)
			{
				var folder = (FolderNode)node;
				return folder.Directory.FullName;
			}

			throw new Exception();
		}

		public static string GetFolderLocationForNode(TreeNode node)
		{
			string folderLocation = "";

			if (node != null)
			{
				if (!(node is Skin))
					folderLocation = GetFolderForNode(node);
				else if (node.Parent != null)
					folderLocation = GetFolderForNode(node.Parent);
				else if (HasOneRoot)
					folderLocation = RootFolderString;
			}
			else if (HasOneRoot)
				folderLocation = RootFolderString;

			return folderLocation;
		}

		public static void GetFolderLocationAndCollectionForNode(TreeView treeView, TreeNode _rightClickedNode,
																 out string folderLocation, out TreeNodeCollection collection)
		{
			folderLocation = "";
			collection = treeView.Nodes;

			if (_rightClickedNode != null)
			{
				if (!(_rightClickedNode is Skin))
				{
					folderLocation = GetFolderForNode(_rightClickedNode);
					collection = _rightClickedNode.Nodes;
				}
				else if (_rightClickedNode.Parent != null)
				{
					folderLocation = GetFolderForNode(_rightClickedNode.Parent);
					collection = _rightClickedNode.Parent.Nodes;
				}
				else if (HasOneRoot)
					folderLocation = RootFolderString;
			}
			else if (HasOneRoot)
				folderLocation = RootFolderString;
		}

		private void ImportSkins(string[] fileName, TreeNode parentNode)
		{
			string folderLocation = GetFolderLocationForNode(parentNode);

			foreach (string f in fileName)
				ImportSkin(f, folderLocation, parentNode);
		}

		public void PerformImportSkin()
		{
			if (!treeView1.Enabled)
				return;

			if (_rightClickedNode == null)
				_rightClickedNode = treeView1.SelectedNode;

			string folderLocation = GetFolderLocationForNode(_rightClickedNode);

			if (string.IsNullOrEmpty(folderLocation))
				return;

			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Minecraft Skins|*.png";
				ofd.Multiselect = true;
				ofd.RestoreDirectory = true;

				if (ofd.ShowDialog() == DialogResult.OK)
					ImportSkins(ofd.FileNames, _rightClickedNode);
			}
		}

		public void PerformNewFolder()
		{
			if (!treeView1.Enabled)
				return;

			string folderLocation;
			TreeNodeCollection collection;

			if (_rightClickedNode == null || _rightClickedNode.Parent == null)
				_rightClickedNode = treeView1.SelectedNode;

			GetFolderLocationAndCollectionForNode(treeView1, _rightClickedNode, out folderLocation, out collection);

			if (collection == null || string.IsNullOrEmpty(folderLocation))
				return;

			string newFolderName = GetLanguageString("M_NEWFOLDER");

			while (Directory.Exists(folderLocation + newFolderName))
				newFolderName = newFolderName.Insert(0, GetLanguageString("C_NEW") + " ");

			Directory.CreateDirectory(folderLocation + newFolderName);

			var newNode = new FolderNode(newFolderName);
			collection.Add(newNode);

			newNode.EnsureVisible();
			treeView1.SelectedNode = newNode;
			treeView1.Invalidate();

			PerformNameChange();
		}

		void FillRectangleAlternating(Bitmap b, int x, int y, int w, int h)
		{
			for (var rx = 0; rx < w; ++rx)
				for (var ry = 0; ry < h; ++ry)
				{
					Color c;

					if (((rx + x + ry + y) & 1) == 1)
						c = Color.LightGray;
					else
						c = Color.DarkGray;

					b.SetPixel(rx + x, ry + y, c);
				}
		}

		public void PerformNewSkin(Model specificModel = null)
		{
			if (!treeView1.Enabled)
				return;

			string folderLocation;
			TreeNodeCollection collection;

			if (_rightClickedNode == null)
				_rightClickedNode = treeView1.SelectedNode;

			GetFolderLocationAndCollectionForNode(treeView1, _rightClickedNode, out folderLocation, out collection);

			if (collection == null || string.IsNullOrEmpty(folderLocation))
				return;
			if (specificModel == null)
				specificModel = CurrentModel;

			if (specificModel == null)
				specificModel = ModelLoader.Models["Players/Steve"];

			string newSkinName = GetLanguageString("M_NEWSKIN") + " " + specificModel.Name;

			while (File.Exists(folderLocation + newSkinName + ".png"))
				newSkinName = newSkinName.Insert(0, GetLanguageString("C_NEW") + " ");

			using (var bmp = new Bitmap((int)specificModel.DefaultWidth, (int)specificModel.DefaultHeight))
			{
				using (Graphics g = Graphics.FromImage(bmp))
				{
					g.Clear(Color.FromArgb(0, 255, 255, 255));

					if (GlobalSettings.UseTextureBases && !string.IsNullOrEmpty(specificModel.DefaultTexture))
					{
						using (var mp = new MemoryStream(Convert.FromBase64String(specificModel.DefaultTexture)))
						using (var bitmap = Bitmap.FromStream(mp))
							g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
					}
					else
					{
						foreach (var mesh in specificModel.Meshes)
						{
							foreach (var face in mesh.Faces)
							{
								var color = new ConvertableColor(ColorRepresentation.RGB, (face.Normal.X / 2) + 0.5f, (face.Normal.Y / 2) + 0.5f, (face.Normal.Z / 2) + 0.5f, !mesh.IsArmor ? 1.0f : 0.5f);

								var baseColor = color.ToColor();
								color.L += 0.1f;
								var lightColor = color.ToColor();

								if (mesh.IsArmor)
									lightColor = baseColor;

								Rectangle coords = face.TexCoordsToInteger(bmp.Width, bmp.Height);

								for (var y = coords.Top; y < coords.Bottom; ++y)
								{
									for (var x = coords.Left; x < coords.Right; ++x)
										bmp.SetPixel(x, y, ((x + y) % 2) == 0 ? baseColor : lightColor);
								}
							}
						}
					}
				}

				bmp.SaveSafe(folderLocation + newSkinName + ".png");

				var md = new Dictionary<string, string>();
				md.Add("Model", specificModel.Path);
				PNGMetadata.WriteMetadata(folderLocation + newSkinName + ".png", md);
			}

			var newSkin = new Skin(folderLocation + newSkinName + ".png");
			collection.Add(newSkin);
			newSkin.SetImages();

			newSkin.EnsureVisible();
			treeView1.SelectedNode = newSkin;
			treeView1.Invalidate();

			PerformNameChange();
		}

		private void RecursiveDeleteSkins(TreeNode node)
		{
			foreach (TreeNode sub in node.Nodes)
			{
				if (!(sub is Skin))
					RecursiveDeleteSkins(sub);
				else
				{
					var skin = (Skin)sub;

					if (_rightClickedNode == skin)
						_rightClickedNode = null;

					if (_lastSkin == skin)
						_lastSkin = null;

					skin.Dispose();
				}
			}

			//Directory.Delete(GetFolderForNode(node), true);
			FileSystem.DeleteDirectory(GetFolderForNode(node), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
		}

		public void PerformDeleteSkin()
		{
			if (!treeView1.Enabled)
				return;

			if (treeView1.SelectedNode is Skin)
			{
				if (
					MessageBox.Show(this, GetLanguageString("B_MSG_DELETESKIN"), GetLanguageString("B_CAP_QUESTION"),
									MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					var skin = (Skin)treeView1.SelectedNode;

					skin.Delete();
					skin.Remove();

					_lastSkin = null;

					if (_rightClickedNode == skin)
						_rightClickedNode = null;

					if (_lastSkin == skin)
						_lastSkin = null;

					skin.Dispose();

					treeView1_AfterSelect(treeView1, new TreeViewEventArgs(treeView1.SelectedNode));

					Invalidate();
				}
			}
			else
			{
				if (
					MessageBox.Show(this, GetLanguageString("B_MSG_DELETEFOLDER"), GetLanguageString("B_CAP_QUESTION"),
									MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					var folder = new DirectoryInfo(GetFolderForNode(treeView1.SelectedNode));

					RecursiveDeleteSkins(treeView1.SelectedNode);

					if (_lastSkin.GetNodeChain().Contains(treeView1.SelectedNode))
						_lastSkin = null;

					treeView1.SelectedNode.Remove();
					treeView1_AfterSelect(treeView1, new TreeViewEventArgs(treeView1.SelectedNode));
					Invalidate();
				}
			}
		}

		public void PerformCloneSkin()
		{
			if (!treeView1.Enabled)
				return;

			if (treeView1.SelectedNode == null ||
				!(treeView1.SelectedNode is Skin))
				return;

			var skin = (Skin)treeView1.SelectedNode;
			string newName = skin.Name;
			string newFileName;

			do
			{
				newName += " - " + GetLanguageString("C_COPY");
				newFileName = skin.Directory.FullName + newName + ".png";
			} while (File.Exists(newFileName));

			File.Copy(skin.File.FullName, newFileName);
			var newSkin = new Skin(newFileName);

			skin.GetParentCollection().Add(newSkin);

			newSkin.SetImages();
		}

		public void PerformNameChange()
		{
			if (!treeView1.Enabled)
				return;

			if (treeView1.SelectedNode != null)
			{
				_currentlyEditing = treeView1.SelectedNode;

				if (_currentlyEditing is Skin)
					labelEditTextBox.Text = ((Skin)_currentlyEditing).Name;
				else
					labelEditTextBox.Text = _currentlyEditing.Text;

				labelEditTextBox.Show();
				labelEditTextBox.Location =
					PointToClient(
						treeView1.PointToScreen(new Point(
													treeView1.SelectedNode.Bounds.Location.X + 22 + (treeView1.SelectedNode.Level * 1),
													treeView1.SelectedNode.Bounds.Location.Y + 2)));
				labelEditTextBox.Size = new Size(treeView1.Width - labelEditTextBox.Location.X - 20, labelEditTextBox.Height);
				labelEditTextBox.BringToFront();
				labelEditTextBox.Focus();
			}
		}

		#endregion

		public void FinishedLoadingLanguages()
		{
			foreach (Language lang in LanguageLoader.Languages)
			{
				lang.Item =
					new ToolStripMenuItem((lang.Culture != null)
											? (char.ToUpper(lang.Culture.NativeName[0]) + lang.Culture.NativeName.Substring(1))
											: lang.Name);
				lang.Item.Tag = lang;
				lang.Item.Click += languageToolStripMenuItem_Click;
				languageToolStripMenuItem.DropDownItems.Add(lang.Item);
			}
		}

		#endregion

		#region Constructor

		ToolStripMenuItem[] _partItems;
		ToolStripButton[] _partButtons;

		public Editor()
		{
			MainForm = this;
			InitializeComponent();

			KeyPreview = true;
			Text = Program.Name + " v" + Program.Version.ToString();

#if BETA
			Text += " [Beta]";
#endif
#if DEBUG
			Text += " [Debug]";
#endif

			// Settings
			mGRIDOPACITYToolStripMenuItem.NumericBox.Minimum = 0;
			mGRIDOPACITYToolStripMenuItem.NumericBox.Maximum = 255;
			mGRIDOPACITYToolStripMenuItem.NumericBox.Value = GlobalSettings.DynamicOverlayGridColor.A;
			mGRIDOPACITYToolStripMenuItem.NumericBox.ValueChanged += new System.EventHandler(mGRIDOPACITYToolStripMenuItem_NumericBox_ValueChanged);
			mOVERLAYTEXTSIZEToolStripMenuItem.NumericBox.Minimum = 1;
			mOVERLAYTEXTSIZEToolStripMenuItem.NumericBox.Maximum = 16;
			mOVERLAYTEXTSIZEToolStripMenuItem.NumericBox.Value = GlobalSettings.DynamicOverlayTextSize;
			mOVERLAYTEXTSIZEToolStripMenuItem.NumericBox.ValueChanged += new System.EventHandler(mOVERLAYTEXTSIZEToolStripMenuItem_NumericBox_ValueChanged);
			mLINESIZEToolStripMenuItem.NumericBox.Minimum = 1;
			mLINESIZEToolStripMenuItem.NumericBox.Maximum = 16;
			mLINESIZEToolStripMenuItem.NumericBox.Value = GlobalSettings.DynamicOverlayLineSize;
			mLINESIZEToolStripMenuItem.NumericBox.ValueChanged += new System.EventHandler(mLINESIZEToolStripMenuItem_NumericBox_ValueChanged);

			grassToolStripMenuItem.Checked = GlobalSettings.Grass;
			ghostHiddenPartsToolStripMenuItem.Checked = GlobalSettings.Ghost;

			alphaCheckerboardToolStripMenuItem.Checked = GlobalSettings.AlphaCheckerboard;
			textureOverlayToolStripMenuItem.Checked = GlobalSettings.TextureOverlay;
			automaticallyCheckForUpdatesToolStripMenuItem.Checked = GlobalSettings.AutoUpdate;

			SetSampleMenuItem(GlobalSettings.Multisamples);

			mLINECOLORToolStripMenuItem.BackColor = GlobalSettings.DynamicOverlayLineColor;
			mTEXTCOLORToolStripMenuItem.BackColor = GlobalSettings.DynamicOverlayTextColor;
			mGRIDCOLORToolStripMenuItem.BackColor = Color.FromArgb(255, GlobalSettings.DynamicOverlayGridColor);
			gridEnabledToolStripMenuItem.Checked = GlobalSettings.GridEnabled;
			useTextureBasesMenuItem.Checked = GlobalSettings.UseTextureBases;
			mINFINITEMOUSEToolStripMenuItem.Checked = GlobalSettings.InfiniteMouse;
			mRENDERSTATSToolStripMenuItem.Checked = GlobalSettings.RenderBenchmark;
			treeView1.ItemHeight = GlobalSettings.TreeViewHeight;
			treeView1.Scrollable = true;
			splitContainer4.SplitterDistance = 74;
		}

		public static string GLExtensions { get; private set; }
		public static string GLRenderer { get; private set; }
		public static string GLVendor { get; private set; }
		public static string GLVersion { get; private set; }

		private void Editor_Load(object sender, EventArgs e)
		{

		}

		public void Initialize(Language language)
		{
			// tools
			DodgeBurnOptions = new DodgeBurnOptions();
			DarkenLightenOptions = new DarkenLightenOptions();
			PencilOptions = new PencilOptions();
			FloodFillOptions = new FloodFillOptions();
			NoiseOptions = new NoiseOptions();
			EraserOptions = new EraserOptions();
			StampOptions = new StampOptions();

			_tools.Add(new ToolIndex(new CameraTool(), null, "T_TOOL_CAMERA", Resources.eye__1_, Keys.C));
			_tools.Add(new ToolIndex(new PencilTool(), PencilOptions, "T_TOOL_PENCIL", Resources.pen, Keys.P));
			_tools.Add(new ToolIndex(new EraserTool(), EraserOptions, "T_TOOL_ERASER", Resources.erase, Keys.E));
			_tools.Add(new ToolIndex(new DropperTool(), null, "T_TOOL_DROPPER", Resources.pipette, Keys.D));
			_tools.Add(new ToolIndex(new DodgeBurnTool(), DodgeBurnOptions, "T_TOOL_DODGEBURN", Resources.dodge, Keys.B));
			_tools.Add(new ToolIndex(new DarkenLightenTool(), DarkenLightenOptions, "T_TOOL_DARKENLIGHTEN", Resources.darkenlighten, Keys.L));
			_tools.Add(new ToolIndex(new FloodFillTool(), FloodFillOptions, "T_TOOL_BUCKET", Resources.fill_bucket, Keys.F));
			_tools.Add(new ToolIndex(new NoiseTool(), NoiseOptions, "T_TOOL_NOISE", Resources.noise, Keys.N));
			_tools.Add(new ToolIndex(new StampTool(), StampOptions, "T_TOOL_STAMP", Resources.stamp_pattern, Keys.M));

			for (int i = _tools.Count - 1; i >= 0; --i)
			{
				toolToolStripMenuItem.DropDownItems.Insert(0, _tools[i].MenuItem);
				_tools[i].MenuItem.Click += ToolMenuItemClicked;
				toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(toolStripSeparator1) + 1, _tools[i].Button);
				_tools[i].Button.Click += ToolMenuItemClicked;

				languageProvider1.SetPropertyNames(_tools[i].MenuItem, "Text");
				languageProvider1.SetPropertyNames(_tools[i].Button, "Text");
			}

			// Shortcuts
			InitShortcuts();
			LoadShortcutKeys(GlobalSettings.ShortcutKeys);
			_shortcutEditor.ShortcutExists += _shortcutEditor_ShortcutExists;

			Editor.CurrentLanguage = language;
			SetSelectedTool(_tools[0]);

			Brushes.LoadBrushes();

			foreach (string x in GlobalSettings.SkinDirectories)
				Directory.CreateDirectory(MacroHandler.ReplaceMacros(x));

			// set up the GL control
			var mode = new GraphicsMode();

			reset:

			Renderer =
				new GLControl(new GraphicsMode(mode.ColorFormat, mode.Depth, mode.Stencil, GlobalSettings.Multisamples));

			if (Renderer.Context == null)
			{
				mode = new GraphicsMode();
				goto reset;
			}

			Renderer.BackColor = Color.Black;
			Renderer.Dock = DockStyle.Fill;
			Renderer.Location = new Point(0, 25);
			Renderer.Name = "rendererControl";
			Renderer.Size = new Size(641, 580);
			Renderer.TabIndex = 4;

			splitContainer4.Panel2.Controls.Add(Renderer);
			Renderer.BringToFront();

			GLVendor = GL.GetString(StringName.Vendor);
			GLVersion = GL.GetString(StringName.Version);
			GLRenderer = GL.GetString(StringName.Renderer);
			GLExtensions = GL.GetString(StringName.Extensions);

			InitGL();

			Renderer.Paint += rendererControl_Paint;
			Renderer.MouseDown += rendererControl_MouseDown;
			Renderer.MouseMove += rendererControl_MouseMove;
			Renderer.MouseUp += rendererControl_MouseUp;
			Renderer.MouseLeave += rendererControl_MouseLeave;
			Renderer.Resize += rendererControl_Resize;
			Renderer.MouseWheel += rendererControl_MouseWheel;
			Renderer.MouseEnter += rendererControl_MouseEnter;

#if NO
			if (!GlobalSettings.Loaded)
				MessageBox.Show(this, GetLanguageString("C_SETTINGSFAILED"));
#endif

			_undoListBox = new UndoRedoPanel();
			_undoListBox.ActionString = "L_UNDOACTIONS";
			languageProvider1.SetPropertyNames(_undoListBox, "ActionString");

			_undoListBox.ListBox.MouseClick += UndoListBox_MouseClick;

			undoToolStripButton.DropDown = new Popup(_undoListBox);
			undoToolStripButton.DropDownOpening += undoToolStripButton_DropDownOpening;

			_redoListBox = new UndoRedoPanel();
			_redoListBox.ActionString = "L_REDOACTIONS";
			languageProvider1.SetPropertyNames(_redoListBox, "ActionString");

			_redoListBox.ListBox.MouseClick += RedoListBox_MouseClick;

			redoToolStripButton.DropDown = new Popup(_redoListBox);
			redoToolStripButton.DropDownOpening += redoToolStripButton_DropDownOpening;

			undoToolStripButton.DropDown.AutoClose = redoToolStripButton.DropDown.AutoClose = true;

			CreatePartList();
			Renderer.Invalidate();
		}

		private void rendererControl_MouseEnter(object sender, EventArgs e)
		{
			Renderer.Focus();
		}

		private void _shortcutEditor_ShortcutExists(object sender, ShortcutExistsEventArgs e)
		{
			//MessageBox.Show(string.Format(GetLanguageString("B_MSG_SHORTCUTEXISTS"), e.ShortcutName, e.OtherName));
		}

		#endregion

		private void gridEnabledToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GlobalSettings.GridEnabled = !GlobalSettings.GridEnabled;
			gridEnabledToolStripMenuItem.Checked = GlobalSettings.GridEnabled;

			Renderer.Invalidate();
		}

		private void useTextureBasesMenuItem_Click(object sender, EventArgs e)
		{
			GlobalSettings.UseTextureBases = !GlobalSettings.UseTextureBases;
			useTextureBasesMenuItem.Checked = GlobalSettings.UseTextureBases;
		}

		private void officialMinecraftForumsThreadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.minecraftforum.net/topic/746941-mcskin3d-new-skinning-program/");
		}

		private void planetMinecraftSubmissionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.planetminecraft.com/mod/mcskin3d/");
		}

		private void toolStripMenuItem4_Click_1(object sender, EventArgs e)
		{
			Dictionary<Vector3, int> vertDict = new Dictionary<Vector3, int>();
			Dictionary<Vector2, int> texCoordDict = new Dictionary<Vector2, int>();

			foreach (var m in CurrentModel.Meshes)
			{
				foreach (var f in m.Faces)
				{
					if (f.Indices.Length != 4)
					{
						Debugger.Break();
						continue;
					}

					for (var i = 0; i < 4; ++i)
					{
						var fv = Vector3.Transform(f.Vertices[i], m.Matrix);
						var ft = f.TexCoords[i];

						if (!vertDict.ContainsKey(fv))
							vertDict.Add(fv, vertDict.Count);
						if (!texCoordDict.ContainsKey(ft))
							texCoordDict.Add(ft, texCoordDict.Count);
					}
				}
			}

			string s = "";

			foreach (var v in vertDict)
				s += String.Format("v {0} {1} {2}\r\n", v.Key.X, -v.Key.Y, v.Key.Z);

			foreach (var v in texCoordDict)
				s += String.Format("vt {0} {1}\r\n", v.Key.X, 1 - v.Key.Y);

			foreach (var m in CurrentModel.Meshes)
			{
				s += String.Format("g {0}\r\n", m.Name);
				s += "s 1\r\n";

				foreach (var f in m.Faces)
				{
					if (f.Indices.Length != 4)
					{
						Debugger.Break();
						continue;
					}

					int[] vi = new int[4];
					int[] vt = new int[4];

					for (var i = 0; i < 4; ++i)
					{
						vi[i] = vertDict[Vector3.Transform(f.Vertices[i], m.Matrix)] + 1;
						vt[i] = texCoordDict[f.TexCoords[i]] + 1;
					}

					s += String.Format("f {0}/{1} {2}/{3} {4}/{5} {6}/{7}\r\n", vi[0], vt[0], vi[1], vt[1], vi[2], vt[2], vi[3], vt[3]);
				}
			}

			Clipboard.SetText(s);
		}
	}
}