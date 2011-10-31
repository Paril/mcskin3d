using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using DevCIL;
using Devcorp.Controls.Design;
using MB.Controls;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Security.Cryptography;
using Paril.Settings;
using Paril.Settings.Serializers;
using Paril.Components;
using Paril.Components.Shortcuts;

namespace MCSkin3D
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			System.Timers.Timer animTimer = new System.Timers.Timer();
			animTimer.Interval = 22;
			animTimer.Elapsed += new System.Timers.ElapsedEventHandler(animTimer_Elapsed);
			animTimer.Start();

			GlobalSettings.Load();

			glControl1.MouseWheel += new MouseEventHandler(glControl1_MouseWheel);

			animateToolStripMenuItem.Checked = GlobalSettings.Animate;
			followCursorToolStripMenuItem.Checked = GlobalSettings.FollowCursor;
			grassToolStripMenuItem.Checked = GlobalSettings.Grass;

			alphaCheckerboardToolStripMenuItem.Checked = GlobalSettings.AlphaCheckerboard;
			textureOverlayToolStripMenuItem.Checked = GlobalSettings.TextureOverlay;

			SetCheckbox(VisiblePartFlags.HeadFlag, headToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.ChestFlag, chestToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.LeftArmFlag, leftArmToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.RightArmFlag, rightArmToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.HelmetFlag, helmetToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.LeftLegFlag, leftLegToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.RightLegFlag, rightLegToolStripMenuItem);

			if (Screen.PrimaryScreen.BitsPerPixel != 32)
			{
				MessageBox.Show("Sorry, but apparently your video mode doesn't support a 32-bit pixel format - this is required, at the moment, for proper functionality of MCSkin3D. 16-bit support will be implemented at a later date, if it is asked for.", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}

			menuStrip1.Renderer = new Szotar.WindowsForms.ToolStripAeroRenderer(Szotar.WindowsForms.ToolbarTheme.Toolbar);
			toolStrip1.Renderer = new Szotar.WindowsForms.ToolStripAeroRenderer(Szotar.WindowsForms.ToolbarTheme.Toolbar);

			redColorSlider.Renderer = redRenderer = new ColorSliderRenderer(redColorSlider);
			greenColorSlider.Renderer = greenRenderer = new ColorSliderRenderer(greenColorSlider);
			blueColorSlider.Renderer = blueRenderer = new ColorSliderRenderer(blueColorSlider);
			alphaColorSlider.Renderer = alphaRenderer = new ColorSliderRenderer(alphaColorSlider);

			KeyPreview = true;
			Text = "MCSkin3D v" + ProductVersion[0] + '.' + ProductVersion[2];

			swatchContainer1.AddDirectory("Swatches");
		}

		void SetCheckbox(VisiblePartFlags flag, ToolStripMenuItem checkbox)
		{
			if ((GlobalSettings.ViewFlags & flag) != 0)
				checkbox.Checked = true;
			else
				checkbox.Checked = false;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			GlobalSettings.ShortcutKeys = CompileShortcutKeys();

			GlobalSettings.Save();
		}

		ColorSliderRenderer redRenderer, greenRenderer, blueRenderer, alphaRenderer;

		static ShortcutEditor _shortcutEditor = new ShortcutEditor();

		string CompileShortcutKeys()
		{
			string c = "";

			for (int i = 0; i < _shortcutEditor.ShortcutCount; ++i)
			{
				var shortcut = _shortcutEditor.ShortcutAt(i);

				if (i != 0)
					c += "|";

				Keys key = shortcut.Keys & ~Keys.Modifiers;
				Keys modifiers = (Keys)((int)shortcut.Keys - (int)key);

				if (modifiers != 0)
					c += shortcut.Name + "=" + key + "+" + modifiers;
				else
					c += shortcut.Name + "=" + key;
			}

			return c;
		}

		IShortcutImplementor FindShortcut(string name)
		{
			foreach (var s in _shortcutEditor.Shortcuts)
			{
				if (s.Name == name)
					return s;
			}

			return null;
		}

		void LoadShortcutKeys(string s)
		{
			if (string.IsNullOrEmpty(s))
				return; // leave defaults

			var shortcuts = s.Split('|');

			foreach (var shortcut in shortcuts)
			{
				var args = shortcut.Split('=');

				string name = args[0];
				string key;
				string modifiers = "0";

				if (args[1].Contains('+'))
				{
					var mods = args[1].Split('+');

					key = mods[0];
					modifiers = mods[1];
				}
				else
					key = args[1];

				var sh = FindShortcut(name);

				if (sh == null)
					continue;

				sh.Keys = (Keys)Enum.Parse(typeof(Keys), key) | (Keys)Enum.Parse(typeof(Keys), modifiers);
			}
		}

		void InitMenuShortcut(ToolStripMenuItem item, Action callback)
		{
			MenuStripShortcut shortcut = new MenuStripShortcut(item);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		void InitUnlinkedShortcut(string name, Keys defaultKeys, Action callback)
		{
			ShortcutBase shortcut = new ShortcutBase(name, defaultKeys);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		void InitShortcuts()
		{
			// shortcut menus
			InitMenuShortcut(undoToolStripMenuItem, PerformUndo);
			InitMenuShortcut(redoToolStripMenuItem, PerformRedo);
			InitMenuShortcut(cameraToolStripMenuItem, () => SetTool(Tools.Camera));
			InitMenuShortcut(pencilToolStripMenuItem, () => SetTool(Tools.Pencil));
			InitMenuShortcut(dropperToolStripMenuItem, () => SetTool(Tools.Dropper));
			InitMenuShortcut(eraserToolStripMenuItem, () => SetTool(Tools.Eraser));
			InitMenuShortcut(dodgeToolStripMenuItem, () => SetTool(Tools.Dodge));
			InitMenuShortcut(burnToolStripMenuItem, () => SetTool(Tools.Burn));
			InitMenuShortcut(addNewSkinToolStripMenuItem, PerformImportSkin);
			InitMenuShortcut(deleteSelectedSkinToolStripMenuItem, PerformDeleteSkin);
			InitMenuShortcut(cloneSkinToolStripMenuItem, PerformCloneSkin);
			InitMenuShortcut(dPerspectiveToolStripMenuItem, () => SetViewMode(ViewMode.Perspective));
			InitMenuShortcut(dTextureToolStripMenuItem, () => SetViewMode(ViewMode.Orthographic));
			InitMenuShortcut(animateToolStripMenuItem, ToggleAnimation);
			InitMenuShortcut(followCursorToolStripMenuItem, ToggleFollowCursor);
			InitMenuShortcut(grassToolStripMenuItem, ToggleGrass);
			InitMenuShortcut(alphaCheckerboardToolStripMenuItem, ToggleAlphaCheckerboard);
			InitMenuShortcut(textureOverlayToolStripMenuItem, ToggleOverlay);
			InitMenuShortcut(offToolStripMenuItem, () => SetTransparencyMode(TransparencyMode.Off));
			InitMenuShortcut(helmetOnlyToolStripMenuItem, () => SetTransparencyMode(TransparencyMode.Helmet));
			InitMenuShortcut(allToolStripMenuItem, () => SetTransparencyMode(TransparencyMode.All));
			InitMenuShortcut(headToolStripMenuItem, () => ToggleVisiblePart(VisiblePartFlags.HeadFlag));
			InitMenuShortcut(helmetToolStripMenuItem, () => ToggleVisiblePart(VisiblePartFlags.HelmetFlag));
			InitMenuShortcut(chestToolStripMenuItem, () => ToggleVisiblePart(VisiblePartFlags.ChestFlag));
			InitMenuShortcut(leftArmToolStripMenuItem, () => ToggleVisiblePart(VisiblePartFlags.LeftArmFlag));
			InitMenuShortcut(rightArmToolStripMenuItem, () => ToggleVisiblePart(VisiblePartFlags.RightArmFlag));
			InitMenuShortcut(leftLegToolStripMenuItem, () => ToggleVisiblePart(VisiblePartFlags.LeftLegFlag));
			InitMenuShortcut(rightLegToolStripMenuItem, () => ToggleVisiblePart(VisiblePartFlags.RightLegFlag));
			InitMenuShortcut(saveToolStripMenuItem, PerformSave);
			InitMenuShortcut(saveAsToolStripMenuItem, PerformSaveAs);
			InitMenuShortcut(saveAllToolStripMenuItem, PerformSaveAll);
			
			// not in the menu
			InitUnlinkedShortcut("Toggle transparency mode", Keys.Shift | Keys.U, ToggleTransparencyMode);
			InitUnlinkedShortcut("Upload skin", Keys.Control | Keys.U, PerformUpload);
			InitUnlinkedShortcut("Toggle view mode", Keys.Control | Keys.V, ToggleViewMode);
			InitUnlinkedShortcut("Screenshot (clipboard)", Keys.Control | Keys.H, TakeScreenshot);
			InitUnlinkedShortcut("Screenshot (save)", Keys.Control | Keys.Shift | Keys.H, SaveScreenshot);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			InitShortcuts();
			LoadShortcutKeys(GlobalSettings.ShortcutKeys);

			SetTool(Tools.Camera);
			SetTransparencyMode(GlobalSettings.Transparency);
			SetViewMode(_currentViewMode);

			glControl1.MakeCurrent();

			foreach (var file in Directory.EnumerateFiles("./Skins/", "*.png"))
				listBox1.Items.Add(new Skin(file));

			listBox1.SelectedIndex = 0;
			foreach (var skin in listBox1.Items)
			{
				Skin s = (Skin)skin;

				if (s.FileName.Equals(GlobalSettings.LastSkin, StringComparison.CurrentCultureIgnoreCase))
				{
					listBox1.SelectedItem = s;
					break;
				}
			}

			SetColor(Color.White);
		}

		private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index == -1)
				return;
			
			e.DrawBackground();
			e.DrawFocusRectangle();
			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

			Skin skin = (Skin)listBox1.Items[e.Index];

			if ((e.State & DrawItemState.Selected) == 0 && skin.FileName.Equals(GlobalSettings.LastSkin, StringComparison.CurrentCultureIgnoreCase))
				e.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.Yellow), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

			TextRenderer.DrawText(e.Graphics, skin.ToString(), listBox1.Font, new Rectangle(e.Bounds.X + 24, e.Bounds.Y, e.Bounds.Width - 1 - 24, e.Bounds.Height - 1), System.Drawing.Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
			e.Graphics.DrawImage(skin.Head, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, 24, 24));
		}

		private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 24;
		}

		int _grassTop;
		int _charPaintTex, _alphaTex, _backgroundTex;
		static int _previewPaint;

		private void glControl1_Load(object sender, EventArgs e)
		{
			glControl1_Resize(this, EventArgs.Empty);   // Ensure the Viewport is set up correctly
			GL.ClearColor(GlobalSettings.BackgroundColor);

			GL.Enable(EnableCap.Texture2D);
			GL.ShadeModel(ShadingModel.Smooth);                        // Enable Smooth Shading
			GL.ClearDepth(1.0f);                         // Depth Buffer Setup
			GL.Enable(EnableCap.DepthTest);                        // Enables Depth Testing
			GL.DepthFunc(DepthFunction.Lequal);                         // The Type Of Depth Testing To Do
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);          // Really Nice Perspective Calculations
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Front);

			IL.ilInit();

			_grassTop = ImageUtilities.LoadImage("grass.png");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			_backgroundTex = ImageUtilities.LoadImage("inverted.png");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

			_charPaintTex = GL.GenTexture();
			_previewPaint = GL.GenTexture();
			GlobalDirtiness.CurrentSkin = GL.GenTexture();
			_alphaTex = GL.GenTexture();

			unsafe
			{
				byte[] arra = new byte[64 * 32 * 4];
				fixed (byte* texData = arra)
				{
					byte *d = texData;

					for (int y = 0; y < 32; ++y)
						for (int x = 0; x < 64; ++x)
						{
							*((int*)d) = (x << 0) | (y << 8) | (0 << 16) | (255 << 24);
							d += 4;
						}
				}

				GL.BindTexture(TextureTarget.Texture2D, _charPaintTex);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arra);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

				GL.BindTexture(TextureTarget.Texture2D, _previewPaint);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arra);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

				GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arra);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

				arra = new byte[4 * 4 * 4];
				fixed (byte* texData = arra)
				{
					byte *d = texData;

					for (int y = 0; y < 4; ++y)
						for (int x = 0; x < 4; ++x)
						{
							bool dark = ((x + (y & 1)) & 1) == 1;

							if (dark)
								*((int*)d) = (80 << 0) | (80 << 8) | (80 << 16) | (255 << 24);
							else
								*((int*)d) = (127 << 0) | (127 << 8) | (127 << 16) | (255 << 24);
							d += 4;
						}
				}

				GL.BindTexture(TextureTarget.Texture2D, _alphaTex);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 4, 4, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arra);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			}
		}

		void DrawSkinnedRectangle
			(float x, float y, float z, float width, float length, float height,
			int frontSkinX, int frontSkinY, int frontSkinW, int frontSkinH,
			int backSkinX, int backSkinY, int backSkinW, int backSkinH,
			int topSkinX, int topSkinY, int topSkinW, int topSkinH,
			int bottomSkinX, int bottomSkinY, int bottomSkinW, int bottomSkinH,
			int leftSkinX, int leftSkinY, int leftSkinW, int leftSkinH,
			int rightSkinX, int rightSkinY, int rightSkinW, int rightSkinH,
			int texture, int skinW = 64, int skinH = 32)
		{
			GL.BindTexture(TextureTarget.Texture2D, texture);

			GL.Begin(BeginMode.Quads);

			width /= 2;
			length /= 2;
			height /= 2;

			float fsX = (float)frontSkinX / skinW;
			float fsY = (float)frontSkinY / skinH;
			float fsW = (float)frontSkinW / skinW;
			float fsH = (float)frontSkinH / skinH;

			float basX = (float)backSkinX / skinW;
			float basY = (float)backSkinY / skinH;
			float basW = (float)backSkinW / skinW;
			float basH = (float)backSkinH / skinH;

			float tsX = (float)topSkinX / skinW;
			float tsY = (float)topSkinY / skinH;
			float tsW = (float)topSkinW / skinW;
			float tsH = (float)topSkinH / skinH;

			float bsX = (float)bottomSkinX / skinW;
			float bsY = (float)bottomSkinY / skinH;
			float bsW = (float)bottomSkinW / skinW;
			float bsH = (float)bottomSkinH / skinH;

			float lsX = (float)leftSkinX / skinW;
			float lsY = (float)leftSkinY / skinH;
			float lsW = (float)leftSkinW / skinW;
			float lsH = (float)leftSkinH / skinH;

			float rsX = (float)rightSkinX / skinW;
			float rsY = (float)rightSkinY / skinH;
			float rsW = (float)rightSkinW / skinW;
			float rsH = (float)rightSkinH / skinH;

			// Front Face
			if (texture != _grassTop)
			{
				GL.TexCoord2(fsX, fsY); GL.Vertex3(x - width, y + length, z + height);  // Bottom Left Of The Texture and Quad
				GL.TexCoord2(fsX + fsW - 0.00005, fsY); GL.Vertex3(x + width, y + length, z + height);  // Bottom Right Of The Texture and Quad
				GL.TexCoord2(fsX + fsW - 0.00005, fsY + fsH - 0.00005); GL.Vertex3(x + width, y - length, z + height);  // Top Right Of The Texture and Quad
				GL.TexCoord2(fsX, fsY + fsH - 0.00005); GL.Vertex3(x - width, y - length, z + height);  // Top Left Of The Texture and Quad
			}
			GL.TexCoord2(tsX, tsY); GL.Vertex3(x - width, y + length, z - height);          // Top Right Of The Quad (Top)
			GL.TexCoord2(tsX + tsW - 0.00005, tsY); GL.Vertex3(x + width, y + length, z - height);          // Top Left Of The Quad (Top)
			GL.TexCoord2(tsX + tsW - 0.00005, tsY + tsH - 0.00005); GL.Vertex3(x + width, y + length, z + height);          // Bottom Left Of The Quad (Top)
			GL.TexCoord2(tsX, tsY + tsH - 0.00005); GL.Vertex3(x - width, y + length, z + height);          // Bottom Right Of The Quad (Top)

			if (texture != _grassTop)
			{
				GL.TexCoord2(bsX, bsY); GL.Vertex3(x - width, y - length, z + height);          // Top Right Of The Quad (Top)
				GL.TexCoord2(bsX + bsW - 0.00005, bsY); GL.Vertex3(x + width, y - length, z + height);          // Top Left Of The Quad (Top)
				GL.TexCoord2(bsX + bsW - 0.00005, bsY + bsH - 0.00005); GL.Vertex3(x + width, y - length, z - height);          // Bottom Left Of The Quad (Top)
				GL.TexCoord2(bsX, bsY + bsH - 0.00005); GL.Vertex3(x - width, y - length, z - height);          // Bottom Right Of The Quad (Top)

				GL.TexCoord2(lsX, lsY); GL.Vertex3(x - width, y + length, z - height);          // Top Right Of The Quad (Left)
				GL.TexCoord2(lsX + lsW - 0.00005, lsY); GL.Vertex3(x - width, y + length, z + height);          // Top Left Of The Quad (Left)
				GL.TexCoord2(lsX + lsW - 0.00005, lsY + lsH - 0.00005); GL.Vertex3(x - width, y - length, z + height);          // Bottom Left Of The Quad (Left)
				GL.TexCoord2(lsX, lsY + lsH - 0.00005); GL.Vertex3(x - width, y - length, z - height);          // Bottom Right Of The Quad (Left)

				GL.TexCoord2(rsX, rsY); GL.Vertex3(x + width, y + length, z + height);          // Top Right Of The Quad (Left)
				GL.TexCoord2(rsX + rsW - 0.00005, rsY); GL.Vertex3(x + width, y + length, z - height);          // Top Left Of The Quad (Left)
				GL.TexCoord2(rsX + rsW - 0.00005, rsY + rsH - 0.00005); GL.Vertex3(x + width, y - length, z - height);          // Bottom Left Of The Quad (Left)
				GL.TexCoord2(rsX, rsY + rsH - 0.00005); GL.Vertex3(x + width, y - length, z + height);          // Bottom Right Of The Quad (Left)

				GL.TexCoord2(basX, basY); GL.Vertex3(x + width, y + length, z - height);  // Bottom Left Of The Texture and Quad
				GL.TexCoord2(basX + basW - 0.00005, basY); GL.Vertex3(x - width, y + length, z - height);  // Bottom Right Of The Texture and Quad
				GL.TexCoord2(basX + basW - 0.00005, basY + basH - 0.00005); GL.Vertex3(x - width, y - length, z - height);  // Top Right Of The Texture and Quad
				GL.TexCoord2(basX, basY + basH - 0.00005); GL.Vertex3(x + width, y - length, z - height);  // Top Left Of The Texture and Quad		
			}

			GL.End();
		}

		float _rotTest = 0;
		void animTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_rotTest += 0.24f;
			glControl1.Invalidate();
		}

		float _zoom = -80;
		void glControl1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (_currentViewMode == ViewMode.Perspective)
				_zoom += e.Delta / 50;
			else
				_2dZoom += e.Delta / 50;

			glControl1.Invalidate();
		}

		float _2dCamOffsetX = 0;
		float _2dCamOffsetY = 0;
		float _2dZoom = 8;

		void DrawPlayer(int tex, bool grass, bool pickView)
		{
			if (_currentViewMode == ViewMode.Orthographic)
			{
				if (!pickView && GlobalSettings.AlphaCheckerboard)
				{
					GL.BindTexture(TextureTarget.Texture2D, _alphaTex);

					GL.Begin(BeginMode.Quads);
					GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
					GL.TexCoord2(glControl1.Width / 32.0f, 0); GL.Vertex2(glControl1.Width, 0);
					GL.TexCoord2(glControl1.Width / 32.0f, glControl1.Height / 32.0f); GL.Vertex2(glControl1.Width, glControl1.Height);
					GL.TexCoord2(0, glControl1.Height / 32.0f); GL.Vertex2(0, glControl1.Height);
					GL.End();
				}

				GL.BindTexture(TextureTarget.Texture2D, tex);

				GL.PushMatrix();

				GL.Translate((glControl1.Width / 2) + -_2dCamOffsetX, (glControl1.Height / 2) + -_2dCamOffsetY, 0);
				GL.Scale(_2dZoom, _2dZoom, 1);

				GL.Enable(EnableCap.Blend);

				float w = 64;
				float h = 32;
				GL.PushMatrix();
				GL.Translate((_2dCamOffsetX), (_2dCamOffsetY), 0);
				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0, 0); GL.Vertex2(-(w / 2), -(h / 2));
				GL.TexCoord2(1, 0); GL.Vertex2((w / 2), -(h / 2));
				GL.TexCoord2(1, 1); GL.Vertex2((w / 2), (h / 2));
				GL.TexCoord2(0, 1); GL.Vertex2(-(w / 2), (h / 2));
				GL.End();

				if (!pickView && GlobalSettings.TextureOverlay)
				{
					GL.BindTexture(TextureTarget.Texture2D, _backgroundTex);

					GL.Begin(BeginMode.Quads);
					GL.TexCoord2(0, 0); GL.Vertex2(-(w / 2), -(h / 2));
					GL.TexCoord2(1, 0); GL.Vertex2((w / 2), -(h / 2));
					GL.TexCoord2(1, 1); GL.Vertex2((w / 2), (h / 2));
					GL.TexCoord2(0, 1); GL.Vertex2(-(w / 2), (h / 2));
					GL.End();
				}
				GL.PopMatrix();

				GL.PopMatrix();

				GL.Disable(EnableCap.Blend);

				return;
			}

			Vector3 vec = new Vector3();
			int count = 0;

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.HeadFlag) != 0)
			{
				vec = Vector3.Add(vec, new Vector3(0, 10, 0));
				count++;
			}

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.ChestFlag) != 0)
			{
				vec = Vector3.Add(vec, new Vector3(0, 0, 0));
				count++;
			}

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.RightLegFlag) != 0)
			{
				vec = Vector3.Add(vec, new Vector3(-2, -12, 0));
				count++;
			}

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.LeftLegFlag) != 0)
			{
				vec = Vector3.Add(vec, new Vector3(2, -12, 0));
				count++;
			}

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.RightArmFlag) != 0)
			{
				vec = Vector3.Add(vec, new Vector3(-6, 0, 0));
				count++;
			}

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.LeftArmFlag) != 0)
			{
				vec = Vector3.Add(vec, new Vector3(6, 0, 0));
				count++;
			}

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.HelmetFlag) != 0)
			{
				vec = Vector3.Add(vec, new Vector3(0, 10, 0));
				count++;
			}

			vec = Vector3.Divide(vec, count);

			GL.Translate(0, 0, _zoom);
			GL.Rotate(_rotX, 1, 0, 0);
			GL.Rotate(_rotY, 0, 1, 0);

			GL.Translate(-vec.X, -vec.Y, 0);
			GL.PushMatrix();

			var clPt = glControl1.PointToClient(Cursor.Position);
			var x = clPt.X - (glControl1.Width / 2);
			var y = clPt.Y - (glControl1.Height / 2);

			GL.PushMatrix();

			if (!pickView && GlobalSettings.Transparency == TransparencyMode.All)
				GL.Enable(EnableCap.Blend);
			else
				GL.Disable(EnableCap.Blend);

			if (grass)
				DrawSkinnedRectangle(0, -20, 0, 1024, 4, 1024, 0, 0, 1024, 1024, 0, 0, 0, 0, 0, 0, 1024, 1024, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, _grassTop, 16, 16);

			if (followCursorToolStripMenuItem.Checked)
			{
				GL.Translate(0, 4, 0);
				GL.Rotate((float)x / 25, 0, 1, 0);
				GL.Rotate((float)y / 25, 1, 0, 0);
				GL.Translate(0, -4, 0);
			}

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.HeadFlag) != 0)
				DrawSkinnedRectangle(0, 10, 0, 8, 8, 8,
				8, 8, 8, 8,
				24, 8, 8, 8,
				8, 0, 8, 8,
				16, 0, 8, 8,
				0, 8, 8, 8,
				16, 8, 8, 8,
				tex);
			GL.PopMatrix();

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.ChestFlag) != 0)
				DrawSkinnedRectangle(0, 0, 0, 8, 12, 4,
				20, 20, 8, 12,
				32, 20, 8, 12,
				20, 16, 8, 4,
				28, 16, 8, 4,
				16, 20, 4, 12,
				28, 20, 4, 12,
				tex);

			// right
			GL.PushMatrix();
			if (animateToolStripMenuItem.Checked)
			{
				GL.Translate(0, -6, 0);
				GL.Rotate(Math.Sin(_rotTest) * 37, 1, 0, 0);
				GL.Translate(0, 6, 0);
			}
			if ((GlobalSettings.ViewFlags & VisiblePartFlags.RightLegFlag) != 0)
				DrawSkinnedRectangle(-2, -12, 0, 4, 12, 4,
				4, 20, 4, 12,
				12, 20, 4, 12,
				4, 16, 4, 4,
				8, 16, 4, 4,
				0, 20, 4, 12,
				8, 20, 4, 12,
				tex);
			GL.PopMatrix();

			// left
			GL.PushMatrix();
			if (animateToolStripMenuItem.Checked)
			{
				GL.Translate(0, -6, 0);
				GL.Rotate(Math.Sin(_rotTest) * -37, 1, 0, 0);
				GL.Translate(0, 6, 0);
			}
			if ((GlobalSettings.ViewFlags & VisiblePartFlags.LeftLegFlag) != 0)
				DrawSkinnedRectangle(2, -12, 0, 4, 12, 4,
				8, 20, -4, 12,
				16, 20, -4, 12,
				8, 16, -4, 4,
				12, 16, -4, 4,
				12, 20, -4, 12,
				4, 20, -4, 12,
				tex);
			GL.PopMatrix();

			// right arm
			GL.PushMatrix();
			if (animateToolStripMenuItem.Checked)
			{
				GL.Translate(0, 5, 0);
				GL.Rotate(Math.Sin(_rotTest) * -37, 1, 0, 0);
				GL.Translate(0, -5, 0);
			}
			if ((GlobalSettings.ViewFlags & VisiblePartFlags.RightArmFlag) != 0)
				DrawSkinnedRectangle(-6, 0, 0, 4, 12, 4,
				44, 20, 4, 12,
				52, 20, 4, 12,
				44, 16, 4, 4,
				48, 16, 4, 4,
				40, 20, 4, 12,
				48, 20, 4, 12,
				tex);
			GL.PopMatrix();

			GL.PushMatrix();
			if (animateToolStripMenuItem.Checked)
			{
				GL.Translate(0, 5, 0);
				GL.Rotate(Math.Sin(_rotTest) * 37, 1, 0, 0);
				GL.Translate(0, -5, 0);
			}
			// left arm
			if ((GlobalSettings.ViewFlags & VisiblePartFlags.LeftArmFlag) != 0)
				DrawSkinnedRectangle(6, 0, 0, 4, 12, 4,
				48, 20, -4, 12,
				56, 20, -4, 12,
				48, 16, -4, 4,
				52, 16, -4, 4,
				52, 20, -4, 12,
				44, 20, -4, 12,
				tex);
			GL.PopMatrix();

			if ((GlobalSettings.ViewFlags & VisiblePartFlags.HelmetFlag) != 0)
			{
				GL.PushMatrix();
				if (followCursorToolStripMenuItem.Checked)
				{
					GL.Translate(0, 4, 0);
					GL.Rotate((float)x / 25, 0, 1, 0);
					GL.Rotate((float)y / 25, 1, 0, 0);
					GL.Translate(0, -4, 0);
				}

				if (!pickView && GlobalSettings.Transparency != TransparencyMode.Off)
					GL.Enable(EnableCap.Blend);
				else
					GL.Disable(EnableCap.Blend);

				DrawSkinnedRectangle(0, 10, 0, 9, 9, 9,
									32 + 8, 8, 8, 8,
									32 + 24, 8, 8, 8,
									32 + 8, 0, 8, 8,
									32 + 16, 0, 8, 8,
									32 + 0, 8, 8, 8,
									32 + 16, 8, 8, 8,
									tex);
				GL.PopMatrix();
			}

			GL.PopMatrix();
		}

		float _rotX = 0, _rotY= 0;
		private void glControl1_Paint(object sender, PaintEventArgs e)
		{
			glControl1.MakeCurrent();
			SetPreview();

			if (_currentViewMode == ViewMode.Orthographic)
				GL.ClearColor(Color.Black);
			else
				GL.ClearColor(GlobalSettings.BackgroundColor);
			
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			DrawPlayer(_previewPaint, grassToolStripMenuItem.Checked, false);

			glControl1.SwapBuffers();
		}

		private void glControl1_Resize(object sender, EventArgs e)
		{
			glControl1.MakeCurrent();
			
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

			if (_currentViewMode == ViewMode.Perspective)
			{
				var mat = OpenTK.Matrix4d.Perspective(45, (double)glControl1.Width / (double)glControl1.Height, 0.01, 100000);
				GL.MultMatrix(ref mat);
			}
			else
				GL.Ortho(0, glControl1.Width, glControl1.Height, 0, -1, 1);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			glControl1.Invalidate();
		}

		PixelsChangedUndoable _changedPixels = null;
		bool _md = false;
		Point _mp;
		private void glControl1_MouseDown(object sender, MouseEventArgs e)
		{
			_md = true;
			_mp = e.Location;

			if (e.Button == MouseButtons.Left)
				UseToolOnViewport(e.X, e.Y);
		}

		void SetPreview()
		{
			if (listBox1.SelectedItem == null)
			{
				int[] array = new int[64 * 32];
				GL.BindTexture(TextureTarget.Texture2D, _previewPaint);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				return;
			}
			else
			{
				Skin skin = (Skin)listBox1.SelectedItem;

				GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				if (_currentTool == Tools.Pencil || _currentTool == Tools.Eraser || _currentTool == Tools.Burn || _currentTool == Tools.Dodge)
				{
					Point p = Point.Empty;

					if (GetPick(_mp.X, _mp.Y, ref p))
						UseToolOnPixel(array, p.X, p.Y);
				}

				GL.BindTexture(TextureTarget.Texture2D, _previewPaint);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
			}
		}

		bool GetPick(int x, int y, ref Point hitPixel)
		{
			glControl1.MakeCurrent();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			GL.ClearColor(Color.White);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.ClearColor(GlobalSettings.BackgroundColor);

			DrawPlayer(_charPaintTex, false, true);

			int[] viewport = new int[4];
			byte[] pixel = new byte[3];

			GL.GetInteger(GetPName.Viewport, viewport);

			GL.ReadPixels(x, viewport[3] - y, 1, 1,
				PixelFormat.Rgb, PixelType.UnsignedByte, pixel);

			if (pixel[2] == 0)
			{
				hitPixel = new Point(pixel[0], pixel[1]);
				return true;
			}

			return false;
		}

		void UseToolOnPixel(int[] pixels, int x, int y)
		{
			var c = pixels[x + (64 * y)];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);

			// blend
			if (_currentTool == Tools.Pencil)
			{
				Color blend = Color.FromArgb(ColorBlending.AlphaBlend(color, oldColor).ToArgb());
				pixels[x + (64 * y)] = blend.R | (blend.G << 8) | (blend.B << 16) | (blend.A << 24);
			}
			else if (_currentTool == Tools.Burn)
			{
				Color burnt = Color.FromArgb(ColorBlending.Burn(oldColor, 0.75f).ToArgb());
				pixels[x + (64 * y)] = burnt.R | (burnt.G << 8) | (burnt.B << 16) | (burnt.A << 24);
			}
			else if (_currentTool == Tools.Dodge)
			{
				Color burnt = Color.FromArgb(ColorBlending.Dodge(oldColor, 0.25f).ToArgb());
				pixels[x + (64 * y)] = burnt.R | (burnt.G << 8) | (burnt.B << 16) | (burnt.A << 24);
			}
			else if (_currentTool == Tools.Eraser)
				pixels[x + (64 * y)] = 0;
		}

		void UseToolOnViewport(int x, int y)
		{
			if (listBox1.SelectedItem == null)
				return;

			Point p = Point.Empty;

			if (GetPick(x, y, ref p))
			{
				Skin skin = (Skin)listBox1.SelectedItem;

				GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				if (_currentTool == Tools.Pencil || _currentTool == Tools.Eraser || _currentTool == Tools.Burn || _currentTool == Tools.Dodge)
				{
					if (_changedPixels == null)
					{
						_changedPixels = new PixelsChangedUndoable();
						_changedPixels.NewColor = color;
					}

					if (!_changedPixels.Points.ContainsKey(new Point(p.X, p.Y)))
					{
						var c = array[p.X + (64 * p.Y)];
						var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
						_changedPixels.Points[new Point(p.X, p.Y)] = oldColor;

						UseToolOnPixel(array, p.X, p.Y);

						SetCanSave(true);
						skin.Dirty = true;
						GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
					}
				}
				else if (_currentTool == Tools.Dropper)
				{
					var c = array[p.X + (64 * p.Y)];
					SetColor(Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF));
				}
			}
		
			glControl1.Invalidate();
		}

		private void glControl1_MouseMove(object sender, MouseEventArgs e)
		{
			if (_md)
			{
				var delta = new Point(e.X - _mp.X, e.Y - _mp.Y);

				if ((_currentTool == Tools.Camera && e.Button == MouseButtons.Left) ||
					((_currentTool != Tools.Camera) && e.Button == MouseButtons.Right))
				{
					if (_currentViewMode == ViewMode.Perspective)
					{
						_rotY += (float)delta.X;
						_rotX += (float)delta.Y;
					}
					else
					{
						_2dCamOffsetX += delta.X / _2dZoom;
						_2dCamOffsetY += delta.Y / _2dZoom;
					}
				}
				else if ((_currentTool == Tools.Camera && e.Button == MouseButtons.Right) ||
					((_currentTool != Tools.Camera) && e.Button == MouseButtons.Middle))
				{
					if (_currentViewMode == ViewMode.Perspective)
						_zoom += (float)-delta.Y;
					else
						_2dZoom += -delta.Y / 25.0f;
				}

				if ((_currentTool != Tools.Camera) && e.Button == MouseButtons.Left)
					UseToolOnViewport(e.X, e.Y);

				glControl1.Invalidate();
			}

			_mp = e.Location;
		}

		private void glControl1_MouseUp(object sender, MouseEventArgs e)
		{
			if (_currentUndoBuffer != null && _changedPixels != null)
			{
				_currentUndoBuffer.AddBuffer(_changedPixels);
				_changedPixels = null;

				undoToolStripMenuItem.Enabled = undoStripButton5.Enabled = _currentUndoBuffer.CanUndo;
				redoToolStripMenuItem.Enabled = redoStripButton4.Enabled = _currentUndoBuffer.CanRedo;		
			}

			_md = false;
		}

		UndoBuffer _currentUndoBuffer = null;
		Skin _lastSkin = null;
		bool _skipListbox = false;
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_skipListbox || listBox1.SelectedItem == _lastSkin)
				return;

			if (_lastSkin != null && listBox1.SelectedItem != _lastSkin)
			{
				// Copy over the current changes to the tex stored in the skin.
				// This allows us to pick up where we left off later, without undoing any work.
				_lastSkin.CommitChanges(GlobalDirtiness.CurrentSkin, false);
			}

			//if (_lastSkin != null)
			//	_lastSkin.Undo.Clear();

			glControl1.MakeCurrent();

			Skin skin = (Skin)listBox1.SelectedItem;
			SetCanSave(skin.Dirty);

			if (skin == null)
			{
				_currentUndoBuffer = null;
				GL.BindTexture(TextureTarget.Texture2D, 0);
				int[] array = new int[64 * 32];
				GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				undoToolStripMenuItem.Enabled = undoStripButton5.Enabled = false;
				redoToolStripMenuItem.Enabled = redoStripButton4.Enabled = false;
			}
			else
			{
				GL.BindTexture(TextureTarget.Texture2D, skin.GLImage);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				_currentUndoBuffer = skin.Undo;
				undoToolStripMenuItem.Enabled = undoStripButton5.Enabled = _currentUndoBuffer.CanUndo;
				redoToolStripMenuItem.Enabled = redoStripButton4.Enabled = _currentUndoBuffer.CanRedo;
			}

			glControl1.Invalidate();
			_lastSkin = (Skin)listBox1.SelectedItem;
		}

		public static Exception HttpUploadFile(string url, string file, string paramName, string contentType, Dictionary<string, byte[]> nvc, CookieContainer cookies)
		{
			//log.Debug(string.Format("Uploading {0} to {1}", file, url));
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
			wr.ContentType = "multipart/form-data; boundary=" + boundary;
			wr.Method = "POST";
			wr.KeepAlive = true;
			wr.CookieContainer = cookies;
			wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
			wr.Timeout = 10000;

			Stream rs = wr.GetRequestStream();

			string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
			foreach (var kvp in nvc)
			{
				rs.Write(boundarybytes, 0, boundarybytes.Length);
				string formitem = string.Format(formdataTemplate, kvp.Key, Encoding.ASCII.GetString(kvp.Value));
				byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
				rs.Write(formitembytes, 0, formitembytes.Length);
			}
			rs.Write(boundarybytes, 0, boundarybytes.Length);

			string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string header = string.Format(headerTemplate, paramName, Path.GetFileName(file), contentType);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
			rs.Write(headerbytes, 0, headerbytes.Length);

			FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
			byte[] buffer = new byte[4096];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
			{
				rs.Write(buffer, 0, bytesRead);
			}
			fileStream.Close();

			byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
			rs.Write(trailer, 0, trailer.Length);
			rs.Close();

			WebResponse wresp = null;
			Exception ret = null;
			try
			{
				wresp = wr.GetResponse();
				Stream stream2 = wresp.GetResponseStream();
				StreamReader reader2 = new StreamReader(stream2);
				//log.Debug(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
			}
			catch (Exception ex)
			{
				//log.Error("Error uploading file", ex);
				if (wresp != null)
				{
					wresp.Close();
					wresp = null;
				}

				ret = ex;
			}
			finally
			{
				wr = null;
			}

			return ret;
		}

		public enum ErrorCodes
		{
			Succeeded,
			TimeOut,
			WrongCredentials,
			Unknown
		}

		class ErrorReturn
		{
			public ErrorCodes Code;
			public Exception Exception;
			public string ReportedError;
		}

		void UploadThread(object param)
		{
			var parms = (object[])param;
			ErrorReturn error = (ErrorReturn)parms[3];

			error.Code = ErrorCodes.Succeeded;
			error.Exception = null;
			error.ReportedError = null;

			try
			{
				CookieContainer cookies = new CookieContainer();
				var request = (HttpWebRequest)HttpWebRequest.Create("http://www.minecraft.net/login");
				request.CookieContainer = cookies;
				request.Timeout = 10000;
				var response = request.GetResponse();
				StreamReader sr = new StreamReader(response.GetResponseStream());
				var text = sr.ReadToEnd();

				var match = Regex.Match(text, @"<input type=""hidden"" name=""authenticityToken"" value=""(.*?)"">");
				string authToken = null;
				if (match.Success)
					authToken = match.Groups[1].Value;

				if (authToken == null)
					return;

				sr.Dispose();

				response.Close();

				string requestTemplate = @"authenticityToken={0}&redirect=http%3A%2F%2Fwww.minecraft.net%2Fprofile&username={1}&password={2}";
				string requestContent = string.Format(requestTemplate, authToken, parms[0].ToString(), parms[1].ToString());
				var inBytes = Encoding.UTF8.GetBytes(requestContent);

				// craft the login request
				request = (HttpWebRequest)HttpWebRequest.Create("https://www.minecraft.net/login");
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.CookieContainer = cookies;
				request.ContentLength = inBytes.Length;
				request.Timeout = 10000;

				using (Stream dataStream = request.GetRequestStream())
					dataStream.Write(inBytes, 0, inBytes.Length);

				response = request.GetResponse();
				sr = new StreamReader(response.GetResponseStream());
				text = sr.ReadToEnd();

				match = Regex.Match(text, @"<p class=""error"">([\w\W]*?)</p>");

				sr.Dispose();
				response.Close();

				if (match.Success)
				{
					error.ReportedError = match.Groups[1].Value.Trim();
					error.Code = ErrorCodes.WrongCredentials;
				}
				else
				{
					var dict = new Dictionary<string, byte[]>();
					dict.Add("authenticityToken", Encoding.ASCII.GetBytes(authToken));
					if ((error.Exception = HttpUploadFile("http://www.minecraft.net/profile/skin", parms[2].ToString(), "skin", "image/png", dict, cookies)) != null)
						error.Code = ErrorCodes.Unknown;
				}
			}
			catch (Exception ex)
			{
				error.Exception = ex;
			}
			finally
			{
				Invoke((Action)delegate() { _pleaseWaitForm.Close(); });
			}
		}

		internal PleaseWait _pleaseWaitForm;
		void PerformUpload()
		{
			if (listBox1.SelectedItem == null)
				return;

			using (Login login = new Login())
			{
				login.Username = GlobalSettings.LastUsername;
				login.Password = GlobalSettings.LastPassword;

				bool dialogRes = true;
				bool didShowDialog = false;

				if ((ModifierKeys & Keys.Shift) != 0 || !GlobalSettings.RememberMe || !GlobalSettings.AutoLogin)
				{
					login.Remember = GlobalSettings.RememberMe;
					login.AutoLogin = GlobalSettings.AutoLogin;
					dialogRes = login.ShowDialog() == System.Windows.Forms.DialogResult.OK;
					didShowDialog = true;
				}

				if (!dialogRes)
					return;

				_pleaseWaitForm = new PleaseWait();

				Thread thread = new Thread(UploadThread);
				ErrorReturn ret = new ErrorReturn();
				thread.Start(new object[] { login.Username, login.Password, ((Skin)listBox1.SelectedItem).FileName, ret });

				_pleaseWaitForm.ShowDialog();
				_pleaseWaitForm.Dispose();

				if (ret.ReportedError != null)
					MessageBox.Show("Error uploading skin:\r\n" + ret.ReportedError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				else if (ret.Exception != null)
					MessageBox.Show("Error uploading skin:\r\n" + ret.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
				{
					MessageBox.Show("Skin upload success! Enjoy!", "Woo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
					GlobalSettings.LastSkin = ((Skin)listBox1.SelectedItem).FileName;
					listBox1.Invalidate();
				}

				if (didShowDialog)
				{
					GlobalSettings.RememberMe = login.Remember;
					GlobalSettings.AutoLogin = login.AutoLogin;

					if (GlobalSettings.RememberMe == false)
						GlobalSettings.LastUsername = GlobalSettings.LastPassword = null;
					else
					{
						GlobalSettings.LastUsername = login.Username;
						GlobalSettings.LastPassword = login.Password;
					}
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			PerformUpload();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		void ToggleAnimation()
		{
			animateToolStripMenuItem.Checked = !animateToolStripMenuItem.Checked;
			GlobalSettings.Animate = animateToolStripMenuItem.Checked;

			Invalidate();
		}

		private void animateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleAnimation();
		}

		void ToggleFollowCursor()
		{
			followCursorToolStripMenuItem.Checked = !followCursorToolStripMenuItem.Checked;
			GlobalSettings.FollowCursor = followCursorToolStripMenuItem.Checked;

			Invalidate();
		}

		private void followCursorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleFollowCursor();
		}

		void ToggleGrass()
		{
			grassToolStripMenuItem.Checked = !grassToolStripMenuItem.Checked;
			GlobalSettings.Grass = grassToolStripMenuItem.Checked;

			glControl1.Invalidate();
		}

		private void grassToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleGrass();
		}

		void PerformImportSkin()
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Minecraft Skins|*.png";
				ofd.Multiselect = true;

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					foreach (var f in ofd.FileNames)
					{
						var name = Path.GetFileNameWithoutExtension(f);

						while (File.Exists("./Skins/" + name + ".png"))
							name += " (New)";

						File.Copy(f, "./Skins/" + name + ".png");

						Skin skin = new Skin("./Skins/" + name + ".png");
						listBox1.Items.Add(skin);
						listBox1.SelectedItem = skin;
					}
				}

				listBox1.Sorted = false;
				listBox1.Sorted = true;
			}
		}

		private void addNewSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformImportSkin();
		}

		void PerformDeleteSkin()
		{
			if (listBox1.SelectedItem == null)
				return;

			if (MessageBox.Show("Delete this skin perminently?\r\nThis will delete the skin from the Skins directory!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				Skin skin = ((Skin)listBox1.SelectedItem);
				if (listBox1.Items.Count != 1)
				{
					if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
						listBox1.SelectedIndex--;
					else
						listBox1.SelectedIndex++;
				}
				listBox1.Items.Remove(skin);

				File.Delete(skin.FileName);

				Invalidate();
			}
		}

		private void deleteSelectedSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformDeleteSkin();
		}

		void PerformCloneSkin()
		{
			if (listBox1.SelectedItem == null)
				return;

			Skin skin = ((Skin)listBox1.SelectedItem);
			string newName = skin.Name;
			string newFileName;

			do
			{
				newName += " - Copy";
				newFileName = Path.GetDirectoryName(skin.FileName) + '/' + newName + ".png";
			} while (File.Exists(newFileName));

			File.Copy(skin.FileName, newFileName);
			Skin newSkin = new Skin(newFileName);
			listBox1.Items.Add(newSkin);
		}

		private void cloneSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformCloneSkin();
		}

		private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listBox1.SelectedItem == null)
				return;

			var skin = ((Skin)listBox1.SelectedItem);

			using (NameChange nc = new NameChange())
			{
				while (true)
				{
					nc.SkinName = skin.Name;

					if (nc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						string newName = Path.GetDirectoryName(skin.FileName) + '/' + nc.SkinName + ".png";

						if (skin.FileName == newName ||
							File.Exists(newName))
						{
							MessageBox.Show("Either that skin exists already or you used the same name...?");
							continue;
						}

						File.Copy(skin.FileName, newName);
						File.Delete(skin.FileName);
						skin.FileName = newName;
						skin.Name = nc.SkinName;

						listBox1.Sorted = false;
						listBox1.Sorted = true;

						break;
					}

					break;
				}
			}
		}

		ToolStripButton[] _buttons = null;
		ToolStripMenuItem[] _toolMenus = null;
		static Tools _currentTool = Tools.Camera;
		void SetTool(Tools tool)
		{
			_currentTool = tool;

			if (_buttons == null)
				_buttons = new ToolStripButton[] { toolStripButton1, toolStripButton2, toolStripButton3, eraserToolStripButton, dodgeToolStripButton, burnToolStripButton };
			if (_toolMenus == null)
				_toolMenus = new ToolStripMenuItem[] { cameraToolStripMenuItem, pencilToolStripMenuItem, dropperToolStripMenuItem, eraserToolStripMenuItem, dodgeToolStripMenuItem, burnToolStripMenuItem };

			for (int i = 0; i < _buttons.Length; ++i)
			{
				if (i == (int)tool)
				{
					_buttons[i].Checked = true;
					_toolMenus[i].Checked = true;
				}
				else
				{
					_buttons[i].Checked = false;
					_toolMenus[i].Checked = false;
				}
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Camera);
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Pencil);
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Dropper);
		}

		private void eraserToolStripButton_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Eraser);
		}

		void SetCanSave(bool value)
		{
			saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = value;
		}

		void PerformSaveAs()
		{
			GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
			int[] pixels = new int[64 * 32];
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

			Bitmap b = new Bitmap(64, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			var locked = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			unsafe
			{
				fixed (void *inPixels = pixels)
				{
					void *outPixels = locked.Scan0.ToPointer();

					int *inInt = (int*)inPixels;
					int *outInt = (int*)outPixels;

					for (int y = 0; y < b.Height; ++y)
						for (int x = 0; x < b.Width; ++x)
						{
							var color = Color.FromArgb((*inInt >> 24) & 0xFF, (*inInt >> 0) & 0xFF, (*inInt >> 8) & 0xFF, (*inInt >> 16) & 0xFF);
							*outInt = color.ToArgb();

							inInt++;
							outInt++;
						}
				}
			}

			b.UnlockBits(locked);

			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter = "Skin Image|*.png";

				if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					b.Save(sfd.FileName);
			}

			b.Dispose();
		}

		void PerformSaveSkin(Skin s)
		{
			glControl1.MakeCurrent();

			s.CommitChanges((s == listBox1.SelectedItem) ? GlobalDirtiness.CurrentSkin : s.GLImage, true);
		}

		void PerformSaveAll()
		{
			foreach (var item in listBox1.Items)
			{
				Skin skin = (Skin)item;

				if (!skin.Dirty)
					continue;

				PerformSaveSkin(skin);
			}

			listBox1.Invalidate();
		}

		void PerformSave()
		{
			Skin skin = (Skin)listBox1.SelectedItem;

			if (!skin.Dirty)
				return;

			SetCanSave(false);
			PerformSaveSkin(skin);
			listBox1.Invalidate();
		}

		void PerformUndo()
		{
			if (!_currentUndoBuffer.CanUndo)
				return;

			glControl1.MakeCurrent();

			_currentUndoBuffer.Undo();

			undoToolStripMenuItem.Enabled = undoStripButton5.Enabled = _currentUndoBuffer.CanUndo;
			redoToolStripMenuItem.Enabled = redoStripButton4.Enabled = _currentUndoBuffer.CanRedo;

			Skin current = (Skin)listBox1.SelectedItem;
			SetCanSave(current.Dirty = true);

			glControl1.Invalidate();
		}

		void PerformRedo()
		{
			if (!_currentUndoBuffer.CanRedo)
				return;

			glControl1.MakeCurrent();

			_currentUndoBuffer.Redo();

			Skin current = (Skin)listBox1.SelectedItem;
			SetCanSave(current.Dirty = true);

			undoToolStripMenuItem.Enabled = undoStripButton5.Enabled = _currentUndoBuffer.CanUndo;
			redoToolStripMenuItem.Enabled = redoStripButton4.Enabled = _currentUndoBuffer.CanRedo;

			glControl1.Invalidate();
		}

		private void undoStripButton5_Click(object sender, EventArgs e)
		{
			PerformUndo();
		}

		private void redoStripButton4_Click(object sender, EventArgs e)
		{
			PerformRedo();
		}

		Color color = Color.FromArgb(255, 255, 255, 255);

		bool skipSet = false;

		void SetColor(Color c)
		{
			color = c;
			panel1.BackColor = color;

			skipSet = true;
			numericUpDown1.Value = c.R;
			numericUpDown2.Value = c.G;
			numericUpDown3.Value = c.B;
			numericUpDown4.Value = c.A;

			var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(c);
			colorSquare1.CurrentHue = (int)hsl.Hue;
			colorSquare1.CurrentSat = (int)(hsl.Saturation * 240);
			saturationSlider1.CurrentLum = (int)(hsl.Luminance * 240);

			redRenderer.StartColor =
				greenRenderer.StartColor =
				blueRenderer.StartColor = color;

			redRenderer.EndColor = Color.FromArgb(255, 255, color.G, color.B);
			greenRenderer.EndColor = Color.FromArgb(255, color.R, 255, color.B);
			blueRenderer.EndColor = Color.FromArgb(255, color.R, color.G, 255);

			redColorSlider.Value = color.R;
			greenColorSlider.Value = color.G;
			blueColorSlider.Value = color.B;
			alphaColorSlider.Value = color.A;

			redColorSlider.Invalidate();
			greenColorSlider.Invalidate();
			blueColorSlider.Invalidate();
			alphaColorSlider.Invalidate();
			colorSquare1.Invalidate();
			
			skipSet = false;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb(color.A, (byte)numericUpDown1.Value, color.G, color.B));
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb(color.A, color.R, (byte)numericUpDown2.Value, color.B));
		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb(color.A, color.R, color.G, (byte)numericUpDown3.Value));
		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb((byte)numericUpDown4.Value, color.R, color.G, color.B));
		}

		private void colorSquare1_HueChanged(object sender, EventArgs e)
		{
			if (skipSet)
				return;

			var c = new HSL(colorSquare1.CurrentHue, (float)colorSquare1.CurrentSat / 240.0f, (float)saturationSlider1.CurrentLum / 240.0f); ;
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		private void colorSquare1_SatChanged(object sender, EventArgs e)
		{
			if (skipSet)
				return;

			var c = new HSL(colorSquare1.CurrentHue, (float)colorSquare1.CurrentSat / 240.0f, (float)saturationSlider1.CurrentLum / 240.0f); ;
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		private void saturationSlider1_LumChanged(object sender, EventArgs e)
		{
			if (skipSet)
				return;

			var c = new HSL(colorSquare1.CurrentHue, (float)colorSquare1.CurrentSat / 240.0f, (float)saturationSlider1.CurrentLum / 240.0f); ;
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		ViewMode _currentViewMode = ViewMode.Perspective;

		void SetViewMode(ViewMode newMode)
		{
			toolStripButton4.Checked = toolStripButton5.Checked = false;
			dPerspectiveToolStripMenuItem.Checked = dTextureToolStripMenuItem.Checked = false;
			_currentViewMode = newMode;

			switch (_currentViewMode)
			{
			case ViewMode.Orthographic:
				toolStripButton5.Checked = true;
				dTextureToolStripMenuItem.Checked = true;
				break;
			case ViewMode.Perspective:
				toolStripButton4.Checked = true;
				dPerspectiveToolStripMenuItem.Checked = true;
				break;
			}

			glControl1_Resize(glControl1, null);
		}

		private void dPerspectiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Perspective);
		}

		private void dTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Orthographic);
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Perspective);
		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Orthographic);
		}

		void SetTransparencyMode(TransparencyMode trans)
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

			glControl1.Invalidate();
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

		void ToggleVisiblePart(VisiblePartFlags flag)
		{
			GlobalSettings.ViewFlags ^= flag;

			bool hasNow = (GlobalSettings.ViewFlags & flag) != 0;

			ToolStripMenuItem item = null;

			// TODO: ugly
			switch (flag)
			{
			case VisiblePartFlags.HeadFlag:
				item = headToolStripMenuItem;
				break;
			case VisiblePartFlags.HelmetFlag:
				item = helmetToolStripMenuItem;
				break;
			case VisiblePartFlags.ChestFlag:
				item = chestToolStripMenuItem;
				break;
			case VisiblePartFlags.LeftArmFlag:
				item = leftArmToolStripMenuItem;
				break;
			case VisiblePartFlags.RightArmFlag:
				item = rightArmToolStripMenuItem;
				break;
			case VisiblePartFlags.LeftLegFlag:
				item = leftLegToolStripMenuItem;
				break;
			case VisiblePartFlags.RightLegFlag:
				item = rightLegToolStripMenuItem;
				break;
			}

			item.Checked = hasNow;

			glControl1.Invalidate();
		}

		private void headToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.HeadFlag);
		}

		private void helmetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.HelmetFlag);
		}

		private void chestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.ChestFlag);
		}

		private void leftArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.LeftArmFlag);
		}

		private void rightArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.RightArmFlag);
		}

		private void leftLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.LeftLegFlag);
		}

		private void rightLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.RightLegFlag);
		}

		void ToggleAlphaCheckerboard()
		{
			GlobalSettings.AlphaCheckerboard = !GlobalSettings.AlphaCheckerboard;
			alphaCheckerboardToolStripMenuItem.Checked = GlobalSettings.AlphaCheckerboard;
			glControl1.Invalidate();
		}

		private void alphaCheckerboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleAlphaCheckerboard();
		}

		void ToggleOverlay()
		{
			GlobalSettings.TextureOverlay = !GlobalSettings.TextureOverlay;
			textureOverlayToolStripMenuItem.Checked = GlobalSettings.TextureOverlay;
			glControl1.Invalidate();
		}

		private void textureOverlayToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleOverlay();
		}

		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.alteredsoftworks.com");
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformUndo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformRedo();
		}

		void ToggleTransparencyMode()
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

		void ToggleViewMode()
		{
			switch (_currentViewMode)
			{
			case ViewMode.Orthographic:
				SetViewMode(ViewMode.Perspective);
				break;
			case ViewMode.Perspective:
				SetViewMode(ViewMode.Orthographic);
				break;
			}
		}

		private bool PerformShortcut(Keys key, Keys modifiers)
		{
			foreach (var shortcut in _shortcutEditor.Shortcuts)
			{
				if ((shortcut.Keys & ~Keys.Modifiers) == key &&
					(shortcut.Keys & ~(shortcut.Keys & ~Keys.Modifiers)) == modifiers)
				{
					shortcut.Pressed();
					return true;
				}
			}

			return false;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (PerformShortcut(e.KeyCode & ~Keys.Modifiers, e.Modifiers))
			{
				e.Handled = true;
				return;
			}
			
			base.OnKeyDown(e);
		}

		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
		}

		private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Camera);
		}

		private void pencilToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Pencil);
		}

		private void dropperToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Dropper);
		}

		private void redColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb(color.A, e.NewValue, color.G, color.B));
		}

		private void greenColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb(color.A, color.R, e.NewValue, color.B));
		}

		private void blueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb(color.A, color.R, color.G, e.NewValue));
		}

		private void swatchContainer1_SwatchChanged(object sender, SwatchChangedEventArgs e)
		{
			SetColor(e.Swatch);
		}

		private void alphaColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (skipSet)
				return;

			SetColor(Color.FromArgb(e.NewValue, color.R, color.G, color.B));
		}

		private void dodgeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Dodge);
		}

		private void burnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Burn);
		}

		private void dodgeToolStripButton_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Dodge);
		}

		private void burnToolStripButton_Click(object sender, EventArgs e)
		{
			SetTool(Tools.Burn);
		}

		private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_shortcutEditor.ShowDialog();
		}

		private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (MultiPainter.ColorPicker picker = new MultiPainter.ColorPicker())
			{
				picker.CurrentColor = GlobalSettings.BackgroundColor;

				if (picker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					GlobalSettings.BackgroundColor = picker.CurrentColor;

					glControl1.Invalidate();
				}
			}
		}

		Bitmap CopyScreenToBitmap()
		{
			glControl1.MakeCurrent();
			Bitmap b = new Bitmap(glControl1.Width, glControl1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int[] pixels = new int[glControl1.Width * glControl1.Height];
			GL.ReadPixels(0, 0, glControl1.Width, glControl1.Height, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

			var locked = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			unsafe
			{
				fixed (void *inPixels = pixels)
				{
					void *outPixels = locked.Scan0.ToPointer();

					int *inInt = (int*)inPixels;
					int *outInt = (int*)outPixels;

					for (int y = 0; y < b.Height; ++y)
						for (int x = 0; x < b.Width; ++x)
						{
							var color = Color.FromArgb((*inInt >> 24) & 0xFF, (*inInt >> 0) & 0xFF, (*inInt >> 8) & 0xFF, (*inInt >> 16) & 0xFF);
							*outInt = color.ToArgb();

							inInt++;
							outInt++;
						}
				}
			}

			b.UnlockBits(locked);
			b.RotateFlip(RotateFlipType.RotateNoneFlipY);

			return b;
		}

		void TakeScreenshot()
		{
			Clipboard.SetImage(CopyScreenToBitmap());
		}

		void SaveScreenshot()
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter = "PNG Image|*.png";

				if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					using (var bmp = CopyScreenToBitmap())
						bmp.Save(sfd.FileName);
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
	}
}
