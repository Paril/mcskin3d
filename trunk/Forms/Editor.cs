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
using Devcorp.Controls.Design;
using MB.Controls;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Security.Cryptography;
using Paril.Settings;
using Paril.Settings.Serializers;
using Paril.Components;
using Paril.Components.Shortcuts;
using Paril.Components.Update;
using System.Runtime.InteropServices;
using System.Collections;
using Paril.Compatibility;
using System.Drawing.Drawing2D;
using Paril.OpenGL;
using OpenTK.Graphics;
using System.Windows.Forms.VisualStyles;
using DragDropLib;
using Paril.Extensions;
using MCSkin3D.Language;

namespace MCSkin3D
{
	public partial class Editor : Form
	{
		// ===============================================
		// Private/Static variables
		// ===============================================
		#region Variables
		Updater _updater;

		ColorSliderRenderer redRenderer, greenRenderer, blueRenderer, alphaRenderer;
		HueSliderRenderer hueRenderer;
		SaturationSliderRenderer saturationRenderer;
		LuminanceSliderRenderer lightnessRenderer;

		static ShortcutEditor _shortcutEditor = new ShortcutEditor();
		int _grassTop;
		int _alphaTex;
		int _previewPaint;
		Dictionary<Size, int> _charPaintSizes = new Dictionary<Size, int>();

		float _animationTime = 0;
		float _3dZoom = -80;
		float _2dCamOffsetX = 0;
		float _2dCamOffsetY = 0;
		float _2dZoom = 8;
		float _3dRotationX = 0, _3dRotationY = 0;
		bool _mouseIsDown = false;
		Point _mousePoint;
		UndoBuffer _currentUndoBuffer = null;
		Skin _lastSkin = null;
		bool _skipListbox = false;
		internal PleaseWait _pleaseWaitForm;
		Color _primaryColor = Color.FromArgb(255, 255, 255, 255), _secondaryColor = Color.FromArgb(255, 0, 0, 0);
		bool _skipColors = false;
		ViewMode _currentViewMode = ViewMode.Perspective;
		Renderer _renderer;
		List<BackgroundImage> _backgrounds = new List<BackgroundImage>();
		int _selectedBackground = 0;
		GLControl rendererControl;
		int _toolboxUpNormal, _toolboxUpHover, _toolboxDownNormal, _toolboxDownHover;

		List<ToolIndex> _tools = new List<ToolIndex>();
		ToolIndex _selectedTool;
		#endregion

		public DodgeBurnOptions DodgeBurnOptions { get; private set; }
		public DarkenLightenOptions DarkenLightenOptions { get; private set; }
		public PencilOptions PencilOptions { get; private set; }
		public FloodFillOptions FloodFillOptions { get; private set; }

		// ===============================================
		// Constructor
		// ===============================================
		#region Constructor
		public Editor()
		{
			Program.MainForm = this;
			InitializeComponent();

			bool settingsLoaded = GlobalSettings.Load();

			Icon = Properties.Resources.Icon_new;

			LanguageLoader.LoadLanguages("Languages");

			DodgeBurnOptions = new DodgeBurnOptions();
			DarkenLightenOptions = new DarkenLightenOptions();
			PencilOptions = new PencilOptions();
			FloodFillOptions = new FloodFillOptions();

			_tools.Add(new ToolIndex(new CameraTool(), null, "T_TOOL_CAMERA", Properties.Resources.eye__1_, Keys.C));
			_tools.Add(new ToolIndex(new PencilTool(), PencilOptions, "T_TOOL_PENCIL", Properties.Resources.pen, Keys.P));
			_tools.Add(new ToolIndex(new EraserTool(), new EraserOptions(), "T_TOOL_ERASER", Properties.Resources.erase, Keys.E));
			_tools.Add(new ToolIndex(new DropperTool(), null, "T_TOOL_DROPPER", Properties.Resources.pipette, Keys.D));
			_tools.Add(new ToolIndex(new DodgeBurnTool(), DodgeBurnOptions, "T_TOOL_DODGEBURN", Properties.Resources.dodge, Keys.B));
			_tools.Add(new ToolIndex(new DarkenLightenTool(), DarkenLightenOptions, "T_TOOL_DARKENLIGHTEN", Properties.Resources.darkenlighten, Keys.L));
			_tools.Add(new ToolIndex(new FloodFillTool(), FloodFillOptions, "T_TOOL_BUCKET", Properties.Resources.fill_bucket, Keys.F));
			_tools.Add(new ToolIndex(new NoiseTool(), null, "T_TOOL_NOISE", Properties.Resources.noise, Keys.F));

			animateToolStripMenuItem.Checked = GlobalSettings.Animate;
			followCursorToolStripMenuItem.Checked = GlobalSettings.FollowCursor;
			grassToolStripMenuItem.Checked = GlobalSettings.Grass;
			ghostHiddenPartsToolStripMenuItem.Checked = GlobalSettings.Ghost;

			alphaCheckerboardToolStripMenuItem.Checked = GlobalSettings.AlphaCheckerboard;
			textureOverlayToolStripMenuItem.Checked = GlobalSettings.TextureOverlay;

			SetCheckbox(VisiblePartFlags.HeadFlag, headToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.ChestFlag, chestToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.LeftArmFlag, leftArmToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.RightArmFlag, rightArmToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.HelmetFlag, helmetToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.LeftLegFlag, leftLegToolStripMenuItem);
			SetCheckbox(VisiblePartFlags.RightLegFlag, rightLegToolStripMenuItem);

			Brushes.LoadBrushes();

			InitShortcuts();
			LoadShortcutKeys(GlobalSettings.ShortcutKeys);

			Language.Language useLanguage = null;
			// stage 1: load from last used language
			useLanguage = LanguageLoader.FindLanguage(GlobalSettings.LanguageFile);
			// stage 2: use English file, if it exists
			if (useLanguage == null)
				useLanguage = LanguageLoader.FindLanguage("English");
			// stage 3: fallback to built-in English file
			if (useLanguage == null)
			{
				MessageBox.Show("For some reason, the default language files were missing (did you extract?) - we'll supply you with a base language of English just so you know what you're doing!");				
				useLanguage = LanguageLoader.LoadDefault();
			}

			foreach (var lang in LanguageLoader.Languages)
			{
				lang.Item = new ToolStripMenuItem((lang.Culture != null) ? (char.ToUpper(lang.Culture.NativeName[0]) + lang.Culture.NativeName.Substring(1)) : lang.Name);
				lang.Item.Tag = lang;
				lang.Item.Click += new EventHandler(languageToolStripMenuItem_Click);
				languageToolStripMenuItem.DropDownItems.Add(lang.Item);
			}

			for (int i = _tools.Count - 1; i >= 0; --i)
			{
				toolToolStripMenuItem.DropDownItems.Insert(0, _tools[i].MenuItem);
				_tools[i].MenuItem.Click += ToolMenuItemClicked;
				toolStrip1.Items.Insert(6, _tools[i].Button);
				_tools[i].Button.Click += ToolMenuItemClicked;

				languageProvider1.SetPropertyNames(_tools[i].MenuItem, "Text");
				languageProvider1.SetPropertyNames(_tools[i].Button, "Text");
			}

			CurrentLanguage = useLanguage;

			SetSelectedTool(_tools[0]);

			if (Screen.PrimaryScreen.BitsPerPixel != 32)
			{
				MessageBox.Show(GetLanguageString("B_MSG_PIXELFORMAT"), GetLanguageString("B_CAP_SORRY"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}

			redColorSlider.Renderer = redRenderer = new ColorSliderRenderer(redColorSlider);
			greenColorSlider.Renderer = greenRenderer = new ColorSliderRenderer(greenColorSlider);
			blueColorSlider.Renderer = blueRenderer = new ColorSliderRenderer(blueColorSlider);
			alphaColorSlider.Renderer = alphaRenderer = new ColorSliderRenderer(alphaColorSlider);

			hueColorSlider.Renderer = hueRenderer = new HueSliderRenderer(hueColorSlider);
			saturationColorSlider.Renderer = saturationRenderer = new SaturationSliderRenderer(saturationColorSlider);
			lightnessColorSlider.Renderer = lightnessRenderer = new LuminanceSliderRenderer(lightnessColorSlider);

			KeyPreview = true;
			Text = "MCSkin3D v" + ProductVersion[0] + '.' + ProductVersion[2];

			if (!Directory.Exists("Swatches") || !Directory.Exists("Skins"))
				MessageBox.Show(GetLanguageString("B_MSG_DIRMISSING"));

			Directory.CreateDirectory("Swatches");
			Directory.CreateDirectory("Skins");
			swatchContainer.AddDirectory("Swatches");

			_updater = new Updater("http://alteredsoftworks.com/mcskin3d/update", "" + ProductVersion[0] + '.' + ProductVersion[2]);
			_updater.UpdateHandler = new AssemblyVersion();
			_updater.NewVersionAvailable += _updater_NewVersionAvailable;
			_updater.SameVersion += _updater_SameVersion;
			_updater.CheckForUpdate();

			automaticallyCheckForUpdatesToolStripMenuItem.Checked = GlobalSettings.AutoUpdate;

			PlayerModel.LoadModel();

			SetSampleMenuItem(GlobalSettings.Multisamples);

			// set up the GL control
			rendererControl = new GLControl(new GraphicsMode(new ColorFormat(32), 16, 0, GlobalSettings.Multisamples));
			rendererControl.BackColor = System.Drawing.Color.Black;
			rendererControl.Dock = System.Windows.Forms.DockStyle.Fill;
			rendererControl.Location = new System.Drawing.Point(0, 25);
			rendererControl.Name = "rendererControl";
			rendererControl.Size = new System.Drawing.Size(641, 580);
			rendererControl.TabIndex = 4;
			rendererControl.VSync = true;
			rendererControl.Load += new System.EventHandler(this.rendererControl_Load);
			rendererControl.Paint += new System.Windows.Forms.PaintEventHandler(this.rendererControl_Paint);
			rendererControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rendererControl_MouseDown);
			rendererControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rendererControl_MouseMove);
			rendererControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rendererControl_MouseUp);
			rendererControl.MouseLeave += new EventHandler(rendererControl_MouseLeave);
			rendererControl.Resize += new System.EventHandler(this.rendererControl_Resize);
			rendererControl.MouseWheel += new MouseEventHandler(rendererControl_MouseWheel);

			splitContainer4.Panel2.Controls.Add(rendererControl);
			rendererControl.BringToFront();

			System.Timers.Timer animTimer = new System.Timers.Timer();
			animTimer.Interval = 22;
			animTimer.Elapsed += new System.Timers.ElapsedEventHandler(animTimer_Elapsed);
			animTimer.Start();

			_animTimer.Elapsed += new System.Timers.ElapsedEventHandler(_animTimer_Elapsed);
			_animTimer.SynchronizingObject = this;

			_shortcutEditor.ShortcutExists += new EventHandler<ShortcutExistsEventArgs>(_shortcutEditor_ShortcutExists);

			if (!settingsLoaded)
				MessageBox.Show(GetLanguageString("C_SETTINGSFAILED"));

			treeView1.ItemHeight = GlobalSettings.TreeViewHeight;
		}

		void _shortcutEditor_ShortcutExists(object sender, ShortcutExistsEventArgs e)
		{
			MessageBox.Show(string.Format(GetLanguageString("B_MSG_SHORTCUTEXISTS"), e.ShortcutName, e.OtherName));
		}
		#endregion

		public GLControl Renderer
		{
			get { return rendererControl; }
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

		// =====================================================================
		// Updating
		// =====================================================================
		#region Update
		public void Invoke(Action action)
		{
			this.Invoke((Delegate)action);
		}

		void _updater_SameVersion(object sender, EventArgs e)
		{
			this.Invoke(() => MessageBox.Show(GetLanguageString("B_MSG_UPTODATE")));
		}

		void _updater_NewVersionAvailable(object sender, EventArgs e)
		{
			this.Invoke(delegate()
			{
				if (MessageBox.Show(GetLanguageString("B_MSG_NEWUPDATE"), "Woo!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
					Process.Start("http://www.minecraftforum.net/topic/746941-mcskin3d-new-skinning-program/");
			});
		}
		#endregion

		// =====================================================================
		// Shortcuts
		// =====================================================================
		#region Shortcuts

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
					c += shortcut.SaveName + "=" + key + "+" + modifiers;
				else
					c += shortcut.SaveName + "=" + key;
			}

			return c;
		}

		IShortcutImplementor FindShortcut(string name)
		{
			foreach (var s in _shortcutEditor.Shortcuts)
			{
				if (s.SaveName == name)
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

		void InitMenuShortcut(ToolStripMenuItem item, Keys keys, Action callback)
		{
			MenuStripShortcut shortcut = new MenuStripShortcut(item, keys);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		void InitUnlinkedShortcut(string name, Keys defaultKeys, Action callback)
		{
			ShortcutBase shortcut = new ShortcutBase(name, defaultKeys);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		void InitControlShortcut(string name, Control control, Keys defaultKeys, Action callback)
		{
			ControlShortcut shortcut = new ControlShortcut(name, defaultKeys, control);
			shortcut.Pressed = callback;

			_shortcutEditor.AddShortcut(shortcut);
		}

		void InitShortcuts()
		{
			// shortcut menus
			InitMenuShortcut(undoToolStripMenuItem, PerformUndo);
			InitMenuShortcut(redoToolStripMenuItem, PerformRedo);
			InitMenuShortcut(perspectiveToolStripMenuItem, () => SetViewMode(ViewMode.Perspective));
			InitMenuShortcut(textureToolStripMenuItem, () => SetViewMode(ViewMode.Orthographic));
			InitMenuShortcut(hybridViewToolStripMenuItem, () => SetViewMode(ViewMode.Hybrid));
			InitMenuShortcut(animateToolStripMenuItem, ToggleAnimation);
			InitMenuShortcut(followCursorToolStripMenuItem, ToggleFollowCursor);
			InitMenuShortcut(grassToolStripMenuItem, ToggleGrass);
			InitMenuShortcut(ghostHiddenPartsToolStripMenuItem, ToggleGhosting);
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
			InitMenuShortcut(uploadToolStripMenuItem, PerformUpload);

			foreach (var item in _tools)
				InitMenuShortcut(item.MenuItem, item.DefaultKeys, () => SetSelectedTool(item));

			// not in the menu
			InitUnlinkedShortcut("S_TOGGLETRANS", Keys.Shift | Keys.U, ToggleTransparencyMode);
			InitUnlinkedShortcut("S_TOGGLEVIEW", Keys.Control | Keys.V, ToggleViewMode);
			InitUnlinkedShortcut("S_SCREENSHOT_CLIP", Keys.Control | Keys.B, TakeScreenshot);
			InitUnlinkedShortcut("S_SCREENSHOT_SAVE", Keys.Control | Keys.Shift | Keys.B, SaveScreenshot);
			InitUnlinkedShortcut("S_DELETE", Keys.Delete, PerformDeleteSkin);
			InitUnlinkedShortcut("S_CLONE", Keys.Control | Keys.C, PerformCloneSkin);
			InitUnlinkedShortcut("S_RENAME", Keys.Control | Keys.N, PerformNameChange);
			InitUnlinkedShortcut("S_COLORSWAP", Keys.S, PerformSwitchColor);
			InitControlShortcut("S_SWATCH_ZOOMIN", swatchContainer.SwatchDisplayer, Keys.Oemplus, PerformSwatchZoomIn);
			InitControlShortcut("S_SWATCH_ZOOMOUT", swatchContainer.SwatchDisplayer, Keys.OemMinus, PerformSwatchZoomOut);
			InitControlShortcut("S_TREEVIEW_ZOOMIN", treeView1, Keys.Control | Keys.Oemplus, PerformTreeViewZoomIn);
			InitControlShortcut("S_TREEVIEW_ZOOMOUT", treeView1, Keys.Control | Keys.OemMinus, PerformTreeViewZoomOut);
		}

		void PerformSwitchColor()
		{
			if (_secondaryIsFront)
				colorPreview1_Click(null, null);
			else
				colorPreview2_Click(null, null);
		}

		void SetSelectedTool(ToolIndex index)
		{
			if (_selectedTool != null)
				_selectedTool.MenuItem.Checked = _selectedTool.Button.Checked = false;

			var oldTool = _selectedTool;
			_selectedTool = index;
			index.MenuItem.Checked = index.Button.Checked = true;

			splitContainer4.Panel1.Controls.Clear();

			if (oldTool != null && oldTool.OptionsPanel != null)
				oldTool.OptionsPanel.BoxHidden();
			if (_selectedTool.OptionsPanel != null)
				_selectedTool.OptionsPanel.BoxShown();

			if (_selectedTool.OptionsPanel != null)
				splitContainer4.Panel1.Controls.Add(_selectedTool.OptionsPanel);

			toolStripStatusLabel1.Text = index.Tool.GetStatusLabelText();
		}

		void ToolMenuItemClicked(object sender, EventArgs e)
		{
			ToolStripItem item = (ToolStripItem)sender;
			SetSelectedTool((ToolIndex)item.Tag);
		}

		void PerformCamera()
		{
		}

		void PerformTreeViewZoomIn()
		{
			treeView1.ZoomIn();
		}

		void PerformTreeViewZoomOut()
		{
			treeView1.ZoomOut();
		}

		void PerformSwatchZoomOut()
		{
			swatchContainer.ZoomOut();
		}

		void PerformSwatchZoomIn()
		{
			swatchContainer.ZoomIn();
		}

		bool PerformShortcut(Keys key, Keys modifiers)
		{
			foreach (var shortcut in _shortcutEditor.Shortcuts)
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

		// =====================================================================
		// Overrides
		// =====================================================================
		#region Overrides
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (PerformShortcut(e.KeyCode & ~Keys.Modifiers, e.Modifiers))
			{
				e.Handled = true;
				return;
			}

			base.OnKeyDown(e);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			_updater.Abort();
			GlobalSettings.ShortcutKeys = CompileShortcutKeys();

			GlobalSettings.Save();
		}

		TreeNode _tempToSelect;
		void RecurseAddDirectories(string path, TreeNodeCollection nodes, List<Skin> skins)
		{
			var di = new DirectoryInfo(path);

			foreach (var file in di.GetFiles("*.png", SearchOption.TopDirectoryOnly))
			{
				var skin = new Skin(file);
				nodes.Add(skin);

				if (_tempToSelect == null)
					_tempToSelect = skin;
				else if (GlobalSettings.LastSkin == skin.Name)
					_tempToSelect = skin;

				skins.Add(skin);
			}

			foreach (var dir in di.GetDirectories())
			{
				if ((dir.Attributes & FileAttributes.Hidden) != 0)
					continue;

				FolderNode folderNode = new FolderNode(dir.Name);
				RecurseAddDirectories(dir.FullName, folderNode.Nodes, skins);
				nodes.Add(folderNode);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			SetTransparencyMode(GlobalSettings.Transparency);
			SetViewMode(_currentViewMode);

			rendererControl.MakeCurrent();

			List<Skin> skins = new List<Skin>();
			RecurseAddDirectories("Skins", treeView1.Nodes, skins);

			foreach (var s in skins)
				s.SetImages();

			treeView1.SelectedNode = _tempToSelect;

			SetColor(Color.White);
			SetVisibleParts();

			toolToolStripMenuItem.DropDown.Closing += DontCloseMe;
			modeToolStripMenuItem.DropDown.Closing += DontCloseMe;
			threeDToolStripMenuItem.DropDown.Closing += DontCloseMe;
			twoDToolStripMenuItem.DropDown.Closing += DontCloseMe;
			transparencyModeToolStripMenuItem.DropDown.Closing += DontCloseMe;
			visiblePartsToolStripMenuItem.DropDown.Closing += DontCloseMe;		
		}

		void DontCloseMe(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked ||
				e.CloseReason == ToolStripDropDownCloseReason.Keyboard)
				e.Cancel = true;
		}

		#endregion

		// =====================================================================
		// Private functions
		// =====================================================================
		#region Private Functions
		// Utility function, sets a tool strip checkbox item's state if the flag is present
		void SetCheckbox(VisiblePartFlags flag, ToolStripMenuItem checkbox)
		{
			if ((GlobalSettings.ViewFlags & flag) != 0)
				checkbox.Checked = true;
			else
				checkbox.Checked = false;
		}

		int GetPaintTexture(int width, int height)
		{
			if (!_charPaintSizes.ContainsKey(new Size(width, height)))
			{
				int id = GL.GenTexture();

				byte[] arra = new byte[width * height * 4];
				unsafe
				{
					fixed (byte* texData = arra)
					{
						byte *d = texData;

						for (int y = 0; y < height; ++y)
							for (int x = 0; x < width; ++x)
							{
								*((int*)d) = (x << 0) | (y << 8) | (0 << 16) | (255 << 24);
								d += 4;
							}
					}
				}

				RenderState.BindTexture(id);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arra);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

				_charPaintSizes.Add(new Size(width, height), id);

				return id;
			}

			return _charPaintSizes[new Size(width, height)];
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
			RenderState.BindTexture(texture);

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

		void DrawPlayer2D(int tex, Skin skin, bool pickView)
		{
			if (!pickView && GlobalSettings.AlphaCheckerboard)
			{
				RenderState.BindTexture(_alphaTex);

				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
				GL.TexCoord2(_currentViewport.Width / 32.0f, 0); GL.Vertex2(_currentViewport.Width, 0);
				GL.TexCoord2(_currentViewport.Width / 32.0f, _currentViewport.Height / 32.0f); GL.Vertex2(_currentViewport.Width, _currentViewport.Height);
				GL.TexCoord2(0, _currentViewport.Height / 32.0f); GL.Vertex2(0, _currentViewport.Height);
				GL.End();
			}

			if (skin != null)
				RenderState.BindTexture(tex);

			GL.PushMatrix();

			GL.Translate((_currentViewport.Width / 2) + -_2dCamOffsetX, (_currentViewport.Height / 2) + -_2dCamOffsetY, 0);
			GL.Scale(_2dZoom, _2dZoom, 1);

			if (pickView)
				GL.Disable(EnableCap.Blend);
			else
				GL.Enable(EnableCap.Blend);

			GL.Translate((_2dCamOffsetX), (_2dCamOffsetY), 0);
			if (skin != null)
			{
				float w = skin.Width;
				float h = skin.Height;
				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0, 0); GL.Vertex2(-(skin.Width / 2), -(skin.Height / 2));
				GL.TexCoord2(1, 0); GL.Vertex2((skin.Width / 2), -(skin.Height / 2));
				GL.TexCoord2(1, 1); GL.Vertex2((skin.Width / 2), (skin.Height / 2));
				GL.TexCoord2(0, 1); GL.Vertex2(-(skin.Width / 2), (skin.Height / 2));
				GL.End();
			}

			if (!pickView && GlobalSettings.TextureOverlay && skin != null &&
				_backgrounds[_selectedBackground].GLImage != 0)
			{
				RenderState.BindTexture(_backgrounds[_selectedBackground].GLImage);

				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0, 0); GL.Vertex2(-(skin.Width / 2), -(skin.Height / 2));
				GL.TexCoord2(1, 0); GL.Vertex2((skin.Width / 2), -(skin.Height / 2));
				GL.TexCoord2(1, 1); GL.Vertex2((skin.Width / 2), (skin.Height / 2));
				GL.TexCoord2(0, 1); GL.Vertex2(-(skin.Width / 2), (skin.Height / 2));
				GL.End();
			}
			GL.PopMatrix();

			GL.Disable(EnableCap.Blend);
		}

		void DrawPlayer(int tex, Skin skin, bool pickView)
		{
			Vector3 vec = new Vector3();
			int count = 0;
			bool grass = !pickView && grassToolStripMenuItem.Checked;

			foreach (var mesh in PlayerModel.HumanModel.Meshes)
			{
				if ((GlobalSettings.ViewFlags & mesh.Part) != 0)
				{
					vec += mesh.Translate;
					count++;
				}
			}

			if (count != 0)
				vec /= count;

			GL.Translate(0, 0, _3dZoom);
			GL.Rotate(_3dRotationX, 1, 0, 0);
			GL.Rotate(_3dRotationY, 0, 1, 0);

			GL.Translate(-vec.X, -vec.Y, 0);

			var clPt = rendererControl.PointToClient(Cursor.Position);
			var x = clPt.X - (_currentViewport.Width / 2);
			var y = clPt.Y - (_currentViewport.Height / 2);

			if (!pickView && GlobalSettings.Transparency == TransparencyMode.All)
				GL.Enable(EnableCap.Blend);
			else
				GL.Disable(EnableCap.Blend);

			if (grass)
				DrawSkinnedRectangle(0, -20, 0, 1024, 4, 1024, 0, 0, 1024, 1024, 0, 0, 0, 0, 0, 0, 1024, 1024, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, _grassTop, 16, 16);

			Vector3 helmetRotate = (GlobalSettings.FollowCursor) ? new Vector3((float)y / 25, (float)x / 25, 0) : Vector3.Zero;
			double sinAnim = (GlobalSettings.Animate) ? Math.Sin(_animationTime) : 0;

			// draw non-transparent meshes
			foreach (var mesh in PlayerModel.HumanModel.Meshes)
			{
				if (mesh.Helmet)
					continue;
				if ((GlobalSettings.ViewFlags & mesh.Part) == 0)
					continue;

				var newMesh = mesh;

				newMesh.Texture = tex;

				if (mesh.FollowCursor && GlobalSettings.FollowCursor)
					newMesh.Rotate = helmetRotate;

				foreach (var f in mesh.Faces)
					for (int i = 0; i < f.Colors.Length; ++i)
						f.Colors[i] = Color4.White;

				if (GlobalSettings.Animate && mesh.RotateFactor != 0)
					newMesh.Rotate += new Vector3((float)sinAnim * mesh.RotateFactor, 0, 0);

				_renderer.Meshes.Add(newMesh);
			}

			_renderer.Render();

			// Draw ghosted parts
			if (GlobalSettings.Ghost && !pickView)
			{
				foreach (var mesh in PlayerModel.HumanModel.Meshes)
				{
					if (mesh.Helmet)
						continue;
					if ((GlobalSettings.ViewFlags & mesh.Part) != 0)
						continue;

					var newMesh = mesh;

					newMesh.Texture = tex;

					if (mesh.FollowCursor && GlobalSettings.FollowCursor)
						newMesh.Rotate = helmetRotate;

					foreach (var f in mesh.Faces)
						for (int i = 0; i < f.Colors.Length; ++i)
							f.Colors[i] = new Color4(1, 1, 1, 0.25f);

					if (GlobalSettings.Animate && mesh.RotateFactor != 0)
						newMesh.Rotate += new Vector3((float)sinAnim * mesh.RotateFactor, 0, 0);

					_renderer.Meshes.Add(newMesh);
				}

				GL.Enable(EnableCap.Blend);
				_renderer.Render();
				GL.Disable(EnableCap.Blend);
			}

			if (!pickView && GlobalSettings.Transparency != TransparencyMode.Off)
				GL.Enable(EnableCap.Blend);
			else
				GL.Disable(EnableCap.Blend);

			// draw transparent meshes
			foreach (var mesh in PlayerModel.HumanModel.Meshes)
			{
				if (!mesh.Helmet)
					continue;
				if ((GlobalSettings.ViewFlags & mesh.Part) == 0)
					continue;

				var newMesh = mesh;

				newMesh.Texture = tex;

				if (mesh.FollowCursor && GlobalSettings.FollowCursor)
					newMesh.Rotate = helmetRotate;

				foreach (var f in mesh.Faces)
					for (int i = 0; i < f.Colors.Length; ++i)
						f.Colors[i] = Color4.White;

				if (GlobalSettings.Animate && mesh.RotateFactor != 0)
					newMesh.Rotate += new Vector3((float)sinAnim * mesh.RotateFactor, 0, 0);

				_renderer.Meshes.Add(newMesh);
			}

			_renderer.Render();

			// Draw ghosted parts
			if (GlobalSettings.Ghost && !pickView)
			{
				foreach (var mesh in PlayerModel.HumanModel.Meshes)
				{
					if (!mesh.Helmet)
						continue;
					if ((GlobalSettings.ViewFlags & mesh.Part) != 0)
						continue;

					var newMesh = mesh;

					newMesh.Texture = tex;

					if (mesh.FollowCursor && GlobalSettings.FollowCursor)
						newMesh.Rotate = helmetRotate;

					foreach (var f in mesh.Faces)
						for (int i = 0; i < f.Colors.Length; ++i)
							f.Colors[i] = new Color4(1, 1, 1, 0.25f);

					if (GlobalSettings.Animate && mesh.RotateFactor != 0)
						newMesh.Rotate += new Vector3((float)sinAnim * mesh.RotateFactor, 0, 0);

					_renderer.Meshes.Add(newMesh);
				}

				GL.Enable(EnableCap.Blend);
				_renderer.Render();
				GL.Disable(EnableCap.Blend);
			}

		}

		void SetPreview()
		{
			if (_lastSkin == null)
			{
				int[] array = new int[64 * 32];
				RenderState.BindTexture(_previewPaint);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
			}
			else
			{
				Skin skin = _lastSkin;

				RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
				int[] array = new int[skin.Width * skin.Height];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				Point p = Point.Empty;

				var pick = GetPick(_mousePoint.X, _mousePoint.Y, ref p);
				{
					if (_selectedTool.Tool.RequestPreview(array, skin, p.X, p.Y))
					{
						RenderState.BindTexture(_previewPaint);
						GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, skin.Width, skin.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
					}
					else
					{
						RenderState.BindTexture(_previewPaint);
						GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, skin.Width, skin.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
					}
				}
			}
		}

		bool GetPick(int x, int y, ref Point hitPixel)
		{
			rendererControl.MakeCurrent();

			GL.ClearColor(Color.White);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.ClearColor(GlobalSettings.BackgroundColor);

			var skin = _lastSkin;

			if (_currentViewMode == ViewMode.Perspective)
			{
				Setup3D(new Rectangle(0, 0, rendererControl.Width, rendererControl.Height));
				DrawPlayer(GetPaintTexture(skin.Width, skin.Height), skin, true);
			}
			else if (_currentViewMode == ViewMode.Orthographic)
			{
				Setup2D(new Rectangle(0, 0, rendererControl.Width, rendererControl.Height));
				DrawPlayer2D(GetPaintTexture(skin.Width, skin.Height), skin, true);
			}
			else
			{
				int halfHeight = (int)Math.Ceiling(rendererControl.Height / 2.0f);

				Setup3D(new Rectangle(0, 0, rendererControl.Width, halfHeight));
				DrawPlayer(GetPaintTexture(skin.Width, skin.Height), skin, true);

				Setup2D(new Rectangle(0, halfHeight, rendererControl.Width, halfHeight));
				DrawPlayer2D(GetPaintTexture(skin.Width, skin.Height), skin, true);
			}

			GL.Flush();

			byte[] pixel = new byte[4];

			GL.ReadPixels(x, rendererControl.Height - y, 1, 1,
				PixelFormat.Rgb, PixelType.UnsignedByte, pixel);

			if (pixel[2] == 0)
			{
				hitPixel = new Point(pixel[0], pixel[1]);
				return true;
			}

			return false;
		}

		public Color SelectedColor
		{
			get { return (_secondaryIsFront) ? _secondaryColor : _primaryColor; }
			set { SetColor(value); }
		}

		public Color UnselectedColor
		{
			get { return (!_secondaryIsFront) ? _secondaryColor : _primaryColor; }
			set
			{
				if (_secondaryIsFront)
					SetColor(colorPreview1, ref _primaryColor, value);
				else
					SetColor(colorPreview2, ref _secondaryColor, value);
			}
		}

		void UseToolOnViewport(int x, int y, bool begin = false)
		{
			if (_lastSkin == null)
				return;

			Point p = Point.Empty;

			if (GetPick(x, y, ref p))
			{
				Skin skin = _lastSkin;

				RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
				int[] array = new int[skin.Width * skin.Height];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				if (_selectedTool.Tool.MouseMoveOnSkin(array, skin, p.X, p.Y))
				{
					SetCanSave(true);
					skin.Dirty = true;
					GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, skin.Width, skin.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				}
			}

			rendererControl.Invalidate();
		}

		#region File uploading (FIXME: REMOVE)
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

		Thread _uploadThread;
		Login login = new Login();

		void PerformUpload()
		{
			if (_lastSkin == null)
				return;

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
			_pleaseWaitForm.FormClosed += new FormClosedEventHandler(_pleaseWaitForm_FormClosed);

			_uploadThread = new Thread(UploadThread);
			ErrorReturn ret = new ErrorReturn();
			_uploadThread.Start(new object[] { login.Username, login.Password, _lastSkin.File.FullName, ret });

			_pleaseWaitForm.DialogResult = DialogResult.OK;
			_pleaseWaitForm.languageProvider1.LanguageChanged(CurrentLanguage);
			_pleaseWaitForm.ShowDialog();
			_uploadThread = null;

			if (ret.ReportedError != null)
				MessageBox.Show(GetLanguageString("B_MSG_UPLOADERROR") + "\r\n" + ret.ReportedError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else if (ret.Exception != null)
				MessageBox.Show(GetLanguageString("B_MSG_UPLOADERROR") + "\r\n" + ret.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else if (_pleaseWaitForm.DialogResult != DialogResult.Abort)
			{
				MessageBox.Show(GetLanguageString("B_MSG_UPLOADSUCCESS"), "Woo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				GlobalSettings.LastSkin = _lastSkin.Name;
				treeView1.Invalidate();
			}

			if (didShowDialog)
			{
				GlobalSettings.RememberMe = login.Remember;
				GlobalSettings.AutoLogin = login.AutoLogin;

				if (GlobalSettings.RememberMe == false)
					GlobalSettings.LastUsername = GlobalSettings.LastPassword = "";
				else
				{
					GlobalSettings.LastUsername = login.Username;
					GlobalSettings.LastPassword = login.Password;
				}
			}
		}

		void _pleaseWaitForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			_uploadThread.Abort();
		}
		#endregion

		void ToggleAnimation()
		{
			animateToolStripMenuItem.Checked = !animateToolStripMenuItem.Checked;
			GlobalSettings.Animate = animateToolStripMenuItem.Checked;

			Invalidate();
		}

		void ToggleFollowCursor()
		{
			followCursorToolStripMenuItem.Checked = !followCursorToolStripMenuItem.Checked;
			GlobalSettings.FollowCursor = followCursorToolStripMenuItem.Checked;

			Invalidate();
		}

		void ToggleGrass()
		{
			grassToolStripMenuItem.Checked = !grassToolStripMenuItem.Checked;
			GlobalSettings.Grass = grassToolStripMenuItem.Checked;

			rendererControl.Invalidate();
		}

		void ToggleGhosting()
		{
			ghostHiddenPartsToolStripMenuItem.Checked = !ghostHiddenPartsToolStripMenuItem.Checked;
			GlobalSettings.Ghost = ghostHiddenPartsToolStripMenuItem.Checked;

			rendererControl.Invalidate();
		}

		#region Skin Management
		void PerformImportSkin()
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Minecraft Skins|*.png";
				ofd.Multiselect = true;

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					string folderLocation;

					if (_rightClickedNode != null)
					{
						if (!(_rightClickedNode is Skin))
							folderLocation = "Skins\\" + _rightClickedNode.FullPath + '\\';
						else if (_rightClickedNode.Parent != null)
							folderLocation = "Skins\\" + _rightClickedNode.Parent.FullPath + '\\';
						else
							folderLocation = "Skins\\";
					}
					else
						folderLocation = "Skins\\";

					foreach (var f in ofd.FileNames)
					{
						var name = Path.GetFileNameWithoutExtension(f);

						while (File.Exists(folderLocation + name + ".png"))
							name += " (New)";

						File.Copy(f, folderLocation + name + ".png");

						Skin skin = new Skin(folderLocation + name + ".png");

						if (_rightClickedNode != null)
						{
							if (!(_rightClickedNode is Skin))
								_rightClickedNode.Nodes.Add(skin);
							else
								_rightClickedNode.Parent.Nodes.Add(skin);
						}
						else
							treeView1.Nodes.Add(skin);

						skin.SetImages();
					}
				}
			}
		}

		void PerformNewFolder()
		{
			string folderLocation;
			TreeNodeCollection collection;

			if (_rightClickedNode != null)
			{
				if (!(_rightClickedNode is Skin))
				{
					folderLocation = "Skins\\" + _rightClickedNode.FullPath + '\\';
					collection = _rightClickedNode.Nodes;
				}
				else if (_rightClickedNode.Parent != null)
				{
					folderLocation = "Skins\\" + _rightClickedNode.Parent.FullPath + '\\';
					collection = _rightClickedNode.Parent.Nodes;
				}
				else
				{
					folderLocation = "Skins\\";
					collection = treeView1.Nodes;
				}
			}
			else
			{
				folderLocation = "Skins\\";
				collection = treeView1.Nodes;
			}

			string newFolderName = "New Folder";

			while (Directory.Exists(folderLocation + newFolderName))
				newFolderName += " (New)";

			Directory.CreateDirectory(folderLocation + newFolderName);
			var newNode = new FolderNode(newFolderName);
			collection.Add(newNode);

			newNode.EnsureVisible();
			treeView1.SelectedNode = newNode;
			treeView1.Invalidate();

			PerformNameChange();
		}

		void RecursiveDeleteSkins(TreeNode node)
		{
			foreach (TreeNode sub in node.Nodes)
			{
				if (!(sub is Skin))
					RecursiveDeleteSkins(sub);
				else
				{
					Skin skin = (Skin)sub;

					if (_lastSkin == skin)
						_lastSkin = null;

					skin.Dispose();
				}
			}

			Directory.Delete("Skins\\" + node.FullPath, true);
		}

		void PerformDeleteSkin()
		{
			if (treeView1.SelectedNode is Skin)
			{
				if (MessageBox.Show(GetLanguageString("B_MSG_DELETESKIN"), GetLanguageString("B_CAP_QUESTION"), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					Skin skin = (Skin)treeView1.SelectedNode;

					skin.File.Delete();
					skin.Remove();
					skin.Dispose();

					Invalidate();
				}
			}
			else
			{
				if (MessageBox.Show(GetLanguageString("B_MSG_DELETEFOLDER"), GetLanguageString("B_CAP_QUESTION"), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
				{
					DirectoryInfo folder = new DirectoryInfo("Skins\\" + treeView1.SelectedNode.FullPath);

					RecursiveDeleteSkins(treeView1.SelectedNode);

					treeView1.SelectedNode.Remove();
					Invalidate();
				}
			}
		}

		void PerformCloneSkin()
		{
			if (treeView1.SelectedNode == null ||
				!(treeView1.SelectedNode is Skin))
				return;

			Skin skin = (Skin)treeView1.SelectedNode;
			string newName = skin.Name;
			string newFileName;

			do
			{
				newName += " - Copy";
				newFileName = skin.Directory.FullName + '\\' + newName + ".png";
			} while (File.Exists(newFileName));

			File.Copy(skin.File.FullName, newFileName);
			Skin newSkin = new Skin(newFileName);

			skin.GetParentCollection().Add(newSkin);

			newSkin.SetImages();
		}

		TreeNode _currentlyEditing = null;
		void PerformNameChange()
		{
			if (treeView1.SelectedNode != null)
			{
				_currentlyEditing = treeView1.SelectedNode;

				if (_currentlyEditing is Skin)
					labelEditTextBox.Text = ((Skin)_currentlyEditing).Name;
				else
					labelEditTextBox.Text = _currentlyEditing.Text;

				labelEditTextBox.Location = new Point(treeView1.SelectedNode.Bounds.Location.X + 24 + (treeView1.SelectedNode.Level * 1), treeView1.Location.Y + treeView1.SelectedNode.Bounds.Location.Y + 4);
				labelEditTextBox.Size = new System.Drawing.Size(treeView1.Width - labelEditTextBox.Location.X - 20, labelEditTextBox.Height);
				labelEditTextBox.BringToFront();
				labelEditTextBox.Show();
				labelEditTextBox.Focus();
			}
		}
		#endregion

		private void DoneEditingNode(string newName, TreeNode _currentlyEditing)
		{
			labelEditTextBox.Hide();

			if (_currentlyEditing is Skin)
			{
				Skin skin = (Skin)_currentlyEditing;

				if (skin.Name == newName)
					return;

				if (skin.ChangeName(newName) == false)
					System.Media.SystemSounds.Question.Play();
			}
			else
			{
				string folderName = _currentlyEditing.Text;
				var folder = new DirectoryInfo("skins\\" + _currentlyEditing.FullPath);
				var newFolder = new DirectoryInfo("skins\\" + ((_currentlyEditing.Parent != null) ? (_currentlyEditing.Parent.FullPath + '\\' + newName) : newName));

				if (folderName == newName)
					return;

				if (Directory.Exists(newFolder.FullName))
				{
					System.Media.SystemSounds.Question.Play();
					return;
				}

				folder.MoveTo(newFolder.FullName);
				_currentlyEditing.Text = newFolder.Name;
			}
		}

		#region Saving
		void SetCanSave(bool value)
		{
			saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = value;
		}

		void PerformSaveAs()
		{
			var skin = _lastSkin;

			RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
			int[] pixels = new int[skin.Width * skin.Height];
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

			Bitmap b = new Bitmap(skin.Width, skin.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

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
			rendererControl.MakeCurrent();

			s.CommitChanges((s == _lastSkin) ? GlobalDirtiness.CurrentSkin : s.GLImage, true);
		}

		void RecursiveNodeSave(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (node is Skin)
				{
					Skin skin = (Skin)node;

					if (skin.Dirty)
						PerformSaveSkin(skin);
				}
				else
					RecursiveNodeSave(node.Nodes);
			}
		}

		void PerformSaveAll()
		{
			RecursiveNodeSave(treeView1.Nodes);
			treeView1.Invalidate();
		}

		void PerformSave()
		{
			Skin skin = _lastSkin;

			if (!skin.Dirty)
				return;

			SetCanSave(false);
			PerformSaveSkin(skin);
			treeView1.Invalidate();
		}
		#endregion

		void PerformUndo()
		{
			if (!_currentUndoBuffer.CanUndo)
				return;

			rendererControl.MakeCurrent();

			_currentUndoBuffer.Undo();

			undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = _currentUndoBuffer.CanUndo;
			redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = _currentUndoBuffer.CanRedo;

			Skin current = _lastSkin;
			SetCanSave(current.Dirty = true);

			rendererControl.Invalidate();
		}

		void PerformRedo()
		{
			if (!_currentUndoBuffer.CanRedo)
				return;

			rendererControl.MakeCurrent();

			_currentUndoBuffer.Redo();

			Skin current = _lastSkin;
			SetCanSave(current.Dirty = true);

			undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = _currentUndoBuffer.CanUndo;
			redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = _currentUndoBuffer.CanRedo;

			rendererControl.Invalidate();
		}

		Paril.Controls.Color.ColorPreview SelectedColorPreview
		{
			get { return (_secondaryIsFront) ? colorPreview2 : colorPreview1; }
		}

		void SetColor(Control colorPreview, ref Color currentColor, Color newColor)
		{
			currentColor = newColor;
			colorPreview.ForeColor = currentColor;

			if (colorPreview != SelectedColorPreview)
				return;

			var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(newColor);

			_skipColors = true;
			redNumericUpDown.Value = newColor.R;
			greenNumericUpDown.Value = newColor.G;
			blueNumericUpDown.Value = newColor.B;
			alphaNumericUpDown.Value = newColor.A;

			colorSquare.CurrentHue = (int)hsl.Hue;
			colorSquare.CurrentSat = (int)(hsl.Saturation * 240);
			saturationSlider.CurrentLum = (int)(hsl.Luminance * 240);

			hueNumericUpDown.Value = colorSquare.CurrentHue;
			saturationNumericUpDown.Value = colorSquare.CurrentSat;
			luminanceNumericUpDown.Value = saturationSlider.CurrentLum;

			redRenderer.StartColor = Color.FromArgb(255, 0, currentColor.G, currentColor.B);
			greenRenderer.StartColor = Color.FromArgb(255, currentColor.R, 0, currentColor.B);
			blueRenderer.StartColor = Color.FromArgb(255, currentColor.R, currentColor.G, 0);

			redRenderer.EndColor = Color.FromArgb(255, 255, currentColor.G, currentColor.B);
			greenRenderer.EndColor = Color.FromArgb(255, currentColor.R, 255, currentColor.B);
			blueRenderer.EndColor = Color.FromArgb(255, currentColor.R, currentColor.G, 255);

			hueRenderer.Saturation = colorSquare.CurrentSat;
			hueRenderer.Luminance = saturationSlider.CurrentLum;

			saturationRenderer.Luminance = saturationSlider.CurrentLum;
			saturationRenderer.Hue = colorSquare.CurrentHue;

			lightnessRenderer.Hue = colorSquare.CurrentHue;
			lightnessRenderer.Saturation = colorSquare.CurrentSat;

			redColorSlider.Value = currentColor.R;
			greenColorSlider.Value = currentColor.G;
			blueColorSlider.Value = currentColor.B;
			alphaColorSlider.Value = currentColor.A;

			hueColorSlider.Value = colorSquare.CurrentHue;
			saturationColorSlider.Value = colorSquare.CurrentSat;
			lightnessColorSlider.Value = saturationSlider.CurrentLum;

			if (!_editingHex)
				textBox1.Text = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", newColor.R, newColor.G, newColor.B, newColor.A);

			_skipColors = false;
		}

		void SetColor(Color c)
		{
			if (_secondaryIsFront)
				SetColor(colorPreview2, ref _secondaryColor, c);
			else
				SetColor(colorPreview1, ref _primaryColor, c);
		}

		void SetViewMode(ViewMode newMode)
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

			rendererControl_Resize(rendererControl, null);
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

			rendererControl.Invalidate();
		}

		ToolStripMenuItem[] _toggleMenuItems;
		ToolStripButton[] _toggleButtons;
		void SetVisibleParts()
		{
			if (_toggleMenuItems == null)
			{
				_toggleMenuItems = new ToolStripMenuItem[] { headToolStripMenuItem, helmetToolStripMenuItem, chestToolStripMenuItem, leftArmToolStripMenuItem, rightArmToolStripMenuItem, leftLegToolStripMenuItem, rightLegToolStripMenuItem };
				_toggleButtons = new ToolStripButton[] { toggleHeadToolStripButton, toggleHelmetToolStripButton, toggleChestToolStripButton, toggleLeftArmToolStripButton, toggleRightArmToolStripButton, toggleLeftLegToolStripButton, toggleRightLegToolStripButton };
			}

			for (int i = 0; i < _toggleButtons.Length; ++i)
				_toggleMenuItems[i].Checked = _toggleButtons[i].Checked = ((GlobalSettings.ViewFlags & (VisiblePartFlags)(1 << i)) != 0);
		}

		void ToggleVisiblePart(VisiblePartFlags flag)
		{
			GlobalSettings.ViewFlags ^= flag;

			bool hasNow = (GlobalSettings.ViewFlags & flag) != 0;

			ToolStripMenuItem item = null;
			ToolStripButton itemButton = null;

			// TODO: ugly
			switch (flag)
			{
			case VisiblePartFlags.HeadFlag:
				item = headToolStripMenuItem;
				itemButton = toggleHeadToolStripButton;
				break;
			case VisiblePartFlags.HelmetFlag:
				item = helmetToolStripMenuItem;
				itemButton = toggleHelmetToolStripButton;
				break;
			case VisiblePartFlags.ChestFlag:
				item = chestToolStripMenuItem;
				itemButton = toggleChestToolStripButton;
				break;
			case VisiblePartFlags.LeftArmFlag:
				item = leftArmToolStripMenuItem;
				itemButton = toggleLeftArmToolStripButton;
				break;
			case VisiblePartFlags.RightArmFlag:
				item = rightArmToolStripMenuItem;
				itemButton = toggleRightArmToolStripButton;
				break;
			case VisiblePartFlags.LeftLegFlag:
				item = leftLegToolStripMenuItem;
				itemButton = toggleLeftLegToolStripButton;
				break;
			case VisiblePartFlags.RightLegFlag:
				item = rightLegToolStripMenuItem;
				itemButton = toggleRightLegToolStripButton;
				break;
			}

			item.Checked = hasNow;
			itemButton.Checked = hasNow;

			rendererControl.Invalidate();
		}

		void ToggleAlphaCheckerboard()
		{
			GlobalSettings.AlphaCheckerboard = !GlobalSettings.AlphaCheckerboard;
			alphaCheckerboardToolStripMenuItem.Checked = GlobalSettings.AlphaCheckerboard;
			rendererControl.Invalidate();
		}

		void ToggleOverlay()
		{
			GlobalSettings.TextureOverlay = !GlobalSettings.TextureOverlay;
			textureOverlayToolStripMenuItem.Checked = GlobalSettings.TextureOverlay;
			rendererControl.Invalidate();
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
				SetViewMode(ViewMode.Hybrid);
				break;
			case ViewMode.Hybrid:
				SetViewMode(ViewMode.Orthographic);
				break;
			}
		}

		#region Screenshots
		Bitmap CopyScreenToBitmap()
		{
			rendererControl.MakeCurrent();
			Bitmap b = new Bitmap(rendererControl.Width, rendererControl.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int[] pixels = new int[rendererControl.Width * rendererControl.Height];
			GL.ReadPixels(0, 0, rendererControl.Width, rendererControl.Height, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

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
		#endregion
		#endregion

		void rendererControl_Load(object sender, EventArgs e)
		{
			rendererControl_Resize(this, EventArgs.Empty);   // Ensure the Viewport is set up correctly
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

			_toolboxUpNormal = ImageUtilities.LoadImage(Properties.Resources.buttong);
			_toolboxUpHover = ImageUtilities.LoadImage(Properties.Resources.buttong_2);
			_toolboxDownNormal = ImageUtilities.LoadImage(Properties.Resources.buttong_down);
			_toolboxDownHover = ImageUtilities.LoadImage(Properties.Resources.buttong_down2);

			_grassTop = ImageUtilities.LoadImage("grass.png");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			_backgrounds.Add(new BackgroundImage("None", 0));

			foreach (var file in Directory.GetFiles("Overlays"))
			{
				try
				{
					var image = ImageUtilities.LoadImage(file);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

					_backgrounds.Add(new BackgroundImage(Path.GetFileNameWithoutExtension(file), image));
				}
				catch
				{
					MessageBox.Show(string.Format(GetLanguageString("B_MSG_OVERLAYERROR"), file));
				}
			}

			int index = 0;
			foreach (var b in _backgrounds)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(b.Name);
				b.Item = item;

				if (b.Name == GlobalSettings.LastBackground)
				{
					item.Checked = true;
					_selectedBackground = index;
				}

				item.Click += item_Clicked;
				item.Tag = index++;

				backgroundsToolStripMenuItem.DropDownItems.Add(item);
			}

			_previewPaint = GL.GenTexture();
			GlobalDirtiness.CurrentSkin = GL.GenTexture();
			_alphaTex = GL.GenTexture();

			unsafe
			{
				byte[] arra = new byte[64 * 32];
				RenderState.BindTexture(_previewPaint);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arra);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

				RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
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

				RenderState.BindTexture(_alphaTex);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 4, 4, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arra);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			}

			if (GL.GetString(StringName.Extensions).Contains("GL_EXT_vertex_array"))
				_renderer = new ClientArrayRenderer();
			else
				_renderer = new ImmediateRenderer();
		}

		void item_Clicked(object sender, EventArgs e)
		{
			var item = (ToolStripMenuItem)sender;
			GlobalSettings.LastBackground = item.Text;
			_backgrounds[_selectedBackground].Item.Checked = false;
			_selectedBackground = (int)item.Tag;
			item.Checked = true;
		}

		static bool _ddh = false;
		void animTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!_ddh)
			{
				new DragDropHelper();
				_ddh = true;
			}

			_animationTime += 0.24f;
			rendererControl.Invalidate();
		}

		void rendererControl_MouseWheel(object sender, MouseEventArgs e)
		{
			CheckMouse(e.Y);

			if (_currentViewMode == ViewMode.Perspective || (_currentViewMode == ViewMode.Hybrid && _mouseIn3D))
				_3dZoom += e.Delta / 50;
			else
				_2dZoom += e.Delta / 50;
            if (_2dZoom < 1) _2dZoom = 1;

			rendererControl.Invalidate();
		}

		void DrawGLToolbar()
		{
			// 2D
			Setup2D(new Rectangle(0, 0, rendererControl.Width, rendererControl.Height));
			RenderState.BindTexture(0);
			GL.Enable(EnableCap.Blend);

			float halfWidth = rendererControl.Width / 2.0f;
			float halfImgWidth = 56.0f / 2.0f;

			var rect = new RectangleF(halfWidth - halfImgWidth, 0, halfImgWidth * 2, 22);

			int img = (splitContainer4.SplitterDistance == 0) ? _toolboxDownNormal : _toolboxUpNormal;

			if (rect.Contains(_mousePoint))
			{
				GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);
				RenderState.BindTexture(img);
			}
			else
			{
				GL.Color4((byte)255, (byte)255, (byte)255, (byte)64);
				RenderState.BindTexture(img);
			}

			const float widSep = 56.0f / 64.0f;
			const float heiSep = 22.0f / 32.0f;

			GL.Begin(BeginMode.Quads);
			GL.TexCoord2(0, 0); GL.Vertex2(halfWidth - halfImgWidth, -1);
			GL.TexCoord2(widSep, 0); GL.Vertex2(halfWidth + halfImgWidth, -1);
			GL.TexCoord2(widSep, heiSep); GL.Vertex2(halfWidth + halfImgWidth, 21);
			GL.TexCoord2(0, heiSep); GL.Vertex2(halfWidth - halfImgWidth, 21);
			GL.End();
		}

		void rendererControl_Paint(object sender, PaintEventArgs e)
		{
			rendererControl.MakeCurrent();
			SetPreview();

			GL.ClearColor(GlobalSettings.BackgroundColor);
			GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			var skin = (Skin)_lastSkin;

			GL.PushMatrix();

			if (_currentViewMode == ViewMode.Perspective)
			{
				Setup3D(new Rectangle(0, 0, rendererControl.Width, rendererControl.Height));
				DrawPlayer(_previewPaint, skin, false);
			}
			else if (_currentViewMode == ViewMode.Orthographic)
			{
				Setup2D(new Rectangle(0, 0, rendererControl.Width, rendererControl.Height));
				DrawPlayer2D(_previewPaint, skin, false);
			}
			else
			{
				int halfHeight = (int)Math.Ceiling(rendererControl.Height / 2.0f);

				Setup3D(new Rectangle(0, 0, rendererControl.Width, halfHeight));
				DrawPlayer(_previewPaint, skin, false);

				Setup2D(new Rectangle(0, halfHeight, rendererControl.Width, halfHeight));
				DrawPlayer2D(_previewPaint, skin, false);
			}
			
			GL.PopMatrix();

			DrawGLToolbar();

			rendererControl.SwapBuffers();
		}

		Rectangle _currentViewport;

		void Setup3D(Rectangle viewport)
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.Viewport(viewport);
			var mat = OpenTK.Matrix4d.Perspective(45, (double)viewport.Width / (double)viewport.Height, 0.01, 100000);
			GL.MultMatrix(ref mat);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			_currentViewport = viewport;
		}

		void Setup2D(Rectangle viewport)
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.Viewport(viewport);
			GL.Ortho(0, viewport.Width, viewport.Height, 0, -1, 1);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			_currentViewport = viewport;
		}

		void rendererControl_Resize(object sender, EventArgs e)
		{
			rendererControl.MakeCurrent();

			rendererControl.Invalidate();
		}

		System.Timers.Timer _animTimer = new System.Timers.Timer(25);
		bool _opening = false;

		void _animTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (_opening)
			{
				splitContainer4.SplitterDistance += splitContainer4.SplitterIncrement;

				if (splitContainer4.SplitterDistance >= 55)
				{
					splitContainer4.SplitterDistance = 55;
					_animTimer.Stop();
				}
			}
			else
			{
				if ((splitContainer4.SplitterDistance - splitContainer4.SplitterIncrement) <= splitContainer4.Panel1MinSize)
				{
					splitContainer4.SplitterDistance = splitContainer4.Panel1MinSize;
					_animTimer.Stop();
					return;
				}

				splitContainer4.SplitterDistance -= splitContainer4.SplitterIncrement;
			}
		}

		public float ToolScale
		{
			get
			{
				const float baseSize = 200.0f;

				return baseSize / rendererControl.Size.Width;
			}
		}

		public void RotateView(Point delta, float factor)
		{
			if (_currentViewMode == ViewMode.Perspective || (_currentViewMode == ViewMode.Hybrid && _mouseIn3D))
			{
				_3dRotationY += (float)(delta.X * ToolScale) * factor;
				_3dRotationX += (float)(delta.Y * ToolScale) * factor;
			}
			else
			{
				_2dCamOffsetX += delta.X / _2dZoom;
				_2dCamOffsetY += delta.Y / _2dZoom;
			}
		}

		public void ScaleView(Point delta, float factor)
		{
			if (_currentViewMode == ViewMode.Perspective || (_currentViewMode == ViewMode.Hybrid && _mouseIn3D))
			{
				_3dZoom += (float)(-delta.Y * ToolScale) * factor;
			}
			else
			{
				_2dZoom += -delta.Y / 25.0f;

				if (_2dZoom < 1)
					_2dZoom = 1;
			}
		}

		void CheckMouse(int y)
		{
			if (y > (rendererControl.Height / 2))
				_mouseIn3D = true;
			else
				_mouseIn3D = false;
		}

		bool _mouseIn3D = false;
		void rendererControl_MouseDown(object sender, MouseEventArgs e)
		{
			CheckMouse(e.Y);

			float halfWidth = rendererControl.Width / 2.0f;
			float halfImgWidth = 56.0f / 2.0f;

			var rect = new RectangleF(halfWidth - halfImgWidth, 0, halfImgWidth * 2, 22);

			_mousePoint = e.Location;

			if (rect.Contains(e.Location))
			{
				if (splitContainer4.SplitterDistance == 0)
					_opening = true;
				else
					_opening = false;

				_animTimer.Start();
				return;
			}

			_mouseIsDown = true;

			if (e.Button == MouseButtons.Left)
			{
				Point p = Point.Empty;

				if (GetPick(e.X, e.Y, ref p))
					_selectedTool.Tool.BeginClick(_lastSkin, p, e);
				else
					_selectedTool.Tool.BeginClick(_lastSkin, new Point(-1, -1), e);
				UseToolOnViewport(e.X, e.Y);
			}
			else
				_tools[(int)Tools.Camera].Tool.BeginClick(_lastSkin, Point.Empty, e);
		}

		void rendererControl_MouseMove(object sender, MouseEventArgs e)
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

				rendererControl.Invalidate();
			}

			_mousePoint = e.Location;
		}

		public void CheckUndo()
		{
			undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = _currentUndoBuffer.CanUndo;
			redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = _currentUndoBuffer.CanRedo;
		}

		void rendererControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (_mouseIsDown)
			{
				if (e.Button == MouseButtons.Left)
					_selectedTool.Tool.EndClick(_lastSkin, e);
				else
					_tools[(int)Tools.Camera].Tool.EndClick(_lastSkin, e);
			}

			_mouseIsDown = false;
		}


		void rendererControl_MouseLeave(object sender, EventArgs e)
		{
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (_skipListbox || treeView1.SelectedNode == _lastSkin ||
				!(e.Node is Skin))
				return;

			if (_lastSkin != null && treeView1.SelectedNode != _lastSkin)
			{
				// Copy over the current changes to the tex stored in the skin.
				// This allows us to pick up where we left off later, without undoing any work.
				_lastSkin.CommitChanges(GlobalDirtiness.CurrentSkin, false);
			}

			//if (_lastSkin != null)
			//	_lastSkin.Undo.Clear();

			rendererControl.MakeCurrent();

			Skin skin = (Skin)treeView1.SelectedNode;
			SetCanSave(skin.Dirty);

			if (skin == null)
			{
				_currentUndoBuffer = null;
				RenderState.BindTexture(0);
				int[] array = new int[64 * 32];
				RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = false;
				redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = false;
			}
			else
			{
				RenderState.BindTexture(skin.GLImage);
				int[] array = new int[skin.Width * skin.Height];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, skin.Width, skin.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				RenderState.BindTexture(_previewPaint);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, skin.Width, skin.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				_currentUndoBuffer = skin.Undo;
				undoToolStripMenuItem.Enabled = undoToolStripButton.Enabled = _currentUndoBuffer.CanUndo;
				redoToolStripMenuItem.Enabled = redoToolStripButton.Enabled = _currentUndoBuffer.CanRedo;
			}

			rendererControl.Invalidate();
			_lastSkin = (Skin)treeView1.SelectedNode;
		}

		void uploadButton_Click(object sender, EventArgs e)
		{
			if (_lastSkin.Width != 64 || _lastSkin.Height != 32)
			{
				MessageBox.Show(GetLanguageString("B_MSG_UPLOADRES"));
				return;
			}

			PerformUpload();
		}

		void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		void animateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleAnimation();
		}

		void followCursorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleFollowCursor();
		}

		void grassToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleGrass();
		}

		void addNewSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformImportSkin();
		}

		void deleteSelectedSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformDeleteSkin();
		}

		void cloneSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformCloneSkin();
		}

		private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			//PerformNameChange();
		}

		TreeNode _rightClickedNode = null;
		private void treeView1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				_rightClickedNode = treeView1.GetSelectedNodeAt(e.Location);
				changeNameToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = cloneToolStripMenuItem.Enabled = cloneToolStripButton.Enabled = true;

				if (treeView1.SelectedNode == null)
					changeNameToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = cloneToolStripMenuItem.Enabled = cloneToolStripButton.Enabled = false;
				else if (!(treeView1.SelectedNode is Skin))
					cloneToolStripMenuItem.Enabled = cloneToolStripButton.Enabled = false;

				contextMenuStrip1.Show(Cursor.Position);
			}
		}

		void undoToolStripButton_Click(object sender, EventArgs e)
		{
			PerformUndo();
		}

		void redoToolStripButton_Click(object sender, EventArgs e)
		{
			PerformRedo();
		}

		void redNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb(SelectedColor.A, (byte)redNumericUpDown.Value, SelectedColor.G, SelectedColor.B));
		}

		void greenNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb(SelectedColor.A, SelectedColor.R, (byte)greenNumericUpDown.Value, SelectedColor.B));
		}

		void blueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, (byte)blueNumericUpDown.Value));
		}

		void alphaNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb((byte)alphaNumericUpDown.Value, SelectedColor.R, SelectedColor.G, SelectedColor.B));
		}

		const float oneDivTwoFourty = 1.0f / 240.0f;

		void colorSquare_HueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL(colorSquare.CurrentHue, (float)colorSquare.CurrentSat * oneDivTwoFourty, (float)saturationSlider.CurrentLum * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void colorSquare_SatChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL(colorSquare.CurrentHue, (float)colorSquare.CurrentSat * oneDivTwoFourty, (float)saturationSlider.CurrentLum * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void saturationSlider_LumChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL(colorSquare.CurrentHue, (float)colorSquare.CurrentSat * oneDivTwoFourty, (float)saturationSlider.CurrentLum * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void hueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL(e.NewValue, (float)saturationColorSlider.Value * oneDivTwoFourty, (float)lightnessColorSlider.Value * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void saturationColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL(hueColorSlider.Value, (float)e.NewValue * oneDivTwoFourty, (float)lightnessColorSlider.Value * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void lightnessColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL(hueColorSlider.Value, (float)saturationColorSlider.Value * oneDivTwoFourty, (float)e.NewValue * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void hueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL((double)hueNumericUpDown.Value, (float)saturationNumericUpDown.Value * oneDivTwoFourty, (float)luminanceNumericUpDown.Value * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void saturationNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL((double)hueNumericUpDown.Value, (float)saturationNumericUpDown.Value * oneDivTwoFourty, (float)luminanceNumericUpDown.Value * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void luminanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			var c = new HSL((double)hueNumericUpDown.Value, (float)saturationNumericUpDown.Value * oneDivTwoFourty, (float)luminanceNumericUpDown.Value * oneDivTwoFourty);
			SetColor(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(c));
		}

		void perspectiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Perspective);
		}

		void textureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Orthographic);
		}

		void perspectiveToolStripButton_Click(object sender, EventArgs e)
		{
			SetViewMode(ViewMode.Perspective);
		}

		void orthographicToolStripButton_Click(object sender, EventArgs e)
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

		void offToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencyMode.Off);
		}

		void helmetOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencyMode.Helmet);
		}

		void allToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencyMode.All);
		}

		void headToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.HeadFlag);
		}

		void helmetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.HelmetFlag);
		}

		void chestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.ChestFlag);
		}

		void leftArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.LeftArmFlag);
		}

		void rightArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.RightArmFlag);
		}

		void leftLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.LeftLegFlag);
		}

		void rightLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.RightLegFlag);
		}

		void alphaCheckerboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleAlphaCheckerboard();
		}

		void textureOverlayToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleOverlay();
		}

		void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_updater.Checking)
				return;

			_updater.PrintOnEqual = true;
			_updater.CheckForUpdate();
		}

		void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformUndo();
		}

		void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformRedo();
		}

		void redColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb(SelectedColor.A, e.NewValue, SelectedColor.G, SelectedColor.B));
		}

		void greenColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb(SelectedColor.A, SelectedColor.R, e.NewValue, SelectedColor.B));
		}

		void blueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, e.NewValue));
		}

		void swatchContainer_SwatchChanged(object sender, SwatchChangedEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (_secondaryIsFront)
					SetColor(colorPreview2, ref _secondaryColor, e.Swatch);
				else
					SetColor(colorPreview1, ref _primaryColor, e.Swatch);
			}
			else
			{
				if (!_secondaryIsFront)
					SetColor(colorPreview2, ref _secondaryColor, e.Swatch);
				else
					SetColor(colorPreview1, ref _primaryColor, e.Swatch);
			}
		}

		void alphaColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(Color.FromArgb(e.NewValue, SelectedColor.R, SelectedColor.G, SelectedColor.B));
		}

		void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_shortcutEditor.ShowDialog();
		}

		void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (MultiPainter.ColorPicker picker = new MultiPainter.ColorPicker())
			{
				picker.CurrentColor = GlobalSettings.BackgroundColor;

				if (picker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					GlobalSettings.BackgroundColor = picker.CurrentColor;

					rendererControl.Invalidate();
				}
			}
		}

		void screenshotToolStripButton_Click(object sender, EventArgs e)
		{
			if ((ModifierKeys & Keys.Shift) != 0)
				SaveScreenshot();
			else
				TakeScreenshot();
		}

		void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformSaveAs();
		}

		void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformSave();
		}

		void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformSaveAll();
		}

		void saveToolStripButton_Click(object sender, EventArgs e)
		{
			PerformSave();
		}

		void saveAlltoolStripButton_Click(object sender, EventArgs e)
		{
			PerformSaveAll();
		}

		void changeNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformNameChange();
		}

		void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformDeleteSkin();
		}

		void cloneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PerformCloneSkin();
		}

		void colorTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (colorTabControl.SelectedIndex == 1 || colorTabControl.SelectedIndex == 2)
			{
				colorTabControl.SelectedTab.Controls.Add(colorSquare);
				colorTabControl.SelectedTab.Controls.Add(saturationSlider);
				colorTabControl.SelectedTab.Controls.Add(colorPreview1);
				colorTabControl.SelectedTab.Controls.Add(colorPreview2);
				colorTabControl.SelectedTab.Controls.Add(label5);
				colorTabControl.SelectedTab.Controls.Add(alphaColorSlider);
				colorTabControl.SelectedTab.Controls.Add(alphaNumericUpDown);

				if (_secondaryIsFront)
				{
					colorPreview2.BringToFront();
					colorPreview1.SendToBack();
				}
				else
				{
					colorPreview2.SendToBack();
					colorPreview1.BringToFront();
				}
			}
		}

		void automaticallyCheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GlobalSettings.AutoUpdate = automaticallyCheckForUpdatesToolStripMenuItem.Checked = !automaticallyCheckForUpdatesToolStripMenuItem.Checked;
        }

		private void toggleHeadToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.HeadFlag);
		}

		private void toggleHelmetToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.HelmetFlag);
		}

		private void toggleChestToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.ChestFlag);
		}

		private void toggleLeftArmToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.LeftArmFlag);
		}

		private void toggleRightArmToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.RightArmFlag);
		}

		private void toggleLeftLegToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.LeftLegFlag);
		}

		private void toggleRightLegToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(VisiblePartFlags.RightLegFlag);
		}

		private void labelEditTextBox_Leave(object sender, EventArgs e)
		{
			DoneEditingNode(labelEditTextBox.Text, _currentlyEditing);
		}

		bool _editingHex = false;
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			if (textBox1.Text.Contains('#'))
				textBox1.Text = textBox1.Text.Replace("#", "");

			string realHex = textBox1.Text;

			while (realHex.Length != 8)
				realHex += 'F';

			byte r = byte.Parse(realHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(realHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(realHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			byte a = byte.Parse(realHex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

			_editingHex = true;
			SetColor(Color.FromArgb(a, r, g, b));
			_editingHex = false;
		}

		private void labelEditTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
			{
				treeView1.Focus();
				e.Handled = true;
			}
		}

		private void labelEditTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
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

		static ToolStripMenuItem[] _antialiasOpts;
		void SetSampleMenuItem(int samples)
		{
			if (_antialiasOpts == null)
				_antialiasOpts = new ToolStripMenuItem[] { xToolStripMenuItem4, xToolStripMenuItem, xToolStripMenuItem1, xToolStripMenuItem2, xToolStripMenuItem3 };

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

			foreach (var item in _antialiasOpts)
				item.Checked = false;

			GlobalSettings.Multisamples = samples;
			_antialiasOpts[index].Checked = true;
		}
		
		private void xToolStripMenuItem4_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(0);
			MessageBox.Show(GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(1);
			MessageBox.Show(GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(2);
			MessageBox.Show(GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(4);
			MessageBox.Show(GetLanguageString("B_MSG_ANTIALIAS"));
		}

		private void xToolStripMenuItem3_Click(object sender, EventArgs e)
		{
			SetSampleMenuItem(8);
			MessageBox.Show(GetLanguageString("B_MSG_ANTIALIAS"));
		}

        private void SetLanguage(string filename)
        {

        }

        private void ReloadLanguage()
        {
        }

		static Language.Language _currentLanguage;
		public static Language.Language CurrentLanguage
		{
			get { return _currentLanguage; }
			set
			{
				if (_currentLanguage != null)
					_currentLanguage.Item.Checked = false;
				
				_currentLanguage = value;
				Program.MainForm.languageProvider1.LanguageChanged(value);
				Program.MainForm.DarkenLightenOptions.languageProvider1.LanguageChanged(value);
				Program.MainForm.PencilOptions.languageProvider1.LanguageChanged(value);
				Program.MainForm.DodgeBurnOptions.languageProvider1.LanguageChanged(value);
				Program.MainForm.FloodFillOptions.languageProvider1.LanguageChanged(value);
				Program.MainForm.swatchContainer.languageProvider1.LanguageChanged(value);
				Program.MainForm.login.languageProvider1.LanguageChanged(value);

				_currentLanguage.Item.Checked = true;
			}
		}

		public static string GetLanguageString(string id)
		{
			if (!_currentLanguage.StringTable.ContainsKey(id))
				return id;
			return _currentLanguage.StringTable[id];
		}

		private void MCSkin3D_Load(object sender, EventArgs e)
		{
		}

		void languageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CurrentLanguage = (Language.Language)((ToolStripMenuItem)sender).Tag;
		}

		bool _secondaryIsFront = false;

		private void colorPreview1_Click(object sender, EventArgs e)
		{
			_secondaryIsFront = false;
			colorPreview1.BringToFront();

			SetColor(_primaryColor);
		}

		private void colorPreview2_Click(object sender, EventArgs e)
		{
			_secondaryIsFront = true;
			colorPreview2.BringToFront();

			SetColor(_secondaryColor);
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			PerformUpload();
		}

		private void importToolStripButton_Click(object sender, EventArgs e)
		{
			PerformImportSkin();
		}

		private void newFolderToolStripButton_Click(object sender, EventArgs e)
		{
			PerformNewFolder();
		}

		private void renameToolStripButton_Click(object sender, EventArgs e)
		{
			PerformNameChange();
		}

		private void deleteToolStripButton_Click(object sender, EventArgs e)
		{
			PerformDeleteSkin();
		}

		private void cloneToolStripButton_Click(object sender, EventArgs e)
		{
			PerformCloneSkin();
		}

		private void uploadToolStripButton_Click(object sender, EventArgs e)
		{
			PerformUpload();
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			PerformTreeViewZoomOut();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			PerformTreeViewZoomIn();
		}
	}
}