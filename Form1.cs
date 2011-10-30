using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using DevCIL;
using Devcorp.Controls.Design;
using MB.Controls;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MCSkin3D
{
	public partial class Form1 : Form
	{
		public const string VersionString = "1.1";

		public static class HexSerializer
		{
			public static string GetHex(string s)
			{
				string h = "";

				foreach (var c in s)
					h += string.Format("{0:X2}", (int)c);

				return h;
			}

			public static string GetString(string hex)
			{
				string s = "";

				for (int i = 0; i < hex.Length; i += 2)
					s += (char)int.Parse("" + hex[i] + hex[i + 1], System.Globalization.NumberStyles.HexNumber);

				return s;
			}
		}

		public class PasswordTypeSerializer<T> : TypeSerializer
			where T : SymmetricAlgorithm, new()
		{
			/// <summary>
			/// Encrypts specified plaintext using Rijndael symmetric key algorithm
			/// and returns a base64-encoded result.
			/// </summary>
			/// <param name="plainText">
			/// Plaintext value to be encrypted.
			/// </param>
			/// <param name="passPhrase">
			/// Passphrase from which a pseudo-random password will be derived. The
			/// derived password will be used to generate the encryption key.
			/// Passphrase can be any string. In this example we assume that this
			/// passphrase is an ASCII string.
			/// </param>
			/// <param name="saltValue">
			/// Salt value used along with passphrase to generate password. Salt can
			/// be any string. In this example we assume that salt is an ASCII string.
			/// </param>
			/// <param name="hashAlgorithm">
			/// Hash algorithm used to generate password. Allowed values are: "MD5" and
			/// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
			/// </param>
			/// <param name="passwordIterations">
			/// Number of iterations used to generate password. One or two iterations
			/// should be enough.
			/// </param>
			/// <param name="initVector">
			/// Initialization vector (or IV). This value is required to encrypt the
			/// first block of plaintext data. For RijndaelManaged class IV must be 
			/// exactly 16 ASCII characters long.
			/// </param>
			/// <param name="keySize">
			/// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
			/// Longer keys are more secure than shorter keys.
			/// </param>
			/// <returns>
			/// Encrypted value formatted as a base64-encoded string.
			/// </returns>
			public static string Encrypt(string plainText,
										 string passPhrase,
										 string saltValue,
										 string hashAlgorithm,
										 int passwordIterations,
										 string initVector,
										 int keySize)
			{
				// Convert strings into byte arrays.
				// Let us assume that strings only contain ASCII codes.
				// If strings include Unicode characters, use Unicode, UTF7, or UTF8 
				// encoding.
				byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
				byte[] saltValueBytes  = Encoding.ASCII.GetBytes(saltValue);

				// Convert our plaintext into a byte array.
				// Let us assume that plaintext contains UTF8-encoded characters.
				byte[] plainTextBytes  = Encoding.UTF8.GetBytes(plainText);

				// First, we must create a password, from which the key will be derived.
				// This password will be generated from the specified passphrase and 
				// salt value. The password will be created using the specified hash 
				// algorithm. Password creation can be done in several iterations.
				PasswordDeriveBytes password = new PasswordDeriveBytes(
																passPhrase,
																saltValueBytes,
																hashAlgorithm,
																passwordIterations);

				// Use the password to generate pseudo-random bytes for the encryption
				// key. Specify the size of the key in bytes (instead of bits).
				byte[] keyBytes = password.GetBytes(keySize / 8);

				// Create uninitialized Rijndael encryption object.
				T symmetricKey = new T();

				// It is reasonable to set encryption mode to Cipher Block Chaining
				// (CBC). Use default options for other symmetric key parameters.
				symmetricKey.Mode = CipherMode.CBC;

				// Generate encryptor from the existing key bytes and initialization 
				// vector. Key size will be defined based on the number of the key 
				// bytes.
				ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
																 keyBytes,
																 initVectorBytes);

				// Define memory stream which will be used to hold encrypted data.
				MemoryStream memoryStream = new MemoryStream();

				// Define cryptographic stream (always use Write mode for encryption).
				CryptoStream cryptoStream = new CryptoStream(memoryStream,
															 encryptor,
															 CryptoStreamMode.Write);
				// Start encrypting.
				cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

				// Finish encrypting.
				cryptoStream.FlushFinalBlock();

				// Convert our encrypted data from a memory stream into a byte array.
				byte[] cipherTextBytes = memoryStream.ToArray();

				// Close both streams.
				memoryStream.Close();
				cryptoStream.Close();

				// Convert encrypted data into a base64-encoded string.
				string cipherText = Convert.ToBase64String(cipherTextBytes);

				// Return encrypted string.
				return cipherText;
			}

			/// <summary>
			/// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
			/// </summary>
			/// <param name="cipherText">
			/// Base64-formatted ciphertext value.
			/// </param>
			/// <param name="passPhrase">
			/// Passphrase from which a pseudo-random password will be derived. The
			/// derived password will be used to generate the encryption key.
			/// Passphrase can be any string. In this example we assume that this
			/// passphrase is an ASCII string.
			/// </param>
			/// <param name="saltValue">
			/// Salt value used along with passphrase to generate password. Salt can
			/// be any string. In this example we assume that salt is an ASCII string.
			/// </param>
			/// <param name="hashAlgorithm">
			/// Hash algorithm used to generate password. Allowed values are: "MD5" and
			/// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
			/// </param>
			/// <param name="passwordIterations">
			/// Number of iterations used to generate password. One or two iterations
			/// should be enough.
			/// </param>
			/// <param name="initVector">
			/// Initialization vector (or IV). This value is required to encrypt the
			/// first block of plaintext data. For RijndaelManaged class IV must be
			/// exactly 16 ASCII characters long.
			/// </param>
			/// <param name="keySize">
			/// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
			/// Longer keys are more secure than shorter keys.
			/// </param>
			/// <returns>
			/// Decrypted string value.
			/// </returns>
			/// <remarks>
			/// Most of the logic in this function is similar to the Encrypt
			/// logic. In order for decryption to work, all parameters of this function
			/// - except cipherText value - must match the corresponding parameters of
			/// the Encrypt function which was called to generate the
			/// ciphertext.
			/// </remarks>
			public static string Decrypt(string cipherText,
										 string passPhrase,
										 string saltValue,
										 string hashAlgorithm,
										 int passwordIterations,
										 string initVector,
										 int keySize)
			{
				// Convert strings defining encryption key characteristics into byte
				// arrays. Let us assume that strings only contain ASCII codes.
				// If strings include Unicode characters, use Unicode, UTF7, or UTF8
				// encoding.
				byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
				byte[] saltValueBytes  = Encoding.ASCII.GetBytes(saltValue);

				// Convert our ciphertext into a byte array.
				byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

				// First, we must create a password, from which the key will be 
				// derived. This password will be generated from the specified 
				// passphrase and salt value. The password will be created using
				// the specified hash algorithm. Password creation can be done in
				// several iterations.
				PasswordDeriveBytes password = new PasswordDeriveBytes(
																passPhrase,
																saltValueBytes,
																hashAlgorithm,
																passwordIterations);

				// Use the password to generate pseudo-random bytes for the encryption
				// key. Specify the size of the key in bytes (instead of bits).
				byte[] keyBytes = password.GetBytes(keySize / 8);

				// Create uninitialized Rijndael encryption object.
				T symmetricKey = new T();

				// It is reasonable to set encryption mode to Cipher Block Chaining
				// (CBC). Use default options for other symmetric key parameters.
				symmetricKey.Mode = CipherMode.CBC;

				// Generate decryptor from the existing key bytes and initialization 
				// vector. Key size will be defined based on the number of the key 
				// bytes.
				ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
																 keyBytes,
																 initVectorBytes);

				// Define memory stream which will be used to hold encrypted data.
				MemoryStream  memoryStream = new MemoryStream(cipherTextBytes);

				// Define cryptographic stream (always use Read mode for encryption).
				CryptoStream  cryptoStream = new CryptoStream(memoryStream,
															  decryptor,
															  CryptoStreamMode.Read);

				// Since at this point we don't know what the size of decrypted data
				// will be, allocate the buffer long enough to hold ciphertext;
				// plaintext is never longer than ciphertext.
				byte[] plainTextBytes = new byte[cipherTextBytes.Length];

				// Start decrypting.
				int decryptedByteCount = cryptoStream.Read(plainTextBytes,
														   0,
														   plainTextBytes.Length);

				// Close both streams.
				memoryStream.Close();
				cryptoStream.Close();

				// Convert decrypted data into a string. 
				// Let us assume that the original plaintext string was UTF8-encoded.
				string plainText = Encoding.UTF8.GetString(plainTextBytes,
														   0,
														   decryptedByteCount);

				// Return decrypted string.   
				return plainText;
			}

			static string   passPhrase         = Environment.UserName;        // can be any string
			static string   saltValue          = Environment.MachineName;        // can be any string
			static string   hashAlgorithm      = "SHA1";             // can be "MD5"
			static int      passwordIterations = 2;                  // can be any number
			static string   initVector         = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
			static int      keySize            = 256;                // can be 192 or 128

			public override object Deserialize(string str)
			{
				return Decrypt(str,
													passPhrase,
													saltValue,
													hashAlgorithm,
													passwordIterations,
													initVector,
													keySize);
			}

			public override string Serialize(object obj)
			{
				return Encrypt((string)obj,
													passPhrase,
													saltValue,
													hashAlgorithm,
													passwordIterations,
													initVector,
													keySize);
			}
		}

		public enum TransparencySetting
		{
			Off,
			Helmet,
			All
		}

		public static class GlobalSettings
		{
			[Save]
			public static bool Animate { get; set; }

			[Save]
			public static bool FollowCursor { get; set; }

			[Save]
			[DefaultValue(true)]
			public static bool Grass { get; set; }

			[Save]
			[DefaultValue("")]
			public static string LastSkin { get; set; }

			[Save]
			public static bool RememberMe { get; set; }

			[Save]
			public static bool AutoLogin { get; set; }
			
			[Save]
			[DefaultValue("")]
			public static string LastUsername { get; set; }

			[Save]
			[DefaultValue("")]
			[TypeSerializer(typeof(PasswordTypeSerializer<AesManaged>))]
			public static string LastPassword { get; set; }

			[Save]
			[DefaultValue(TransparencySetting.Helmet)]
			public static TransparencySetting Transparency { get; set; }

			[Save]
			[DefaultValue(HeadFlag | ChestFlag | LeftArmFlag | RightArmFlag | LeftLegFlag | RightLegFlag)]
			public static int ViewFlags { get; set; }

			[Save]
			[DefaultValue(true)]
			public static bool AlphaCheckerboard { get; set; }

			[Save]
			[DefaultValue(true)]
			public static bool TextureOverlay { get; set; }

			[Save]
			[DefaultValue("")]
			public static string ShortcutKeys { get; set; }
		}

		public const int HeadFlag = 1;
		public const int HelmetFlag = 2;
		public const int ChestFlag = 4;
		public const int LeftArmFlag = 8;
		public const int RightArmFlag = 16;
		public const int LeftLegFlag = 32;
		public const int RightLegFlag = 64;

		public static Settings Settings;

		public Form1()
		{
			InitializeComponent();

			System.Timers.Timer animTimer = new System.Timers.Timer();
			animTimer.Interval = 22;
			animTimer.Elapsed += new System.Timers.ElapsedEventHandler(animTimer_Elapsed);
			animTimer.Start();

			glControl1.MouseWheel += new MouseEventHandler(glControl1_MouseWheel);

			Settings = new Settings();
			Settings.Structures.Add(typeof(GlobalSettings));
			Settings.Load("settings.ini");

			animateToolStripMenuItem.Checked = GlobalSettings.Animate;
			followCursorToolStripMenuItem.Checked = GlobalSettings.FollowCursor;
			grassToolStripMenuItem.Checked = GlobalSettings.Grass;

			alphaCheckerboardToolStripMenuItem.Checked = GlobalSettings.AlphaCheckerboard;
			textureOverlayToolStripMenuItem.Checked = GlobalSettings.TextureOverlay;

			SetCheckbox(HeadFlag, headToolStripMenuItem);
			SetCheckbox(ChestFlag, chestToolStripMenuItem);
			SetCheckbox(LeftArmFlag, leftArmToolStripMenuItem);
			SetCheckbox(RightArmFlag, rightArmToolStripMenuItem);
			SetCheckbox(HelmetFlag, helmetToolStripMenuItem);
			SetCheckbox(LeftLegFlag, leftLegToolStripMenuItem);
			SetCheckbox(RightLegFlag, rightLegToolStripMenuItem);
		}

		void SetCheckbox(int flag, ToolStripMenuItem checkbox)
		{
			if ((GlobalSettings.ViewFlags & flag) != 0)
				checkbox.Checked = true;
			else
				checkbox.Checked = false;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			GlobalSettings.ShortcutKeys = CompileShortcutKeys();

			Settings.Save("settings.ini");
		}

		// Function load a image, turn it into a texture, and return the texture ID as a GLuint for use
		static int loadImage(string theFileName)
		{
			uint imageID;				// Create an image ID as a ULuint
			int textureID;			// Create a texture ID as a GLuint
			bool success;			// Create a flag to keep track of success/failure
			IL.ErrorCode error;				// Create a flag to keep track of the IL error state

			imageID = IL.ilGenImage(); 		// Generate the image ID

			IL.ilBindImage(imageID); 			// Bind the image

			success = IL.ilLoadImage(theFileName); 	// Load the image file

			// If we managed to load the image, then we can start to do things with it...
			if (success)
			{
				// If the image is flipped (i.e. upside-down and mirrored, flip it the right way up!)
				//Ilu.iluFlipImage();

				// Convert the image into a suitable format to work with
				// NOTE: If your image contains alpha channel you can replace IL_RGB with IL_RGBA
				success = IL.ilConvertImage(IL.ColorFormats.RGBA, IL.Types.UnsignedByte);

				// Quit out if we failed the conversion
				if (!success)
				{
					error = IL.ilGetError();
					throw new Exception();
				}

				// Generate a new texture
				textureID = GL.GenTexture();

				// Bind the texture to a name
				GL.BindTexture(TextureTarget.Texture2D, textureID);

				// Set texture interpolation method to use linear interpolation (no MIPMAPS)
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

				// Specify the texture specification
				GL.TexImage2D(TextureTarget.Texture2D, 				// Type of texture
							 0,				// Pyramid level (for mip-mapping) - 0 is the top level
							 (PixelInternalFormat)IL.ilGetInteger(IL.Parameters.ImageBPP),	// Image colour depth
							 IL.ilGetInteger(IL.Parameters.ImageWidth),	// Image width
							 IL.ilGetInteger(IL.Parameters.ImageHeight),	// Image height
							 0,				// Border width in pixels (can either be 1 or 0)
							 (PixelFormat)IL.ilGetInteger(IL.Parameters.ImageFormat),	// Image format (i.e. RGB, RGBA, BGR etc.)
							 PixelType.UnsignedByte,		// Image data type
							 IL.ilGetData());			// The actual image data itself
			}
			else // If we failed to open the image file in the first place...
			{
				error = IL.ilGetError();
				throw new Exception();
			}

			IL.ilDeleteImage(imageID); // Because we have already copied image data into texture data we can release memory used by image.

			return textureID; // Return the GLuint to the texture so you can use it!
		}

		interface Undoable
		{
			void Undo(object obj);
			void Redo(object obj);
		}

		class PixelsChanged : Undoable
		{
			public Color NewColor;
			public Dictionary<Point, Color> Points = new Dictionary<Point, Color>();

			public void Undo(object obj)
			{
				Skin skin = (Skin)obj;

				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				foreach (var kvp in Points)
				{
					var p = kvp.Key;
					var color = kvp.Value;
					array[p.X + (64 * p.Y)] = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
				}

				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
			}

			public void Redo(object obj)
			{
				Skin skin = (Skin)obj;

				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				foreach (var p in Points.Keys)
					array[p.X + (64 * p.Y)] = NewColor.R | (NewColor.G << 8) | (NewColor.B << 16) | (NewColor.A << 24);

				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
			}
		}

		class UndoBuffer
		{
			List<Undoable> Undos = new List<Undoable>();
			public int _depth = -1;
			public object Object;

			public int CurrentIndex
			{
				get
				{
					if (_depth == -1)
						return Undos.Count;
					return _depth;
				}

				set
				{
					_depth = value;

					if (_depth == Undos.Count)
						_depth = -1;
				}
			}

			public UndoBuffer(object obj)
			{
				Object = obj;
			}

			public void AddBuffer(Undoable undoable)
			{
				if (CurrentIndex == Undos.Count)
					Undos.Add(undoable);
				else
				{
					Undos.RemoveRange(CurrentIndex, Undos.Count - CurrentIndex);
					Undos.Add(undoable);
					CurrentIndex = Undos.Count;
				}
			}

			public bool CanUndo
			{
				get { return CurrentIndex != 0; }
			}

			public bool CanRedo
			{
				get { return Undos.Count > CurrentIndex; }
			}

			public void Undo()
			{
				CurrentIndex--;
				Undos[CurrentIndex].Undo(Object);
			}

			public void Redo()
			{
				Undos[CurrentIndex].Redo(Object);
				CurrentIndex++;
			}

			public void Clear()
			{
				Undos.Clear();
				_depth = -1;
			}
		}

		class Skin
		{
			public string Name;
			public Bitmap Image;
			public Bitmap Head;
			public int GLImage;
			public string FileName;
			public UndoBuffer Undo;
			public bool Dirty;

			public Skin(string fileName)
			{
				Undo = new UndoBuffer(this);
				FileName = fileName;
				Name = Path.GetFileNameWithoutExtension(FileName);

				SetImages();
			}

			void SetImages()
			{
				if (Head != null)
				{
					Head.Dispose();
					GL.DeleteTexture(GLImage);
				}

				Image = new Bitmap(FileName);
				Head = new Bitmap(8, 8);

				using (Graphics g = Graphics.FromImage(Head))
					g.DrawImage(Image, new Rectangle(0, 0, 8, 8), new Rectangle(8, 8, 8, 8), GraphicsUnit.Pixel);

				Image.Dispose();
				Image = null;
				GLImage = loadImage(FileName);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
			}

			public override string ToString()
			{
				if (Dirty)
					return Name + " *";
				return Name;
			}

			public void CommitChanges(int _currentSkin, bool save)
			{
				byte[] data = new byte[64 * 32 * 4];
				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

				GL.BindTexture(TextureTarget.Texture2D, GLImage);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

				if (save)
				{
					uint ilim = IL.ilGenImage();
					IL.ilBindImage(ilim);
					IL.ilLoadDataL(data, (uint)data.Length, 64, 32, 1, 4);
					File.Delete(FileName);
					IL.ilSave(IL.ImageType.PNG, FileName);

					SetImages();

					IL.ilDeleteImage(ilim);
					Dirty = false;
				}
			}
		}

		public class MySliderRenderer : SliderRenderer
		{
			public MySliderRenderer(ColorSlider slider) :
				base(slider)
			{
			}

			public Color StartColor
			{
				get;
				set;
			}

			public Color EndColor
			{
				get;
				set;
			}

			public override void Render(Graphics g)
			{
				//TrackBarRenderer.DrawHorizontalTrack(g, new Rectangle(0, (Slider.Height / 2) - 2, Slider.Width, 4));
				var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);
				var brush = new System.Drawing.Drawing2D.LinearGradientBrush(colorRect, StartColor, EndColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
				g.FillRectangle(brush, colorRect);
				g.DrawRectangle(Pens.Black, colorRect);

				TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
			}
		}

		MySliderRenderer redRenderer, greenRenderer, blueRenderer, alphaRenderer;

		class MenuStripShortcut : IShortcutImplementor
		{
			ToolStripMenuItem _menuItem;

			string _name;
			public string Name
			{
				get { return _name; }
			}

			Keys _keys;
			public Keys Keys
			{
				get { return _keys; }
				set { _keys = value; _menuItem.ShortcutKeyDisplayString = ShortcutEditor.KeysToString(_keys); }
			}

			public Action Pressed { get; set; }

			public MenuStripShortcut(ToolStripMenuItem item)
			{
				_menuItem = item;
				_name = _menuItem.Text.Replace("&", "");
				Keys = item.ShortcutKeys;
				_menuItem.ShortcutKeys = 0;
			}

			public override string ToString()
			{
				return Name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
			}
		}

		static ShortcutEditor _shortcutEditor = new ShortcutEditor();

		string CompileShortcutKeys()
		{
			string c = "";

			for (int i = 0; i < _shortcutEditor.Shortcuts.Count; ++i)
			{
				var shortcut = _shortcutEditor.Shortcuts[i];

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

			_shortcutEditor.Shortcuts.Add(shortcut);
		}

		void InitUnlinkedShortcut(string name, Keys defaultKeys, Action callback)
		{
			ShortcutBase shortcut = new ShortcutBase(name, defaultKeys);
			shortcut.Pressed = callback;

			_shortcutEditor.Shortcuts.Add(shortcut);
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
			InitMenuShortcut(offToolStripMenuItem, () => SetTransparencyMode(TransparencySetting.Off));
			InitMenuShortcut(helmetOnlyToolStripMenuItem, () => SetTransparencyMode(TransparencySetting.Helmet));
			InitMenuShortcut(allToolStripMenuItem, () => SetTransparencyMode(TransparencySetting.All));
			InitMenuShortcut(headToolStripMenuItem, () => ToggleVisiblePart(HeadFlag));
			InitMenuShortcut(helmetToolStripMenuItem, () => ToggleVisiblePart(HelmetFlag));
			InitMenuShortcut(chestToolStripMenuItem, () => ToggleVisiblePart(ChestFlag));
			InitMenuShortcut(leftArmToolStripMenuItem, () => ToggleVisiblePart(LeftArmFlag));
			InitMenuShortcut(rightArmToolStripMenuItem, () => ToggleVisiblePart(RightArmFlag));
			InitMenuShortcut(leftLegToolStripMenuItem, () => ToggleVisiblePart(LeftLegFlag));
			InitMenuShortcut(rightLegToolStripMenuItem, () => ToggleVisiblePart(RightLegFlag));
			
			// not in the menu
			InitUnlinkedShortcut("Toggle transparency mode", Keys.Control | Keys.U, ToggleTransparencyMode);
			InitUnlinkedShortcut("Save skin", Keys.Control | Keys.S, PerformCommit);
			InitUnlinkedShortcut("Upload skin", Keys.Control | Keys.U, PerformUpload);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			InitShortcuts();
			LoadShortcutKeys(GlobalSettings.ShortcutKeys);

			SetTool(Tools.Camera);
			SetTransparencyMode(GlobalSettings.Transparency);
			SetViewMode(_currentViewMode);

			menuStrip1.Renderer = new Szotar.WindowsForms.ToolStripAeroRenderer(Szotar.WindowsForms.ToolbarTheme.Toolbar);
			toolStrip1.Renderer = new Szotar.WindowsForms.ToolStripAeroRenderer(Szotar.WindowsForms.ToolbarTheme.Toolbar);

			if (Screen.PrimaryScreen.BitsPerPixel != 32)
			{
				MessageBox.Show("Sorry, but apparently your video mode doesn't support a 32-bit pixel format - this is required, at the moment, for proper functionality of MCSkin3D. 16-bit support will be implemented at a later date, if it is asked for.", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}

			redColorSlider.Renderer = redRenderer = new MySliderRenderer(redColorSlider);
			greenColorSlider.Renderer = greenRenderer = new MySliderRenderer(greenColorSlider);
			blueColorSlider.Renderer = blueRenderer = new MySliderRenderer(blueColorSlider);
			alphaColorSlider.Renderer = alphaRenderer = new MySliderRenderer(alphaColorSlider);

			KeyPreview = true;
			Text = "MCSkin3D v" + VersionString;

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

			swatchContainer1.AddDirectory("Swatches");
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

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{

		}

		int _grassTop;
		int _charPaintTex, _alphaTex, _backgroundTex;
		static int _previewPaint, _currentSkin;

		private void glControl1_Load(object sender, EventArgs e)
		{
			glControl1_Resize(this, EventArgs.Empty);   // Ensure the Viewport is set up correctly
			GL.ClearColor(Color.SkyBlue);

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

			_grassTop = loadImage("grass.png");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			_backgroundTex = loadImage("inverted.png");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			_charPaintTex = GL.GenTexture();
			_previewPaint = GL.GenTexture();
			_currentSkin = GL.GenTexture();
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

				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
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
			_zoom += e.Delta / 50;

			glControl1.Invalidate();
		}

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

				const float ratio = 32.0f / 64.0f;
				GL.Enable(EnableCap.Blend);

				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
				GL.TexCoord2(1, 0); GL.Vertex2(glControl1.Width, 0);
				GL.TexCoord2(1, 1); GL.Vertex2(glControl1.Width, (float)glControl1.Width * ratio);
				GL.TexCoord2(0, 1); GL.Vertex2(0, (float)glControl1.Width * ratio);
				GL.End();

				if (!pickView && GlobalSettings.TextureOverlay)
				{
					GL.BindTexture(TextureTarget.Texture2D, _backgroundTex);

					GL.Begin(BeginMode.Quads);
					GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
					GL.TexCoord2(1, 0); GL.Vertex2(glControl1.Width, 0);
					GL.TexCoord2(1, 1); GL.Vertex2(glControl1.Width, (float)glControl1.Width * ratio);
					GL.TexCoord2(0, 1); GL.Vertex2(0, (float)glControl1.Width * ratio);
					GL.End();
				}

				GL.Disable(EnableCap.Blend);

				return;
			}

			Vector3 vec = new Vector3();
			int count = 0;
			if ((GlobalSettings.ViewFlags & HeadFlag) != 0)
			{
				vec.Add(new Vector3(0, 10, 0));
				count++;
			}
			if ((GlobalSettings.ViewFlags & ChestFlag) != 0)
			{
				vec.Add(new Vector3(0, 0, 0));
				count++;
			}
			if ((GlobalSettings.ViewFlags & RightLegFlag) != 0)
			{
				vec.Add(new Vector3(-2, -12, 0));
				count++;
			}
			if ((GlobalSettings.ViewFlags & LeftLegFlag) != 0)
			{
				vec.Add(new Vector3(2, -12, 0));
				count++;
			}
			if ((GlobalSettings.ViewFlags & RightArmFlag) != 0)
			{
				vec.Add(new Vector3(-6, 0, 0));
				count++;
			}
			if ((GlobalSettings.ViewFlags & LeftArmFlag) != 0)
			{
				vec.Add(new Vector3(6, 0, 0));
				count++;
			}
			if ((GlobalSettings.ViewFlags & HelmetFlag) != 0)
			{
				vec.Add(new Vector3(0, 10, 0));
				count++;
			}
			vec.Div(count);

			GL.Translate(0, 0, _zoom);
			GL.Rotate(_rotX, 1, 0, 0);
			GL.Rotate(_rotY, 0, 1, 0);

			GL.Translate(-vec.X, -vec.Y, 0);
			GL.PushMatrix();

			var clPt = glControl1.PointToClient(Cursor.Position);
			var x = clPt.X - (glControl1.Width / 2);
			var y = clPt.Y - (glControl1.Height / 2);

			GL.PushMatrix();

			if (!pickView && GlobalSettings.Transparency == TransparencySetting.All)
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

			if ((GlobalSettings.ViewFlags & HeadFlag) != 0)
				DrawSkinnedRectangle(0, 10, 0, 8, 8, 8,
				8, 8, 8, 8,
				24, 8, 8, 8,
				8, 0, 8, 8,
				16, 0, 8, 8,
				0, 8, 8, 8,
				16, 8, 8, 8,
				tex);
			GL.PopMatrix();

			if ((GlobalSettings.ViewFlags & ChestFlag) != 0)
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
			if ((GlobalSettings.ViewFlags & RightLegFlag) != 0)
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
			if ((GlobalSettings.ViewFlags & LeftLegFlag) != 0)
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
			if ((GlobalSettings.ViewFlags & RightArmFlag) != 0)
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
			if ((GlobalSettings.ViewFlags & LeftArmFlag) != 0)
				DrawSkinnedRectangle(6, 0, 0, 4, 12, 4,
				48, 20, -4, 12,
				56, 20, -4, 12,
				48, 16, -4, 4,
				52, 16, -4, 4,
				52, 20, -4, 12,
				44, 20, -4, 12,
				tex);
			GL.PopMatrix();

			if ((GlobalSettings.ViewFlags & HelmetFlag) != 0)
			{
				GL.PushMatrix();
				if (followCursorToolStripMenuItem.Checked)
				{
					GL.Translate(0, 4, 0);
					GL.Rotate((float)x / 25, 0, 1, 0);
					GL.Rotate((float)y / 25, 1, 0, 0);
					GL.Translate(0, -4, 0);
				}

				if (!pickView && GlobalSettings.Transparency != TransparencySetting.Off)
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
				GL.ClearColor(Color.SkyBlue);
			
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

		PixelsChanged _changedPixels = null;
		bool _md = false;
		Point _mp;
		private void glControl1_MouseDown(object sender, MouseEventArgs e)
		{
			_md = true;
			_mp = e.Location;

			if ((_currentTool == Tools.Pencil || _currentTool == Tools.Dropper ||
				_currentTool == Tools.Burn || _currentTool == Tools.Eraser ||
				_currentTool == Tools.Dodge) && e.Button == MouseButtons.Left)
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

				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				if (_currentTool == Tools.Pencil || _currentTool == Tools.Eraser || _currentTool == Tools.Burn || _currentTool == Tools.Dodge)
				{
					Point p = Point.Empty;

					if (GetPick(_mp.X, _mp.Y, ref p))
					{
						var c = array[p.X + (64 * p.Y)];
						var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);

						if (_currentTool == Tools.Pencil)
						{
							Color blend = Color.FromArgb(ColorBlending.AlphaBlend(color, oldColor).ToArgb());
							array[p.X + (64 * p.Y)] = blend.R | (blend.G << 8) | (blend.B << 16) | (blend.A << 24);
						}
						else if (_currentTool == Tools.Burn)
						{
							Color burnt = Color.FromArgb(ColorBlending.Burn(oldColor, 0.75f).ToArgb());
							array[p.X + (64 * p.Y)] = burnt.R | (burnt.G << 8) | (burnt.B << 16) | (burnt.A << 24);
						}
						else if (_currentTool == Tools.Dodge)
						{
							Color burnt = Color.FromArgb(ColorBlending.Dodge(oldColor, 0.25f).ToArgb());
							array[p.X + (64 * p.Y)] = burnt.R | (burnt.G << 8) | (burnt.B << 16) | (burnt.A << 24);
						}
						else
							array[p.X + (64 * p.Y)] = 0;
					}
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
			GL.ClearColor(Color.SkyBlue);

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

		void UseToolOnViewport(int x, int y)
		{
			if (listBox1.SelectedItem == null)
				return;

			Point p = Point.Empty;

			if (GetPick(x, y, ref p))
			{
				Skin skin = (Skin)listBox1.SelectedItem;

				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

				if (_currentTool == Tools.Pencil || _currentTool == Tools.Eraser || _currentTool == Tools.Burn || _currentTool == Tools.Dodge)
				{
					if (_changedPixels == null)
					{
						_changedPixels = new PixelsChanged();
						_changedPixels.NewColor = color;
					}

					if (!_changedPixels.Points.ContainsKey(new Point(p.X, p.Y)))
					{
						var c = array[p.X + (64 * p.Y)];
						var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
						_changedPixels.Points[new Point(p.X, p.Y)] = oldColor;

						// blend
						if (_currentTool == Tools.Pencil)
						{
							Color blend = Color.FromArgb(ColorBlending.AlphaBlend(color, oldColor).ToArgb());
							array[p.X + (64 * p.Y)] = blend.R | (blend.G << 8) | (blend.B << 16) | (blend.A << 24);
						}
						else if (_currentTool == Tools.Burn)
						{
							Color burnt = Color.FromArgb(ColorBlending.Burn(oldColor, 0.75f).ToArgb());
							array[p.X + (64 * p.Y)] = burnt.R | (burnt.G << 8) | (burnt.B << 16) | (burnt.A << 24);
						}
						else if (_currentTool == Tools.Dodge)
						{
							Color burnt = Color.FromArgb(ColorBlending.Dodge(oldColor, 0.25f).ToArgb());
							array[p.X + (64 * p.Y)] = burnt.R | (burnt.G << 8) | (burnt.B << 16) | (burnt.A << 24);
						}
						else if (_currentTool == Tools.Eraser)
							array[p.X + (64 * p.Y)] = 0;

						button2.Enabled = true;
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
					_rotY += (float)delta.X;
					_rotX += (float)delta.Y;
				}
				else if ((_currentTool == Tools.Camera && e.Button == MouseButtons.Right) ||
					((_currentTool != Tools.Camera) && e.Button == MouseButtons.Middle))
				{
					_zoom += (float)-delta.Y;
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

			if (button2.Enabled && listBox1.SelectedItem != _lastSkin)
			{
				/*if (MessageBox.Show("You have uncommited changes; you will lose all of your changes if you switch skins.\r\nAre you sure you want to switch?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
				{
					_skipListbox = true;
					listBox1.SelectedItem = _lastSkin;
					_skipListbox = false;
					return;
				}*/
				
				// Copy over the current changes to the tex stored in the skin.
				// This allows us to pick up where we left off later, without undoing any work.
				_lastSkin.CommitChanges(_currentSkin, false);
			}

			//if (_lastSkin != null)
			//	_lastSkin.Undo.Clear();

			glControl1.MakeCurrent();

			Skin skin = (Skin)listBox1.SelectedItem;
			button2.Enabled = skin.Dirty;

			if (skin == null)
			{
				_currentUndoBuffer = null;
				GL.BindTexture(TextureTarget.Texture2D, 0);
				int[] array = new int[64 * 32];
				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				undoToolStripMenuItem.Enabled = undoStripButton5.Enabled = false;
				redoToolStripMenuItem.Enabled = redoStripButton4.Enabled = false;
			}
			else
			{
				GL.BindTexture(TextureTarget.Texture2D, skin.GLImage);
				int[] array = new int[64 * 32];
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
				GL.BindTexture(TextureTarget.Texture2D, _currentSkin);
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
				Invoke((Action)delegate() { DoneUpload(); });
			}
		}

		void DoneUpload()
		{
			_pleaseWaitForm.Close();
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
			using (AddSkinDialog dlg = new AddSkinDialog())
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					var name = Path.GetFileNameWithoutExtension(dlg.FileName);

					File.Copy(dlg.FileName, "./Skins/" + dlg.SkinName + ".png");

					Skin skin = new Skin("./Skins/" + dlg.SkinName + ".png");
					listBox1.Items.Add(skin);
					listBox1.SelectedItem = skin;
				}
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
				}
			}
		}

		enum Tools
		{
			Camera,
			Pencil,
			Dropper,
			Eraser,
			Dodge,
			Burn
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

		void PerformCommit()
		{
			button2.Enabled = false;

			glControl1.MakeCurrent();

			Skin skin = (Skin)listBox1.SelectedItem;
			skin.CommitChanges(_currentSkin, true);

			listBox1.Invalidate();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			PerformCommit();
		}

		void PerformUndo()
		{
			glControl1.MakeCurrent();

			_currentUndoBuffer.Undo();

			undoToolStripMenuItem.Enabled = undoStripButton5.Enabled = _currentUndoBuffer.CanUndo;
			redoToolStripMenuItem.Enabled = redoStripButton4.Enabled = _currentUndoBuffer.CanRedo;

			Skin current = (Skin)listBox1.SelectedItem;
			button2.Enabled = current.Dirty = true;

			glControl1.Invalidate();
		}

		void PerformRedo()
		{
			glControl1.MakeCurrent();

			_currentUndoBuffer.Redo();

			Skin current = (Skin)listBox1.SelectedItem;
			button2.Enabled = current.Dirty = true;

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

		enum ViewMode
		{
			Perspective,
			Orthographic
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

		void SetTransparencyMode(TransparencySetting trans)
		{
			offToolStripMenuItem.Checked = helmetOnlyToolStripMenuItem.Checked = allToolStripMenuItem.Checked = false;
			GlobalSettings.Transparency = trans;

			switch (GlobalSettings.Transparency)
			{
			case TransparencySetting.Off:
				offToolStripMenuItem.Checked = true;
				break;
			case TransparencySetting.Helmet:
				helmetOnlyToolStripMenuItem.Checked = true;
				break;
			case TransparencySetting.All:
				allToolStripMenuItem.Checked = true;
				break;
			}

			glControl1.Invalidate();
		}

		private void offToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencySetting.Off);
		}

		private void helmetOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencySetting.Helmet);
		}

		private void allToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetTransparencyMode(TransparencySetting.All);
		}

		void ToggleVisiblePart(int flag)
		{
			GlobalSettings.ViewFlags ^= flag;

			bool hasNow = (GlobalSettings.ViewFlags & flag) != 0;

			ToolStripMenuItem item = null;

			// TODO: ugly
			switch (flag)
			{
			case HeadFlag:
				item = headToolStripMenuItem;
				break;
			case HelmetFlag:
				item = helmetToolStripMenuItem;
				break;
			case ChestFlag:
				item = chestToolStripMenuItem;
				break;
			case LeftArmFlag:
				item = leftArmToolStripMenuItem;
				break;
			case RightArmFlag:
				item = rightArmToolStripMenuItem;
				break;
			case LeftLegFlag:
				item = leftLegToolStripMenuItem;
				break;
			case RightLegFlag:
				item = rightLegToolStripMenuItem;
				break;
			}

			item.Checked = hasNow;

			glControl1.Invalidate();
		}

		private void headToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(HeadFlag);
		}

		private void helmetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(HelmetFlag);
		}

		private void chestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(ChestFlag);
		}

		private void leftArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(LeftArmFlag);
		}

		private void rightArmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(RightArmFlag);
		}

		private void leftLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(LeftLegFlag);
		}

		private void rightLegToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToggleVisiblePart(RightLegFlag);
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

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		void ToggleTransparencyMode()
		{
			switch (GlobalSettings.Transparency)
			{
			case TransparencySetting.Off:
				SetTransparencyMode(TransparencySetting.Helmet);
				break;
			case TransparencySetting.Helmet:
				SetTransparencyMode(TransparencySetting.All);
				break;
			case TransparencySetting.All:
				SetTransparencyMode(TransparencySetting.Off);
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

		private void colorSquare1_Click(object sender, EventArgs e)
		{

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
	}

	class CustomGLControl : GLControl
	{
		// 32bpp color, 24bpp z-depth, 8bpp stencil and 4x antialiasing
		// OpenGL version is major=3, minor=0
			public CustomGLControl()
			: base(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 0), 3, 0, OpenTK.Graphics.GraphicsContextFlags.Default)
		{
		}
	}
}
