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

using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using Paril.Settings;
using Paril.Settings.Serializers;
using System.IO;
using MCSkin3D.Macros;
using System;

namespace MCSkin3D
{
	public static class GlobalSettings
	{
		private static Settings Settings;
		public static bool Loaded;

		[Savable]
		public static bool Animate { get; set; }

		[Savable]
		public static bool FollowCursor { get; set; }

		[Savable]
		[DefaultValue(true)]
		public static bool Grass { get; set; }

		[Savable]
		[DefaultValue(false)]
		public static bool Ghost { get; set; }

		[Savable]
		[DefaultValue("")]
		public static string LastSkin { get; set; }

		[Savable]
		public static bool RememberMe { get; set; }

		[Savable]
		public static bool AutoLogin { get; set; }

		[Savable]
		[DefaultValue("")]
		public static string LastUsername { get; set; }

		[Savable]
		[DefaultValue("")]
		[TypeSerializer(typeof (PasswordSerializer<AesManaged>), false)]
		public static string LastPassword { get; set; }

		[Savable]
		[DefaultValue(TransparencyMode.Helmet)]
		[TypeSerializer(typeof (EnumSerializer<TransparencyMode>), true)]
		public static TransparencyMode Transparency { get; set; }

		[Savable]
		[DefaultValue(true)]
		public static bool AlphaCheckerboard { get; set; }

		[Savable]
		[DefaultValue(true)]
		public static bool TextureOverlay { get; set; }

		[Savable]
		[DefaultValue("Dynamic")]
		public static string LastBackground { get; set; }

		[Savable]
		[DefaultValue("")]
		public static string ShortcutKeys { get; set; }

		[Savable]
		[DefaultValue("135 206 235 255")]
		[TypeSerializer(typeof (ColorSerializer), true)]
		public static Color BackgroundColor { get; set; }

		[Savable]
		[DefaultValue(true)]
		public static bool AutoUpdate { get; set; }

		[Savable]
		[DefaultValue(0)]
		public static int Multisamples { get; set; }

		[Savable]
		public static bool PencilIncremental { get; set; }

		[Savable]
		public static bool DodgeBurnIncremental { get; set; }

		[Savable]
		[DefaultValue(0.70f)]
		public static float DodgeBurnExposure { get; set; }

		[Savable]
		public static bool DarkenLightenIncremental { get; set; }

		[Savable]
		[DefaultValue(0.25f)]
		public static float DarkenLightenExposure { get; set; }

		[Savable]
		[DefaultValue(0.20f)]
		public static float NoiseSaturation { get; set; }

		[Savable]
		public static string LanguageFile { get; set; }

		[Savable]
		[DefaultValue(24)]
		public static int TreeViewHeight { get; set; }

		[Savable]
		[DefaultValue(0)]
		public static float FloodFillThreshold { get; set; }

		[Savable]
		public static bool ResChangeDontShowAgain { get; set; }

		[Savable]
		[DefaultValue("$(DataLocation)Skins\\")]
		[TypeSerializer(typeof (StringArraySerializer), true)]
		public static string[] SkinDirectories { get; set; }

		[Savable]
		[DefaultValue(false)]
		public static bool OnePointEightMode { get; set; }

		[Savable]
		[DefaultValue("255 255 255 255")]
		[TypeSerializer(typeof (ColorSerializer), true)]
		public static Color DynamicOverlayLineColor { get; set; }

		[Savable]
		[DefaultValue("255 255 255 255")]
		[TypeSerializer(typeof (ColorSerializer), true)]
		public static Color DynamicOverlayTextColor { get; set; }

		[Savable]
		[DefaultValue(1)]
		public static int DynamicOverlayLineSize { get; set; }

		[Savable]
		[DefaultValue(1)]
		public static int DynamicOverlayTextSize { get; set; }

		[Savable]
		[DefaultValue("255 255 255 127")]
		[TypeSerializer(typeof (ColorSerializer), true)]
		public static Color DynamicOverlayGridColor { get; set; }

		[Savable]
		[DefaultValue(true)]
		public static bool InfiniteMouse { get; set; }

		[Savable]
		[DefaultValue(false)]
		public static bool RenderBenchmark { get; set; }

		[Savable]
		[DefaultValue(false)]
		public static bool GridEnabled { get; set; }

		[Savable]
		[DefaultValue(-1)]
		public static int RenderMode { get; set; }

		public static class InstallData
		{
			[Savable]
			[DefaultValue(".\\")]
			public static string DataLocation { get; set; }
		}

		public static string GetDataURI(string fileOrFolder)
		{
			return new DirectoryInfo(Environment.ExpandEnvironmentVariables(InstallData.DataLocation)).FullName + fileOrFolder;
		}

		public static void Load()
		{
			if (File.Exists("installData.ini"))
			{
				var installData = new Settings();
				installData.Structures.Add(typeof(InstallData));
				installData.Load("installData.ini");
			}
			else
				InstallData.DataLocation = ".\\";

			if (!InstallData.DataLocation.EndsWith("\\"))
				InstallData.DataLocation += '\\';

			MacroHandler.RegisterMacro("DataLocation", InstallData.DataLocation);

			try
			{
				Settings = new Settings();
				Settings.Structures.Add(typeof(GlobalSettings));

				Settings.Load(GetDataURI("settings.ini"));
				Loaded = true;
			}
			catch
			{
				Settings.LoadDefaults();
				Loaded = false;
			}

			MacroHandler.RegisterMacro("DefaultSkinFolder", MacroHandler.ReplaceMacros(GlobalSettings.SkinDirectories[0]));
		}

		public static void Save()
		{
			Settings.Save(GetDataURI("settings.ini"));
		}
	}
}